namespace BayuOrtak.Core.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Extensions;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Net.Mail;
    using static BayuOrtak.Core.Helper.OrtakTools;
    /// <summary>
    /// T.C. Bayburt Üniversitesi e-Posta adresini doğrulamak için kullanılan özel bir doğrulama niteliği. Bu nitelik, verilen e-Posta adresinin geçerli bir biçimde olup olmadığını ve &quot;bayburt.edu.tr&quot; uzantılı bir e-Posta adresi olup olmadığını kontrol eder.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class Validation_BayburtUniEMailAddressAttribute : ValidationAttribute
    {
        /// <summary>
        /// Varsayılan kurucu metod.
        /// </summary>
        public Validation_BayburtUniEMailAddressAttribute() { }
        /// <summary>
        /// Verilen değeri doğrulamak için çağrılır.
        /// </summary>
        /// <param name="value">Doğrulanacak değer.</param>
        /// <param name="validationContext">Doğrulama bağlamı.</param>
        /// <returns>
        /// <see cref="ValidationResult.Success"/> eğer değer geçerliyse; 
        /// aksi takdirde bir <see cref="ValidationResult"/> döndürülür.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value.ToStringOrEmpty();
            if (_try.TryMailAddress(email, out MailAddress _ma) && _ma.IsBayburtUniEPosta())
            {
                validationContext.SetValidatePropertyValue(_ma.Address);
                return ValidationResult.Success;
            }
            if (!validationContext.IsRequiredAttribute() && email == "")
            {
                validationContext.SetValidatePropertyValue(null);
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName}, \"bayburt.edu.tr\" uzantılı geçerli bir e-Posta adresi olmalıdır!", new List<string> { validationContext.MemberName });
        }
    }
}