namespace BayuOrtak.Core.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Extensions;
    using System;
    using System.ComponentModel.DataAnnotations;
    /// <summary>
    /// Belirtilen değerlerin bir koleksiyonda bulunup bulunmadığını doğrulayan özel bir nitelik.
    /// </summary>
    /// <typeparam name="T">Doğrulanan değer türü.</typeparam>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class Validation_IncludesAttribute<T> : ValidationAttribute
    {
        /// <summary>
        /// Değerlerin eşit olup olmadığını kontrol eder.
        /// </summary>
        public bool isequal { get; }
        /// <summary>
        /// Eşleşen değerler listesi.
        /// </summary>
        public T[] values { get; }
        /// <summary>
        /// Yeni bir <see cref="Validation_IncludesAttribute{T}"/> örneği oluşturur.
        /// </summary>
        /// <param name="isequal">Değerlerin eşit olup olmadığını belirtir.</param>
        /// <param name="values">Eşleşmesi gereken değerler.</param>
        public Validation_IncludesAttribute(bool isequal, params T[] values)
        {
            this.isequal = isequal;
            this.values = (values ?? Array.Empty<T>());
        }
        /// <summary>
        /// Belirtilen değerin geçerliliğini doğrular.
        /// </summary>
        /// <param name="value">Doğrulanan değer.</param>
        /// <param name="validationContext">Doğrulama bağlamı.</param>
        /// <returns>Geçerli ise <see cref="ValidationResult.Success"/>; aksi takdirde hata mesajı ile birlikte <see cref="ValidationResult"/> döner.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var valueIsNull = value == null;
            var _v = valueIsNull ? default(T) : (T)Convert.ChangeType(value, typeof(T));
            if (!valueIsNull && this.values.Contains(_v) == this.isequal) { return ValidationResult.Success; }
            if (!validationContext.IsRequiredAttribute())
            {
                if (valueIsNull) { return ValidationResult.Success; }
                if (typeof(T) == typeof(string) && value is String _s && _s.IsNullOrEmpty_string()) { return ValidationResult.Success; } // if (typeof(T) == typeof(int) && value is Int32 _i && _i == 0) { return ValidationResult.Success; }
            }
            if (this.isequal) { return new ValidationResult(this.ErrorMessage ?? $"{validationContext.DisplayName}, [{String.Join(", ", this.values)}] değerlerinden biri olmalıdır!", new List<string> { validationContext.MemberName }); }
            return new ValidationResult(this.ErrorMessage ?? $"{validationContext.DisplayName}, [{String.Join(", ", this.values)}] değerleri dışında farklı bir değer olmalıdır!", new List<string> { validationContext.MemberName });
        }
    }
}