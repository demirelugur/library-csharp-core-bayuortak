namespace BayuOrtak.Core.Helper
{
    using BayuOrtak.Core.Enums;
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper.Results;
    using BayuOrtak.Core.Wcf.Nhr.Helper;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.Text.RegularExpressions;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public interface ILDAPHelper
    {
        (bool statuswarning, Exception ex, LDAPResult? result) Check(LDAPTip tip, string username, string password, bool istcknrequired, string dil);
        (bool statuswarning, Exception ex, LDAPResult? result) Search(LDAPTip tip, string username, bool istcknrequired, string dil);
        void Insert(LDAPTip tip, string path, string username, string cn, string password, Dictionary<string, object> properties, string method, string dil, out Exception outvalue, out bool inserted);
        void Update(bool isdebug, LDAPTip tip, string username, Dictionary<string, object> properties, string method, string dil, out Exception outvalue);
        void UpdatePassword(bool isdebug, LDAPTip tip, string username, string newpassword, string method, string dil, out Exception outvalue);
        void TryAddUserToGroup(LDAPTip tip, string username, string groupprincipalidentityvalue, string principalcontextname, out Exception outvalue);
    }
    public sealed class LDAPHelper : ILDAPHelper
    {
        #region Private
        private readonly string baydc, uzem;
        private const string _administrator = "administrator";
        private const string _samaccountname = "sAMAccountName";
        private const string _islemlog = "islemlog";
        private const string _setpassword = "SetPassword";
        private record class record_setislemlog(string m, DateTime d);
        private void guardvalidation(ref string username, LDAPTip tip, string dil)
        {
            Guard.CheckEmpty(username, nameof(username));
            Guard.CheckEnumDefined<LDAPTip>(tip, nameof(tip));
            Guard.UnSupportLanguage(dil, nameof(dil));
            username = username.ToLower();
            if (tip == LDAPTip.stu && username[0] != 'o') { username = $"o{username}"; }
        }
        private LDAPResult getresult(LDAPTip tip, SearchResult searchresult)
        {
            if (searchresult?.Properties == null) { return new LDAPResult(); }
            ADUserAccountControl useraccountcontrol = default;
            long tckn = 0;
            string sicilno = "", ad = "", soyad = "";
            if (this.trygetpropertyvalue(searchresult, "useraccountcontrol", out object _o) && _o != null && Enum.TryParse(_o.ToString(), out ADUserAccountControl _useraccountcontrol)) { useraccountcontrol = _useraccountcontrol; }
            if (this.trygetpropertyvalue(searchresult, "sicilno", out _o) && _o is String _ssicilno && NHRTools.TrySicilNo(_ssicilno, out string _sicilno)) { sicilno = _sicilno; }
            if (this.trygetpropertyvalue(searchresult, "givenname", out _o) && _o is String _sad) { ad = _sad.ToTitleCase(true, new char[] { '.' }); }
            if (this.trygetpropertyvalue(searchresult, "sn", out _o) && _o is String _ssoyad) { soyad = _ssoyad.ToStringOrEmpty().ToUpper(); }
            if (this.trygetpropertyvalue(searchresult, "tckno", out _o) && _o != null)
            {
                if (tip == LDAPTip.stu && _try.TryTCKimlikNoOrSW98(_o.ToString(), out long _tckn)) { tckn = _tckn; }
                else if (_try.TryTCKimlikNo(_o.ToString(), out _tckn)) { tckn = _tckn; }
            }
            else if (this.trygetpropertyvalue(searchresult, "description", out _o) && _o != null)
            {
                if (tip == LDAPTip.stu && _try.TryTCKimlikNoOrSW98(_o.ToString(), out long _tckn)) { tckn = _tckn; }
                else if (_try.TryTCKimlikNo(_o.ToString(), out _tckn)) { tckn = _tckn; }
            }
            var _dic = new Dictionary<string, object>();
            foreach (string item in searchresult.Properties.PropertyNames) { _dic.Add(item, searchresult.Properties[item][0]); }
            return new LDAPResult(useraccountcontrol, sicilno, ad, soyad, tckn, _dic.OrderBy(x => x.Key).ToDictionary());
        }
        private string getldappath(LDAPTip value) => $"LDAP://192.168.10.{(value == LDAPTip.personelkurum ? "11" : "111")}";
        private bool trygetpropertyvalue(SearchResult searchresult, string propertyname, out object outvalue)
        {
            try
            {
                if (searchresult.Properties.Contains(propertyname))
                {
                    outvalue = searchresult.Properties[propertyname][0];
                    return true;
                }
                outvalue = null;
                return false;
            }
            catch
            {
                outvalue = null;
                return false;
            }
        }
        private bool tryparsedaperror(DirectoryServicesCOMException dscomex, out string errorcode)
        {
            try
            {
                var _pattern = @"data\s+(\w+),";
                var _match = Regex.Match(dscomex.ExtendedErrorMessage, _pattern);
                if (_match.Success)
                {
                    errorcode = _match.Groups[1].Value;
                    return true;
                }
                errorcode = "";
                return false;
            }
            catch
            {
                errorcode = "";
                return false;
            }
        }
        private string getadminpassword(LDAPTip tip) => (tip == LDAPTip.personelkurum ? this.baydc : this.uzem);
        private Exception translateldapexception(DirectoryServicesCOMException dscomex, string dil)
        {
            var _errorcode = this.tryparsedaperror(dscomex, out string _e) ? _e : "";
            if (_errorcode == "52e") { return new Exception(dil == "en" ? "Error: Invalid username or password" : "Hata: Geçersiz kullanıcı adı veya şifre"); } //8009030C: LdapErr: DSID-0C09075E, comment: AcceptSecurityContext error, data 52e, v4563
            else if (_errorcode == "533") { return new Exception(String.Concat((dil == "en" ? "Error: User account is disabled" : "Hata: Kullanıcı hesabı devre dışı"), ", UserAccountControl: ", ADUserAccountControl.ACCOUNTDISABLE.ToString("g"))); } //8009030C: LdapErr: DSID-0C09075E, comment: AcceptSecurityContext error, data 533, v4563
            else if (_errorcode == "701") { return new Exception(String.Concat((dil == "en" ?  "Error: User account has expired" : "Hata: Kullanıcı hesabının süresi dolmuş"), ", UserAccountControl: ", ADUserAccountControl.PASSWORD_EXPIRED.ToString("g"))); } //8009030C: LdapErr: DSID-0C09075E, comment: AcceptSecurityContext error, data 701, v4563
            else { return new Exception(dil == "en" ? "Unknown error" : "Bilinmeyen hata", dscomex); }
        }
        private object setislemlog(object value, string method)
        {
            var _p = new record_setislemlog(method.CoalesceOrDefault($"/{nameof(LDAPHelper)}/{nameof(this.setislemlog)}"), DateTime.Now);
            if (value != null && _try.TryJson(Convert.ToString(value), JTokenType.Object, out JObject _jo) && _jo.Count > 0)
            {
                var _data = new List<record_setislemlog> { _p };
                if (_jo.TryGetValue(_islemlog, out JToken _jt) && _jt.Type == JTokenType.Array)
                {
                    _data.AddRange(_jt.Select(x => new
                    {
                        m = x[nameof(record_setislemlog.m)],
                        d = x[nameof(record_setislemlog.d)]
                    }).Select(x => new
                    {
                        m = (x.m != null && x.m.Type == JTokenType.String) ? x.m.ToStringOrEmpty() : "",
                        d = (x.d != null && x.d.Type == JTokenType.Date) ? Convert.ToDateTime(x.d) : DateTime.MinValue
                    }).Where(x => x.m != "" && x.d.Ticks > 0).Select(x => new record_setislemlog(x.m, x.d)).ToArray());
                }
                _jo[_islemlog] = JToken.FromObject(_data.OrderByDescending(x => x.d).Take(5).ToArray());
                return _jo.ToString(Formatting.None);
            }
            return _to.ToJSON(new Dictionary<string, object> { { _islemlog, new record_setislemlog[] { _p } } });
        }
        #endregion
        public LDAPHelper(string baydc, string uzem)
        {
            this.baydc = baydc;
            this.uzem = uzem;
        }
        /// <summary>
        /// Belirtilen kullanıcı adı ve şifre ile LDAP üzerinde kullanıcı doğrulama işlemini gerçekleştirir.
        /// </summary>
        /// <param name="tip">LDAP bağlantı tipi.</param>
        /// <param name="username">Doğrulanacak kullanıcı adı.</param>
        /// <param name="password">Kullanıcı şifresi.</param>
        /// <param name="istcknrequired">T.C. Kimlik numarası kontrolünün zorunlu olup olmadığını belirtir.</param>
        /// <param name="dil">Hata mesajlarının döndürüleceği dil.</param>
        /// <returns>İşlem sonucu, hata (varsa) ve kullanıcı bilgilerini içeren bir tuple.</returns>
        public (bool statuswarning, Exception ex, LDAPResult? result) Check(LDAPTip tip, string username, string password, bool istcknrequired, string dil)
        {
            this.guardvalidation(ref username, tip, dil);
            Guard.CheckEmpty(password, nameof(password));
            using (var mainde = new DirectoryEntry(this.getldappath(tip), username, password))
            {
                using (var ds = new DirectorySearcher(mainde, $"(&(objectClass=user)(objectCategory=person)({_samaccountname}={username}))", Array.Empty<string>(), SearchScope.Subtree))
                {
                    try
                    {
                        var _data = this.getresult(tip, ds.FindOne());
                        _data.tryiswarning(dil, istcknrequired, out Exception _ex);
                        if (_ex == null) { return (false, default, _data); }
                        throw _ex;
                    }
                    catch (Exception ex) { return (true, (ex is DirectoryServicesCOMException dscomex ? this.translateldapexception(dscomex, dil) : ex), default); }
                }
            }
        }
        /// <summary>
        /// Administrator üzerinden <paramref name="username"/> kullanıcı kontrol yapar
        /// </summary>
        /// <param name="tip">LDAP bağlantı tipi.</param>
        /// <param name="username">Arama yapılan kullanıcı</param>
        /// <param name="istcknrequired">T.C. Kimlik numarası kontrolünün zorunlu olup olmadığını belirtir.</param>
        /// <param name="dil">Hata mesajlarının döndürüleceği dil.</param>
        /// <returns>İşlem sonucu, hata (varsa) ve kullanıcı bilgilerini içeren bir tuple.</returns>
        public (bool statuswarning, Exception ex, LDAPResult? result) Search(LDAPTip tip, string username, bool istcknrequired, string dil)
        {
            this.guardvalidation(ref username, tip, dil);
            using (var mainde = new DirectoryEntry(this.getldappath(tip), _administrator, this.getadminpassword(tip)))
            {
                using (var ds = new DirectorySearcher(mainde, $"(&(objectClass=user)(objectCategory=person)({_samaccountname}={username}))", Array.Empty<string>(), SearchScope.Subtree))
                {
                    try
                    {
                        var _data = this.getresult(tip, ds.FindOne());
                        _data.tryiswarning(dil, istcknrequired, out Exception _ex);
                        if (_ex == null) { return (false, default, _data); }
                        throw _ex;
                    }
                    catch (Exception ex) { return (true, (ex is DirectoryServicesCOMException dscomex ? this.translateldapexception(dscomex, dil) : ex), default); }
                }
            }
        }
        /// <summary>
        /// LDAP üzerine yeni bir kullanıcı ekler.
        /// </summary>
        /// <param name="tip">LDAP bağlantı tipi.</param>
        /// <param name="path">Kullanıcının ekleneceği LDAP dizin yolu.</param>
        /// <param name="username">Eklenecek kullanıcının kullanıcı adı.</param>
        /// <param name="cn">Kullanıcının ortak adı (Common Name).</param>
        /// <param name="password">Kullanıcının şifresi.</param>
        /// <param name="properties">Kullanıcıya atanacak özelliklerin bilgileri</param>
        /// <param name="method">Çağrıyı yapan metodun adı (log için).</param>
        /// <param name="dil">Hata mesajlarının döndürüleceği dil.</param>
        /// <param name="outvalue">Olası hata durumunda döndürülecek istisna.</param>
        /// <param name="inserted">Ekleme işleminin başarılı olup olmadığını belirtir.</param>
        public void Insert(LDAPTip tip, string path, string username, string cn, string password, Dictionary<string, object> properties, string method, string dil, out Exception outvalue, out bool inserted)
        {
            Guard.CheckEmpty(path, nameof(path));
            this.guardvalidation(ref username, tip, dil);
            Guard.CheckEmpty(cn, nameof(cn));
            Guard.CheckEmpty(password, nameof(password));
            Guard.CheckEmpty(properties, nameof(properties));
            try
            {
                inserted = false;
                if (path[0] != '/') { path = $"/{path}"; }
                using (var mainde = new DirectoryEntry(String.Concat(this.getldappath(tip), path), _administrator, this.getadminpassword(tip)))
                {
                    using (var ds = new DirectorySearcher(mainde, $"(&(objectClass=user)(objectCategory=person)({_samaccountname}={username}))", Array.Empty<string>(), SearchScope.Subtree))
                    {
                        if (ds.FindOne() == null)
                        {
                            var _newde = mainde.Children.Add($"CN={cn.ToUpper()}", "User");
                            properties.Upsert(_samaccountname, username);
                            foreach (var item in properties) { _newde.Properties[item.Key].Value = item.Value; }
                            if (_newde.Properties.Contains(_islemlog)) { _newde.Properties[_islemlog].Value = this.setislemlog(null, method); }
                            _newde.CommitChanges();
                            _newde.Invoke("SetPassword", new object[] { password });
                            _newde.Properties["userAccountControl"].Value = (int)ADUserAccountControl.NORMAL_ACCOUNT;
                            _newde.CommitChanges();
                            mainde.CommitChanges();
                            inserted = true;
                        }
                    }
                }
                outvalue = null;
            }
            catch (Exception ex)
            {
                outvalue = (ex is DirectoryServicesCOMException dscomex ? this.translateldapexception(dscomex, dil) : ex);
                inserted = false;
            }
        }
        /// <summary>
        /// LDAP üzerindeki bir kullanıcının özelliklerini günceller.
        /// </summary>
        /// <param name="isdebug">Hata ayıklama modunun açık olup olmadığını belirtir.</param>
        /// <param name="tip">LDAP bağlantı tipi.</param>
        /// <param name="username">Özellikleri güncellenecek kullanıcı adı.</param>
        /// <param name="properties">Güncellenecek özelliklerin anahtar-değer çiftleri.</param>
        /// <param name="method">Çağrıyı yapan metodun adı (log için).</param>
        /// <param name="dil">Hata mesajlarının döndürüleceği dil.</param>
        /// <param name="outvalue">Olası hata durumunda döndürülecek istisna.</param>
        public void Update(bool isdebug, LDAPTip tip, string username, Dictionary<string, object> properties, string method, string dil, out Exception outvalue)
        {
            this.guardvalidation(ref username, tip, dil);
            Guard.CheckEmpty(properties, nameof(properties));
            if (isdebug) { outvalue = null; }
            using (var mainde = new DirectoryEntry(this.getldappath(tip), _administrator, this.getadminpassword(tip)))
            {
                using (var ds = new DirectorySearcher(mainde, $"(&(objectClass=user)(objectCategory=person)({_samaccountname}={username}))", Array.Empty<string>(), SearchScope.Subtree))
                {
                    try
                    {
                        var _searchresult = ds.FindOne();
                        using (var de = _searchresult.GetDirectoryEntry())
                        {
                            foreach (var item in properties)
                            {
                                if (item.Key == _setpassword) { continue; }
                                else { de.Properties[item.Key].Value = item.Value; }
                            }
                            if (de.Properties.Contains(_islemlog)) { de.Properties[_islemlog].Value = this.setislemlog(de.Properties[_islemlog].Value, method); }
                            de.CommitChanges();
                            if (properties.TryGetValue(_setpassword, out object _setPassword))
                            {
                                de.Invoke(_setpassword, new object[] { _setPassword });
                                de.Properties["pwdLastSet"].Value = -1;
                                de.CommitChanges();
                            }
                            de.RefreshCache();
                            outvalue = null;
                        }
                    }
                    catch (Exception ex) { outvalue = (ex is DirectoryServicesCOMException dscomex ? this.translateldapexception(dscomex, dil) : ex); }
                }
            }
        }
        /// <summary>
        /// LDAP üzerindeki bir kullanıcının şifresini günceller.
        /// </summary>
        /// <param name="isdebug">Hata ayıklama modunun açık olup olmadığını belirtir.</param>
        /// <param name="tip">LDAP bağlantı tipi.</param>
        /// <param name="username">Şifresi güncellenecek kullanıcı adı.</param>
        /// <param name="newpassword">Yeni şifre.</param>
        /// <param name="method">Çağrıyı yapan metodun adı (log için).</param>
        /// <param name="dil">Hata mesajlarının döndürüleceği dil.</param>
        /// <param name="outvalue">Olası hata durumunda döndürülecek istisna.</param>
        public void UpdatePassword(bool isdebug, LDAPTip tip, string username, string newpassword, string method, string dil, out Exception outvalue)
        {
            newpassword = newpassword.ToStringOrEmpty();
            Guard.CheckEmpty(newpassword, nameof(newpassword));
            this.Update(isdebug, tip, username, new Dictionary<string, object> { { _setpassword, newpassword } }, method, dil, out outvalue);
        }
        /// <summary>
        /// Bir kullanıcıyı LDAP üzerindeki bir gruba ekler.
        /// </summary>
        /// <param name="tip">LDAP bağlantı tipi.</param>
        /// <param name="username">Gruba eklenecek kullanıcı adı.</param>
        /// <param name="groupprincipalidentityvalue">Hedef grup adı.</param>
        /// <param name="principalcontextname"><see cref="PrincipalContext.Name"/> değeri</param>
        /// <param name="outvalue">Olası hata durumunda döndürülecek istisna.</param>
        public void TryAddUserToGroup(LDAPTip tip, string username, string groupprincipalidentityvalue, string principalcontextname, out Exception outvalue)
        {
            try
            {
                using (var pc = new PrincipalContext(ContextType.Domain, principalcontextname, _administrator, this.getadminpassword(tip)))
                {
                    using (var gp = GroupPrincipal.FindByIdentity(pc, groupprincipalidentityvalue))
                    {
                        if (gp != null && !gp.Members.Any(x => x.SamAccountName == username))
                        {
                            gp.Members.Add(pc, IdentityType.SamAccountName, username);
                            gp.Save();
                        }
                    }
                }
                outvalue = default;
            }
            catch (Exception ex) { outvalue = ex; }
        }
    }
}