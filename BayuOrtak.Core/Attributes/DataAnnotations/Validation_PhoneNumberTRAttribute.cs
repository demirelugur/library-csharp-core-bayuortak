namespace BayuOrtak.Core.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Extensions;
    using System.ComponentModel.DataAnnotations;
    using static BayuOrtak.Core.Helper.OrtakTools;
    /// <summary>
    /// Türkiye&#39;deki telefon numaralarının geçerliliğini doğrulamak için kullanılan bir <see cref="ValidationAttribute"/> sınıfıdır.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class Validation_PhoneNumberTRAttribute : ValidationAttribute
    {
        /// <summary>
        /// Varsayılan kurucu metod.
        /// </summary>
        public Validation_PhoneNumberTRAttribute() { }
        /// <summary>
        /// Verilen telefon numarasının Türkiye biçimine uygun olup olmadığını doğrular.
        /// </summary>
        /// <param name="value">Doğrulanacak telefon numarası değeri.</param>
        /// <param name="validationContext">Doğrulama bağlamı bilgileri.</param>
        /// <returns>Başarı durumu veya hata sonucu.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var phoneTR = value.ToStringOrEmpty();
            if (_try.TryPhoneNumberTR(phoneTR, out string _phoneTR))
            {
                validationContext.SetValidatePropertyValue(_phoneTR);
                return ValidationResult.Success;
            }
            if (!validationContext.IsRequiredAttribute() && phoneTR == "")
            {
                validationContext.SetValidatePropertyValue(null);
                return ValidationResult.Success;
            }
            return new ValidationResult(this.ErrorMessage ?? $"{validationContext.DisplayName}, (xxx)xxx-xxxx, (xxx) xxx-xxxx biçimine uygun telefon numarası olmalıdır!", new List<string> { validationContext.MemberName });
        }
    }
}