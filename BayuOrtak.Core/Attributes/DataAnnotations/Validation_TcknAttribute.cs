namespace BayuOrtak.Core.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Enums;
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using System;
    using System.ComponentModel.DataAnnotations;
    /// <summary>
    /// Türkiye Cumhuriyeti Kimlik Numarası (T.C. Kimlik No) doğrulaması yapan <see cref="ValidationAttribute"/> sınıfıdır. Bu sınıf, verilen değerin T.C. Kimlik Numarası biçimine uygun olup olmadığını kontrol eder.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class Validation_TcknAttribute : ValidationAttribute
    {
        private readonly bool ischeckstartwith98;
        public Validation_TcknAttribute() : this(false) { }
        public Validation_TcknAttribute(bool ischeckstartwith98) { this.ischeckstartwith98 = ischeckstartwith98; }
        /// <summary>
        /// Verilen değerin geçerli bir T.C. Kimlik Numarası olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="value">Kontrol edilecek değer.</param>
        /// <param name="validationContext">Geçerlilik bağlamı.</param>
        /// <returns>Geçerli ise <see cref="ValidationResult.Success"/>; aksi takdirde hata mesajı döner.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (LDAPHelper.IsTCKimlikNo((this.ischeckstartwith98 ? LDAPTip.stu : default), value.ToLong())) { return ValidationResult.Success; }
            if (value == null && !validationContext.IsRequiredAttribute()) { return ValidationResult.Success; }
            return new ValidationResult(this.ErrorMessage ?? $"{validationContext.DisplayName}, T.C. Kimlik Numarası biçimine uygun olmalıdır!", new List<string> { validationContext.MemberName });
        }
    }
}