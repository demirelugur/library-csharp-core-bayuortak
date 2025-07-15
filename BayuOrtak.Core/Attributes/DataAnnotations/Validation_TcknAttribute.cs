namespace BayuOrtak.Core.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Extensions;
    using System;
    using System.ComponentModel.DataAnnotations;
    using static BayuOrtak.Core.Helper.OrtakTools;
    /// <summary>
    /// Türkiye Cumhuriyeti Kimlik Numarası (T.C. Kimlik No) doğrulaması yapan <see cref="ValidationAttribute"/> sınıfıdır.
    /// Bu sınıf, verilen değerin T.C. Kimlik Numarası biçimine uygun olup olmadığını kontrol eder.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class Validation_TcknAttribute : ValidationAttribute
    {
        /// <summary>
        /// Varsayılan kurucu metod.
        /// </summary>
        public Validation_TcknAttribute() { }
        /// <summary>
        /// Verilen değerin geçerli bir T.C. Kimlik Numarası olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="value">Kontrol edilecek değer.</param>
        /// <param name="validationContext">Geçerlilik bağlamı.</param>
        /// <returns>Geçerli ise <see cref="ValidationResult.Success"/>; aksi takdirde hata mesajı döner.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (_try.TryTCKimlikNo(value.ToStringOrEmpty(), out _)) { return ValidationResult.Success; }
            if (value == null && !validationContext.IsRequiredAttribute()) { return ValidationResult.Success; }
            return new ValidationResult(this.ErrorMessage ?? $"{validationContext.DisplayName}, T.C. Kimlik Numarası biçimine uygun olmalıdır!", new List<string> { validationContext.MemberName });
        }
    }
}