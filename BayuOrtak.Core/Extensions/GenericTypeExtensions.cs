namespace BayuOrtak.Core.Extensions
{
    using System;
    using System.ComponentModel;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public static class GenericTypeExtensions
    {
        /// <summary>
        /// Verilen değeri varsayılan (default) değerine eşit ise null döner; aksi halde değeri döner.
        /// </summary>
        /// <typeparam name="T">Değer türü.</typeparam>
        /// <param name="value">Değer.</param>
        /// <returns>Null veya verilen değeri döner.</returns>
        public static T? NullIfOrDefault<T>(this T value) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(value, default(T))) { return default; }
            return value;
        }
        /// <summary>
        /// Nullable türde verilen değeri varsayılan (default) değere eşit ise null döner; aksi halde değeri döner.
        /// </summary>
        /// <typeparam name="T">Değer türü.</typeparam>
        /// <param name="value">Nullable değer.</param>
        /// <returns>Null veya verilen değeri döner.</returns>
        public static T? NullIfOrDefault<T>(this T? value) where T : struct => (value.HasValue ? value.Value.NullIfOrDefault() : default);
        /// <summary>
        /// Verilen değerin belirtilen değerler arasında olup olmadığını kontrol eder.
        /// </summary>
        /// <typeparam name="T">Değer türü.</typeparam>
        /// <param name="value">Değer.</param>
        /// <param name="values">Kontrol edilecek değerler.</param>
        /// <returns>True, eğer değer belirtilen değerler arasında ise; aksi halde false döner.</returns>
        public static bool Includes<T>(this T value, params T[] values) => (values ?? Array.Empty<T>()).Contains(value);
        /// <summary>
        /// Verilen enum değerinin açıklamasını döner.
        /// </summary>
        /// <typeparam name="T">Enum türü.</typeparam>
        /// <param name="value">Enum değeri.</param>
        /// <returns>Enum açıklaması; açıklama yoksa boş dize döner.</returns>
        public static string GetDescription<T>(this T value) where T : Enum
        {
            var t = typeof(T);
            try { return (_try.TryCustomAttribute(t.GetField(Enum.GetName(t, value)), out DescriptionAttribute _d) ? _d.Description : ""); }
            catch { return ""; }
        }
        /// <summary>
        /// Tekil bir değeri enumerable (koleksiyon) olarak döner.
        /// </summary>
        /// <typeparam name="T">Değer türü.</typeparam>
        /// <param name="source">Kaynak değer.</param>
        /// <returns>Koleksiyon.</returns>
        public static IEnumerable<T> ToEnumerable<T>(this T source)
        {
            yield return source;
        }
    }
}