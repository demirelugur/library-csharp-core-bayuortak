namespace BayuOrtak.Core.Extensions
{
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Helper.Results;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public static class IQueryableExtensions
    {
        /// <summary>
        /// IQueryable kaynaklarının sayfalama işlemini gerçekleştirir.
        /// </summary>
        /// <typeparam name="T">Sayfalama yapılacak tür.</typeparam>
        /// <param name="source">Sayfalama işlemi yapılacak IQueryable kaynağı.</param>
        /// <param name="pagenumber">Sayfa numarası (1 tabanlı).</param>
        /// <param name="take">Her sayfada gösterilecek kayıt sayısı.</param>
        /// <returns>Paginasyon yapılmış IQueryable kaynak.</returns>
        public static IQueryable<T> Paginate<T>(this IQueryable<T> source, int pagenumber, int take) where T : class
        {
            Guard.CheckZeroOrNegative(pagenumber, nameof(pagenumber));
            return source.Skip((pagenumber - 1) * take).Take(take);
        }
        /// <summary>
        /// İki IQueryable arasında 1-1 sol birleştirme (left join) işlemi gerçekleştirir.
        /// </summary>
        /// <typeparam name="TLeft">Sol taraftaki nesne türü.</typeparam>
        /// <typeparam name="TRight">Sağ taraftaki nesne türü.</typeparam>
        /// <typeparam name="TKey">Birleştirme için anahtar türü.</typeparam>
        /// <param name="left">Sol IQueryable kaynak.</param>
        /// <param name="right">Sağ IQueryable kaynak.</param>
        /// <param name="leftkey">Sol kaynak için anahtar ifadesi.</param>
        /// <param name="rightkey">Sağ kaynak için anahtar ifadesi.</param>
        /// <returns>Birleştirilmiş sonuçları içeren IQueryable kaynak.</returns>
        public static IQueryable<LeftJoinResult<TLeft, TRight>> LeftJoinQueryable_onetoone<TLeft, TRight, TKey>(this IQueryable<TLeft> left, IQueryable<TRight> right, Expression<Func<TLeft, TKey>> leftkey, Expression<Func<TRight, TKey>> rightkey) where TLeft : class where TRight : class => left.GroupJoin(right, leftkey, rightkey, (l, r) => new
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
        /// Seçilen ifadeye göre ilk kayıt veya varsayılan değeri asenkron olarak getirir.
        /// </summary>
        public static Task<TObject> SelectThenFirstOrDefaultAsync<T, TObject>(this IQueryable<T> source, Expression<Func<T, TObject>> selector, CancellationToken cancellationtoken) where T : class => source.Select(selector).FirstOrDefaultAsync(cancellationtoken);
        /// <summary>
        /// Verilen ifade ile maksimum değeri asenkron olarak getirir, yoksa varsayılan değeri döner.
        /// </summary>
        public static async Task<TKey> MaxOrDefaultAsync<T, TKey>(this IQueryable<T> source, Expression<Func<T, TKey>> selector, CancellationToken cancellationtoken) where T : class => (await source.AnyAsync(cancellationtoken) ? await source.MaxAsync(selector, cancellationtoken) : default(TKey));
        /// <summary>
        /// Verilen ifade ile minimum değeri asenkron olarak getirir, yoksa varsayılan değeri döner.
        /// </summary>
        public static async Task<TKey> MinOrDefaultAsync<T, TKey>(this IQueryable<T> source, Expression<Func<T, TKey>> selector, CancellationToken cancellationtoken) where T : class => (await source.AnyAsync(cancellationtoken) ? await source.MinAsync(selector, cancellationtoken) : default(TKey));
        /// <summary>
        /// Verilen IQueryable kaynağındaki sayısal değerlerin toplamını asenkron olarak getirir, yoksa varsayılan değeri döner.
        /// </summary>
        public static async Task<TKey> SumOrDefaultAsync<TKey>(this IQueryable<TKey> source, CancellationToken cancellationtoken) where TKey : struct, IConvertible
        {
            if (!await source.AnyAsync(cancellationtoken)) { return default(TKey); }
            object value;
            var _elementtype = (source.ElementType.IsEnum ? Enum.GetUnderlyingType(source.ElementType) : source.ElementType);
            if (_elementtype == typeof(int)) { value = await source.Cast<int>().SumAsync(cancellationtoken); }
            else if (_elementtype == typeof(long)) { value = await source.Cast<long>().SumAsync(cancellationtoken); }
            else if (_elementtype == typeof(decimal)) { value = await source.Cast<decimal>().SumAsync(cancellationtoken); }
            else if (_elementtype == typeof(double)) { value = await source.Cast<double>().SumAsync(cancellationtoken); }
            else if (_elementtype == typeof(float)) { value = await source.Cast<float>().SumAsync(cancellationtoken); }
            else { throw new NotSupportedException($"\"{source.ElementType.FullName}\" türü uyumsuzdur!"); }
            return (TKey)_other.ChangeType(value, typeof(TKey));
        }
        /// <summary>
        /// Verilen metin için benzersiz bir SEO dostu string oluşturur.
        /// </summary>
        public static async Task<string> GenerateUniqueSEOStringAsync(this IQueryable<string> source, string text, int maxlength, string dil, CancellationToken cancellationtoken)
        {
            var i = 0;
            string r, _t = text.ToSeoFriendly();
            Guard.CheckEmpty(_t, nameof(_t));
            while (true)
            {
                r = (i == 0 ? _t : String.Join("-", _t, i.ToString()));
                if (!await source.ContainsAsync(r, cancellationtoken)) { break; }
                i++;
            }
            if (r.Length <= maxlength) { return r; }
            Guard.UnSupportLanguage(dil, nameof(dil));
            if (dil == "en") { throw new ArgumentOutOfRangeException($"The generated SEO data exceeds the maximum length of {maxlength.ToString()} characters!", new Exception($"Value: \"{text}\"")); }
            throw new ArgumentOutOfRangeException($"Oluşturulan SEO verisi {maxlength.ToString()} karakterlik maksimum uzunluğu aşıyor!", new Exception($"Gelen değer: \"{text}\""));
        }
    }
}