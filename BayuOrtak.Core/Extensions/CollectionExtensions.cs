namespace BayuOrtak.Core.Extensions
{
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Helper.Results;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    public static class CollectionExtensions
    {
        #region IDictionary
        /// <summary>
        /// Belirtilen anahtar ve değeri bir sözlüğe ekler. Eğer anahtar zaten mevcutsa, değeri günceller; 
        /// mevcut değilse, yeni bir anahtar-değer çifti olarak ekler.
        /// </summary>
        /// <typeparam name="K">Sözlükteki anahtar türü.</typeparam>
        /// <typeparam name="V">Sözlükteki değer türü.</typeparam>
        /// <param name="dictionary">Üzerinde işlem yapılacak sözlük.</param>
        /// <param name="key">Eklenecek veya güncellenecek anahtar.</param>
        /// <param name="value">Anahtarla ilişkilendirilecek değer.</param>
        public static void Upsert<K, V>(this IDictionary<K, V> dictionary, K key, V value)
        {
            if (!dictionary.TryAdd(key, value)) { dictionary[key] = value; }
        }
        /// <summary>
        /// Verilen koşula uyan tüm anahtar-değer çiftlerini sözlükten kaldırır.
        /// </summary>
        /// <typeparam name="K">Anahtar tipi.</typeparam>
        /// <typeparam name="V">Değer tipi.</typeparam>
        /// <param name="dictionary">Anahtar-değer çiftlerinin bulunduğu sözlük.</param>
        /// <param name="predicate">Kaldırılacak öğeleri belirleyen bir koşul.</param>
        public static void RemoveWhere<K, V>(this IDictionary<K, V> dictionary, Func<KeyValuePair<K, V>, bool> predicate)
        {
            foreach (var key in dictionary.Where(predicate).Select(x => x.Key).ToArray()) { dictionary.Remove(key); }
        }
        /// <summary>
        /// Bir <see cref="Dictionary{TKey, TValue}"/> kaynağından açılır liste oluşturur.
        /// </summary>
        /// <param name="source">Açılır liste için kullanılan kaynak sözlüğü. Null değerler için varsayılan boş bir sözlük oluşturulur.</param>
        /// <param name="optiontitle">Açılır listede gösterilecek metin.</param>
        /// <param name="optionvalue">Açılır listede kullanılacak değer.</param>
        /// <param name="selectedvalue">Varsayılan olarak seçili öğenin değeri (boş dize varsayılan).</param>
        /// <param name="firstitem">Listede ilk eleman olarak eklenebilecek bir <see cref="SelectListItem"/>. Varsayılan olarak null.</param>
        /// <returns>Oluşturulan <see cref="List{SelectListItem}"/>.</returns>
        public static List<SelectListItem> ToDropDownList_group(this IDictionary<string, object[]> source, string optiontitle, string optionvalue, string selectedvalue = "", SelectListItem firstitem = null)
        {
            var ret = new List<SelectListItem>();
            source = source ?? new Dictionary<string, object[]>();
            foreach (var item in source)
            {
                ret.AddRange(item.Value.Select(x => new
                {
                    t = Convert.ToString(x.GetType().GetProperty(optiontitle).GetValue(x)),
                    v = Convert.ToString(x.GetType().GetProperty(optionvalue).GetValue(x))
                }).ToDropDownList_private("t", "v", selectedvalue, default, new SelectListGroup
                {
                    Name = item.Key
                }));
            }
            if (firstitem != null) { ret.Insert(0, firstitem); }
            return ret;
        }
        /// <summary>
        /// Belirtilen anahtara (key) göre bir sözlükten (IDictionary) değer çekip, belirli bir türe <typeparamref name="T"/> dönüştürür.
        /// Eğer sözlük veya anahtar geçersizse, varsayılan değeri döndürür.
        /// </summary>
        /// <typeparam name="T">Dönüştürülecek veri türü.</typeparam>
        /// <param name="dic">Anahtar-değer çiftleri içeren sözlük.</param>
        /// <param name="key">Sözlükte aranan anahtar (key).</param>
        /// <returns>
        /// Sözlükte belirtilen anahtara karşılık gelen değeri <typeparamref name="T"/> türüne dönüştürülmüş şekilde döndürür.
        /// Anahtar yoksa veya geçersizse, <typeparamref name="T"/> türünün varsayılan değerini döndürür.
        /// </returns>
        public static T ToKeyValueParseOrDefault_dictionary<T>(this IDictionary<string, string> dic, string key)
        {
            dic = dic ?? new Dictionary<string, string>();
            if (!key.IsNullOrEmpty_string() && dic.TryGetValue(key, out string _value)) { return _value.ParseOrDefault<T>(); }
            return default;
        }
        /// <summary>
        /// ModelStateDictionary nesnesine bir dizi hata mesajını topluca eklemek için kullanılan bir genişletme metodu.
        /// </summary>
        /// <param name="modelstate">Hataların ekleneceği ModelStateDictionary nesnesi.</param>
        /// <param name="errors">Eklenecek hata mesajlarını içeren string listesi.</param>
        /// <remarks>
        /// Bu metot, verilen hata mesajları listesindeki her bir öğeyi
        /// ModelState&#39;e tek tek ekler. Key olarak boş bir string (&quot;&quot;) kullanılır.
        /// Eğer <paramref name="errors"/> null ise işlem yapılmaz.
        /// </remarks>
        public static void AddModelErrorRange(this ModelStateDictionary modelstate, IEnumerable<string> errors)
        {
            if (errors != null) { foreach (var item in errors) { modelstate.AddModelError("", item); } }
        }
        #endregion
        #region IEnumerable
        /// <summary>
        /// Verilen koleksiyondaki tüm nesneleri temizler ve dispose eder.
        /// </summary>
        /// <param name="values">Dispose edilecek nesneleri içeren koleksiyon.</param>
        public static void DisposeAll<T>(this IEnumerable<T> values) where T : IDisposable
        {
            if (values != null) { foreach (var value in values) { value.Dispose(); } }
        }
        /// <summary>
        /// İki koleksiyon arasında bir sol dış birleşim (left join) gerçekleştiren bir yöntemdir. 
        /// Her öğe için bir anahtar kullanır ve sağdaki koleksiyondan bir eşleşme olup olmadığını kontrol eder.
        /// </summary>
        /// <typeparam name="TLeft">Sol koleksiyon tipi.</typeparam>
        /// <typeparam name="TRight">Sağ koleksiyon tipi.</typeparam>
        /// <typeparam name="TKey">Anahtar tipi.</typeparam>
        /// <param name="left">Sol koleksiyon.</param>
        /// <param name="right">Sağ koleksiyon.</param>
        /// <param name="leftKey">Sol koleksiyondaki anahtar için bir seçici.</param>
        /// <param name="rightKey">Sağ koleksiyondaki anahtar için bir seçici.</param>
        /// <returns>Sol dış birleşim sonucu içeren bir IEnumerable.</returns>
        public static IEnumerable<LeftJoinResult<TLeft, TRight>> LeftJoinEnumerable_onetoone<TLeft, TRight, TKey>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TKey> leftKey, Func<TRight, TKey> rightKey) where TLeft : class where TRight : class => left.GroupJoin(right, leftKey, rightKey, (l, r) => new
        {
            l,
            r
        }).SelectMany(x => x.r.DefaultIfEmpty(), (l, r) => new LeftJoinResult<TLeft, TRight>
        {
            left = l.l,
            righthasvalue = r != null,
            right = r
        });
        /// <summary>
        /// İki koleksiyon arasında bir sol dış birleşim (left join) gerçekleştiren bir yöntemdir. 
        /// Sağ koleksiyondan birden fazla eşleşmeyi destekler.
        /// </summary>
        /// <typeparam name="TLeft">Sol koleksiyon tipi.</typeparam>
        /// <typeparam name="TRight">Sağ koleksiyon tipi.</typeparam>
        /// <typeparam name="TKey">Anahtar tipi.</typeparam>
        /// <param name="left">Sol koleksiyon.</param>
        /// <param name="right">Sağ koleksiyon.</param>
        /// <param name="leftKey">Sol koleksiyondaki anahtar için bir seçici.</param>
        /// <param name="rightKey">Sağ koleksiyondaki anahtar için bir seçici.</param>
        /// <returns>Sol dış birleşim sonucu içeren bir IEnumerable.</returns>
        public static IEnumerable<LeftJoinResult<TLeft, TRight[]>> LeftJoinEnumerable_onetomany<TLeft, TRight, TKey>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TKey> leftKey, Func<TRight, TKey> rightKey) where TLeft : class where TRight : class => left.GroupJoin(right, leftKey, rightKey, (l, r) => new
        {
            l,
            r = (r == null ? Array.Empty<TRight>() : r.ToArray())
        }).Select(x => new LeftJoinResult<TLeft, TRight[]>
        {
            left = x.l,
            righthasvalue = x.r.Length > 0,
            right = x.r
        });
        /// <summary>
        /// Belirtilen bir kaynak koleksiyonundan bir <see cref="List{SelectListItem}"/> oluşturur.
        /// </summary>
        /// <typeparam name="T">Koleksiyondaki öğelerin türü.</typeparam>
        /// <param name="source">Açılır liste için kullanılan kaynak koleksiyonu. Null olabilir.</param>
        /// <param name="optiontitle">Açılır listede gösterilecek metin.</param>
        /// <param name="optionvalue">Açılır listede kullanılacak değer.</param>
        /// <param name="selectedvalue">Varsayılan olarak seçili öğenin değeri (boş dize varsayılan).</param>
        /// <param name="firstitem">Listede ilk eleman olarak eklenebilecek bir <see cref="SelectListItem"/>. Varsayılan olarak null.</param>
        /// <returns>Oluşturulan <see cref="List{SelectListItem}"/>.</returns>
        public static List<SelectListItem> ToDropDownList<T>(this IEnumerable<T> source, string optiontitle, string optionvalue, string selectedvalue = "", SelectListItem firstitem = null) where T : class => source.ToDropDownList_private(optiontitle, optionvalue, selectedvalue, firstitem, default);
        private static List<SelectListItem> ToDropDownList_private<T>(this IEnumerable<T> source, string optiontitle, string optionvalue, string selectedvalue, SelectListItem firstitem, SelectListGroup group) where T : class
        {
            var ret = new List<SelectListItem>();
            if (source != null && source.Any())
            {
                var _hv = !selectedvalue.IsNullOrEmpty_string();
                ret.AddRange(source.Select(item => new
                {
                    item,
                    type = item.GetType()
                })
                .Select(x => new
                {
                    Text = x.type.GetProperty(optiontitle).GetValue(x.item).ToStringOrEmpty(),
                    Value = x.type.GetProperty(optionvalue).GetValue(x.item).ToStringOrEmpty()
                })
                .Select(x => new SelectListItem
                {
                    Text = x.Text,
                    Value = x.Value,
                    Selected = (_hv && x.Value == selectedvalue),
                    Group = group
                }).ToArray());
            }
            if (firstitem != null) { ret.Insert(0, firstitem); }
            return ret;
        }
        /// <summary>
        /// Kaynakdaki elemanların sırasını <b>Fisher-Yates algoritmasını</b> kullanarak rastgele karıştırır ve 
        /// karıştırılmış bir ICollection olarak geri döner.
        /// </summary>
        /// <typeparam name="T">Kaynağın eleman türü.</typeparam>
        /// <param name="source">Rastgele sıralanacak orijinal kaynak.</param>
        /// <returns>Karıştırılmış elemanları içeren yeni bir ICollection&lt;T&gt; örneği.</returns>
        /// <remarks>
        /// Bu metot, verilen IEnumerable&lt;T&gt; kaynakdaki elemanların yerini <b>Fisher-Yates algoritması</b> ile rastgele değiştirir.
        /// Karıştırılmış elemanları yeni bir ICollection&lt;T&gt; olarak döndürür.
        /// &quot;Random.Shared&quot; ile tek bir Random örneği paylaşılır, bu da çoklu iş parçacıklı senaryolarda daha güvenilir bir kullanım sağlar.
        /// </remarks>
        public static ICollection<T> Shuffle<T>(this IEnumerable<T> source)
        {
            T temp;
            var r = source.ToList();
            int i, j, n = r.Count, nm = (n - 1);
            for (i = nm; i > 0; i--)
            {
                j = Random.Shared.Next(0, i + 1);
                temp = r[i];
                r[i] = r[j];
                r[j] = temp;
            }
            return r;
        }
        #endregion
        #region ICollection
        /// <summary>
        /// İki koleksiyonun eşit olup olmadığını kontrol eder.
        /// </summary>
        /// <typeparam name="T">Koleksiyon tipi.</typeparam>
        /// <param name="left">Soldaki koleksiyon.</param>
        /// <param name="right">Sağdaki koleksiyon.</param>
        /// <returns>İki koleksiyon eşitse <see langword="true"/>, aksi halde <see langword="false"/> döner.</returns>
        public static bool IsEqual<T>(this ICollection<T> left, ICollection<T> right)
        {
            var leftisNull = left == null;
            var rightisNull = right == null;
            if (((leftisNull || rightisNull) && leftisNull == rightisNull)) { return true; }
            if (!leftisNull && !rightisNull && left.All(right.Contains) && left.Count == right.Count) { return true; }
            return false;
        }
        /// <summary>
        /// Verilen koleksiyonun boş veya null olup olmadığını kontrol eder.
        /// </summary>
        /// <typeparam name="T">Koleksiyon tipi.</typeparam>
        /// <param name="values">Kontrol edilecek koleksiyon.</param>
        /// <returns>Boş veya null ise <see langword="true"/>, aksi halde <see langword="false"/> döner.</returns>
        public static bool IsNullOrEmpty_collection<T>(this ICollection<T> values) => (values == null || values.Count == 0);
        /// <summary>
        /// Başka bir koleksiyondan mevcut koleksiyona öğeleri topluca ekler. 
        /// List&lt;T&gt; için optimize edilmiş bir yöntemdir.
        /// </summary>
        /// <typeparam name="T">Koleksiyon tipi.</typeparam>
        /// <param name="initial">Öğelerin ekleneceği mevcut koleksiyon.</param>
        /// <param name="other">Eklenecek öğeleri içeren diğer koleksiyon.</param>
        public static void AddRangeOptimized<T>(this ICollection<T> initial, IEnumerable<T> other)
        {
            if (other != null && other.Any())
            {
                if (initial is List<T> _l) { _l.AddRange(other); }
                else { foreach (var l in other) { initial.Add(l); } }
            }
        }
        /// <summary>
        /// Başka bir NameValueCollection&#39;dan mevcut NameValueCollection&#39;a tüm anahtarları ve değerleri ekler.
        /// </summary>
        /// <param name="initial">Anahtar-değer çiftlerini alacak mevcut NameValueCollection.</param>
        /// <param name="other">Eklenecek anahtar-değer çiftlerini içeren diğer NameValueCollection.</param>
        public static void AddAllKeysAndValues(this NameValueCollection initial, NameValueCollection other)
        {
            Guard.CheckNull(initial, nameof(initial));
            if (other != null) { foreach (var item in other.AllKeys) { initial.Add(item, other[item]); } }
        }
        #endregion
        #region string[]
        /// <summary>
        /// Hata mesajları dizisini iç içe geçmiş istisnalara dönüştürür.
        /// Dizi boş ise bir <see cref="ArgumentNullException"/> döndürür.
        /// </summary>
        /// <param name="errors">Hata mesajlarının yer aldığı dizi.</param>
        /// <returns>İç içe geçmiş <see cref="Exception"/> nesnesi.</returns>
        /// <exception cref="ArgumentNullException">Hata mesajları dizisi null veya boş olduğunda.</exception>
        public static Exception ToNestedException(this string[] errors)
        {
            errors = (errors ?? Array.Empty<string>()).Reverse().ToArray();
            if (errors.Length == 0) { return new ArgumentNullException(nameof(errors)); }
            Exception r = null;
            var i = errors.Length - 1;
            while (i >= 0)
            {
                if (r == null) { r = new Exception(errors[i]); }
                else { r = new Exception(errors[i], r); }
                i--;
            }
            return r;
        }
        #endregion
    }
}