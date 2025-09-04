namespace BayuOrtak.Core.Helper
{
    using BayuOrtak.Core.Extensions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Linq.Expressions;
    using System.Net;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public sealed class Guard
    {
        /// <summary>
        /// Belirtilen nesnenin null olup olmadığını kontrol eder. Null ise <see cref="ArgumentNullException"/> fırlatır.
        /// </summary>
        /// <param name="value">Kontrol edilecek nesne.</param>
        /// <param name="argname">Argüman adı, hata mesajında kullanılmak üzere belirtilmelidir.</param>
        public static void CheckNull(object value, string argname)
        {
            if (value == null || value == DBNull.Value) { throw new ArgumentNullException(argname); }
        }
        /// <summary>
        /// Belirtilen string değerin boş olup olmadığını kontrol eder. Boşsa <see cref="ArgumentNullException"/> fırlatır.
        /// </summary>
        /// <param name="value">Kontrol edilecek string değer.</param>
        /// <param name="argname">Argüman adı, hata mesajında kullanılmak üzere belirtilmelidir.</param>
        public static void CheckEmpty(string value, string argname)
        {
            if (value.IsNullOrEmpty_string()) { throw new ArgumentNullException(argname, $"\"{argname}\" argümanı boş (null) veya sadece boşluk olamaz!"); }
        }
        /// <summary>
        /// Belirtilen <see cref="Guid"/> değerinin boş olup olmadığını kontrol eder. Boşsa <see cref="ArgumentNullException"/> fırlatır.
        /// </summary>
        /// <param name="guid">Kontrol edilecek Guid değeri.</param>
        /// <param name="argname">Argüman adı, hata mesajında kullanılmak üzere belirtilmelidir.</param>
        public static void CheckEmpty(Guid guid, string argname)
        {
            if (guid == Guid.Empty) { throw new ArgumentNullException(argname, $"\"{argname}\" argümanı \"{Guid.Empty.ToString()}\" değerini alamaz!"); }
        }
        /// <summary>
        /// Belirtilen koleksiyonun boş olup olmadığını kontrol eder. Koleksiyon boşsa <see cref="ArgumentNullException"/> fırlatır.
        /// </summary>
        /// <typeparam name="T">Koleksiyonun öğe türü.</typeparam>
        /// <param name="collection">Kontrol edilecek koleksiyon.</param>
        /// <param name="argname">Argüman adı, hata mesajında kullanılmak üzere belirtilmelidir.</param>
        public static void CheckEmpty<T>(ICollection<T> collection, string argname)
        {
            if (collection.IsNullOrEmpty_collection()) { throw new ArgumentNullException(argname, $"\"{argname}\" argümanı boş (null) olamaz ve en az bir öğe içermelidir!"); }
        }
        /// <summary>
        /// Belirtilen string&#39;in geçerli bir JSON olup olmadığını kontrol eder. Geçersizse <see cref="JsonReaderException"/> fırlatır.
        /// </summary>
        /// <param name="json">Kontrol edilecek JSON stringi.</param>
        /// <param name="jtokentype">Beklenen JSON token türü.</param>
        /// <param name="argname">Argüman adı, hata mesajında kullanılmak üzere belirtilmelidir.</param>
        public static void CheckJson(string json, JTokenType jtokentype, string argname)
        {
            CheckEmpty(json, argname);
            if (!json.IsJson(jtokentype, false)) { throw new JsonReaderException($"\"{argname}\" argümanı, \"JSON\" biçimine uygun olmalı ve türü \"{typeof(JTokenType).FullName}\" olmalıdır!"); }
        }
        /// <summary>
        /// Verilen string&#39;in Türkiye&#39;ye ait bir telefon numarası biçiminde olup olmadığını kontrol eder. Geçersizse <see cref="ArgumentException"/> fırlatır.
        /// </summary>
        /// <param name="phonenumberTR">Kontrol edilecek telefon numarası.</param>
        /// <param name="argname">Argüman adı, hata mesajında kullanılmak üzere belirtilmelidir.</param>
        public static void CheckPhoneNumberTR(string phonenumberTR, string argname)
        {
            CheckEmpty(phonenumberTR, phonenumberTR);
            if (!_try.TryPhoneNumberTR(phonenumberTR, out _)) { throw new ArgumentException($"\"{argname}\" argümanının değeri telefon numarası \"(5xx) (xxx-xxxx)\" biçimine uygun olmalıdır!", argname, new Exception($"Gelen değer: \"{phonenumberTR}\"")); }
        }
        /// <summary>
        /// Verilen T.C. Kimlik Numarasının geçerli olup olmadığını kontrol eder. Eğer T.C. Kimlik Numarası geçersizse, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <param name="tckn">Kontrol edilecek T.C. Kimlik Numarası.</param>
        /// <param name="argname">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckTCKN(long tckn, string argname)
        {
            CheckZeroOrNegative(tckn, argname);
            if (!_is.IsTCKimlikNo(tckn)) { throw new ArgumentException($"\"{argname}\" argümanı, T.C. Kimlik Numarası biçimine uygun olmalıdır!", argname, new Exception($"Gelen değer: \"{tckn}\"")); }
        }
        /// <summary>
        /// Verilen T.C. Vergi Kimlik Numarasının (VKN) geçerli olup olmadığını kontrol eder. Eğer VKN geçersizse, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <param name="vkn">Kontrol edilecek T.C. Vergi Kimlik Numarası.</param>
        /// <param name="argname">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckVKN(long vkn, string argname)
        {
            CheckZeroOrNegative(vkn, argname);
            if (!_is.IsVergiKimlikNo(vkn)) { throw new ArgumentException($"\"{argname}\" argümanı, T.C. Vergi Kimlik Numarası biçimine uygun olmalıdır!", argname, new Exception($"Gelen değer: \"{vkn}\"")); }
        }
        /// <summary>
        /// Verilen ISBN numarasının geçerli olup olmadığını kontrol eder. Eğer ISBN geçersizse, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <param name="isbn">Kontrol edilecek ISBN numarası.</param>
        /// <param name="argname">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckISBN(string isbn, string argname)
        {
            CheckEmpty(isbn, argname);
            if (!ISBNHelper.IsValid(isbn)) { throw new ArgumentException($"\"{argname}\" argümanı, {_title.isbn} biçimine uygun olmalıdır!", argname, new Exception($"Gelen değer: \"{isbn}\"")); }
        }
        /// <summary>
        /// Verilen e-Posta adresinin boş olmadığını ve genel e-Posta formatına uygun olup olmadığını kontrol eder.  Eğer geçerli değilse <see cref="ArgumentException"/> fırlatılır.
        /// </summary>
        /// <param name="argname">Hata mesajında kullanılacak argüman adı.</param>
        /// <param name="mail">Kontrol edilecek e-Posta adresi.</param>
        /// <exception cref="ArgumentNullException">Eğer <paramref name="mail"/> boş ya da null ise.</exception>
        /// <exception cref="ArgumentException">Eğer <paramref name="mail"/> e-posta formatına uygun değilse.</exception>
        public static void CheckMail(string argname, string mail)
        {
            CheckEmpty(mail, argname);
            if (!mail.IsMail()) { throw new ArgumentException($"\"{argname}\" argümanı, e-Posta yapısına uygun olmalıdır!", argname, new Exception($"Gelen değer: \"{mail}\"")); }
        }
        /// <summary>
        /// Verilen e-Posta adresinin boş olmadığını, genel e-Posta formatına uygun olduğunu ve belirtilen <paramref name="host"/> alanına ait olup olmadığını kontrol eder. Eğer geçerli değilse <see cref="ArgumentException"/> fırlatılır.
        /// </summary>
        /// <param name="argname">Hata mesajında kullanılacak argüman adı.</param>
        /// <param name="mail">Kontrol edilecek e-Posta adresi.</param>
        /// <param name="host">e-Posta adresinde olması gereken alan adı (örnek: &quot;@example.com&quot;).</param>
        /// <exception cref="ArgumentNullException">Eğer <paramref name="mail"/> veya <paramref name="host"/> boş ya da null ise.</exception>
        /// <exception cref="ArgumentException">Eğer <paramref name="mail"/> e-posta formatına uygun değilse veya verilen <paramref name="host"/> alanına ait değilse.</exception>
        public static void CheckMailFromHost(string argname, string mail, string host)
        {
            CheckMail(argname, mail);
            CheckEmpty(host, nameof(host));
            if (!mail.IsMailFromHost(host)) { throw new ArgumentException($"\"{argname}\" argümanı, e-Posta(example@{(host[0] == '@' ? host.Substring(1) : host)}) yapısına uygun olmalıdır!", argname, new Exception($"Gelen değer: \"{mail}\"")); }
        }
        /// <summary>
        /// Verilen URI&#39;nın geçerli olup olmadığını kontrol eder. Eğer URI geçersizse, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <param name="uri">Kontrol edilecek URI değeri.</param>
        /// <param name="argname">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckUri(string uri, string argname)
        {
            CheckEmpty(uri, argname);
            if (!uri.IsUri()) { throw new ArgumentException($"\"{argname}\" argümanı, URL biçimine uygun olmalıdır!", argname, new Exception($"Gelen değer: \"{uri}\"")); }
        }
        /// <summary>
        /// Verilen IP adresinin geçerli olup olmadığını kontrol eder. Eğer IP adresi geçersizse, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <param name="ipstring">Kontrol edilecek IP adresi.</param>
        /// <param name="argname">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckIPAddress(string ipstring, string argname)
        {
            CheckEmpty(ipstring, argname);
            if (!IPAddress.TryParse(ipstring, out _)) { throw new ArgumentException($"\"{argname}\" argümanı, IP adresi biçiminde olmalıdır!", argname); }
        }
        /// <summary>
        /// Verilen string ifadenin maksimum uzunluğu aşıp aşmadığını kontrol eder. Eğer ifade belirtilen uzunluktan daha uzunsa, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <param name="value">Kontrol edilecek string ifade.</param>
        /// <param name="maxlength">Maksimum izin verilen uzunluk.</param>
        /// <param name="argname">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckOutOfLength(string value, int maxlength, string argname)
        {
            var _l = value.ToStringOrEmpty().Length;
            if (_l > maxlength) { throw new ArgumentException($"\"{argname}\" argümanı, karakter uzunluğu \"{maxlength}\" değerinden uzun olamaz!", argname, new Exception($"Gelen değer: \"{_l}\"")); }
        }
        /// <summary>
        /// Verilen string ifadenin, modelde belirtilen maksimum uzunluğu aşıp aşmadığını kontrol eder. Eğer ifade belirtilen uzunluktan daha uzunsa, hata mesajıyla birlikte argüman adı ve model adı fırlatılır.
        /// </summary>
        /// <typeparam name="T">Model türü.</typeparam>
        /// <param name="value">Kontrol edilecek string ifade.</param>
        /// <param name="expression">Modeldeki ifade uzunluğu ile karşılaştırma yapılacak alanı belirten lambda ifadesi.</param>
        public static void CheckOutOfLength<T>(string value, Expression<Func<T, string>> expression) where T : class
        {
            var _p = expression.GetExpressionName();
            var _m = _get.GetStringOrMaxLength<T>(_p);
            CheckZeroOrNegative(_m, _p);
            CheckOutOfLength(value, _m, _p);
        }
        /// <summary>
        /// Verilen değerin belirtilen değerler arasında olup olmadığını kontrol eder. Eğer değer belirtilen değerler arasında yer <b>alıyorsa</b>, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <typeparam name="T">Kontrol edilecek değerin türü.</typeparam>
        /// <param name="argname">Hata mesajında kullanılacak argüman adı.</param>
        /// <param name="value">Kontrol edilecek değer.</param>
        /// <param name="values">Geçerli değerlerin listesi.</param>
        public static void CheckIncludes<T>(string argname, T value, params T[] values)
        {
            if (value.Includes(values)) { throw new ArgumentOutOfRangeException($"\"{argname}\" argümanı, \"{String.Join(", ", values)}\" değerlerinden biri olmamalıdır!", new Exception($"Gelen değer: \"{value}\"")); }
        }
        /// <summary>
        /// Verilen değerin belirtilen değerler arasında olup olmadığını kontrol eder. Eğer değer belirtilen değerler arasında yer <b>almıyorsa</b>, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <typeparam name="T">Kontrol edilecek değerin türü.</typeparam>
        /// <param name="argname">Hata mesajında kullanılacak argüman adı.</param>
        /// <param name="value">Kontrol edilecek değer.</param>
        /// <param name="values">Geçerli değerlerin listesi.</param>
        public static void CheckNotIncludes<T>(string argname, T value, params T[] values)
        {
            if (!value.Includes(values)) { throw new ArgumentOutOfRangeException($"\"{argname}\" argümanı, \"{String.Join(", ", values)}\" değerlerinden biri olabilir!", new Exception($"Gelen değer: \"{value}\"")); }
        }
        /// <summary>
        /// Verilen değerin belirli bir aralıkta olup olmadığını (min ve max değerleri de dahil) kontrol eder. Eğer değer belirlenen minimum ve maksimum aralık dışında kalıyorsa, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <typeparam name="T">Kontrol edilecek değerin türü.</typeparam>
        /// <param name="value">Kontrol edilecek değer.</param>
        /// <param name="min">Minimum kabul edilen değer.</param>
        /// <param name="max">Maksimum kabul edilen değer.</param>
        /// <param name="argname">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckRange<T>(T value, T min, T max, string argname) where T : struct, IComparable<T>
        {
            if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0) { throw new ArgumentOutOfRangeException($"\"{argname}\" argümanı, [{min} - {max}] değerleri arasında olmalıdır!", new Exception($"Gelen değer: \"{value}\"")); }
        }
        /// <summary>
        /// Verilen değerin sıfır olup olmadığını kontrol eder. Eğer değer sıfırsa, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <typeparam name="T">Kontrol edilecek değerin türü.</typeparam>
        /// <param name="value">Kontrol edilecek değer.</param>
        /// <param name="argname">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckZero<T>(T value, string argname) where T : struct, IComparable<T>
        {
            if (value.CompareTo(default(T)) == 0) { throw new ArgumentException($"\"{argname}\" argümanı, \"0 (sıfır)\" olamaz!", argname); }
        }
        /// <summary>
        /// Verilen değerin sıfır ya da negatif olup olmadığını kontrol eder. Eğer değer sıfır ya da negatifse, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <typeparam name="T">Kontrol edilecek değerin türü.</typeparam>
        /// <param name="value">Kontrol edilecek değer.</param>
        /// <param name="argname">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckZeroOrNegative<T>(T value, string argname) where T : struct, IComparable<T>
        {
            CheckZero(value, argname);
            CheckNegative(value, argname);
        }
        /// <summary>
        /// Verilen değerin negatif olup olmadığını kontrol eder. Eğer değer negatifse, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <typeparam name="T">Kontrol edilecek değerin türü.</typeparam>
        /// <param name="value">Kontrol edilecek değer.</param>
        /// <param name="argname">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckNegative<T>(T value, string argname) where T : struct, IComparable<T>
        {
            if (value.CompareTo(default(T)) < 0) { throw new ArgumentOutOfRangeException($"\"{argname}\" argümanı, negatif olamaz!", new Exception($"Gelen değer: \"{value}\"")); }
        }
        /// <summary>
        /// Verilen değerin belirtilen Enum türü içinde tanımlı olup olmadığını kontrol eder. Eğer değer Enum türünde tanımlı değilse, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <param name="type">Kontrol edilecek Enum türü.</param>
        /// <param name="value">Kontrol edilecek değer.</param>
        /// <param name="argname">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckEnumDefined(Type type, object value, string argname)
        {
            CheckNull(type, nameof(type));
            if (!type.IsEnum) { throw new ArgumentException($"\"{type.FullName}\" türü geçerli bir \"{nameof(Enum)}\" türü olmalıdır!", argname); }
            CheckNull(value, argname);
            if (!Enum.IsDefined(type, value)) { throw new ArgumentException($"\"{type.FullName}\" için sağlanan \"{argname}\" argümanının değeri geçersizdir!", argname, new Exception($"Gelen değer: \"{value}\"")); }
        }
        /// <summary>
        /// Belirtilen Enum türünde, verilen değerin tanımlı olup olmadığını kontrol eder. Eğer değer belirtilen Enum türünde tanımlı değilse, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <typeparam name="T">Kontrol edilecek Enum türü.</typeparam>
        /// <param name="value">Kontrol edilecek değer.</param>
        /// <param name="argname">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckEnumDefined<T>(object value, string argname) where T : Enum => CheckEnumDefined(typeof(T), value, argname);
        /// <summary>
        /// İki koleksiyonun (ICollection) eleman sayılarının eşit olup olmadığını kontrol eder. Eğer koleksiyonlardan herhangi biri boşsa veya iki koleksiyonun eleman sayıları eşit değilse,
        /// bir <see cref="ArgumentException"/> fırlatır.
        /// </summary>
        /// <typeparam name="T">Koleksiyonların eleman türü.</typeparam>
        /// <param name="collection1">Karşılaştırılacak ilk koleksiyon.</param>
        /// <param name="collection2">Karşılaştırılacak ikinci koleksiyon.</param>
        /// <exception cref="ArgumentException">
        /// Eğer koleksiyonlardan biri boşsa veya koleksiyonların eleman sayıları eşit değilse fırlatılır. Örneğin, &quot;collection1 (3)&quot; ve &quot;collection2 (5)&quot; şeklinde bir hata mesajı döner.
        /// </exception>
        public static void CheckEqualCount<T>(ICollection<T> collection1, ICollection<T> collection2)
        {
            CheckEmpty(collection1, nameof(collection1));
            CheckEmpty(collection2, nameof(collection2));
            if (collection1.Count != collection2.Count) { throw new ArgumentException($"\"{nameof(collection1)} ({collection1.Count})\" ve \"{nameof(collection2)} ({collection2.Count})\" nesne sayıları eşit olmalıdır!"); }
        }
        /// <summary>
        /// Parametre olarak verilen dil değerinin desteklenip desteklenmediğini kontrol eder. Desteklenen diller: &quot;tr&quot;, &quot;en&quot;.
        /// </summary>
        /// <param name="value">Kontrol edilecek dil kodu.</param>
        /// <param name="argname">Parametre adı (hata mesajında kullanılmak üzere).</param>
        /// <exception cref="ArgumentNullException">Eğer <paramref name="value"/> boş ya da null ise.</exception>
        /// <exception cref="NotSupportedException">Eğer <paramref name="value"/> desteklenmeyen bir dil ise.</exception>
        public static void UnSupportLanguage(string value, string argname)
        {
            CheckEmpty(value, argname);
            var _defaultlanguages = new string[] { "tr", "en" };
            if (!_defaultlanguages.Contains(value)) { throw new NotSupportedException($"{argname}; {String.Join(", ", _defaultlanguages)} değerlerinden biri olabilir!", new Exception("Yönetici ile iletişime geçiniz!")); }
        }
    }
}