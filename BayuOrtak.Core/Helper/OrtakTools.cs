namespace BayuOrtak.Core.Helper
{
    using BayuOrtak.Core.Extensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Data.SqlClient;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Drawing;
    using System.Dynamic;
    using System.Globalization;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net.Http.Headers;
    using System.Net.Mail;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Transactions;
    using System.Web;
    using System.Xml.Serialization;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    public sealed class OrtakTools
    {
        public sealed class _file
        {
            /// <summary>
            /// Verilen fiziksel dosya yolunda bir dosya varsa onu siler.
            /// </summary>
            /// <param name="physicallyPath">Silinecek dosyanın fiziksel yolu.</param>
            public static void FileExiststhenDelete(string physicallyPath) { if (File.Exists(physicallyPath)) { File.Delete(physicallyPath); } }
            /// <summary>
            /// Verilen dosya yolunda bir dosya varsa belirtilen hedefe kopyalar.
            /// Eğer hedefte dosya mevcutsa ve overwrite parametresi true ise, mevcut dosyanın üzerine yazar.
            /// </summary>
            /// <param name="physicallyPath">Kopyalanacak dosyanın fiziksel yolu.</param>
            /// <param name="targetPhysicallyPath">Hedef dosya yolu.</param>
            /// <param name="overwrite">Hedefteki dosyanın üzerine yazılıp yazılmayacağını belirten parametre.</param>
            /// <exception cref="Exception">Eğer hedefte dosya varsa ve overwrite false ise bir istisna fırlatılır.</exception>
            /// <exception cref="FileNotFoundException">Verilen dosya yolunda dosya bulunmazsa istisna fırlatılır.</exception>
            public static void FileExiststhenCopy(string physicallyPath, string targetPhysicallyPath, bool overwrite)
            {
                if (!File.Exists(physicallyPath)) { throw new FileNotFoundException($"\"{physicallyPath}\" yolunda belge bulunmadı. Kontrol ediniz!"); }
                var fi = new FileInfo(targetPhysicallyPath);
                DirectoryCreate(fi.DirectoryName);
                if (fi.Exists && !overwrite) { throw new Exception($"\"{fi.DirectoryName}\" klasöründe \"{fi.Name.Split('.')[0]}\" adında \"{fi.Name.Split('.')[1]}\" uzantılı belge bulunmaktadır. Kontrol ediniz!"); }
                File.Copy(physicallyPath, targetPhysicallyPath, overwrite);
            }
            /// <summary>
            /// Verilen dosya yolunda bir dosya varsa hedef konuma taşır.
            /// Hedefte aynı isimde bir dosya mevcutsa işlem gerçekleştirilmez ve bir istisna fırlatılır.
            /// </summary>
            /// <param name="physicallyPath">Taşınacak dosyanın fiziksel yolu.</param>
            /// <param name="targetPhysicallyPath">Dosyanın taşınacağı hedef yol.</param>
            /// <exception cref="Exception">Hedef konumda aynı isimde bir dosya varsa istisna fırlatılır.</exception>
            /// <exception cref="FileNotFoundException">Verilen dosya yolunda dosya bulunmazsa istisna fırlatılır.</exception>
            public static void FileExiststhenMove(string physicallyPath, string targetPhysicallyPath)
            {
                if (!File.Exists(physicallyPath)) { throw new FileNotFoundException($"\"{physicallyPath}\" yolunda belge bulunmadı. Kontrol ediniz!"); }
                var fi = new FileInfo(targetPhysicallyPath);
                DirectoryCreate(fi.DirectoryName);
                if (fi.Exists) { throw new Exception($"\"{fi.DirectoryName}\" klasöründe \"{fi.Name.Split('.')[0]}\" adında \"{fi.Name.Split('.')[1]}\" uzantılı belge bulunmaktadır. Kontrol ediniz!"); }
                File.Move(physicallyPath, targetPhysicallyPath);
            }
            /// <summary>
            /// Verilen klasör yolunda bir klasör varsa, isteğe bağlı olarak içindekilerle birlikte siler.
            /// </summary>
            /// <param name="physicallyPath">Silinecek klasörün fiziksel yolu.</param>
            /// <param name="recursive">Eğer <see langword="true"/> verilirse, dizin ve altındaki tüm dosyalar ve alt dizinler silinir. <see langword="false"/> verilirse, dizin yalnızca boşsa silinir; aksi halde bir <see cref="IOException"/> fırlatılır.</param>
            public static void DirectoryExiststhenDelete(string physicallyPath, bool recursive) { if (Directory.Exists(physicallyPath)) { Directory.Delete(physicallyPath, recursive); } }
            /// <summary>
            /// Verilen klasör yolunda bir klasör varsa hedef konuma kopyalar.
            /// </summary>
            /// <param name="physicallyPath">Kopyalanacak klasörün fiziksel yolu.</param>
            /// <param name="targetPhysicallyPath">Klasörün kopyalanacağı hedef yol.</param>
            /// <exception cref="DirectoryNotFoundException">Verilen klasör yolunda klasör bulunmazsa istisna fırlatılır.</exception>
            public static void DirectoryExiststhenCopy(string physicallyPath, string targetPhysicallyPath)
            {
                if (Directory.Exists(physicallyPath)) { new DirectoryInfo(physicallyPath).CopyAll(new DirectoryInfo(targetPhysicallyPath)); }
                throw new DirectoryNotFoundException($"\"{physicallyPath}\" yolunda klasör bulunmadı! Kontrol ediniz.");
            }
            /// <summary>
            /// Verilen klasör yolunda bir klasör varsa hedef konuma taşır.
            /// </summary>
            /// <param name="physicallyPath">Taşınacak klasörün fiziksel yolu.</param>
            /// <param name="targetPhysicallyPath">Klasörün taşınacağı hedef yol.</param>
            /// <exception cref="DirectoryNotFoundException">Verilen klasör yolunda klasör bulunmazsa istisna fırlatılır.</exception>
            public static void DirectoryExiststhenMove(string physicallyPath, string targetPhysicallyPath)
            {
                if (!Directory.Exists(physicallyPath)) { throw new DirectoryNotFoundException($"\"{physicallyPath}\" yolunda klasör bulunmadı! Kontrol ediniz!"); }
                var di = new DirectoryInfo(targetPhysicallyPath);
                Guard.CheckNull(di, nameof(di));
                Guard.CheckNull(di.Parent, nameof(di.Parent));
                DirectoryCreate(di.Parent.FullName);
                Directory.Move(physicallyPath, targetPhysicallyPath);
            }
            /// <summary>
            /// Verilen fiziksel dosya yolunda klasör mevcut değilse, ilgili klasörü ve varsa üst dizinlerini oluşturur.
            /// </summary>
            /// <param name="physicallyPath">Oluşturulacak klasörün fiziksel yolu.</param>
            public static void DirectoryCreate(string physicallyPath)
            {
                var di = new DirectoryInfo(physicallyPath);
                if (di.Parent != null) { DirectoryCreate(di.Parent.FullName); }
                if (!di.Exists) { di.Create(); }
            }
        }
        public sealed class _get
        {
            /// <summary>
            /// Verilen bir JSON Web Token (JWT) için belirtilen bir özelliğin son kullanma tarihini alır.
            /// <code>DateTimeOffset.FromUnixTimeSeconds(_to.ToJObjectFromToken(token)[&quot;exp&quot;].Value&lt;long&gt;()).UtcDateTime;</code>
            /// </summary>
            /// <param name="token">Son kullanma tarihi alınacak JWT.</param>
            /// <returns>JWT&#39;nin Unix zaman damgasına göre belirlenen UTC tarih ve saat.</returns>
            /// <exception cref="ArgumentNullException">Verilen token null ise.</exception>
            /// <exception cref="FormatException">Token geçerli bir JWT formatında değilse.</exception>
            /// <exception cref="KeyNotFoundException">Token içindeki belirtilen anahtar mevcut değilse.</exception>
            public static DateTime GetExpValueFromToken(string token) => DateTimeOffset.FromUnixTimeSeconds(_to.ToJObjectFromToken(token)["exp"].Value<long>()).UtcDateTime;
            /// <summary>
            /// Verilen sınıfın belirtilen özelliğindeki maksimum karakter uzunluğunu döner.
            /// Eğer <see cref="StringLengthAttribute"/> veya <see cref="MaxLengthAttribute"/> gibi uzunluk sınırlayıcı öznitelikler atanmışsa, bu değeri alır.
            /// Aksi takdirde 0 döner.
            /// </summary>
            /// <typeparam name="T">Kontrol edilecek sınıf türü.</typeparam>
            /// <param name="name">Kontrol edilecek özelliğin adı.</param>
            /// <returns>Özellik için maksimum uzunluk değeri; öznitelik bulunmazsa 0 döner.</returns>
            public static int GetStringOrMaxLength<T>(string name) where T : class
            {
                var _n = typeof(T).GetProperty(name);
                if (_n != null)
                {
                    if (_try.TryCustomAttribute(_n, out StringLengthAttribute _s)) { return _s.MaximumLength; }
                    if (_try.TryCustomAttribute(_n, out MaxLengthAttribute _m)) { return _m.Length; }
                }
                return 0;
            }
            /// <summary>
            /// Verilen sınıfın belirli bir string ifadesi için maksimum karakter uzunluğunu döner.
            /// <see cref="StringLengthAttribute"/> veya <see cref="MaxLengthAttribute"/> atanmışsa bu değeri alır, aksi takdirde 0 döner.
            /// </summary>
            /// <typeparam name="T">Kontrol edilecek sınıf türü.</typeparam>
            /// <param name="expression">Özellik ismini içeren ifade.</param>
            /// <returns>Özellik için maksimum uzunluk değeri; öznitelik bulunmazsa 0 döner.</returns>
            public static int GetStringOrMaxLength<T>(Expression<Func<T, string>> expression) where T : class => GetStringOrMaxLength<T>(expression.GetExpressionName());
            /// <summary>
            /// Verilen string dizisinden kısaltma oluşturur.
            /// Her bir kelimenin baş harfini alarak noktalarla ayrılmış bir kısaltma döner.
            /// Boş veya null değerler atlanır.
            /// </summary>
            /// <param name="names">Kısaltma için kullanılacak isim dizisi.</param>
            /// <returns>Verilen isimlerin baş harflerinden oluşan kısaltma. Eğer parametreler boş veya geçersizse boş string döner.</returns>
            public static string GetNameShort(params string[] names)
            {
                names = (names ?? Array.Empty<string>()).Select(x => x.ToStringOrEmpty()).Where(x => x != "").ToArray();
                return names.Length > 0 ? String.Join(".", names.Select(x => String.Join("", x.ToUpper().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x[0])))) : "";
            }
            /// <summary>
            /// Belirtilen dil koduna göre CultureInfo nesnesini döner.
            /// Dil kodu &quot;tr&quot; veya &quot;en&quot; olabilir. Diğer diller desteklenmemektedir.
            /// </summary>
            /// <param name="dil">Dil kodu (örnek: &quot;tr&quot;, &quot;en&quot;)</param>
            /// <returns>CultureInfo nesnesi</returns>
            /// <exception cref="NotSupportedException">Eğer dil desteklenmiyorsa hata fırlatılır</exception>
            public static CultureInfo GetCultureInfo(string dil)
            {
                Guard.UnSupportLanguage(dil, nameof(dil));
                if (dil == "tr") { return new CultureInfo("tr-TR"); }
                return new CultureInfo("en-US");
            }
            /// <summary>
            /// Verilen nesneden &quot;isldate&quot; ve &quot;isluser&quot; bilgilerini çıkarır ve bu bilgileri belirtilen tarih formatında birleştirerek string olarak döndürür.
            /// Nesne tipi IFormCollection, JToken veya bir enumerable koleksiyon olabilir.
            /// Eğer nesne null ise, &quot;isldate&quot; geçerli bir tarih değilse veya &quot;isluser&quot; boşsa, boş string döndürür.
            /// </summary>
            /// <param name="model">Bilgilerin alınacağı nesne.</param>
            /// <param name="dateformat">Tarih biçimi.</param>
            /// <returns>&quot;isldate&quot; ve &quot;isluser&quot; bilgilerinin birleştirilmiş string hali; geçerli veri yoksa boş string.</returns>
            public static string GetIslemInfoFromObject(object model, string dateformat = _date.ddMMyyyy_HHmmss)
            {
                if (model == null) { return ""; }
                if (model is IFormCollection _form)
                {
                    return GetIslemInfoFromObject(new
                    {
                        isldate = _form.ToKeyValueParseOrDefault_formcollection<DateTime>("isldate"),
                        isluser = _form.ToKeyValueParseOrDefault_formcollection<string>("isluser") ?? ""
                    }, dateformat);
                }
                if (model is JToken _j)
                {
                    return GetIslemInfoFromObject(new
                    {
                        isldate = _j["isldate"].ToEnumerable().Select(x => (x != null && x.Type == JTokenType.Date) ? Convert.ToDateTime(x) : DateTime.MinValue).FirstOrDefault(),
                        isluser = _j["isluser"].ToEnumerable().Select(x => (x != null && x.Type == JTokenType.String) ? Convert.ToString(x) : "").FirstOrDefault()
                    }, dateformat);
                }
                return model.ToEnumerable().Select(x => x.ToDynamic()).Select(x => new
                {
                    isldate = (DateTime)x.isldate,
                    isluser = (string)x.isluser
                }).Select(x => String.Join(", ", new string[] { (x.isldate.Ticks > 0 ? x.isldate.ToString(dateformat) : ""), x.isluser.ToStringOrEmpty() }.Where(y => y != "").ToArray())).FirstOrDefault() ?? "";
            }
        }
        public sealed class _other
        {
            /// <summary>
            /// Verilen string&#39;in HTML tag&#39;leri içerip içermediğini kontrol eder. String null, boş veya yalnızca boşluklardan oluşuyorsa <see langword="false"/> döner. HTML tag&#39;leri, düzenli ifade (regex) kullanılarak tespit edilir.
            /// <code>(!s.IsNullOrEmpty_string() &amp;&amp; Regex.IsMatch(s, @&quot;&lt;/?\w+\s*[^&gt;]*&gt;&quot;, RegexOptions.Compiled))</code>
            /// </summary>
            /// <param name="s">Kontrol edilecek string.</param>
            /// <returns>String HTML tag&#39;i içeriyorsa <see langword="true"/>, aksi takdirde <see langword="false"/> döner.</returns>
            public static bool IsHtml(string s) => (!s.IsNullOrEmpty_string() && Regex.IsMatch(s, @"</?\w+\s*[^>]*>", RegexOptions.Compiled));
            /// <summary>
            /// Belirtilen dosyanın tarayıcı tarafından indirilebilir olup olmadığını kontrol eder.
            /// PDF veya görüntü dosyaları (image/*) indirme işlemi için uygun değilse <see langword="false"/> döner.
            /// Aksi takdirde <see langword="true"/> döner.
            /// </summary>
            /// <param name="path">Kontrol edilecek dosyanın yolu.</param>
            /// <returns>Dosya indirilebilir ise <see langword="true"/>, değilse <see langword="false"/>.</returns>
            public static bool IsDownloadableFile(string path)
            {
                if (path.IsNullOrEmpty_string()) { return false; }
                try
                {
                    var _uzn = Path.GetExtension(path).ToLower();
                    if (_uzn == ".pdf") { return false; }
                    if (imageextensions.Contains(_uzn)) { return false; }
                    return true;
                }
                catch { return false; }
            }
            /// <summary>
            /// Asenkron işlemler için TransactionScope oluşturur.
            /// TransactionScope, işlem bütünlüğünü sağlamak için kullanılır. Bu metod, asenkron işlemlerin
            /// TransactionScope ile birlikte kullanılabilmesi için ayarlanmıştır.
            /// </summary>
            public static TransactionScope TransactionScopeForAsync => new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
            /// <summary>
            /// Bir değeri belirtilen türe dönüştürür. Eğer değer null ise ve tip nullable ise null döner.
            /// Enum türlerini destekler ve enum değerlerini ilgili türe dönüştürür.
            /// </summary>
            /// <param name="value">Dönüştürülecek değer</param>
            /// <param name="type">Dönüştürülecek hedef tür</param>
            /// <returns>Dönüştürülmüş değer</returns>
            public static object ChangeType(object value, Type type)
            {
                var _t = _try.TryTypeIsNullable(type, out Type _bt);
                return (_t && value == null) ? null : (_bt.IsEnum ? Enum.ToObject(_bt, value) : Convert.ChangeType(value, _t ? Nullable.GetUnderlyingType(type) : _bt));
            }
            /// <summary>
            /// Belirtilen bir işlemi, başarısız olursa belirli bir süre bekleyerek ve belirli sayıda deneme ile çalıştırır.
            /// İşlem başarısız olduğunda onfailed aksiyonu çağrılır.
            /// </summary>
            /// <param name="action">Çalıştırılacak işlem</param>
            /// <param name="attempts">Maksimum deneme sayısı</param>
            /// <param name="wait">Başarısız denemeler arasında bekleme süresi (opsiyonel)</param>
            /// <param name="onfailed">Başarısızlık durumunda çağrılacak aksiyon (opsiyonel)</param>
            public static void Run(Action action, int attempts, TimeSpan? wait, Action<int, Exception> onfailed = null)
            {
                Guard.CheckNull(action, nameof(action));
                Run(new Func<bool>(() =>
                {
                    action();
                    return true;
                }), attempts, wait, onfailed);
            }
            /// <summary>
            /// Belirtilen bir işlemi, başarısız olursa belirli bir süre bekleyerek ve belirli sayıda deneme ile çalıştırır.
            /// İşlem başarısız olduğunda onfailed aksiyonu çağrılır ve işlem geri döner.
            /// </summary>
            /// <typeparam name="T">Döndürülecek değerin türü</typeparam>
            /// <param name="func">Çalıştırılacak işlev</param>
            /// <param name="attempts">Maksimum deneme sayısı</param>
            /// <param name="wait">Başarısız denemeler arasında bekleme süresi (opsiyonel)</param>
            /// <param name="onfailed">Başarısızlık durumunda çağrılacak aksiyon (opsiyonel)</param>
            /// <returns>İşlem sonucu başarılı olursa döndürülen değer</returns>
            public static T Run<T>(Func<T> func, int attempts, TimeSpan? wait, Action<int, Exception> onfailed = null)
            {
                Guard.CheckNull(func, nameof(func));
                if (attempts < 1) { throw new ArgumentOutOfRangeException(nameof(attempts), attempts, "Maksimum deneme sayısı 1'den az olmamalıdır"); }
                var attempt = 0;
                while (true)
                {
                    if (attempt > 0 && wait.HasValue) { Thread.Sleep(wait.Value); }
                    try { return func(); }
                    catch (Exception ex)
                    {
                        attempt++;
                        if (onfailed != null) { onfailed(attempt, ex); }
                        if (attempt >= attempts) { throw; }
                    }
                }
            }
            /// <summary>
            /// Verilen metni Sezar şifreleme algoritması ile şifreler. Belirtilen anahtar (key) değeri kadar
            /// harfler kaydırılarak şifreleme yapılır.
            /// </summary>
            /// <param name="value">Şifrelenecek metin</param>
            /// <param name="key">Harflerin kaydırılacağı değer</param>
            /// <returns>Şifrelenmiş metin</returns>
            public static string CaesarCipherOperation(string value, int key)
            {
                if (key < 0) { return CaesarCipherOperation(value, key + 26); }
                var r = "";
                foreach (var itemChar in value.ToStringOrEmpty().ToCharArray())
                {
                    if ((itemChar >= 'A' && itemChar <= 'Z')) { r = String.Concat(r, Convert.ToChar(((itemChar - 'A' + key) % 26) + 'A').ToString()); }
                    else if ((itemChar >= 'a' && itemChar <= 'z')) { r = String.Concat(r, Convert.ToChar(((itemChar - 'a' + key) % 26) + 'a').ToString()); }
                    else { r = String.Concat(r, itemChar.ToString()); }
                }
                return r;
            }
            /// <summary>
            /// Bir nesnenin belirtilen özelliğine veri atar.
            /// Eğer özellik bulunamazsa veya nesne bir sınıf değilse hata fırlatılır.
            /// </summary>
            /// <param name="value">Özelliği atanacak nesne</param>
            /// <param name="propertyName">Atanacak özelliğin adı</param>
            /// <param name="data">Atanacak veri</param>
            public static void SetPropertyValue(object value, string propertyName, object data)
            {
                Guard.CheckNull(value, nameof(value));
                Guard.CheckEmpty(propertyName, nameof(propertyName));
                var type = value.GetType();
                if (!type.IsCustomClass()) { throw new ArgumentException($"\"{value}\" argümanı türü class olmalıdır!", nameof(value)); }
                var pi = type.GetProperty(propertyName);
                Guard.CheckNull(pi, nameof(pi));
                pi.SetValue(value, data == null ? null : ChangeType(data, pi.PropertyType));
            }
            /// <summary>
            /// Enum türleri için desteklenmeyen değer hatası oluşturur.
            /// Belirtilen Enum türü ve ek detaylarla birlikte bir hata mesajı üretir.
            /// </summary>
            /// <typeparam name="T">Enum türü (generic).</typeparam>
            /// <param name="details">Hata mesajına eklenecek isteğe bağlı ek detaylar.</param>
            /// <returns>Desteklenmeyen Enum değerine ait NotSupportedException nesnesi döner.</returns>
            public static NotSupportedException ThrowNotSupportedForEnum<T>(params string[] details) where T : Enum
            {
                var r = new HashSet<string> { typeof(T).FullName, $"{nameof(Enum)} değeri uyumsuzdur! Kontrol ediniz." };
                details = (details ?? Array.Empty<string>()).Where(x => !x.IsNullOrEmpty_string()).ToArray();
                if (details.Length > 0) { r.AddRangeOptimized(details); }
                return new NotSupportedException(String.Join(" ", r));
            }
        }
        public sealed class _to
        {
            /// <summary>
            /// Verilen nesneyi JSON formatına dönüştürür.
            /// JSON çıktısı None formatında ve bazı özel ayarlarla döner.
            /// </summary>
            /// <param name="value">JSON&#39;a dönüştürülecek nesne.</param>
            /// <returns>Nesnenin JSON string formatındaki temsili.</returns>
            public static string ToJSON(object value) => JsonConvert.SerializeObject(value, Formatting.None, jsonserializersettings);
            /// <summary>
            /// XML string&#39;ini belirtilen türde bir nesneye dönüştürür.
            /// </summary>
            /// <typeparam name="T">XML&#39;den deserialize edilecek nesnenin türü.</typeparam>
            /// <param name="xml">Deserialize edilecek XML string.</param>
            /// <returns>Verilen XML string&#39;e karşılık gelen nesne. Eğer XML geçersizse null döner.</returns>
            public static T ToEntityFromXML<T>(string xml) where T : class
            {
                if (xml.IsNullOrEmpty_string()) { return default; }
                var xs = new XmlSerializer(typeof(T));
                using (var sr = new StringReader(xml)) { return (T)xs.Deserialize(sr); }
            }
            /// <summary>
            /// JWT token içindeki yük verilerinden bir JObject oluşturur.
            /// </summary>
            /// <param name="token">İşlenecek JWT token.</param>
            /// <returns>Token yük verilerini içeren JObject.</returns>
            public static JObject ToJObjectFromToken(string token) => JObject.FromObject(new JwtSecurityTokenHandler().ReadJwtToken(token).Payload);
            /// <summary>
            /// Verilen nesneyi SHA256 hash string formatına dönüştürür.
            /// Eğer değer null ise boş string döner.
            /// </summary>
            /// <param name="value">Hashlenecek nesne.</param>
            /// <returns>Nesnenin SHA256 hash string temsili.</returns>
            /// <remarks>Not: MSSQL&#39;deki karşılığı SELECT SUBSTRING([sys].[fn_varbintohexstr](HASHBYTES(&#39;SHA2_256&#39;, &#39;Lorem Ipsum&#39;)), 3, 64) AS HashValue</remarks>
            public static string ToHashSHA256FromObject(object value)
            {
                if (value == null) { return ""; } // SELECT SUBSTRING([sys].[fn_varbintohexstr](HASHBYTES('SHA2_256', 'Lorem Ipsum')), 3, 64)
                var r = new List<string>();
                foreach (var itemBinary in SHA256.HashData(Encoding.UTF8.GetBytes(value.GetType() == typeof(string) ? value.ToString() : ToJSON(value)))) { r.Add(itemBinary.ToString("x2")); }
                return String.Join("", r);
            }
            /// <summary>
            /// Belirtilen enum türündeki değerleri ve karşılık gelen long değerlerini içeren bir sözlük oluşturur.
            /// </summary>
            /// <typeparam name="T">Enum türü.</typeparam>
            /// <returns>Enum isimlerini ve long karşılıklarını içeren sözlük.</returns>
            public static Dictionary<string, long> ToDictionaryFromEnum<T>() where T : Enum
            {
                var t = typeof(T);
                return ((T[])Enum.GetValues(t)).Select(x => Convert.ToInt64(_other.ChangeType(x, typeof(long)))).ToDictionary(x => Enum.GetName(t, x));
            }
            /// <summary>
            /// Verilen nesneyi, özellik isimlerini ve değerlerini içeren bir sözlüğe dönüştürür.
            /// Yalnızca özel sınıf türlerinde çalışır.
            /// </summary>
            /// <param name="obj">Dönüştürülecek nesne.</param>
            /// <returns>Nesnenin özellik isimlerini ve değerlerini içeren sözlük.</returns>
            public static Dictionary<string, object> ToDictionaryFromObject(object obj)
            {
                if (obj == null) { return new Dictionary<string, object>(); }
                var t = obj.GetType();
                if (t.IsCustomClass()) { return t.GetProperties().ToDictionary(x => x.Name, x => x.GetValue(obj)); }
                throw new Exception($"{nameof(obj)} türü uygun biçimde değildir!");
            }
            /// <summary>
            /// Verilen nesneyi SQL parametrelerine dönüştürür.
            /// Eğer nesne <see cref="SqlParameter"/> türünde ise doğrudan SQL parametreleri olarak döner.
            /// Özel sınıf türlerinde çalışır ve özellik isimlerine göre SQL parametrelerini oluşturur.
            /// <para>obj için tanımlanan nesneler: SqlParameter, IEnumerable&lt;SqlParameter&gt;, IDictionary&lt;string, object&gt;, AnonymousObjectClass</para>
            /// </summary>
            /// <param name="obj">Dönüştürülecek nesne.</param>
            /// <returns>Nesneyi temsil eden SQL parametrelerinin dizisi.</returns>
            public static SqlParameter[] ToSqlParameterFromObject(object obj)
            {
                if (obj == null) { return Array.Empty<SqlParameter>(); }
                if (obj is SqlParameter _sp) { return new SqlParameter[] { _sp }; }
                if (obj is IEnumerable<SqlParameter> _sps) { return _sps.ToArray(); }
                return (obj is IDictionary<string, object> _dic ? _dic : ToDictionaryFromObject(obj)).Select(x => new SqlParameter
                {
                    ParameterName = x.Key,
                    Value = x.Value ?? DBNull.Value
                }).ToArray();
            }
            /// <summary>
            /// Verilen nesneyi DateOnly tipine dönüştürür.
            /// <para>obj için tanımlanan nesneler: DateOnly, DateTime, Int64, String(DateOnly, DateTime, Int64 türlerine uygun biçimde olmalıdır)</para>
            /// </summary>
            /// <param name="obj">Dönüştürülecek nesne.</param>
            /// <returns>DateOnly değeri.</returns>
            public static DateOnly ToDateOnlyFromObject(object obj)
            {
                if (obj is DateOnly _do) { return _do; }
                if (obj is DateTime _dt) { return _dt.ToDateOnly(); }
                if (obj is Int64 _l) { return new DateTime(_l).ToDateOnly(); }
                if (obj is String _s)
                {
                    if (DateOnly.TryParse(_s, out _do)) { return _do; }
                    if (DateTime.TryParse(_s, out _dt)) { return _dt.ToDateOnly(); }
                    if (Int64.TryParse(_s, out _l)) { return new DateTime(_l).ToDateOnly(); }
                }
                return default;
            }
            /// <summary>
            /// Verilen nesneyi DateTime tipine dönüştürür ve isteğe bağlı bir zaman değeri ekler.
            /// <para>obj için tanımlanan nesneler: DateTime, DateOnly, Int64, String(DateTime, DateOnly, Int64 türlerine uygun biçimde olmalı)</para>
            /// </summary>
            /// <param name="obj">Dönüştürülecek nesne.</param>
            /// <param name="timeOnly">Zaman bilgisi (isteğe bağlı). <paramref name="obj"/> değeri türü DateOnly iken girilecek değer anlamlıdır</param>
            /// <returns>DateTime değeri.</returns>
            public static DateTime ToDateTimeFromObject(object obj, TimeOnly? timeOnly)
            {
                if (obj is DateTime _dt) { return _dt; }
                if (obj is DateOnly _do) { return _do.ToDateTime(timeOnly ?? default); }
                if (obj is Int64 _l) { return new DateTime(_l); }
                if (obj is String _s)
                {
                    if (DateTime.TryParse(_s, out _dt)) { return _dt; }
                    if (DateOnly.TryParse(_s, out _do)) { return _do.ToDateTime(timeOnly ?? default); }
                    if (Int64.TryParse(_s, out _l)) { return new DateTime(_l); }
                }
                return default;
            }
            /// <summary>
            /// Verilen string değeri ve hedef tipini kullanarak, değeri uygun tipe dönüştürmeye çalışır.
            /// Dönüştürülemezse varsayılan bir değer döner.
            /// Bu metot özellikle nullable tipler, enumlar ve temel tiplerle çalışmak için tasarlanmıştır.
            /// </summary>
            /// <param name="value">Dönüştürülecek string değeri.</param>
            /// <param name="propertyType">Hedef tip. Bu tip nullable olabilir.</param>
            public static (object value, Type genericbasetype) ParseOrDefault_valuetuple(string value, Type propertyType)
            {
                try
                {
                    value = value.ToStringOrEmpty();
                    if (value == "") { return (default, default); }
                    _ = _try.TryTypeIsNullable(propertyType, out Type genericBaseType);
                    if (genericBaseType.IsEnum) { return (((Enum.TryParse(genericBaseType, value, out object _enum) && Enum.IsDefined(genericBaseType, _enum)) ? _enum : null), genericBaseType); }
                    if (genericBaseType == typeof(bool)) { return ((Boolean.TryParse(Int32.TryParse(value, out int _vint) ? (_vint == 0 ? Boolean.FalseString : Boolean.TrueString) : value, out bool _vbool) ? _vbool : null), genericBaseType); }
                    if (value.IndexOf('.') > -1 && genericBaseType.Includes(typeof(float), typeof(double), typeof(decimal))) { value = value.Replace(".", ",", StringComparison.InvariantCulture); }
                    return (TypeDescriptor.GetConverter(propertyType).ConvertFrom(value), genericBaseType);
                }
                catch { return (default, default); }
            }
            /// <summary>
            /// Belirtilen bir dosya yolundan <see cref="IFormFile"/> nesnesi oluşturur.
            /// </summary>
            /// <param name="filePath">Dosyanın tam yolu.</param>
            /// <param name="contentType">Dosyanın içerik türü (örneğin, &quot;application/pdf&quot;, &quot;image/jpeg&quot;)</param>
            /// <param name="name">Dosya form alanında kullanılacak ad. Varsayılan olarak &quot;file&quot; değeri atanır. </param>
            public static IFormFile ToIFormFileFromPath(string filePath, string contentType, string name = "file")
            {
                Guard.CheckEmpty(filePath, nameof(filePath));
                Guard.CheckEmpty(contentType, nameof(contentType));
                Guard.CheckEmpty(name, nameof(name));
                if (!File.Exists(filePath)) { throw new FileNotFoundException("Belirtilen dosya bulunamadı.", filePath); }
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    return new FormFile(fs, 0, fs.Length, name, Path.GetFileName(filePath))
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = contentType
                    };
                }
            }
            /// <summary>
            /// SQL Server&#39;ın sistem tür kimliğini <c>([system_type_id])</c> <see cref="SqlDbType"/> enum değerine dönüştürür.
            /// </summary>
            /// <param name="systemtypeid">SQL Server [sys].[types] tablosundaki [system_type_id] değeri.</param>
            /// <returns>Eşleşen <see cref="SqlDbType"/> enum değeri.</returns>
            /// <exception cref="ArgumentException">Geçersiz veya desteklenmeyen bir sistem tür kimliği verildiğinde fırlatılır.</exception>
            public static SqlDbType ToSqlDbTypeFromSystemTypeID(int systemtypeid)
            {
                switch (systemtypeid)
                {
                    case 34: return SqlDbType.Image;
                    case 35: return SqlDbType.Text;
                    case 36: return SqlDbType.UniqueIdentifier;
                    case 40: return SqlDbType.Date;
                    case 41: return SqlDbType.Time;
                    case 42: return SqlDbType.DateTime2;
                    case 43: return SqlDbType.DateTimeOffset;
                    case 48: return SqlDbType.TinyInt;
                    case 52: return SqlDbType.SmallInt;
                    case 56: return SqlDbType.Int;
                    case 58: return SqlDbType.SmallDateTime;
                    case 59: return SqlDbType.Real;
                    case 60: return SqlDbType.Money;
                    case 61: return SqlDbType.DateTime;
                    case 62: return SqlDbType.Float;
                    case 99: return SqlDbType.NText;
                    case 104: return SqlDbType.Bit;
                    case 106: return SqlDbType.Decimal;
                    case 122: return SqlDbType.SmallMoney;
                    case 127: return SqlDbType.BigInt;
                    case 165: return SqlDbType.VarBinary;
                    case 167: return SqlDbType.VarChar;
                    case 173: return SqlDbType.Binary;
                    case 175: return SqlDbType.Char;
                    case 189: return SqlDbType.Timestamp;
                    case 231: return SqlDbType.NVarChar;
                    case 239: return SqlDbType.NChar;
                    case 241: return SqlDbType.Xml;
                    default: throw new ArgumentException($"Geçersiz veya desteklenmeyen {nameof(systemtypeid)}: {systemtypeid}");
                }
            }
        }
        public sealed class _try
        {
            /// <summary>
            /// Verilen nesnenin doğrulama kurallarına göre geçerliliğini kontrol eder. Eğer nesne geçerli değilse, doğrulama hatalarını içeren bir dizi döner.
            /// </summary>
            /// <param name="instance">Doğrulama işlemi yapılacak nesne.</param>
            /// <param name="outvalue">Geçersiz olduğu tespit edilen durumlarda hata mesajlarını içeren dizi.</param>
            /// <returns>Doğrulama işlemi sonucunu belirtir; geçerli ise <see langword="true"/>, geçersiz ise <see langword="false"/> döner.</returns>
            public static bool TryWarningValidateObject(object instance, out string[] outvalue)
            {
                var _vrs = new List<ValidationResult>();
                Validator.TryValidateObject(instance, new ValidationContext(instance), _vrs, true); // Not: TryValidateObject kontrolünde instance içerisinde her hangi bir problem yoksa sonuç true gelmekte
                outvalue = _vrs.Select(x => x.ErrorMessage).ToArray();
                return _vrs.Count > 0;
            }
            /// <summary>
            /// Verilen JSON dizisini belirli bir JToken türüne (<see cref="JTokenType"/>) dönüştürmeye çalışır. Dönüşüm başarılı olursa, dönüştürülen değeri döner.
            /// </summary>
            /// <typeparam name="T">Hedef JToken türü.</typeparam>
            /// <param name="json">Dönüştürülmek istenen JSON dizisi.</param>
            /// <param name="jTokenType">Beklenen JToken türü.</param>
            /// <param name="outvalue">Başarılı dönüşümde dönen JToken değeri.</param>
            /// <returns>JSON dizisinin beklenen türe uygun olup olmadığını belirtir; uygun ise <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
            public static bool TryJson<T>(string json, JTokenType jTokenType, out T outvalue) where T : JToken
            {
                try
                {
                    var jt = JToken.Parse(json.ToStringOrEmpty());
                    var r = jt.Type == jTokenType;
                    outvalue = r ? (T)jt : default;
                    return r;
                }
                catch
                {
                    outvalue = default;
                    return false;
                }
            }
            /// <summary>
            /// Verilen e-Posta adresinin geçerli bir MailAddress nesnesine dönüştürülmesini sağlar. Eğer dönüşüm başarılı olursa, MailAddress nesnesini döner.
            /// </summary>
            /// <param name="address">Geçerliliği kontrol edilecek e-Posta adresi.</param>
            /// <param name="outvalue">Başarılı dönüşümde dönen MailAddress nesnesi.</param>
            /// <returns>e-Posta adresinin geçerli olup olmadığını belirtir; geçerli ise <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
            public static bool TryMailAddress(string address, out MailAddress outvalue)
            {
                try
                {
                    outvalue = new MailAddress(address.ToStringOrEmpty().ToLower());
                    return true;
                }
                catch
                {
                    outvalue = default;
                    return false;
                }
            }
            /// <summary>
            /// Verilen EBYS ID&#39;sinin geçerliliğini kontrol eder. Geçerli bir EBYS ID&#39;si, belirli bir format ve tarihlere göre doğrulanır.
            /// </summary>
            /// <param name="value">Doğrulanacak EBYS ID&#39;si.</param>
            /// <param name="outvalue">Geçerli olduğu tespit edilen EBYS ID&#39;si.</param>
            /// <returns>EBYS ID&#39;sinin geçerli olup olmadığını belirtir; geçerli ise <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
            public static bool TryEBYSID(string value, out string outvalue)
            {
                value = value.ToStringOrEmpty();
                var _v = (value.Length == _maximumlength.ebyscode ? value.Split('-') : Array.Empty<string>());
                if (_v.Length == 2 && DateTime.TryParse(_v[0], out DateTime _d) && _d > _date.bayburtUniversityFoundationDate && Int32.TryParse(_v[1], out int _n) && _n > 0)
                {
                    outvalue = value;
                    return true;
                }
                outvalue = "";
                return false;
            }
            /// <summary>
            /// Dinamik bir ExpandoObject içinde belirtilen anahtarın varlığını kontrol eder. Anahtar mevcutsa, ilgili değeri belirtilen türde döner.
            /// </summary>
            /// <typeparam name="T">Dönüştürülecek tür.</typeparam>
            /// <param name="value">Geçerli ExpandoObject nesnesi.</param>
            /// <param name="key">Kontrol edilecek anahtar.</param>
            /// <param name="outvalue">Anahtarın mevcut olduğu durumda dönecek değer.</param>
            /// <returns>Anahtarın mevcut olup olmadığını belirtir; mevcut ise <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
            public static bool TryMemberDynamic<T>(ExpandoObject value, string key, out T outvalue)
            {
                try
                {
                    if (value is IDictionary<string, object> _dic && !key.IsNullOrEmpty_string() && _dic.TryGetValue(key, out object _value) && _value is T _result)
                    {
                        outvalue = _result;
                        return true;
                    }
                    outvalue = default;
                    return false;
                }
                catch
                {
                    outvalue = default;
                    return false;
                }
            }
            /// <summary>
            /// Türkiye için verilen telefon numarasının geçerli biçimde olup olmadığını kontrol eder.
            /// <para>Geçerli örnekler:</para>
            /// <list type="bullet">
            ///     <item><description>5051234567</description></item>
            ///     <item><description>05051234567</description></item>
            ///     <item><description>905051234567</description></item>
            ///     <item><description>+905051234567</description></item>
            ///     <item><description>00905051234567</description></item>
            /// </list>
            /// </summary>
            /// <param name="phoneNumberTR">Kontrol edilecek telefon numarası.</param>
            /// <param name="outvalue">Geçerli olduğu tespit edilen telefon numarası.</param>
            /// <returns>Telefon numarasının geçerli biçimde olup olmadığını belirtir; geçerli ise <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
            public static bool TryPhoneNumberTR(string phoneNumberTR, out string outvalue)
            {
                phoneNumberTR = new String(phoneNumberTR.ToStringOrEmpty().Where(x => (x == '+' || Char.IsDigit(x))).ToArray());
                if (phoneNumberTR.Length == 14 && phoneNumberTR.StartsWith("0090")) { phoneNumberTR = phoneNumberTR.Substring(4); }
                else if (phoneNumberTR.Length == 13 && phoneNumberTR.StartsWith("+90")) { phoneNumberTR = phoneNumberTR.Substring(3); }
                else if (phoneNumberTR.Length == 12 && phoneNumberTR.StartsWith("90")) { phoneNumberTR = phoneNumberTR.Substring(2); }
                else if (phoneNumberTR.Length == 11 && phoneNumberTR[0] == '0') { phoneNumberTR = phoneNumberTR.Substring(1); }
                var r = phoneNumberTR.Length == 10 && Regex.IsMatch(phoneNumberTR, @"^\d+$");
                outvalue = r ? phoneNumberTR : "";
                return r;
            }
            /// <summary>
            /// Verilen T.C. Kimlik numarasının geçerliliğini kontrol eder. T.C. Kimlik numarası, belirli bir format ve kurallara göre doğrulanır.
            /// </summary>
            /// <param name="tckn">Doğrulanacak T.C. Kimlik numarası.</param>
            /// <param name="outvalue">Geçerli olduğu tespit edilen T.C. Kimlik numarası.</param>
            /// <returns>T.C. Kimlik numarasının geçerli olup olmadığını belirtir; geçerli ise <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
            public static bool TryTCKimlikNo(string tckn, out long outvalue)
            {
                var r = false;
                tckn = tckn.ToStringOrEmpty();
                if (tckn.Length == _maximumlength.tckn && Regex.IsMatch(tckn, @"^\d+$") && tckn[0] != '0')
                {
                    var t = tckn.ToCharArray().Select(x => Convert.ToInt32(Convert.ToString(x))).ToArray();
                    r = ((((t[0] + t[2] + t[4] + t[6] + t[8]) * 7) - (t[1] + t[3] + t[5] + t[7])) % 10) == t[9] && (t.Take(10).Sum() % 10) == t[10];
                }
                outvalue = (r ? Convert.ToInt64(tckn) : 0);
                return r;
            }
            /// <summary>
            /// Verilen kimlik numarasının geçerli olup olmadığını kontrol eder. Geçerli bir kimlik numarası şu kriterlere uygun olmalıdır: &quot;98&quot; ile başlamalı veya bir Türkiye Cumhuriyeti Kimlik Numarası (TCKN) biçiminde olmalıdır.
            /// <para>
            /// Yükseköğretim Kurulu (YÖK) tarafından, yabancı uyruklu öğrenciler için 
            /// oturumları oluşmadığı sürece, geçici olarak &quot;98&quot; ile başlayan ve 
            /// TCKN algoritmasıyla uyumsuz bir değer atanmaktadır.
            /// </para>
            /// </summary>
            /// <param name="tckn">Doğrulanacak TCKN değeri.</param>
            /// <param name="outvalue">Geçerli TCKN değeri, dönüş değeri olarak atanır.</param>
            /// <returns>Geçerli bir TCKN ise <see langword="true"/>, aksi takdirde <see langword="false"/> döner.</returns>
            public static bool TryTCKimlikNoOrSW98(string tckn, out long outvalue)
            {
                if (TryTCKimlikNo(tckn, out outvalue) || (Int64.TryParse(tckn, out outvalue) && outvalue > 0 && tckn.Length == _maximumlength.tckn && tckn.StartsWith("98"))) { return true; }
                outvalue = 0;
                return false;
            }
            /// <summary>
            /// T.C. Vergi Kimlik Numarası (VKN) doğrulaması yapan bir yöntemdir. VKN geçerli ise, dönen outValue parametresine VKN değeri atanır. VKN, belirtilen maksimum uzunluğa sahip olmalı ve sadece rakamlardan oluşmalıdır. Ayrıca kontrol için belirli matematiksel hesaplamalar yapılır.
            /// </summary>
            /// <param name="vkn">Doğrulanacak VKN değeri.</param>
            /// <param name="outvalue">Geçerli VKN değeri, dönüş değeri olarak atanır.</param>
            /// <returns>Geçerli bir VKN ise <see langword="true"/>, aksi takdirde <see langword="false"/> döner.</returns>
            public static bool TryVergiKimlikNo(string vkn, out long outvalue)
            {
                vkn = vkn.ToStringOrEmpty();
                outvalue = 0;
                if (vkn.Length < _maximumlength.vkn) { vkn = vkn.Replicate(_maximumlength.vkn, '0', 'l'); }
                if (vkn.Length == _maximumlength.vkn && Regex.IsMatch(vkn, @"^\d+$"))
                {
                    int i, t;
                    var numbers = new int[9];
                    for (i = 0; i < 9; i++)
                    {
                        t = (Convert.ToInt32(vkn[i].ToString()) + (9 - i)) % 10;
                        numbers[i] = (t * Convert.ToInt32(Math.Pow(2, (9 - i)))) % 9;
                        if (t != 0 && numbers[i] == 0) { numbers[i] = 9; }
                    }
                    if (((10 - ((numbers.Sum() % 10) % 10)) % 10) == Convert.ToInt32(vkn[9].ToString())) { outvalue = Convert.ToInt64(vkn); }
                }
                return outvalue > 0;
            }
            /// <summary>
            /// Verilen türün Nullable (null değeri alabilen) olup olmadığını kontrol eder. Eğer tür nullable ise, outvalue parametresine nullable olmayan tür atanır.
            /// </summary>
            /// <param name="type">Kontrol edilecek tür.</param>
            /// <param name="outvalue">Nullable olmayan tür, dönüş değeri olarak atanır.</param>
            /// <returns>Tür nullable ise <see langword="true"/>, aksi takdirde <see langword="false"/> döner.</returns>
            public static bool TryTypeIsNullable(Type type, out Type outvalue)
            {
                var _t = (type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
                outvalue = (_t ? type.GenericTypeArguments[0] : type);
                return _t;
            }
            /// <summary>
            /// Verilen adresin geçerli bir URI olup olmadığını kontrol eder. Geçerli bir URI ise, outvalue parametresine URI değeri atanır. URI&#39;nın HTTP veya HTTPS protokolüne sahip olması gerekmektedir.
            /// </summary>
            /// <param name="address">Doğrulanacak adres.</param>
            /// <param name="outvalue">Geçerli URI değeri, dönüş değeri olarak atanır.</param>
            /// <returns>Geçerli bir URI ise <see langword="true"/>, aksi takdirde <see langword="false"/> döner.</returns>
            public static bool TryUri(string address, out Uri outvalue)
            {
                try { outvalue = (Uri.TryCreate(address.ToStringOrEmpty(), UriKind.Absolute, out Uri _uri) && _uri.Scheme.Includes(Uri.UriSchemeHttp, Uri.UriSchemeHttps)) ? _uri : default; }
                catch { outvalue = default; }
                return outvalue != null;
            }
            /// <summary>
            /// Belirtilen türdeki özelliklerden, <see cref="KeyAttribute"/> (anahtar niteliği) ile işaretlenmiş olanları döner. Eğer en az bir anahtar niteliği içeren özellik bulunursa, outValue parametresine özellik isimleri atanır.
            /// </summary>
            /// <param name="type">Anahtar niteliği kontrol edilecek tür.</param>
            /// <param name="outvalue">Anahtar niteliğine sahip özellik isimleri dizisi, dönüş değeri olarak atanır.</param>
            /// <returns>En az bir anahtar niteliğine sahip özellik varsa <see langword="true"/>, aksi takdirde <see langword="false"/> döner.</returns>
            public static bool TryTableisKeyAttribute(Type type, out string[] outvalue)
            {
                try
                {
                    outvalue = type.GetProperties().Where(x => x.IsMappedProperty() && TryCustomAttribute(x, out KeyAttribute _)).Select(x => x.Name).ToArray();
                    return outvalue.Length > 0;
                }
                catch
                {
                    outvalue = default;
                    return false;
                }
            }
            /// <summary>
            /// Belirtilen öğe üzerinde verilen türde bir özel niteliğin (attribute) var olup olmadığını kontrol eder. Eğer nitelik bulunursa, outValue parametresine niteliğin değeri atanır.
            /// </summary>
            /// <typeparam name="T">Öğenin türü.</typeparam>
            /// <typeparam name="Y">Kontrol edilecek özel niteliğin türü.</typeparam>
            /// <param name="element">Özel niteliği kontrol edilecek öğe.</param>
            /// <param name="outvalue">Geçerli özel niteliğin değeri, dönüş değeri olarak atanır.</param>
            /// <returns>Geçerli bir özel nitelik varsa <see langword="true"/>, aksi takdirde <see langword="false"/> döner.</returns>
            public static bool TryCustomAttribute<T, Y>(T element, out Y outvalue) where T : ICustomAttributeProvider where Y : Attribute
            {
                try
                {
                    outvalue = element.GetCustomAttributes(typeof(Y), false).Cast<Y>().FirstOrDefault();
                    return outvalue != null;
                }
                catch
                {
                    outvalue = default;
                    return false;
                }
            }
            /// <summary>
            /// Verilen byte dizisinden bir resim (<see cref="Image"/>) oluşturmayı dener.
            /// Başarılı olursa, outValue parametresine oluşturulan resim atanır.
            /// Resim nesnesinin kullanımında dikkatli olunmalı ve gerektiğinde dispose edilmelidir.
            /// </summary>
            /// <param name="bytes">Resim verilerini içeren byte dizisi.</param>
            /// <param name="outvalue">Oluşturulan resim nesnesi, dönüş değeri olarak atanır.</param>
            /// <returns>Resim başarıyla oluşturulursa <see langword="true"/>, aksi takdirde <see langword="false"/> döner.</returns>
            /// <remarks>Not: Image kullanıldığı yerde Dispose edilmelidir!</remarks>
            public static bool TryImage(byte[] bytes, out Image outvalue)
            {
                try
                {
                    using (var ms = new MemoryStream(bytes))
                    {
                        outvalue = Image.FromStream(ms);
                        return true;
                    }
                }
                catch
                {
                    outvalue = default;
                    return false;
                }
            }
            /// <summary>
            /// Verilen değerin geçerli bir MAC adresi olup olmadığını kontrol eder. Eğer geçerliyse, outValue parametresine temizlenmiş MAC adresi atanır. MAC adresinin belirli bir biçimde olması gerekmektedir.
            /// </summary>
            /// <param name="value">Kontrol edilecek MAC adresi.</param>
            /// <param name="outvalue">Geçerli MAC adresi, dönüş değeri olarak atanır.</param>
            /// <returns>Geçerli bir MAC adresi varsa <see langword="true"/>, aksi takdirde <see langword="false"/> döner.</returns>
            public static bool TryMACAddress(string value, out string outvalue)
            {
                try
                {
                    value = value.ToStringOrEmpty().ToUpper();
                    if (value.Length == _maximumlength.mac && Regex.IsMatch(value, @"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$"))
                    {
                        outvalue = value.Replace("-", ":");
                        return true;
                    }
                    outvalue = "";
                    return false;
                }
                catch
                {
                    outvalue = "";
                    return false;
                }
            }
            /// <summary>
            /// Verilen değerin geçerli bir koordinat (enlem, boylam) olup olmadığını kontrol eder. Koordinat geçerli ise, Google Maps bağlantısı oluşturularak <paramref name="outvalue"/> parametresine atanır. Koordinat değeri, virgülle ayrılmış iki decimal sayıdan oluşmalıdır.
            /// </summary>
            /// <param name="value">Doğrulanacak koordinat değeri. <c>Örnek: 40.250230335582955,40.23025617808133</c></param>
            /// <param name="outvalue">Geçerli koordinat için oluşturulan URI değeri, dönüş değeri olarak atanır.</param>
            /// <returns>Geçerli bir koordinat ise <see langword="true"/>, aksi takdirde <see langword="false"/> döner.</returns>
            public static bool TryGoogleMapsCoordinate(string value, out Uri outvalue)
            {
                try
                {
                    var vs = (value ?? "").Split(',').Select(x => x.ToStringOrEmpty()).ToArray();
                    if (vs.Length == 2 && vs.All(isCoordinateDecimal_private))
                    {
                        outvalue = new Uri($"https://maps.google.com/?q={String.Join(",", vs)}");
                        return true;
                    }
                    outvalue = default;
                    return false;
                }
                catch
                {
                    outvalue = default;
                    return false;
                }
            }
            private static bool isCoordinateDecimal_private(string value) => (Decimal.TryParse(value.Trim(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out decimal _r) && _r >= Convert.ToDecimal(-180) && _r <= Convert.ToDecimal(180));
            /// <summary>
            /// Verilen bir metni, belirtilen diller arasında asenkron olarak çevirir.
            /// <para>
            /// Bu metot, çeviri işlemi için Google Çeviri API&#39;sini kullanarak, 
            /// verilen &quot;value&quot; parametresindeki metni &quot;from&quot; dilinden &quot;to&quot; diline 
            /// çevirir. Varsayılan olarak &quot;from&quot; dili Türkçe (tr) olarak ayarlanmıştır. 
            /// Eğer çeviri işlemi başarılı olursa, metnin çevirisi ve işlem durumu 
            /// döndürülür. Hata durumunda, boş bir değer ve false durumu döner.
            /// </para>
            /// </summary>
            public static async Task<(bool statuswarning, string value)> TryGoogleTranslateAsync(string value, TimeSpan timeout, string to = "en", string from = "tr", CancellationToken cancellationToken = default)
            {
                value = value.ToStringOrEmpty();
                if (value == "") { return (false, ""); }
                Guard.CheckEmpty(to, nameof(to));
                Guard.CheckEmpty(from, nameof(from));
                try
                {
                    using (var client = new HttpClient
                    {
                        Timeout = timeout,
                        DefaultRequestHeaders = { UserAgent = { new ProductInfoHeaderValue("Mozilla", "4.0") } }
                    })
                    {
                        var response = await client.GetStringAsync($"https://translate.googleapis.com/translate_a/single?client=gtx&sl={from}&tl={to}&dt=t&q={Uri.EscapeDataString(HttpUtility.HtmlEncode(value))}", cancellationToken);
                        using (var doc = JsonDocument.Parse(response))
                        {
                            return (false, doc.RootElement[0][0][0].GetString().ToStringOrEmpty());
                        }
                    }
                }
                catch { return (true, ""); }
            }
        }
    }
}