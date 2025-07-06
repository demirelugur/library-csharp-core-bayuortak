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
        /// <param name="argName">Argüman adı, hata mesajında kullanılmak üzere belirtilmelidir.</param>
        public static void CheckNull(object value, string argName)
        {
            if (value == null || value == DBNull.Value) { throw new ArgumentNullException(argName); }
        }
        /// <summary>
        /// Belirtilen string değerin boş olup olmadığını kontrol eder. Boşsa <see cref="ArgumentNullException"/> fırlatır.
        /// </summary>
        /// <param name="value">Kontrol edilecek string değer.</param>
        /// <param name="argName">Argüman adı, hata mesajında kullanılmak üzere belirtilmelidir.</param>
        public static void CheckEmpty(string value, string argName)
        {
            if (value.IsNullOrEmpty_string()) { throw new ArgumentNullException(argName, $"\"{argName}\" argümanı boş (null) veya sadece boşluk olamaz!"); }
        }
        /// <summary>
        /// Belirtilen <see cref="Guid"/> değerinin boş olup olmadığını kontrol eder. Boşsa <see cref="ArgumentNullException"/> fırlatır.
        /// </summary>
        /// <param name="guid">Kontrol edilecek Guid değeri.</param>
        /// <param name="argName">Argüman adı, hata mesajında kullanılmak üzere belirtilmelidir.</param>
        public static void CheckEmpty(Guid guid, string argName)
        {
            if (guid == Guid.Empty) { throw new ArgumentNullException(argName, $"\"{argName}\" argümanı \"{Guid.Empty.ToString()}\" değerini alamaz!"); }
        }
        /// <summary>
        /// Belirtilen koleksiyonun boş olup olmadığını kontrol eder. Koleksiyon boşsa <see cref="ArgumentNullException"/> fırlatır.
        /// </summary>
        /// <typeparam name="T">Koleksiyonun öğe türü.</typeparam>
        /// <param name="collection">Kontrol edilecek koleksiyon.</param>
        /// <param name="argName">Argüman adı, hata mesajında kullanılmak üzere belirtilmelidir.</param>
        public static void CheckEmpty<T>(ICollection<T> collection, string argName)
        {
            if (collection.IsNullOrEmpty_collection()) { throw new ArgumentNullException(argName, $"\"{argName}\" argümanı boş (null) olamaz ve en az bir öğe içermelidir!"); }
        }
        /// <summary>
        /// Belirtilen string&#39;in geçerli bir JSON olup olmadığını kontrol eder. Geçersizse <see cref="JsonReaderException"/> fırlatır.
        /// </summary>
        /// <param name="json">Kontrol edilecek JSON stringi.</param>
        /// <param name="jTokenType">Beklenen JSON token türü.</param>
        /// <param name="argName">Argüman adı, hata mesajında kullanılmak üzere belirtilmelidir.</param>
        public static void CheckJson(string json, JTokenType jTokenType, string argName)
        {
            CheckEmpty(json, argName);
            if (!json.IsJson(jTokenType, false)) { throw new JsonReaderException($"\"{argName}\" argümanı, \"JSON\" biçimine uygun olmalı ve türü \"{nameof(JTokenType)}.{jTokenType.ToString("g")}\" olmalıdır!"); }
        }
        /// <summary>
        /// Verilen string&#39;in Türkiye&#39;ye ait bir telefon numarası biçiminde olup olmadığını kontrol eder.
        /// Geçersizse <see cref="ArgumentException"/> fırlatır.
        /// </summary>
        /// <param name="phoneNumberTR">Kontrol edilecek telefon numarası.</param>
        /// <param name="argName">Argüman adı, hata mesajında kullanılmak üzere belirtilmelidir.</param>
        public static void CheckPhoneNumberTR(string phoneNumberTR, string argName)
        {
            CheckEmpty(phoneNumberTR, phoneNumberTR);
            if (!_try.TryPhoneNumberTR(phoneNumberTR, out _)) { throw new ArgumentException($"\"{argName}\" argümanının değeri telefon numarası \"(5xx) (xxx-xxxx)\" biçimine uygun olmalıdır!", argName, new Exception($"Gelen değer: \"{phoneNumberTR}\"")); }
        }
        /// <summary>
        /// Verilen T.C. Kimlik Numarasının (TCKN) geçerli olup olmadığını kontrol eder.
        /// Eğer TCKN geçersizse, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <param name="tckn">Kontrol edilecek T.C. Kimlik Numarası.</param>
        /// <param name="argName">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckTCKN(string tckn, string argName)
        {
            CheckEmpty(tckn, argName);
            if (!_try.TryTCKimlikNo(tckn, out _)) { throw new ArgumentException($"\"{argName}\" argümanı, T.C. Kimlik Numarası biçimine uygun olmalıdır!", argName, new Exception($"Gelen değer: \"{tckn}\"")); }
        }
        /// <summary>
        /// Verilen T.C. Vergi Kimlik Numarasının (VKN) geçerli olup olmadığını kontrol eder.
        /// Eğer VKN geçersizse, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <param name="vkn">Kontrol edilecek T.C. Vergi Kimlik Numarası.</param>
        /// <param name="argName">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckVKN(string vkn, string argName)
        {
            CheckEmpty(vkn, argName);
            if (!_try.TryVergiKimlikNo(vkn, out _)) { throw new ArgumentException($"\"{argName}\" argümanı, T.C. Vergi Kimlik Numarası biçimine uygun olmalıdır!", argName, new Exception($"Gelen değer: \"{vkn}\"")); }
        }
        /// <summary>
        /// Verilen ISBN numarasının geçerli olup olmadığını kontrol eder.
        /// Eğer ISBN geçersizse, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <param name="isbn">Kontrol edilecek ISBN numarası.</param>
        /// <param name="argName">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckISBN(string isbn, string argName)
        {
            CheckEmpty(isbn, argName);
            if (!ISBNHelper.IsValid(isbn)) { throw new ArgumentException($"\"{argName}\" argümanı, {_title.isbn} biçimine uygun olmalıdır!", argName, new Exception($"Gelen değer: \"{isbn}\"")); }
        }
        /// <summary>
        /// Belirtilen e-Posta adresinin geçerliliğini kontrol eder ve verilen host ile uyumlu olup olmadığını doğrular.
        /// Eğer host belirtilmemişse, e-Posta adresinin genel yapısı kontrol edilir. 
        /// Aksi takdirde, e-Posta adresinin belirtilen host ile eşleşmesi gerektiği doğrulanır.
        /// </summary>
        /// <param name="argName">Hata mesajlarında kullanılacak parametre adı.</param>
        /// <param name="mail">Kontrol edilecek e-Posta adresi.</param>
        /// <param name="host">e-Posta adresinin uyumlu olması gereken host (örneğin, &quot;bayburt.edu.tr&quot;).</param>
        /// <exception cref="ArgumentException">e-Posta adresi geçersiz veya host ile uyumsuzsa fırlatılır.</exception>
        public static void CheckMail(string argName, string mail, string host)
        {
            CheckEmpty(mail, argName);
            if (host.IsNullOrEmpty_string() && !mail.IsMail()) { throw new ArgumentException($"\"{argName}\" argümanı, e-Posta yapısına uygun olmalıdır!", argName, new Exception($"Gelen değer: \"{mail}\"")); }
            else if (!mail.IsMailFromHost(host)) { throw new ArgumentException($"\"{argName}\" argümanı, e-Posta(example@{(host[0] == '@' ? host.Substring(1) : host)}) yapısına uygun olmalıdır!", argName, new Exception($"Gelen değer: \"{mail}\"")); }
        }
        /// <summary>
        /// Verilen URI&#39;nın geçerli olup olmadığını kontrol eder.
        /// Eğer URI geçersizse, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <param name="uri">Kontrol edilecek URI değeri.</param>
        /// <param name="argName">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckUri(string uri, string argName)
        {
            CheckEmpty(uri, argName);
            if (!uri.IsUri()) { throw new ArgumentException($"\"{argName}\" argümanı, URL biçimine uygun olmalıdır!", argName, new Exception($"Gelen değer: \"{uri}\"")); }
        }
        /// <summary>
        /// Verilen IP adresinin geçerli olup olmadığını kontrol eder.
        /// Eğer IP adresi geçersizse, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <param name="ipString">Kontrol edilecek IP adresi.</param>
        /// <param name="argName">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckIPAddress(string ipString, string argName)
        {
            CheckEmpty(ipString, argName);
            if (!IPAddress.TryParse(ipString, out _)) { throw new ArgumentException($"\"{argName}\" argümanı, IP adresi biçiminde olmalıdır!", argName); }
        }
        /// <summary>
        /// Verilen string ifadenin maksimum uzunluğu aşıp aşmadığını kontrol eder.
        /// Eğer ifade belirtilen uzunluktan daha uzunsa, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <param name="value">Kontrol edilecek string ifade.</param>
        /// <param name="maxLength">Maksimum izin verilen uzunluk.</param>
        /// <param name="argName">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckOutOfLength(string value, int maxLength, string argName)
        {
            var _l = value.ToStringOrEmpty().Length;
            if (_l > maxLength) { throw new ArgumentException($"\"{argName}\" argümanı, karakter uzunluğu \"{maxLength}\" değerinden uzun olamaz!", argName, new Exception($"Gelen değer: \"{_l}\"")); }
        }
        /// <summary>
        /// Verilen string ifadenin, modelde belirtilen maksimum uzunluğu aşıp aşmadığını kontrol eder.
        /// Eğer ifade belirtilen uzunluktan daha uzunsa, hata mesajıyla birlikte argüman adı ve model adı fırlatılır.
        /// </summary>
        /// <typeparam name="T">Model türü.</typeparam>
        /// <param name="value">Kontrol edilecek string ifade.</param>
        /// <param name="expression">Modeldeki ifade uzunluğu ile karşılaştırma yapılacak alanı belirten lambda ifadesi.</param>
        public static void CheckOutOfLength<T>(string value, Expression<Func<T, string>> expression) where T : class
        {
            var p = expression.GetExpressionName();
            var m = _get.GetStringOrMaxLength<T>(p);
            CheckZeroOrNegative(m, p);
            CheckOutOfLength(value, m, p);
        }
        /// <summary>
        /// Verilen değerin belirtilen değerler arasında olup olmadığını kontrol eder.
        /// Eğer değer belirtilen değerler arasında yer <b>alıyorsa</b>, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <typeparam name="T">Kontrol edilecek değerin türü.</typeparam>
        /// <param name="argName">Hata mesajında kullanılacak argüman adı.</param>
        /// <param name="value">Kontrol edilecek değer.</param>
        /// <param name="values">Geçerli değerlerin listesi.</param>
        public static void CheckIncludes<T>(string argName, T value, params T[] values)
        {
            if (value.Includes(values)) { throw new ArgumentOutOfRangeException($"\"{argName}\" argümanı, \"{String.Join(", ", values)}\" değerlerinden biri olmamalıdır!", new Exception($"Gelen değer: \"{value}\"")); }
        }
        /// <summary>
        /// Verilen değerin belirtilen değerler arasında olup olmadığını kontrol eder.
        /// Eğer değer belirtilen değerler arasında yer <b>almıyorsa</b>, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <typeparam name="T">Kontrol edilecek değerin türü.</typeparam>
        /// <param name="argName">Hata mesajında kullanılacak argüman adı.</param>
        /// <param name="value">Kontrol edilecek değer.</param>
        /// <param name="values">Geçerli değerlerin listesi.</param>
        public static void CheckNotIncludes<T>(string argName, T value, params T[] values)
        {
            if (!value.Includes(values)) { throw new ArgumentOutOfRangeException($"\"{argName}\" argümanı, \"{String.Join(", ", values)}\" değerlerinden biri olabilir!", new Exception($"Gelen değer: \"{value}\"")); }
        }
        /// <summary>
        /// Verilen değerin belirli bir aralıkta olup olmadığını (min ve max değerleri de dahil) kontrol eder.
        /// Eğer değer belirlenen minimum ve maksimum aralık dışında kalıyorsa, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <typeparam name="T">Kontrol edilecek değerin türü.</typeparam>
        /// <param name="value">Kontrol edilecek değer.</param>
        /// <param name="min">Minimum kabul edilen değer.</param>
        /// <param name="max">Maksimum kabul edilen değer.</param>
        /// <param name="argName">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckRange<T>(T value, T min, T max, string argName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0) { throw new ArgumentOutOfRangeException($"\"{argName}\" argümanı, [{min} - {max}] değerleri arasında olmalıdır!", new Exception($"Gelen değer: \"{value}\"")); }
        }
        /// <summary>
        /// Verilen değerin sıfır olup olmadığını kontrol eder.
        /// Eğer değer sıfırsa, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <typeparam name="T">Kontrol edilecek değerin türü.</typeparam>
        /// <param name="value">Kontrol edilecek değer.</param>
        /// <param name="argName">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckZero<T>(T value, string argName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(default(T)) == 0) { throw new ArgumentException($"\"{argName}\" argümanı, \"0 (sıfır)\" olamaz!", argName); }
        }
        /// <summary>
        /// Verilen değerin sıfır ya da negatif olup olmadığını kontrol eder.
        /// Eğer değer sıfır ya da negatifse, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <typeparam name="T">Kontrol edilecek değerin türü.</typeparam>
        /// <param name="value">Kontrol edilecek değer.</param>
        /// <param name="argName">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckZeroOrNegative<T>(T value, string argName) where T : struct, IComparable<T>
        {
            CheckZero(value, argName);
            CheckNegative(value, argName);
        }
        /// <summary>
        /// Verilen değerin negatif olup olmadığını kontrol eder.
        /// Eğer değer negatifse, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <typeparam name="T">Kontrol edilecek değerin türü.</typeparam>
        /// <param name="value">Kontrol edilecek değer.</param>
        /// <param name="argName">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckNegative<T>(T value, string argName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(default(T)) < 0) { throw new ArgumentOutOfRangeException($"\"{argName}\" argümanı, negatif olamaz!", new Exception($"Gelen değer: \"{value}\"")); }
        }
        /// <summary>
        /// Verilen değerin belirtilen Enum türü içinde tanımlı olup olmadığını kontrol eder.
        /// Eğer değer Enum türünde tanımlı değilse, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <param name="type">Kontrol edilecek Enum türü.</param>
        /// <param name="value">Kontrol edilecek değer.</param>
        /// <param name="argName">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckEnumDefined(Type type, object value, string argName)
        {
            CheckNull(type, nameof(type));
            if (!type.IsEnum) { throw new ArgumentException($"\"{type.FullName}\" türü geçerli bir \"{nameof(Enum)}\" türü olmalıdır!", argName); }
            CheckNull(value, argName);
            if (!Enum.IsDefined(type, value)) { throw new ArgumentException($"\"{type.FullName}\" için sağlanan \"{argName}\" argümanının değeri geçersizdir!", argName, new Exception($"Gelen değer: \"{value}\"")); }
        }
        /// <summary>
        /// Belirtilen Enum türünde, verilen değerin tanımlı olup olmadığını kontrol eder.
        /// Eğer değer belirtilen Enum türünde tanımlı değilse, ilgili argüman adıyla birlikte bir hata fırlatır.
        /// </summary>
        /// <typeparam name="T">Kontrol edilecek Enum türü.</typeparam>
        /// <param name="value">Kontrol edilecek değer.</param>
        /// <param name="argName">Hata mesajında kullanılacak argüman adı.</param>
        public static void CheckEnumDefined<T>(object value, string argName) where T : Enum => CheckEnumDefined(typeof(T), value, argName);
        /// <summary>
        /// İki koleksiyonun (ICollection) eleman sayılarının eşit olup olmadığını kontrol eder.
        /// Eğer koleksiyonlardan herhangi biri boşsa veya iki koleksiyonun eleman sayıları eşit değilse,
        /// bir <see cref="ArgumentException"/> fırlatır.
        /// </summary>
        /// <typeparam name="T">Koleksiyonların eleman türü.</typeparam>
        /// <param name="collection1">Karşılaştırılacak ilk koleksiyon.</param>
        /// <param name="collection2">Karşılaştırılacak ikinci koleksiyon.</param>
        /// <exception cref="ArgumentException">
        /// Eğer koleksiyonlardan biri boşsa veya koleksiyonların eleman sayıları eşit değilse fırlatılır.
        /// Örneğin, &quot;collection1 (3)&quot; ve &quot;collection2 (5)&quot; şeklinde bir hata mesajı döner.
        /// </exception>
        public static void CheckEqualCount<T>(ICollection<T> collection1, ICollection<T> collection2)
        {
            CheckEmpty(collection1, nameof(collection1));
            CheckEmpty(collection2, nameof(collection2));
            if (collection1.Count != collection2.Count) { throw new ArgumentException($"\"{nameof(collection1)} ({collection1.Count})\" ve \"{nameof(collection2)} ({collection2.Count})\" nesne sayıları eşit olmalıdır!"); }
        }
        /// <summary>
        /// Belirtilen dil değerinin desteklenen diller arasında olup olmadığını kontrol eder.
        /// Eğer dil değeri, varsayılan desteklenen diller arasında değilse bir <see cref="NotSupportedException"/> hatası fırlatır.
        /// </summary>
        /// <param name="value">Kontrol edilecek dil değeri.</param>
        /// <param name="argName">Dil değerini temsil eden argüman adı.</param>
        /// <param name="defaultLanguages">
        /// Desteklenen varsayılan dil değerleri. 
        /// Eğer boş bırakılırsa veya null değer verilirse &quot;tr&quot; ve &quot;en&quot; olarak varsayılır.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// Belirtilen dil değeri <paramref name="defaultLanguages"/> listesinde yoksa fırlatılır.
        /// Hata mesajında desteklenen dil değerleri ve yöneticilerle iletişime geçilmesi gerektiğine dair bilgi içerir.
        /// </exception>
        public static void UnSupportLanguage(string value, string argName, params string[] defaultLanguages)
        {
            CheckEmpty(value, argName);
            defaultLanguages = (defaultLanguages ?? Array.Empty<string>()).Distinct().ToArray();
            if (defaultLanguages.Length == 0) { defaultLanguages = new string[] { "tr", "en" }; }
            if (!defaultLanguages.Contains(value)) { throw new NotSupportedException($"{argName}; {String.Join(", ", defaultLanguages)} değerlerinden biri olabilir!", new Exception("Yönetici ile iletişime geçiniz!")); }
        }
    }
}