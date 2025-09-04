namespace BayuOrtak.Core.Helper
{
    using BayuOrtak.Core.Enums;
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper.Results;
    using BayuOrtak.Core.Wcf.Nhr.Helper;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.Text.RegularExpressions;
    using static BayuOrtak.Core.Enums.CRetMesaj;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public interface ILDAPHelper
    {
        (bool statuswarning, Exception ex, LDAPResult? result) Check(LDAPTip tip, string username, string password, bool istcknrequired, string dil);
        (bool statuswarning, Exception ex, LDAPResult? result) Search(LDAPTip tip, string username, bool istcknrequired, string dil);
        bool Any(LDAPTip tip, string username, bool istcknrequired);
        void Insert(bool isdebug, LDAPTip tip, string path, string username, string cn, string password, Dictionary<string, object> properties, string? method, string dil, out Exception? outvalue, out bool inserted);
        void Update(bool isdebug, LDAPTip tip, string username, Dictionary<string, object> properties, string? method, string dil, out Exception? outvalue);
        void UpdatePassword(bool isdebug, LDAPTip tip, string username, string newpassword, string? method, string dil, out Exception? outvalue);
        void UpdateAccountDisable(bool isdebug, LDAPTip tip, string username, bool ismovetotargetou, string? method, string dil, out Exception? outvalue);
        void UpdateAccountExpires(bool isdebug, LDAPTip tip, string username, DateTime expiresdate, bool ismovetotargetou, string? method, string dil, out Exception? outvalue);
        void TryAddUserToGroup(LDAPTip tip, string username, string principalcontextname, string groupprincipalidentityvalue, out Exception? outvalue);
    }
    public sealed class LDAPHelper : ILDAPHelper
    {
        #region Private - Internal
        private readonly string baydc, uzem;
        private const string _administrator = "administrator";
        private const string _samaccountname = "sAMAccountName";
        private const string _setpassword = "SetPassword";
        private const string _useraccountcontrol = "userAccountControl";
        private const string _islemlog = "islemlog";
        private record class record_setislemlog(string method, string islem, DateTime adddateUTC)
        {
            public bool isempty() => (this.method.IsNullOrEmpty_string() || this.islem.IsNullOrEmpty_string() || this.adddateUTC == DateTime.MinValue);
        }
        private void guardvalidation(ref string username, LDAPTip tip, string dil)
        {
            username = username.ToStringOrEmpty().ToLower();
            Guard.CheckEmpty(username, nameof(username));
            Guard.CheckEnumDefined<LDAPTip>(tip, nameof(tip));
            Guard.UnSupportLanguage(dil, nameof(dil));
            if (tip == LDAPTip.stu && username[0] != 'o') { username = $"o{username}"; }
        }
        private LDAPResult? getresult(LDAPTip tip, SearchResult? searchresult)
        {
            if (searchresult?.Properties == null) { return null; }
            ADUserAccountControl useraccountcontrol = default;
            long tckn = 0;
            string sicilno = "", ad = "", soyad = "";
            if (this.trygetpropertyvalue(searchresult, _useraccountcontrol.ToLower(), out object _o) && Enum.TryParse(_o == null ? "" : _o.ToString(), out ADUserAccountControl _aduseraccountcontrol)) { useraccountcontrol = _aduseraccountcontrol; }
            if (this.trygetpropertyvalue(searchresult, "sicilno", out _o) && _o is String _ssicilno && NHRTools.TrySicilNo(_ssicilno, out string _sicilno)) { sicilno = _sicilno; }
            if (this.trygetpropertyvalue(searchresult, "givenname", out _o) && _o is String _sad) { ad = _sad.ToTitleCase(true, new char[] { '.' }); }
            if (this.trygetpropertyvalue(searchresult, "sn", out _o) && _o is String _ssoyad) { soyad = _ssoyad.ToStringOrEmpty().ToUpper(); }
            if (this.trygetpropertyvalue(searchresult, "tckno", out _o) && Int64.TryParse(_o == null ? "0" : _o.ToString(), out long _l) && IsTCKimlikNo(tip, _l)) { tckn = _l; }
            else if (this.trygetpropertyvalue(searchresult, "description", out _o) && Int64.TryParse(_o == null ? "0" : _o.ToString(), out _l) && IsTCKimlikNo(tip, _l)) { tckn = _l; }
            var _dic = new Dictionary<string, object>();
            foreach (string item in searchresult.Properties.PropertyNames) { _dic.Add(item, searchresult.Properties[item][0]); }
            return new LDAPResult(useraccountcontrol, sicilno, ad, soyad, tckn, _dic.OrderBy(x => x.Key).ToDictionary());
        }
        private string getldappath(LDAPTip tip) => $"LDAP://192.168.10.{(tip == LDAPTip.personelkurum ? "11" : "111")}";
        private string getadminpassword(LDAPTip tip) => (tip == LDAPTip.personelkurum ? this.baydc : this.uzem);
        private string gettargetoupath(LDAPTip tip)
        {
            switch (tip)
            {
                case LDAPTip.personelkurum: return "LDAP://OU=BU_Disable_Users,OU=Kullanicilar,OU=Bayburt,DC=bayburt,DC=local"; // bayburt.local/Bayburt/Kullanicilar/BU_Disable_Users/
                case LDAPTip.uzem: return "LDAP://OU=Offline_Personal,DC=stu,DC=bayburt,DC=edu,DC=tr"; // stu.bayburt.edu.tr/Offline_Personal/
                case LDAPTip.stu: return "LDAP://OU=Offline_Students,DC=stu,DC=bayburt,DC=edu,DC=tr"; // stu.bayburt.edu.tr/Offline_Students/
                default: throw _other.ThrowNotSupportedForEnum<LDAPTip>();
            }
        }
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
        private bool tryparsedaperror(Exception ex, out string errorcode)
        {
            try
            {
                if (ex is DirectoryServicesCOMException _dscomex)
                {
                    var _match = Regex.Match(_dscomex.ExtendedErrorMessage, @"data\s+(\w+),");
                    if (_match.Success)
                    {
                        errorcode = _match.Groups[1].Value;
                        return true;
                    }
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
        private string setislemlog(object value, string? method, string basemethod, string islem)
        {
            var _data = new List<record_setislemlog> { new(method.CoalesceOrDefault(basemethod.GetRouteName<LDAPHelper>(false)), islem, DateTime.UtcNow) };
            if (value != null && _try.TryJson(Convert.ToString(value), JTokenType.Array, out JArray _ja)) { _data.AddRange(_ja.ToObject<record_setislemlog[]>().Where(x => !x.isempty()).ToArray()); }
            return _to.ToJSON(_data.OrderByDescending(x => x.adddateUTC).Take(5).ToArray());
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
        /// <param name="username">Doğrulanacak kullanıcı adı. (sAMAccountName)</param>
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
                        if (_data == null) { throw new Exception(GetDescriptionLocalizationValue(RetMesaj.kayityok, dil)); }
                        if (istcknrequired && _data.trytckncheckwarning(tip, dil, out Exception _ex)) { throw _ex; }
                        return (false, default, _data);
                    }
                    catch (Exception ex)
                    {
                        if (this.tryparsedaperror(ex, out string _e))
                        {
                            if (_e == "52e") { return (true, new Exception(dil == "en" ? "Error: Invalid username or password" : "Hata: Geçersiz kullanıcı adı veya şifre"), default); } //8009030C: LdapErr: DSID-0C09075E, comment: AcceptSecurityContext error, data 52e, v4563
                            if (_e == "533") { return (true, new Exception(String.Join(", ", (dil == "en" ? "Error: User account is disabled" : "Hata: Kullanıcı hesabı devre dışı"), ADUserAccountControl.ACCOUNTDISABLE.ToString("g"))), default); } //8009030C: LdapErr: DSID-0C09075E, comment: AcceptSecurityContext error, data 533, v4563
                            if (_e == "701") { return (true, new Exception(String.Join(", ", (dil == "en" ? "Error: User account has expired" : "Hata: Kullanıcı hesabının süresi dolmuş"), "ACCOUNTEXPIRES")), default); } //8009030C: LdapErr: DSID-0C09075E, comment: AcceptSecurityContext error, data 701, v4563
                        }
                        return (true, ex, default);
                    }
                }
            }
        }
        /// <summary>
        /// Administrator üzerinden <paramref name="username"/> kullanıcı kontrol yapar
        /// </summary>
        /// <param name="tip">LDAP bağlantı tipi.</param>
        /// <param name="username">Arama yapılan kullanıcı (sAMAccountName)</param>
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
                        if (_data == null) { throw new Exception(GetDescriptionLocalizationValue(RetMesaj.kayityok, dil)); }
                        if (istcknrequired && _data.trytckncheckwarning(tip, dil, out Exception _ex)) { throw _ex; }
                        return (false, default, _data);
                    }
                    catch (Exception ex) { return (true, ex, default); }
                }
            }
        }
        /// <summary>
        /// Belirtilen kullanıcı adına ait Active Directory hesabının geçerliliğini kontrol eder.
        /// <para>
        /// Aşağıdaki koşullar sağlanıyorsa <see langword="true"/> döner:
        /// <list type="bullet">
        ///   <item>Hesap Active Directory&#39;de mevcutsa,</item>
        ///   <item>Hesap &quot;disabled&quot; durumda değilse,</item>
        ///   <item>Hesabın &quot;accountExpires&quot; değeri boş (null) ya da ileri bir tarihse.</item>
        /// </list>
        /// </para>
        /// Yukarıdaki koşulların herhangi biri sağlanmıyorsa <see langword="false"/> döner.
        /// </summary>
        public bool Any(LDAPTip tip, string username, bool istcknrequired)
        {
            var _t = this.Search(tip, username, istcknrequired, "tr");
            return !_t.statuswarning && _t.result.isaccountstatus;
        }
        /// <summary>
        /// LDAP üzerine yeni bir kullanıcı ekler.
        /// </summary>
        /// <param name="isdebug"><see langword="true"/> ise, işlem yapılmaz ve metot sonlanır.</param>
        /// <param name="tip">LDAP bağlantı tipi.</param>
        /// <param name="path">Kullanıcının ekleneceği LDAP dizin yolu.</param>
        /// <param name="username">Eklenecek kullanıcının kullanıcı adı. (sAMAccountName)</param>
        /// <param name="cn">Kullanıcının ortak adı (Common Name).</param>
        /// <param name="password">Kullanıcının şifresi.</param>
        /// <param name="properties">Kullanıcıya atanacak özelliklerin bilgileri</param>
        /// <param name="method">İşlemi yapan metot adı</param>
        /// <param name="dil">Hata mesajlarının döndürüleceği dil.</param>
        /// <param name="outvalue">Olası hata durumunda döndürülecek istisna.</param>
        /// <param name="inserted">Ekleme işleminin başarılı olup olmadığını belirtir.</param>
        public void Insert(bool isdebug, LDAPTip tip, string path, string username, string cn, string password, Dictionary<string, object> properties, string? method, string dil, out Exception? outvalue, out bool inserted)
        {
            Guard.CheckEmpty(path, nameof(path));
            this.guardvalidation(ref username, tip, dil);
            Guard.CheckEmpty(cn, nameof(cn));
            Guard.CheckEmpty(password, nameof(password));
            Guard.CheckEmpty(properties, nameof(properties));
            if (isdebug)
            {
                outvalue = null;
                inserted = false;
                return;
            }
            try
            {
                inserted = false;
                if (path[0] != '/') { path = $"/{path}"; }
                using (var mainde = new DirectoryEntry(String.Concat(this.getldappath(tip), path), _administrator, this.getadminpassword(tip)))
                {
                    using (var ds = new DirectorySearcher(mainde, $"(&(objectClass=user)(objectCategory=person)({_samaccountname}={username}))", Array.Empty<string>(), SearchScope.Subtree))
                    {
                        var _searchresult = ds.FindOne();
                        if (_searchresult == null)
                        {
                            var _newuser = mainde.Children.Add($"CN={cn.ToUpper()}", "User");
                            properties.Upsert(_samaccountname, username);
                            foreach (var item in properties) { if (_newuser.Properties.Contains(item.Key)) { _newuser.Properties[item.Key].Value = item.Value; } }
                            if (_newuser.Properties.Contains(_islemlog)) { _newuser.Properties[_islemlog].Value = this.setislemlog(null, method, nameof(this.Insert), "insert"); }
                            _newuser.CommitChanges();
                            _newuser.Invoke(_setpassword, new object[] { password });
                            _newuser.Properties[_useraccountcontrol].Value = (int)ADUserAccountControl.NORMAL_ACCOUNT;
                            _newuser.CommitChanges();
                            _newuser.RefreshCache();
                            inserted = true;
                        }
                    }
                }
                outvalue = null;
            }
            catch (Exception ex)
            {
                outvalue = ex;
                inserted = false;
            }
        }
        /// <summary>
        /// LDAP üzerindeki bir kullanıcının özelliklerini günceller.
        /// </summary>
        /// <param name="isdebug"><see langword="true"/> ise, işlem yapılmaz ve metot sonlanır.</param>
        /// <param name="tip">LDAP bağlantı tipi.</param>
        /// <param name="username">Özellikleri güncellenecek kullanıcı adı. (sAMAccountName)</param>
        /// <param name="properties">Güncellenecek özelliklerin anahtar-değer çiftleri.</param>
        /// <param name="method">İşlemi yapan metot adı</param>
        /// <param name="dil">Hata mesajlarının döndürüleceği dil.</param>
        /// <param name="outvalue">Olası hata durumunda döndürülecek istisna.</param>
        public void Update(bool isdebug, LDAPTip tip, string username, Dictionary<string, object> properties, string? method, string dil, out Exception? outvalue)
        {
            this.guardvalidation(ref username, tip, dil);
            Guard.CheckEmpty(properties, nameof(properties));
            if (isdebug)
            {
                outvalue = null;
                return;
            }
            using (var mainde = new DirectoryEntry(this.getldappath(tip), _administrator, this.getadminpassword(tip)))
            {
                using (var ds = new DirectorySearcher(mainde, $"(&(objectClass=user)(objectCategory=person)({_samaccountname}={username}))", Array.Empty<string>(), SearchScope.Subtree))
                {
                    try
                    {
                        var _searchresult = ds.FindOne();
                        if (_searchresult == null) { throw new Exception(dil == "en" ? $"No record found for \"{username}\"!" : $"\"{username}\" üzerine kayıt bulunamadı!"); }
                        using (var user = _searchresult.GetDirectoryEntry())
                        {
                            foreach (var item in properties) { if (user.Properties.Contains(item.Key)) { user.Properties[item.Key].Value = item.Value; } }
                            if (user.Properties.Contains(_islemlog)) { user.Properties[_islemlog].Value = this.setislemlog(user.Properties[_islemlog].Value, method, nameof(this.Update), "update"); }
                            user.CommitChanges();
                            user.RefreshCache();
                            outvalue = null;
                        }
                    }
                    catch (Exception ex) { outvalue = ex; }
                }
            }
        }
        /// <summary>
        /// LDAP üzerindeki bir kullanıcının şifresini günceller.
        /// </summary>
        /// <param name="isdebug"><see langword="true"/> ise, işlem yapılmaz ve metot sonlanır.</param>
        /// <param name="tip">LDAP bağlantı tipi.</param>
        /// <param name="username">Şifresi güncellenecek kullanıcı adı (sAMAccountName).</param>
        /// <param name="newpassword">Yeni şifre değeri.</param>
        /// <param name="method">İşlemi yapan metot adı</param>
        /// <param name="dil">Hata mesajlarının döndürüleceği dil.</param>
        /// <param name="outvalue">Metot tamamlandığında oluşan hata (varsa) burada döndürülür.</param>
        public void UpdatePassword(bool isdebug, LDAPTip tip, string username, string newpassword, string? method, string dil, out Exception? outvalue)
        {
            this.guardvalidation(ref username, tip, dil);
            newpassword = newpassword.ToStringOrEmpty();
            Guard.CheckEmpty(newpassword, nameof(newpassword));
            if (isdebug)
            {
                outvalue = null;
                return;
            }
            using (var mainde = new DirectoryEntry(this.getldappath(tip), _administrator, this.getadminpassword(tip)))
            {
                using (var ds = new DirectorySearcher(mainde, $"(&(objectClass=user)(objectCategory=person)({_samaccountname}={username}))", Array.Empty<string>(), SearchScope.Subtree))
                {
                    try
                    {
                        var _searchresult = ds.FindOne();
                        if (_searchresult == null) { throw new Exception(dil == "en" ? $"No record found for \"{username}\"!" : $"\"{username}\" üzerine kayıt bulunamadı!"); }
                        using (var user = _searchresult.GetDirectoryEntry())
                        {
                            user.Invoke(_setpassword, new object[] { newpassword });
                            user.Properties["pwdLastSet"].Value = -1;
                            if (user.Properties.Contains(_islemlog)) { user.Properties[_islemlog].Value = this.setislemlog(user.Properties[_islemlog].Value, method, nameof(this.UpdatePassword), "updatepassword"); }
                            user.CommitChanges();
                            user.RefreshCache();
                            outvalue = null;
                        }
                    }
                    catch (Exception ex) { outvalue = ex; }
                }
            }
        }
        /// <summary>
        /// Active Directory üzerinde belirtilen kullanıcı hesabını devre dışı bırakır (disable) ve ilgili OU&#39;ya taşır. Kullanıcı, <paramref name="ismovetotargetou"/> <see langword="true"/> ise <paramref name="tip"/> parametresine göre aşağıdaki OU konumlarından birine taşınabilir:
        /// <list type="bullet">
        ///     <item>
        ///         <description>LDAPTip.personelkurum: <c>OU=BU_Disable_Users,OU=Kullanicilar,OU=Bayburt,DC=bayburt,DC=local</c></description>
        ///     </item>
        ///     <item>
        ///         <description>LDAPTip.uzem: <c>OU=Offline_Personal,DC=stu,DC=bayburt,DC=edu,DC=tr</c></description>
        ///     </item>
        ///     <item>
        ///         <description>LDAPTip.stu: <c>OU=Offline_Students,DC=stu,DC=bayburt,DC=edu,DC=tr</c></description>
        ///     </item>
        /// </list>
        /// Not: Kullanıcı bulunamazsa veya işlem sırasında hata oluşursa, <paramref name="outvalue"/> parametresine Exception atanır.
        /// </summary>
        /// <param name="isdebug"><see langword="true"/> ise, işlem yapılmaz ve metot sonlanır.</param>
        /// <param name="tip">LDAP tipi</param>
        /// <param name="username">Devre dışı bırakılacak kullanıcı adı (sAMAccountName).</param>
        /// <param name="ismovetotargetou">true ise <paramref name="tip"/> değerine göre hesabı belirtilen konumlardan birine taşır</param>
        /// <param name="method">İşlemi yapan metot adı</param>
        /// <param name="dil">Hata mesajının döneceği dil (&quot;tr&quot; veya &quot;en&quot;).</param>
        /// <param name="outvalue">Metot tamamlandığında oluşan hata (varsa) burada döndürülür.</param>
        public void UpdateAccountDisable(bool isdebug, LDAPTip tip, string username, bool ismovetotargetou, string? method, string dil, out Exception? outvalue)
        {
            this.guardvalidation(ref username, tip, dil);
            if (isdebug)
            {
                outvalue = null;
                return;
            }
            using (var mainde = new DirectoryEntry(this.getldappath(tip), _administrator, this.getadminpassword(tip)))
            {
                using (var ds = new DirectorySearcher(mainde, $"(&(objectClass=user)(objectCategory=person)({_samaccountname}={username}))", Array.Empty<string>(), SearchScope.Subtree))
                {
                    try
                    {
                        var _searchresult = ds.FindOne();
                        if (_searchresult == null) { throw new Exception(dil == "en" ? $"No record found for \"{username}\"!" : $"\"{username}\" üzerine kayıt bulunamadı!"); }
                        using (var user = _searchresult.GetDirectoryEntry())
                        {
                            user.Properties[_useraccountcontrol].Value = (ADUserAccountControl)Convert.ToInt32(user.Properties[_useraccountcontrol].Value) | ADUserAccountControl.ACCOUNTDISABLE;
                            if (user.Properties.Contains(_islemlog)) { user.Properties[_islemlog].Value = this.setislemlog(user.Properties[_islemlog].Value, method, nameof(this.UpdateAccountDisable), "accountdisable"); }
                            if (ismovetotargetou) { using (var tau = new DirectoryEntry(gettargetoupath(tip))) { user.MoveTo(tau, user.Name); } }
                            user.CommitChanges();
                            user.RefreshCache();
                            outvalue = null;
                        }
                    }
                    catch (Exception ex) { outvalue = ex; }
                }
            }
        }
        /// <summary>
        /// Active Directory üzerinde belirtilen kullanıcı hesabının son geçerlilik tarihini (accountExpires) ayarlar ve gerekirse ilgili OU&#39;ya taşır. Kullanıcı, <paramref name="ismovetotargetou"/> <see langword="true"/> ise <paramref name="tip"/> parametresine göre aşağıdaki OU konumlarından birine taşınabilir:
        /// <list type="bullet">
        ///     <item>
        ///         <description>LDAPTip.personelkurum: <c>OU=BU_Disable_Users,OU=Kullanicilar,OU=Bayburt,DC=bayburt,DC=local</c></description>
        ///     </item>
        ///     <item>
        ///         <description>LDAPTip.uzem: <c>OU=Offline_Personal,DC=stu,DC=bayburt,DC=edu,DC=tr</c></description>
        ///     </item>
        ///     <item>
        ///         <description>LDAPTip.stu: <c>OU=Offline_Students,DC=stu,DC=bayburt,DC=edu,DC=tr</c></description>
        ///     </item>
        /// </list>
        /// Not: Kullanıcı bulunamazsa veya işlem sırasında hata oluşursa, <paramref name="outvalue"/> parametresine Exception atanır.
        /// </summary>
        /// <param name="isdebug"><see langword="true"/> ise, işlem yapılmaz ve metot sonlanır.</param>
        /// <param name="tip">LDAP tipi.</param>
        /// <param name="username">Geçerlilik tarihi ayarlanacak kullanıcı adı (sAMAccountName).</param>
        /// <param name="expiresdate">Hesabın geçerli olacağı son tarih.</param>
        /// <param name="ismovetotargetou">true ise <paramref name="tip"/> değerine göre hesabı belirtilen konumlardan birine taşır.</param>
        /// <param name="method">İşlemi yapan metot adı</param>
        /// <param name="dil">Hata mesajının döneceği dil ("tr" veya "en").</param>
        /// <param name="outvalue">Metot tamamlandığında oluşan hata (varsa) burada döndürülür.</param>
        public void UpdateAccountExpires(bool isdebug, LDAPTip tip, string username, DateTime expiresdate, bool ismovetotargetou, string? method, string dil, out Exception? outvalue)
        {
            this.guardvalidation(ref username, tip, dil);
            if (isdebug)
            {
                outvalue = null;
                return;
            }
            using (var mainde = new DirectoryEntry(this.getldappath(tip), _administrator, this.getadminpassword(tip)))
            {
                using (var ds = new DirectorySearcher(mainde, $"(&(objectClass=user)(objectCategory=person)({_samaccountname}={username}))", Array.Empty<string>(), SearchScope.Subtree))
                {
                    try
                    {
                        var _searchresult = ds.FindOne();
                        if (_searchresult == null) { throw new Exception(dil == "en" ? $"No record found for \"{username}\"!" : $"\"{username}\" üzerine kayıt bulunamadı!"); }
                        using (var user = _searchresult.GetDirectoryEntry())
                        {
                            user.Properties["accountExpires"].Value = expiresdate.ToUniversalTime().ToFileTimeUtc().ToString();
                            if (user.Properties.Contains(_islemlog)) { user.Properties[_islemlog].Value = this.setislemlog(user.Properties[_islemlog].Value, method, nameof(this.UpdateAccountDisable), "accountdisable"); }
                            if (ismovetotargetou) { using (var tau = new DirectoryEntry(gettargetoupath(tip))) { user.MoveTo(tau, user.Name); } }
                            user.CommitChanges();
                            user.RefreshCache();
                            outvalue = null;
                        }
                    }
                    catch (Exception ex) { outvalue = ex; }
                }
            }
        }
        /// <summary>
        /// Bir kullanıcıyı LDAP üzerindeki bir gruba ekler.
        /// </summary>
        /// <param name="tip">LDAP bağlantı tipi.</param>
        /// <param name="username">Gruba eklenecek kullanıcı adı. (sAMAccountName)</param>
        /// <param name="principalcontextname"><see cref="PrincipalContext.Name"/> değeri</param>
        /// <param name="groupprincipalidentityvalue">Hedef grup adı.</param>
        /// <param name="outvalue">Olası hata durumunda döndürülecek istisna.</param>
        public void TryAddUserToGroup(LDAPTip tip, string username, string principalcontextname, string groupprincipalidentityvalue, out Exception? outvalue)
        {
            this.guardvalidation(ref username, tip, "tr");
            Guard.CheckEmpty(principalcontextname, nameof(principalcontextname));
            Guard.CheckEmpty(groupprincipalidentityvalue, nameof(groupprincipalidentityvalue));
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
                outvalue = null;
            }
            catch (Exception ex) { outvalue = ex; }
        }
        /// <summary>
        /// Verilen değerin geçerli bir T.C. Kimlik Numarası olup olmadığını kontrol eder.
        /// </summary>
        public static bool IsTCKimlikNo(LDAPTip tip, long tckn)
        {
            if (_is.IsTCKimlikNo(tckn)) { return true; }
            var _tckn = ((tip == LDAPTip.stu && tckn > 0) ? tckn.ToString() : "0");
            return (_tckn.Length == _maximumlength.tckn && _tckn.StartsWith("98"));
        }
    }
}