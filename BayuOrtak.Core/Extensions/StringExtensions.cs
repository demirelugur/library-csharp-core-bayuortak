namespace BayuOrtak.Core.Extensions
{
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Helper.Results;
    using Ganss.Xss;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Globalization;
    using System.Net.Mail;
    using System.Numerics;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public static class StringExtensions
    {
        /// <summary>
        /// Bir string&#39;i Guid&#39;e dönüştürür. String null veya geçersizse varsayılan Guid döner.
        /// </summary>
        /// <param name="value">Dönüştürülecek string.</param>
        /// <returns>Dönüştürülmüş Guid.</returns>
        public static Guid ToGuid(this string value) => value.ParseOrDefault<Guid>();
        /// <summary>
        /// Bir dizeyi <see cref="DateTime"/> türüne dönüştürür. Dize geçerli bir tarih formatında değilse, varsayılan <see cref="DateTime"/> değeri döndürülür.
        /// </summary>
        /// <param name="value">Dönüştürülecek tarih içeren dize.</param>
        /// <returns>Geçerli bir <see cref="DateTime"/> nesnesi veya varsayılan <see cref="DateTime"/> değeri.</returns>
        public static DateTime ToDate(this string value) => value.ParseOrDefault<DateTime>();
        /// <summary>
        /// Verilen telefon numarasını Türk telefon biçime dönüştürür. Eğer telefon numarası geçerli bir Türk telefon numarası değilse, boş bir string döner.
        /// <para>Biçim: (###) ###-####</para>
        /// <para>Örneğin: &quot;5001112233&quot; girişi &quot;(500) 111-2233&quot; biçiminde döner.</para>
        /// </summary>
        /// <param name="phonenumberTR">Dönüştürülmek istenen telefon numarası.</param>
        /// <returns>Biçimlenmiş Türk telefon numarası ya da geçerli değilse boş bir string.</returns>
        public static string BeautifyPhoneNumberTR(this string phonenumberTR) => (_try.TryPhoneNumberTR(phonenumberTR, out string _s) ? $"({_s.Substring(0, 3)}) {_s.Substring(3, 3)}-{_s.Substring(6, 4)}" : "");
        /// <summary>
        /// Verilen dize değerinin null veya boş olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="value">Kontrol edilecek dize.</param>
        /// <returns><see langword="true"/>, eğer dize null veya boşsa; aksi takdirde <see langword="false"/>.</returns>
        public static bool IsNullOrEmpty_string(this string value) => String.IsNullOrEmpty(value.ToStringOrEmpty());
        /// <summary>
        /// String verilerinde null-coalescing operatörünün (null birleştirme operatörünün) etkili bir şekilde kullanılabilmesi için, eğer string değeri null veya boşsa (trimlenmiş haliyle) sağdaki değeri döndüren bir fonksiyon oluşturulmuştur
        /// </summary>
        /// <param name="value">Kontrol edilecek string.</param>
        /// <param name="defaultvalue">Eğer value null veya boşsa dönecek olan değer.</param>
        /// <returns>value string&#39;i veya defaultvalue.</returns>
        public static string CoalesceOrDefault(this string value, string defaultvalue)
        {
            value = value.ToStringOrEmpty();
            return (value == "" ? defaultvalue.ToStringOrEmpty() : value);
        }
        /// <summary>
        /// Verilen dize değerinin sayısal bir değere dönüştürülüp dönüştürülemeyeceğini kontrol eder.
        /// </summary>
        /// <param name="value">Kontrol edilecek dize.</param>
        /// <param name="numberstyles">Sayının biçimlendirilmesi için kullanılacak sayı stilleri.</param>
        /// <returns><see langword="true"/>, eğer dize bir sayıya dönüştürülebiliyorsa; aksi takdirde <see langword="false"/>.</returns>
        public static bool IsNumeric(this string value, NumberStyles numberstyles = NumberStyles.Integer) => BigInteger.TryParse(value.ToStringOrEmpty(), numberstyles, NumberFormatInfo.InvariantInfo, out _);
        /// <summary>
        /// Belirtilen string değerinin geçerli bir e-Posta adresi olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="value">Kontrol edilecek e-Posta adresi.</param>
        /// <returns>Geçerli bir e-Posta adresi ise <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
        public static bool IsMail(this string value) => _try.TryMailAddress(value, out _);
        /// <summary>
        /// Verilen string&#39;in geçerli bir e-Posta adresi olup olmadığını ve bu adresin host kısmının belirtilen host ile eşleşip eşleşmediğini kontrol eder. Host karşılaştırması büyük/küçük harfe duyarlı değildir ve host parametresi &#39;@&#39; ile başlıyorsa bu karakter yok sayılır.
        /// </summary>
        public static bool IsMailFromHost(this string value, string host)
        {
            host = host.ToStringOrEmpty().ToLower();
            if (host == "") { return false; }
            if (host[0] == '@') { host = host.Substring(1); }
            return _try.TryMailAddress(value, out MailAddress _ma) && _ma.Host == host;
        }
        /// <summary>
        /// Verilen dize değerinin geçerli bir JSON olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="value">Kontrol edilecek dize (JSON).</param>
        /// <param name="jtokentype">Kontrol edilecek JToken türü.</param>
        /// <param name="haschildren">Çocukların kontrol edilip edilmeyeceğini belirten bir değer.</param>
        /// <returns><see langword="true"/>, eğer dize geçerli bir JSON ise; aksi takdirde <see langword="false"/>.</returns>
        public static bool IsJson(this string value, JTokenType jtokentype, bool haschildren)
        {
            var _r = _try.TryJson(value, jtokentype, out JToken _jt);
            if (_r && haschildren) { return _jt.Children().Any(); }
            return _r;
        }
        /// <summary>
        /// Verilen dize değerinin geçerli bir URI olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="value">Kontrol edilecek dize (URI).</param>
        /// <returns><see langword="true"/>, eğer dize geçerli bir URI ise; aksi takdirde <see langword="false"/>.</returns>
        public static bool IsUri(this string value) => _try.TryUri(value, out _);
        private static readonly Dictionary<char, char> _charreplacements = new Dictionary<char, char>
        {
            { 'ş', 's' }, { 'Ş', 's' },
            { 'ö', 'o' }, { 'Ö', 'o' },
            { 'ü', 'u' }, { 'Ü', 'u' },
            { 'ç', 'c' }, { 'Ç', 'c' },
            { 'ğ', 'g' }, { 'Ğ', 'g' },
            { 'ı', 'i' }, { 'I', 'i' }, { 'İ', 'i' }
        };
        private static readonly char[] _charstoremove = new char[] { '?', '/', '.', '\'', '"', '#', '%', '&', '*', '!', '@', '+' };
        /// <summary>
        /// Verilen dizeyi SEO dostu bir hale getirir.
        /// </summary>
        /// <param name="value">Dönüştürülecek dize.</param>
        /// <returns>SEO dostu hale getirilmiş dize.</returns>
        public static string ToSeoFriendly(this string value)
        {
            value = value.ToStringOrEmpty();
            if (value == "") { return ""; }
            var sb = new StringBuilder(value.Length);
            foreach (var item in value.ToCharArray())
            {
                if (_charreplacements.TryGetValue(item, out char _v)) { sb.Append(_v); }
                else if (item == ' ') { sb.Append('-'); }
                else if (Array.IndexOf(_charstoremove, item) == -1) { sb.Append(item); }
            }
            value = sb.ToString().ToLower().Trim();
            value = Regex.Replace(value, @"[^a-z0-9-]", "-");
            value = Regex.Replace(value, @"-+", "-");
            return value.Trim('-');
        }
        /// <summary>
        /// Verilen dizeyi bir nesnenin üyeleri ile biçimlendirir.
        /// </summary>
        /// <typeparam name="T">Biçimlendirilecek nesnenin türü.</typeparam>
        /// <param name="value">Dize.</param>
        /// <param name="argument">Biçimlendirme için kullanılan nesne.</param>
        /// <returns>Biçimlendirilmiş dize.</returns>
        public static string FormatVar<T>(this string value, T argument) where T : class
        {
            HashSet<string> arm;
            string f;
            foreach (var pi in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToArray())
            {
                arm = new HashSet<string>();
                foreach (Match item in Regex.Matches(value, String.Concat(@"\{", pi.Name, @"(\:.*?)?\}")))
                {
                    if (arm.Contains(item.Value)) { continue; }
                    arm.Add(item.Value);
                    f = String.Concat("{0", item.Groups[1].Value, "}");
                    value = value.Replace(item.Value, String.Format(f, pi.GetValue(argument)));
                }
            }
            return value;
        }
        /// <summary>
        /// Verilen metot ismi ve tip bilgisi kullanılarak bir route ismi oluşturur.
        /// </summary>
        /// <typeparam name="T">Route&#39;un ilişkilendirileceği sınıf tipi (class olmalıdır)</typeparam>
        /// <param name="methodname">Route ile ilişkilendirilecek metot ismi</param>
        /// <param name="usefulltypename">Tam tip ismi (<see cref="Type.FullName"/>) kullanılacak mı? <see langword="false"/> ise kısa tip ismi (<see cref="MemberInfo.Name"/>) kullanılır</param>
        /// <returns>Biçimli route string&#39;i (örn: &quot;/ControllerName/Method&quot; veya &quot;/Namespace.ControllerName/Method&quot;)</returns>
        /// <exception cref="ArgumentException">method parametresi boş veya null olduğunda fırlatılır</exception>
        public static string GetRouteName<T>(this string methodname, bool usefulltypename) where T : class
        {
            Guard.CheckEmpty(methodname, nameof(methodname));
            return $"/{(usefulltypename ? typeof(T).FullName : typeof(T).Name)}/{methodname}";
        }
        /// <summary>
        /// Verilen string içindeki tab (\t), satır başı (\r) ve yeni satır (\n) karakterlerini tek boşlukla değiştirir, baştaki ve sondaki boşlukları temizler ve birden fazla boşluğu tek boşluğa indirger.
        /// </summary>
        /// <param name="value">İşlenecek string değer.</param>
        /// <returns>Düzenlenmiş string.</returns>
        public static string ReplaceTRNSpace(this string value) => Regex.Replace(value.ToStringOrEmpty().Replace("\t", " ").Replace("\r", " ").Replace("\n", " ").Trim(), @"\s+", " ");
        /// <summary>
        /// Verilen dizeyi belirtilen miktarda çoğaltır ve istenen karakterle doldurur.
        /// </summary>
        /// <param name="value">Çoğaltılacak dize.</param>
        /// <param name="quantity">Miktar.</param>
        /// <param name="c">Doldurma karakteri.</param>
        /// <param name="direction">Doldurma yönü (sol için &quot;l&quot;, sağ için &quot;r&quot;)</param>
        /// <returns>Çoğaltılmış dize.</returns>
        public static string Replicate(this string value, int quantity = 2, char c = '0', char direction = 'l')
        {
            Guard.CheckNotIncludes(nameof(direction), direction, 'l', 'r');
            value = value.ToStringOrEmpty();
            if (value == "" || quantity < 1 || quantity < value.Length) { return ""; }
            if (quantity == value.Length) { return value; }
            if (direction == 'r') { return String.Concat(value, new String(c, quantity - value.Length)); }
            return String.Concat(new String(c, quantity - value.Length), value);
        }
        /// <summary>
        /// Verilen dizeyi belirtilen uzunluğa kadar keser.
        /// </summary>
        /// <param name="value">Kesilecek dize.</param>
        /// <param name="length">Kesim uzunluğu.</param>
        /// <returns>Kesilmiş dize.</returns>
        public static string SubstringUpToLength(this string value, int length)
        {
            Guard.CheckZeroOrNegative(length, nameof(length));
            value = value.ToStringOrEmpty();
            return (value.Length > length ? value.Substring(0, length).Trim() : value);
        }
        private static ArgumentNullException _setargumentnullexception(string argname) => new ArgumentNullException($"\"{argname}\" değeri boş geçilemez!", new Exception(_title.xss)); // HttpUtility.HtmlEncode(value)
        /// <summary>
        /// Verilen bir dizeyi HTML sanitizasyon işleminden geçirir ve gerekli kontrolleri yapar. Eğer dize boşsa ve isrequired true ise, bir hata fırlatır. Sanitizasyon işlemi sırasında script etiketleri kaldırılır, yalnızca https şemasına izin verilir ve veri öznitelikleri kabul edilir.
        /// </summary>
        /// <param name="value">Sanitize edilecek dize.</param>
        /// <param name="isrequired">Dizenin zorunlu olup olmadığını belirtir.</param>
        /// <param name="argname">Hata mesajında kullanılacak argüman adı.</param>
        /// <returns>Sanitize edilmiş dize veya uygun durumlarda null.</returns>
        public static string? ReplaceSanitize(this string value, bool isrequired, string argname)
        {
            value = value.ToStringOrEmpty();
            if (value == "")
            {
                if (isrequired) { throw _setargumentnullexception(argname); }
                return null;
            }
            string v;
            try
            {
                var _hs = new HtmlSanitizer
                {
                    AllowDataAttributes = true
                };
                _hs.AllowedTags.Remove("script");
                _hs.AllowedSchemes.Add("https");
                _hs.AllowedSchemes.Remove("javascript");
                v = _hs.Sanitize(value).Trim();
            }
            catch { v = ""; }
            if (v == "")
            {
                if (isrequired) { throw _setargumentnullexception(argname); }
                return null;
            }
            return v;
        }
        /// <summary>
        /// Bir string&#39;i belirtilen noktalama işaretleri kurallarına göre Başlık Durumuna dönüştürür.
        /// </summary>
        /// <param name="value">Dönüştürülecek string.</param>
        /// <param name="iswhitespace">Boşluk karakterlerinin yeni kelimeleri ayırmak için dikkate alınıp alınmayacağını belirtir.</param>
        /// <param name="punctuations">Kelime ayıran noktalama karakterleri.</param>
        /// <returns>Başlık durumuna dönüştürülmüş string.</returns>
        public static string ToTitleCase(this string value, bool iswhitespace, char[] punctuations)
        {
            value = value.ToStringOrEmpty();
            if (value == "") { return ""; }
            if (value.Length == 1) { return value.ToUpper(); }
            punctuations = (punctuations ?? Array.Empty<char>()).Where(Char.IsPunctuation).ToArray();
            bool _haspunc = punctuations.Length > 0, newword = true;
            var _sb = new StringBuilder();
            foreach (var item in value.ToCharArray())
            {
                if ((iswhitespace && Char.IsWhiteSpace(item)) || (_haspunc && punctuations.Contains(item)))
                {
                    _sb.Append(item);
                    newword = true;
                }
                else if (newword)
                {
                    _sb.Append(Convert.ToString(item).ToUpper());
                    newword = false;
                }
                else { _sb.Append(Convert.ToString(item).ToLower()); }
            }
            return _sb.ToString();
        }
        /// <summary>
        /// Verilen bir dizeyi, belirtilen türde bir değere dönüştürür. Dönüşüm başarısız olursa, varsayılan değeri döner.
        /// </summary>
        /// <typeparam name="TKey">Dönüşüm yapılacak hedef tür.</typeparam>
        /// <param name="value">Dönüştürülecek dize değeri.</param>
        /// <returns>Dönüştürülen değeri veya dönüşüm başarısızsa varsayılan değeri döner.</returns>
        public static TKey ParseOrDefault<TKey>(this string value)
        {
            var _t = typeof(TKey);
            var _pd = _to.ParseOrDefault_valuetuple(value, _t);
            if (_pd.value == null) { return default; }
            try { return (TKey)Convert.ChangeType(_pd.value, _pd.genericbasetype); }
            catch { return default; }
        }
        /// <summary>
        /// Verilen hata mesajını başarısız sonucu temsil edecek şekilde döndürür.
        /// </summary>
        public static IslemSonucResult<T> ReturnFailed<T>(this string error) => new string[] { error }.ReturnFailed<T>();
        /// <summary>
        /// Verilen hata mesajını başarısız sonucu temsil edecek şekilde döndürür.
        /// </summary>
        public static IslemSonucResult<object[]> ReturnFailedObjectArray(this string error) => new string[] { error }.ReturnFailedObjectArray();
    }
}