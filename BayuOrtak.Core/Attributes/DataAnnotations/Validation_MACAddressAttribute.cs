namespace BayuOrtak.Core.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Extensions;
    using System.ComponentModel.DataAnnotations;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    using static BayuOrtak.Core.Helper.OrtakTools;
    /// <summary>
    /// MAC (Media Access Control) adresini doğrulamak için bir <see cref="ValidationAttribute"/> sınıfıdır.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class Validation_MACAddressAttribute : ValidationAttribute
    {
        /// <summary>
        /// Varsayılan kurucu metod.
        /// </summary>
        public Validation_MACAddressAttribute() { }
        /// <summary>
        /// Verilen değerin geçerli bir MAC adresi olup olmadığını doğrular.
        /// </summary>
        /// <param name="value">Doğrulanacak değer.</param>
        /// <param name="validationContext">Doğrulama bağlamı bilgileri.</param>
        /// <returns>Başarı durumu veya hata sonucu.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var macValue = value.ToStringOrEmpty();
            if (_try.TryMACAddress(macValue, out string _mac))
            {
                validationContext.SetValidatePropertyValue(_mac);
                return ValidationResult.Success;
            }
            if (!validationContext.IsRequiredAttribute() && macValue == "")
            {
                validationContext.SetValidatePropertyValue(null);
                return ValidationResult.Success;
            }
            return new ValidationResult(this.ErrorMessage ?? $"{validationContext.DisplayName}, {_title.mac} Adresi biçimine uygun olmalıdır!", new List<string> { validationContext.MemberName });
        }
    }
}