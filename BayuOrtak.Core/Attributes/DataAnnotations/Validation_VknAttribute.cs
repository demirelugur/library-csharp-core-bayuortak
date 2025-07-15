namespace BayuOrtak.Core.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Extensions;
    using System;
    using System.ComponentModel.DataAnnotations;
    using static BayuOrtak.Core.Helper.OrtakTools;
    /// <summary>
    /// Türkiye Cumhuriyeti Vergi Kimlik Numarası (T.C. Vergi Kimlik No) doğrulaması yapan <see cref="ValidationAttribute"/> sınıfıdır.
    /// Bu sınıf, verilen vergi kimlik numarasının geçerliliğini kontrol eder.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class Validation_VknAttribute : ValidationAttribute
    {
        /// <summary>
        /// Varsayılan kurucu metod.
        /// </summary>
        public Validation_VknAttribute() { }
        /// <summary>
        /// Verilen değerin geçerli bir T.C. Vergi Kimlik Numarası olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="value">Kontrol edilecek değer.</param>
        /// <param name="validationContext">Geçerlilik bağlamı.</param>
        /// <returns>Geçerli ise <see cref="ValidationResult.Success"/>; aksi takdirde hata mesajı döner.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (_try.TryVergiKimlikNo(value.ToStringOrEmpty(), out long outvalue))
            {
                validationContext.SetValidatePropertyValue(outvalue); // Set edilmesinin sebebi 9 rakamlı VKN değerleri gelebilir başına 0 koyarak yeni değeri değiştirmek gerekiyor. Örnek 602883151 değeri girilmişşe property 0602883151 biçiminde set edilecek
                return ValidationResult.Success;
            }
            if (value == null && !validationContext.IsRequiredAttribute()) { return ValidationResult.Success; }
            return new ValidationResult(this.ErrorMessage ?? $"{validationContext.DisplayName}, T.C. Vergi Numarası biçimine uygun olmalıdır!", new List<string> { validationContext.MemberName });
        }
    }
}