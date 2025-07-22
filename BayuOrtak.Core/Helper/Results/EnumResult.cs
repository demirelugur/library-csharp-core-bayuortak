namespace BayuOrtak.Core.Helper.Results
{
    using BayuOrtak.Core.Extensions;
    using Microsoft.AspNetCore.Http;
    using System;
    public class EnumResult : IEquatable<EnumResult>
    {
        #region Equals
        public override bool Equals(object other) => this.Equals(other as EnumResult);
        public override int GetHashCode() => HashCode.Combine(this.vl, this.tx, this.desc);
        public bool Equals(EnumResult other) => other != null && this.vl == other.vl && this.tx == other.tx && this.desc == other.desc;
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
        /// <see cref="desc"/> değerinin SEO dostu bir dizeye dönüştürür.
        /// </summary>
        public string descseo => this.desc.ToSeoFriendly();
        public EnumResult() : this(default, "", "") { }
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