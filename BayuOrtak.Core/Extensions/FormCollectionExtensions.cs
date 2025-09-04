namespace BayuOrtak.Core.Extensions
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;
    public static class FormCollectionExtensions
    {
        /// <summary>
        /// Belirtilen anahtar ile form verilerinden bir değeri alır ve belirtilen türde bir nesneye dönüştürür.
        /// </summary>
        /// <typeparam name="T">Dönüştürülecek nesne türü.</typeparam>
        /// <param name="form">Form koleksiyonu.</param>
        /// <param name="key">Anahtar adı.</param>
        /// <returns>Belirtilen türdeki değeri döndürür; anahtar bulunamazsa varsayılan değer döner.</returns>
        public static T ToKeyValueParseOrDefault_formcollection<T>(this IFormCollection form, string key)
        {
            if (form.TryGetValue_string(key, out string _value)) { return _value.ParseOrDefault<T>(); }
            return default;
        }
        /// <summary>
        /// Form koleksiyonundan belirtilen anahtar ile bir dize değerini alır.
        /// </summary>
        /// <param name="form">Form koleksiyonu.</param>
        /// <param name="key">Anahtar adı.</param>
        /// <param name="outvalue">Dönüştürülen dize değeri.</param>
        /// <returns>Değer başarıyla alındıysa <see langword="true"/>, aksi takdirde <see langword="false"/> döner.</returns>
        public static bool TryGetValue_string(this IFormCollection form, string key, out string outvalue)
        {
            var _r = form.TryGetValue(key, out StringValues _sv);
            if (_r)
            {
                outvalue = _sv.ToStringOrEmpty();
                return true;
            }
            outvalue = "";
            return false;
        }
        /// <summary>
        /// Bir IFormCollection nesnesinden belirtilen anahtara karşılık gelen değerleri belirli bir türde dizi olarak elde etmeye çalışır.
        /// </summary>
        /// <typeparam name="T">Dönüştürülmek istenen değerlerin türü.</typeparam>
        /// <param name="form">Değerin aranacağı IFormCollection nesnesi.</param>
        /// <param name="key">Hedef değerin anahtarı. Anahtarın &quot;[]&quot; ile bitmesi beklenir, aksi takdirde otomatik olarak eklenir.</param>
        /// <param name="outvalues">Belirtilen türdeki değerleri içeren çıktı dizisi. Anahtar bulunamazsa boş bir dizi döner.</param>
        /// <returns>
        /// Anahtar bulunduğunda ve değerler belirtilen türe dönüştürüldüğünde <see langword="true"/> döner, aksi takdirde <see langword="false"/> döner.
        /// </returns>
        /// <remarks>
        /// Bu metot, bir form verisindeki (IFormCollection) belirli bir anahtara karşılık gelen değerleri belirtilen türe dönüştürerek bir dizi olarak döndürmek için kullanılır. Eğer anahtar &quot;[]&quot; ile bitmiyorsa, otomatik olarak eklenir. Dönüştürme sırasında hata oluşursa, varsayılan değerler kullanılır.
        /// </remarks>
        public static bool TryGetValue_array<T>(this IFormCollection form, string key, out T[] outvalues)
        {
            key = key.ToStringOrEmpty();
            if (key != "")
            {
                if (!key.EndsWith("[]")) { key = String.Concat(key, "[]"); }
                if (form.TryGetValue(key, out StringValues _sv))
                {
                    outvalues = _sv.Select(x => x.ParseOrDefault<T>()).ToArray();
                    return true;
                }
            }
            outvalues = Array.Empty<T>();
            return false;
        }
        /// <summary>
        /// Belirtilen anahtar - değer çiftini form koleksiyonuna ekler veya günceller. Eğer <paramref name="key"/> null veya boş ise, form koleksiyonu değiştirilmeden geri döndürülür. Aksi takdirde, belirtilen <paramref name="key"/> ve <paramref name="value"/> ile yeni bir form koleksiyonu oluşturulur. Eğer anahtar koleksiyonda zaten varsa, değeri güncellenir; yoksa yeni bir anahtar-değer çifti eklenir.
        /// <para>
        /// Bu metod, formun dosya koleksiyonunu (<see cref="IFormCollection.Files"/>) korur.
        /// </para>
        /// </summary>
        /// <param name="form">İşlem yapılacak form koleksiyonu.</param>
        /// <param name="key">Eklenecek veya güncellenecek anahtarın adı.</param>
        /// <param name="value">Anahtara atanacak değer(ler). Tek bir string veya birden fazla string içerebilir.</param>
        /// <returns>
        /// Güncellenmiş veya eklenmiş anahtar - değer çifti ile yeni bir <see cref="IFormCollection"/> nesnesi. Eğer <paramref name="key"/> null veya boş ise, orijinal form koleksiyonu döndürülür.
        /// </returns>
        public static IFormCollection Upsert(this IFormCollection form, string key, StringValues value)
        {
            if (key.IsNullOrEmpty_string()) { return form; }
            var _dic = new Dictionary<string, StringValues>(form);
            if (!_dic.TryAdd(key, value)) { _dic[key] = value; }
            return new FormCollection(_dic, form.Files);
        }
    }
}