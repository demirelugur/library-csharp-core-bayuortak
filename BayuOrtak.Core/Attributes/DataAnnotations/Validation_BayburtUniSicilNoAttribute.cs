namespace BayuOrtak.Core.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Wcf.Nhr.Helper;
    using System;
    using System.ComponentModel.DataAnnotations;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    /// <summary>
    /// T.C. Bayburt Üniversitesi Kurum Sicil Numarasını doğrulamak için kullanılan özel bir doğrulama niteliği.
    /// <para>Bu nitelik, verilen sicil numarasının geçerli bir biçimde olup olmadığını kontrol eder.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class Validation_BayburtUniSicilNoAttribute : ValidationAttribute
    {
        /// <summary>
        /// Varsayılan kurucu metod.
        /// </summary>
        public Validation_BayburtUniSicilNoAttribute() { }
        /// <summary>
        /// Verilen değeri doğrulamak için çağrılır.
        /// </summary>
        /// <param name="value">Doğrulanacak sicil numarası değeri.</param>
        /// <param name="validationContext">Doğrulama bağlamı.</param>
        /// <returns>
        /// <see cref="ValidationResult.Success"/> eğer değer geçerliyse; 
        /// aksi takdirde bir <see cref="ValidationResult"/> döndürülür.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var sicilno = value.ToStringOrEmpty().ToUpper();
            if (NHRTools.IsSicilNo(sicilno))
            {
                validationContext.SetValidatePropertyValue(sicilno);
                return ValidationResult.Success;
            }
            if (!validationContext.IsRequiredAttribute() && sicilno == "")
            {
                validationContext.SetValidatePropertyValue(null);
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName}, T.C. {_title.name_bayburtuniversitesi} Kurum Sicil Numarasına uygun biçimde olmalıdır!", new List<string> { validationContext.MemberName });
        }
    }
}