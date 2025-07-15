namespace BayuOrtak.Core.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Extensions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    using static BayuOrtak.Core.Helper.OrtakTools;
    /// <summary>
    /// T.C. Bayburt Üniversitesi EBYS (Elektronik Belge Yönetim Sistemi) kodlarının doğruluğunu kontrol eden özel bir doğrulama niteliği.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class Validation_BayburtUniEBYSAttribute : ValidationAttribute
    {
        /// <summary>
        /// Varsayılan kurucu metod.
        /// </summary>
        public Validation_BayburtUniEBYSAttribute() { }
        /// <summary>
        /// Verilen değerin EBYS kodu biçimine uygun olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="value">Doğrulanacak değer.</param>
        /// <param name="validationContext">Doğrulama bağlamı hakkında bilgi içeren nesne.</param>
        /// <returns>
        /// <see cref="ValidationResult.Success"/> başarılı doğrulama durumunu belirtir; aksi takdirde hata mesajı içeren bir <see cref="ValidationResult"/> döner.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var ebys = value.ToStringOrEmpty();
            if (_try.TryEBYSID(ebys, out string _ebys))
            {
                validationContext.SetValidatePropertyValue(_ebys);
                return ValidationResult.Success;
            }
            if (ebys == "" && !validationContext.IsRequiredAttribute())
            {
                validationContext.SetValidatePropertyValue(null);
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName}, {_title.ebys} Kodu (Örnek: 01.01.2024-12345) uygun biçimde olmalıdır!", new List<string> { validationContext.MemberName });
        }
    }
}