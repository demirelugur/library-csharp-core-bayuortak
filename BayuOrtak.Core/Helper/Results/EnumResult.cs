namespace BayuOrtak.Core.Helper.Results
{
    using BayuOrtak.Core.Extensions;
    using Microsoft.AspNetCore.Http;
    using System;
    public class EnumResult : IEquatable<EnumResult>
    {
        #region Equals
        /// <summary>
        /// Enum değerinin eşitliğini kontrol eder.
        /// </summary>
        /// <param name="other">Karşılaştırılacak diğer <see cref="EnumResult"/> nesnesi.</param>
        /// <returns>Eşitlik durumu.</returns>
        public override bool Equals(object other) => this.Equals(other as EnumResult);
        /// <summary>
        /// Hash kodunu döndürür.
        /// </summary>
        /// <returns>Hash kodu.</returns>
        public override int GetHashCode() => HashCode.Combine(this.vl, this.tx, this.desc);
        /// <summary>
        /// Başka bir <see cref="EnumResult"/> nesnesiyle eşitliği kontrol eder.
        /// </summary>
        /// <param name="other">Karşılaştırılacak diğer <see cref="EnumResult"/> nesnesi.</param>
        /// <returns>Eşitlik durumu.</returns>
        public bool Equals(EnumResult other)
        {
            if (other == null) { return false; }
            return (this.vl == other.vl && this.tx == other.tx && this.desc == other.desc);
        }
        #endregion
        /// <summary>
        /// Enum değerinin sayısal karşılığını alır.
        /// </summary>
        public long vl { get; }
        /// <summary>
        /// Enum değerinin metin karşılığını alır.
        /// </summary>
        public string tx { get; }
        /// <summary>
        /// Enum değerinin açıklamasını alır.
        /// </summary>
        public string desc { get; }
        /// <summary>
        /// Enum açıklamasını SEO dostu bir dizeye dönüştürür.
        /// </summary>
        public string descseo => this.desc.ToSeoFriendly();
        /// <summary>
        /// Yeni bir <see cref="EnumResult"/> nesnesi oluşturur. Varsayılan değerlerle başlatılır.
        /// </summary>
        public EnumResult() : this(default, "", "") { }
        /// <summary>
        /// Belirtilen değerle birlikte yeni bir <see cref="EnumResult"/> nesnesi oluşturur.
        /// </summary>
        /// <param name="vl">Enum değerinin sayısal karşılığı.</param>
        /// <param name="tx">Enum değerinin metin karşılığı.</param>
        /// <param name="desc">Enum değerinin açıklaması.</param>
        public EnumResult(long vl, string tx, string desc)
        {
            this.vl = vl;
            this.tx = tx;
            this.desc = desc;
        }
        /// <summary>
        /// value için tanımlanan nesneler: EnumResult, IFormCollection, String, Enum, AnonymousObjectClass
        /// </summary>
        public static EnumResult ToEntityFromObject(object value)
        {
            if (value == null) { return new EnumResult(); }
            if (value is EnumResult _er) { return _er; }
            if (value is IFormCollection _form)
            {
                return ToEntityFromObject(new
                {
                    vl = _form.ToKeyValueParseOrDefault_formcollection<long>(nameof(vl)),
                    tx = _form.ToKeyValueParseOrDefault_formcollection<string>(nameof(tx)) ?? "",
                    desc = _form.ToKeyValueParseOrDefault_formcollection<string>(nameof(desc)) ?? ""
                });
            }
            var _t = value.GetType();
            if (value is String _name && Enum.IsDefined(_t, _name)) { return _t.ToEnumArray().Where(x => x.tx == _name).FirstOrDefault() ?? new EnumResult(); }
            if (_t.IsEnum)
            {
                var _v = Convert.ToInt64(value);
                return _t.ToEnumArray().Where(x => x.vl == _v).FirstOrDefault() ?? new EnumResult();
            }
            return value.ToEnumerable().Select(x => x.ToDynamic()).Select(x => new EnumResult((long)x.vl, (string)x.tx, (string)x.desc)).FirstOrDefault();
        }
    }
}