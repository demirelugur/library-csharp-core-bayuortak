namespace BayuOrtak.Core.Base
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper.Results;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static BayuOrtak.Core.Helper.OrtakTools;
    /// <summary>
    /// Enum&#39;lar için temel bir sınıf.
    /// </summary>
    /// <typeparam name="T">Enum türü</typeparam>
    public abstract class BaseEnum<T> where T : Enum
    {
        private static readonly Type _type = typeof(T);
        /// <summary>
        /// Enum&#39;un alttaki veri türü.
        /// </summary>
        public static readonly Type UnderlyingType = _type.GetEnumUnderlyingType();
        /// <summary>
        /// Enum türündeki tüm değerleri içeren dizi.
        /// </summary>
        public static readonly T[] EnumArray = (T[])Enum.GetValues(_type);
        /// <summary>
        /// Enum türündeki detayları içeren dizi.
        /// </summary>
        public static readonly EnumResult[] EnumArrayDetail = _type.ToEnumArray();
        /// <summary>
        /// Enum&#39;dan bir sözlük olarak değerleri döndürür.
        /// </summary>
        public static readonly Dictionary<string, long> ToDictionaryFromEnum = _to.ToDictionaryFromEnum<T>();
        /// <summary>
        /// Belirtilen değerin enum&#39;da tanımlı olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="value">Kontrol edilecek değer</param>
        /// <returns>Tanımlı ise <see langword="true"/>, aksi halde <see langword="false"/></returns>
        public static bool IsDefined(object value) => Enum.IsDefined(_type, value);
        /// <summary>
        /// Belirtilen bayrak değerine sahip enum değerlerini döndürür.
        /// </summary>
        /// <param name="flagValue">Bayrak değeri</param>
        /// <returns>Bayrak değerine sahip enum değerleri</returns>
        public static T[] FlagEnumArray(T flagValue) => EnumArray.Where(x => flagValue.HasFlag(x)).ToArray();
    }
}