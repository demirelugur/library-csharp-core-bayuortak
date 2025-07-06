namespace BayuOrtak.Core.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using System.ComponentModel.DataAnnotations;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    /// <summary>
    /// ISBN(Uluslararası Standart Kitap Numarası) numarasını doğrulamak için kullanılan bir doğrulama niteliği.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class Validation_ISBNAttribute : ValidationAttribute
    {
        /// <summary>
        /// Varsayılan kurucu metod.
        /// </summary>
        public Validation_ISBNAttribute() { }
        /// <summary>
        /// Verilen değerin geçerli bir ISBN(Uluslararası Standart Kitap Numarası) numarası olup olmadığını doğrular.
        /// Eğer değer boşsa ve gerekli değilse, doğrulama başarılı kabul edilir.
        /// </summary>
        /// <param name="value">Doğrulanan değer (ISBN numarası).</param>
        /// <param name="validationContext">Doğrulama bağlamı.</param>
        /// <returns>Geçerli ise <see cref="ValidationResult.Success"/>; aksi takdirde hata mesajı ile birlikte <see cref="ValidationResult"/> döner.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var isbn = value.ToStringOrEmpty();
            if (ISBNHelper.IsValid(isbn))
            {
                validationContext.SetValidatePropertyValue(isbn);
                return ValidationResult.Success;
            }
            if (!validationContext.IsRequiredAttribute() && isbn == "")
            {
                validationContext.SetValidatePropertyValue(null);
                return ValidationResult.Success;
            }
            return new ValidationResult(this.ErrorMessage ?? $"{validationContext.DisplayName}, {_title.isbn} biçimine uygun olmalıdır!", new List<string> { validationContext.MemberName });
        }
    }
}