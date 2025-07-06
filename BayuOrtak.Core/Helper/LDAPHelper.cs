namespace BayuOrtak.Core.Helper
{
    using BayuOrtak.Core.Enums;
    using BayuOrtak.Core.Extensions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices;
    using static BayuOrtak.Core.Helper.CLDAPTip;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    using static BayuOrtak.Core.Helper.OrtakTools;
    /// <summary>
    /// LDAP türlerini temsil eden sınıf.
    /// </summary>
    public sealed class CLDAPTip
    {
        /// <summary>
        /// LDAP türlerini tanımlayan enum.
        /// </summary>
        [Flags]
        public enum LDAPTip
        {
            personelkurum = 1,
            uzem = 2,
            stu = 4
        }
        /// <summary>
        /// Verilen <see cref="LDAPTip"/> değerine göre LDAP yolunu döndürür.
        /// </summary>
        /// <param name="value">LDAP türünü temsil eden <see cref="LDAPTip"/> enum değeri.</param>
        /// <returns>Belirtilen LDAP türüne göre oluşturulmuş LDAP yolunu döndürür.</returns>
        public static string GetPath(LDAPTip value) => $"LDAP://192.168.10.{(value == LDAPTip.personelkurum ? "11" : "111")}";
    }
    public sealed class LDAPHelper
    {
        /// <summary>
        /// LDAP işlem sonucunu gösteren bool tipi durum. 
        /// Eğer işlem başarılı ise <see langword="false"/>, başarısız ise <see langword="true"/> döner.
        /// </summary>
        public bool statuswarning { get; }
        /// <summary>
        /// Kullanıcının T.C. Kimlik Numarası (TCKN). 
        /// Eğer işlem başarısız olursa varsayılan değer (0) atanır.
        /// </summary>
        public long tckn { get; }
        /// <summary>
        /// Kullanıcının LDAP sisteminde kayıtlı olan kullanıcı adı.
        /// Eğer işlem başarısız olursa boş bir string atanır.
        /// </summary>
        public string kuladi { get; }
        /// <summary>
        /// LDAP işlemi sırasında meydana gelen hata. 
        /// Eğer bir hata yoksa bu değer null olur.
        /// </summary>
        public Exception ex { get; }
        /// <summary>
        /// LDAP işlemiyle ilgili ek bilgilerin saklandığı, 
        /// anahtar-değer çiftlerinden oluşan sözlük yapısı.
        /// Eğer işlem başarısız olursa veya ek bilgi yoksa, 
        /// boş bir sözlük atanır.
        /// </summary>
        public Dictionary<string, object> dictionary { get; }
        /// <summary>
        /// LDAP işlemi sırasında meydana gelen tüm hataların 
        /// mesajlarını içeren bir string dizisi.
        /// Eğer bir hata oluşmadıysa boş bir dizi döner.
        /// </summary>
        public string[] errors => (this.ex == null ? Array.Empty<string>() : this.ex.AllExceptionMessage());
        public LDAPHelper() : this(default, default, "", default, default) { }
        public LDAPHelper(bool statuswarning, long tckn, string kuladi, Exception ex, Dictionary<string, object> dictionary)
        {
            this.statuswarning = statuswarning;
            this.tckn = tckn;
            this.kuladi = kuladi;
            this.ex = ex;
            this.dictionary = dictionary ?? new Dictionary<string, object>();
        }
        /// <summary>
        /// Test ortamında <paramref name="tckn"/> veya <paramref name="kuladi"/> belirtilen kullanıcı üzerinden giriş yapabilmek adına oluşturulmuştur
        /// </summary>
        public static LDAPHelper Set_debug(long tckn, string kuladi) => new LDAPHelper(false, tckn, kuladi, default, default);
        /// <summary>
        /// Kullanıcı adı ve şifre bilgileri ile LDAP sisteminde kullanıcıyı doğrular ve LDAPHelper nesnesi döner.
        /// Kullanıcının LDAP sistemindeki varlığı sorgulanır ve sonuç döndürülür.
        /// </summary>
        /// <param name="userName">Kullanıcının LDAP sistemindeki kullanıcı adı.</param>
        /// <param name="password">Kullanıcının LDAP sistemindeki şifresi.</param>
        /// <param name="tip">LDAP sunucusunun tipi.</param>
        /// <param name="isDescriptionTcknCheck">Açıklama alanının TCKN doğrulaması yapılıp yapılmadığını belirten parametre.</param>
        /// <param name="dil">İşlem sırasında kullanılacak dil (örneğin: &quot;tr&quot; veya &quot;en&quot;).</param>
        /// <returns>LDAPHelper nesnesi döner. Eğer hata oluşursa, Exception bilgisi LDAPHelper içinde saklanır.</returns>
        public static LDAPHelper Check(string userName, string password, LDAPTip tip, bool isDescriptionTcknCheck, string dil = "tr")
        {
            userName = usernamedilkontrol(userName, tip, nameof(userName), dil);
            Guard.CheckEmpty(password, nameof(password));
            using (var mainde = new DirectoryEntry(GetPath(tip), userName, password))
            {
                using (var ds = new DirectorySearcher(mainde, $"(&(objectClass=user)(objectCategory=person)({_samaccountname}={userName}))", Array.Empty<string>(), SearchScope.Subtree))
                {
                    try { return getdata(ds.FindOne(), tip, false, isDescriptionTcknCheck, dil); }
                    catch (Exception ex) { return set_exception(ex); }
                }
            }
        }
        /// <summary>
        /// Admin kullanıcı bilgilerini kullanarak <c>aktif</c> bir kullanıcıyı LDAP sisteminde arar ve sonucunu döner.
        /// Arama sonucu kullanıcı bilgileri başarılıysa, LDAPHelper nesnesi döner; aksi takdirde hata oluşur.
        /// </summary>
        /// <param name="searchUserName">Aranacak kullanıcının LDAP sistemindeki kullanıcı adı.</param>
        /// <param name="adminPassword">Admin kullanıcı için LDAP sistemine giriş şifresi.</param>
        /// <param name="tip">LDAP sunucusunun tipi.</param>
        /// <param name="isDescriptionTcknCheck">Açıklama alanının TCKN doğrulaması yapılıp yapılmadığını belirten parametre.</param>
        /// <param name="dil">İşlem sırasında kullanılacak dil (örneğin: &quot;tr&quot; veya &quot;en&quot;).</param>
        /// <returns>LDAPHelper nesnesi döner. Eğer hata oluşursa, Exception bilgisi LDAPHelper içinde saklanır.</returns>
        public static LDAPHelper Search(string searchUserName, string adminPassword, LDAPTip tip, bool isDescriptionTcknCheck, string dil = "tr")
        {
            searchUserName = usernamedilkontrol(searchUserName, tip, nameof(searchUserName), dil);
            Guard.CheckEmpty(adminPassword, nameof(adminPassword));
            using (var mainde = new DirectoryEntry(GetPath(tip), _administrator, adminPassword))
            {
                using (var ds = new DirectorySearcher(mainde, $"(&(objectClass=user)(objectCategory=person)({_samaccountname}={searchUserName}))", Array.Empty<string>(), SearchScope.Subtree))
                {
                    try { return getdata(ds.FindOne(), tip, false, isDescriptionTcknCheck, dil); }
                    catch (Exception ex) { return set_exception(ex); }
                }
            }
        }
        /// <summary>
        /// Admin kullanıcı bilgilerini kullanarak <c>herhangi</c> bir kullanıcıyı LDAP sisteminde arar ve sonucunu döner.
        /// Arama sonucu kullanıcı bilgileri başarılıysa, LDAPHelper nesnesi döner; aksi takdirde hata oluşur.
        /// </summary>
        /// <param name="searchUserName">Aranacak kullanıcının LDAP sistemindeki kullanıcı adı.</param>
        /// <param name="adminPassword">Admin kullanıcı için LDAP sistemine giriş şifresi.</param>
        /// <param name="tip">LDAP sunucusunun tipi.</param>
        /// <param name="dil">İşlem sırasında kullanılacak dil (örneğin: &quot;tr&quot; veya &quot;en&quot;).</param>
        /// <returns>LDAPHelper nesnesi döner. Eğer hata oluşursa, Exception bilgisi LDAPHelper içinde saklanır.</returns>
        public static LDAPHelper SearchAll(string searchUserName, string adminPassword, LDAPTip tip, string dil = "tr")
        {
            searchUserName = usernamedilkontrol(searchUserName, tip, nameof(searchUserName), dil);
            Guard.CheckEmpty(adminPassword, nameof(adminPassword));
            using (var mainde = new DirectoryEntry(GetPath(tip), _administrator, adminPassword))
            {
                using (var ds = new DirectorySearcher(mainde, $"(&(objectClass=user)(objectCategory=person)({_samaccountname}={searchUserName}))", Array.Empty<string>(), SearchScope.Subtree))
                {
                    try { return getdata(ds.FindOne(), tip, true, false, dil); }
                    catch (Exception ex) { return set_exception(ex); }
                }
            }
        }
        /// <summary>
        /// Bir kullanıcı şifresi güncellenirken LDAP sisteminde gerekli yetkilendirmeler yapılır.
        /// Test ortamında işlem yapılamaz. Gerçek ortamda şifre belirleme işlemi yapılır.
        /// </summary>
        /// <param name="isDebug">Eğer debug modundaysa, işlem devam etmez ve hata döner.</param>
        /// <param name="searchUsername">Şifresi değiştirilecek kullanıcının LDAP kullanıcı adı.</param>
        /// <param name="userPassword">Yeni şifre.</param>
        /// <param name="adminPassword">Admin kullanıcı için LDAP sistemine giriş şifresi.</param>
        /// <param name="logMethod">Loglama metodu.</param>
        /// <param name="tip">LDAP sunucusunun tipi.</param>
        /// <param name="dil">İşlem sırasında kullanılacak dil (örneğin: &quot;tr&quot; veya &quot;en&quot;).</param>
        /// <returns>İşlem sonucunda boolean tipinde işlem durumu ve varsa <see cref="Exception"/> döner.</returns>
        public static (bool statuswarning, Exception ex) SetPassword(bool isDebug, string searchUsername, string userPassword, string adminPassword, string logMethod, LDAPTip tip, string dil = "tr")
        {
            Guard.UnSupportLanguage(dil, nameof(dil));
            if (isDebug)
            {
                if (dil == "tr") { return (true, new Exception("TEST ortamında işleme devam edilemez!")); }
                return (true, new Exception("The process cannot be continued in the TEST environment!"));
            }
            userPassword = userPassword.ToStringOrEmpty();
            Guard.CheckEmpty(userPassword, nameof(userPassword));
            return SetProperties(searchUsername, new Dictionary<string, object> { { "SetPassword", userPassword } }, adminPassword, logMethod, tip, dil);
        }
        /// <summary>
        /// Kullanıcının özelliklerini günceller.
        /// </summary>
        /// <param name="searchUsername">Güncellenecek kullanıcının arama adı.</param>
        /// <param name="properties">Güncellenmesi gereken özelliklerin anahtar-değer çiftleri.</param>
        /// <param name="adminPassword">Yönetici kullanıcı adı için şifre.</param>
        /// <param name="logMethod">Loglama metodu.</param>
        /// <param name="tip">LDAP tipi.</param>
        /// <param name="dil">Hata mesajlarının döndürülmesi için kullanılan dil (varsayılan Türkçe, &quot;tr&quot; Türkçe için, &quot;en&quot; İngilizce için).</param>
        public static (bool statuswarning, Exception ex) SetProperties(string searchUsername, Dictionary<string, object> properties, string adminPassword, string logMethod, LDAPTip tip, string dil = "tr")
        {
            searchUsername = usernamedilkontrol(searchUsername, tip, nameof(searchUsername), dil);
            Guard.CheckEmpty(properties, nameof(properties));
            Guard.CheckEmpty(adminPassword, nameof(adminPassword));
            using (var mainde = new DirectoryEntry(GetPath(tip), _administrator, adminPassword))
            {
                using (var ds = new DirectorySearcher(mainde, $"(&(objectClass=user)(objectCategory=person)({_samaccountname}={searchUsername}))", Array.Empty<string>(), SearchScope.Subtree))
                {
                    try
                    {
                        var sr = ds.FindOne();
                        var _t = adaccountvalidation(tip, sr, true, false, dil);
                        if (_t.statuswarning) { return (true, _t.ex); }
                        using (var de = sr.GetDirectoryEntry())
                        {
                            foreach (var item in properties)
                            {
                                if (item.Key == "SetPassword") { continue; }
                                else { de.Properties[item.Key].Value = item.Value; }
                            }
                            de.Properties["info"].Value = SetInfo(de.Properties["info"].Value, logMethod);
                            de.CommitChanges();
                            if (properties.TryGetValue("SetPassword", out object _setPassword))
                            {
                                de.Invoke("SetPassword", new object[] { _setPassword });
                                de.Properties["pwdLastSet"].Value = -1;
                                de.CommitChanges();
                            }
                            de.RefreshCache();
                            return (false, default);
                        }
                    }
                    catch (Exception ex) { return (true, ex); }
                }
            }
        }
        /// <summary>
        /// Yeni bir kullanıcı ekler.
        /// </summary>
        /// <param name="path">Kullanıcının eklenmesi gereken dizin yolu.</param>
        /// <param name="userName">Eklenecek kullanıcının kullanıcı adı.</param>
        /// <param name="cn">Kullanıcının tam adı.</param>
        /// <param name="password">Kullanıcı için belirlenen şifre.</param>
        /// <param name="properties">Kullanıcı ile ilişkili özellikler.</param>
        /// <param name="tip">LDAP tipi.</param>
        /// <param name="adminPassword">Yönetici kullanıcı adı için şifre.</param>
        /// <param name="logMethod">Loglama metodu.</param>
        /// <param name="userAccountControl">Kullanıcının hesap kontrol ayarları.</param>
        /// <param name="dil">Hata mesajlarının döndürülmesi için kullanılan dil (varsayılan Türkçe, &quot;tr&quot; Türkçe için, &quot;en&quot; İngilizce için).</param>
        public static (bool statuswarning, Exception ex, bool inserted) Insert(string path, string userName, string cn, string password, Dictionary<string, object> properties, LDAPTip tip, string adminPassword, string logMethod, ADUserAccountControl userAccountControl = ADUserAccountControl.NORMAL_ACCOUNT, string dil = "tr")
        {
            Guard.CheckEmpty(path, nameof(path));
            userName = usernamedilkontrol(userName, tip, nameof(userName), dil);
            Guard.CheckEmpty(cn, nameof(cn));
            Guard.CheckEmpty(password, nameof(password));
            Guard.CheckEmpty(properties, nameof(properties));
            Guard.CheckEmpty(adminPassword, nameof(adminPassword)); // userAccountControl için bir kontrol konulmamalıdır!
            try
            {
                var inserted = false;
                if (path[0] != '/') { path = $"/{path}"; }
                using (var mainde = new DirectoryEntry(String.Concat(GetPath(tip), path), _administrator, adminPassword))
                {
                    using (var ds = new DirectorySearcher(mainde, $"(&(objectClass=user)(objectCategory=person)({_samaccountname}={userName}))", Array.Empty<string>(), SearchScope.Subtree))
                    {
                        var sr = ds.FindOne();
                        if (sr == null)
                        {
                            var newDirEntry = mainde.Children.Add($"CN={cn.ToUpper()}", "User");
                            properties.Upsert(_samaccountname, userName);
                            foreach (var item in properties) { newDirEntry.Properties[item.Key].Value = item.Value; }
                            newDirEntry.Properties["info"].Value = SetInfo(null, logMethod);
                            newDirEntry.CommitChanges();
                            newDirEntry.Invoke("SetPassword", new object[] { password });
                            newDirEntry.Properties["userAccountControl"].Value = Convert.ToInt32(userAccountControl);
                            newDirEntry.CommitChanges();
                            mainde.CommitChanges();
                            inserted = true;
                        }
                    }
                }
                return (false, default, inserted);
            }
            catch (Exception ex)
            {
                if (ex is DirectoryServicesCOMException exp)
                {
                    if (dil == "tr") { return (true, new Exception($"Hata Kodu: \"{exp.ExtendedError.ToString()}\"", new Exception($"Hata Mesajı: \"{exp.ExtendedErrorMessage}\"", ex)), false); }
                    return (true, new Exception($"Error Code: \"{exp.ExtendedError.ToString()}\"", new Exception($"Error Message: \"{exp.ExtendedErrorMessage}\"", ex)), false);
                }
                else { return (true, ex, false); }
            }
        }
        /// <summary>
        /// Loglama bilgilerini JSON biçimine döndürür.
        /// </summary>
        /// <param name="value">Loglama için kullanılacak değer. Eğer null ise varsayılan değerler kullanılır.</param>
        /// <param name="logmethod">Loglama yöntemi bilgisi.</param>
        /// <returns>
        /// JSON biçimindeki loglama verilerini içeren bir string döner.
        /// </returns>
        public static string SetInfo(object value, string logmethod)
        {
            var p = new
            {
                m = logmethod.CoalesceOrDefault($"/{typeof(LDAPHelper).FullName}/{nameof(SetInfo)}"),
                d = DateTime.Now
            };
            if (value != null && _try.TryJson(Convert.ToString(value), JTokenType.Object, out JObject _jo) && _jo.Count > 0)
            {
                if (_jo.TryGetValue("logdata", out JToken _jt) && _jt.Type == JTokenType.Array)
                {
                    var jts = _jt.Select(x => new
                    {
                        m = x["m"],
                        d = x["d"]
                    }).Select(x => new
                    {
                        m = (x.m != null && x.m.Type == JTokenType.String) ? x.m.ToStringOrEmpty() : "",
                        d = (x.d != null && x.d.Type == JTokenType.Date) ? Convert.ToDateTime(x.d) : DateTime.MinValue
                    }).Where(x => x.m != "" && x.d.Ticks > 0).ToList();
                    jts.Add(p);
                    _jo["logdata"] = JToken.FromObject(jts.OrderByDescending(x => x.d).Take(5).Select(x => new
                    {
                        x.m,
                        x.d
                    }).ToArray());
                }
                else { _jo["logdata"] = JToken.FromObject(new object[] { p }); }
                return _jo.ToString(Formatting.None);
            }
            return _to.ToJSON(new
            {
                logdata = new object[] { p }
            });
        }
        #region Private
        private const string _administrator = "administrator";
        private const string _samaccountname = "sAMAccountName";
        private static LDAPHelper set_exception(Exception ex) => new LDAPHelper(true, default, "", ex, default);
        private static LDAPHelper getdata(SearchResult searchresult, LDAPTip tip, bool issearchall, bool isdescriptiontckncheck, string dil)
        {
            var _t = adaccountvalidation(tip, searchresult, issearchall, isdescriptiontckncheck, dil);
            if (_t.statuswarning) { return set_exception(_t.ex); }
            var dic = new Dictionary<string, object>();
            foreach (string item in searchresult.Properties.PropertyNames) { dic.Add(item, searchresult.Properties[item][0]); }
            return new LDAPHelper(false, _t.tckn, Convert.ToString(dic[_samaccountname.ToLower()]), default, dic.OrderBy(x => x.Key).ToDictionary());
        }
        private static (bool statuswarning, long tckn, Exception ex) adaccountvalidation(LDAPTip tip, SearchResult searchresult, bool issearchall, bool isdescriptiontckncheck, string dil)
        {
            long tckn = 0;
            if (searchresult == null) { return (true, tckn, searchresultnotfoundexception(dil)); }
            string message;
            var _sDesc = searchresult.Properties["description"];
            if (!Enum.TryParse(searchresult.Properties["useraccountcontrol"][0].ToStringOrEmpty(), out ADUserAccountControl _aduac)) { message = "USERACCOUNTCONTROL"; }
            else if (issearchall) { message = ""; } // Not: Pasif Hesapların yakalabilmesi için alttaki kontrol es geçilmeli
            else if (isdescriptiontckncheck && tip != LDAPTip.stu && !_try.TryTCKimlikNo(((_sDesc == null || _sDesc.Count == 0) ? "" : _sDesc[0].ToStringOrEmpty()), out tckn)) { message = "DESCRIPTION"; }
            else if (isdescriptiontckncheck && tip == LDAPTip.stu && !_try.TryTCKimlikNoOrSW98(((_sDesc == null || _sDesc.Count == 0) ? "" : _sDesc[0].ToStringOrEmpty()), out tckn)) { message = "DESCRIPTION"; }
            else if (_aduac.HasFlag(ADUserAccountControl.PASSWORD_EXPIRED)) { message = ADUserAccountControl.PASSWORD_EXPIRED.ToString("g"); }
            else if (_aduac.HasFlag(ADUserAccountControl.ACCOUNTDISABLE)) { message = ADUserAccountControl.ACCOUNTDISABLE.ToString("g"); }
            else
            {
                var dt = getaccountexpires(searchresult);
                if (dt.HasValue && dt.Value <= DateTime.Today) { message = $"ACCOUNTEXPIRES ({dt.Value.ToString(_date.ddMMyyyy)})"; }
                else { message = ""; }
            }
            if (message == "") { return (false, tckn, default(Exception)); }
            if (dil == "tr") { return (true, tckn, new Exception($"Hata Sebebi: \"LDAP, {message}\"", new Exception(_title.iletisim_bilgiislem))); }
            return (true, tckn, new Exception($"Error Cause: \"LDAP, {message}\"", new Exception("Please contact the Department of Information Technologies")));
        }
        private static DateTime? getaccountexpires(SearchResult searchresult)
        {
            var _ae = "accountExpires"; // 01.01.1601 02:00:00(DateTime.FromFileTime Başlangıç tarihi)
            var _p = searchresult.Properties;
            if (_p != null && _p.Contains(_ae) && _p[_ae].Count > 0 && Int64.TryParse(_p[_ae][0].ToString(), out long _v) && _v > 0)
            {
                try { return DateTime.FromFileTime(_v); }
                catch { return null; }
            }
            return null;
        }
        private static Exception searchresultnotfoundexception(string dil)
        {
            if (dil == "tr") { return new Exception($"e-Posta kaydı bulunamadı!", new Exception($"Hata Sebebi: \"LDAP, {_samaccountname}, {typeof(SearchResult).FullName}\"", new Exception(_title.iletisim_bilgiislem))); }
            return new Exception("e-Mail record not found!", new Exception($"Error Reason: \"LDAP, {_samaccountname}, {typeof(SearchResult).FullName}\"", new Exception("Please contact the Department of Information Technologies")));
        }
        private static string usernamedilkontrol(string username, LDAPTip tip, string usernameargname, string dil)
        {
            Guard.CheckEmpty(username, usernameargname);
            Guard.CheckEnumDefined<LDAPTip>(tip, nameof(tip));
            Guard.UnSupportLanguage(dil, nameof(dil));
            username = username.ToLower();
            if (tip == LDAPTip.stu && username[0] != 'o') { return $"o{username}"; }
            return username;
        }
        #endregion
    }
}