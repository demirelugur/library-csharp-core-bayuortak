namespace BayuOrtak.Core.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Extensions;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static BayuOrtak.Core.Helper.OrtakTools;
    /// <summary>
    /// Bu sınıf, bir URL&#39;nin geçerliliğini doğrulamak için kullanılan bir doğrulama özelliğidir.
    /// <list type="bullet">
    /// <item>
    /// Eğer değer geçerli bir &#39;http&#39; veya &#39;https&#39; URI&#39;si ise, doğrulama başarılı olur.
    /// </item>
    /// <item>
    /// Eğer özellik zorunlu değilse (&#39;[Required]&#39; eklenmemişse) ve değer boş bir dize ise, doğrulama yine başarılı olur.
    /// </item>
    /// <item>
    /// Geçersiz bir URI olduğunda hata mesajı döner ve doğrulama başarısız olur.
    /// </item>
    /// </list>
    /// Kullanımı, &#39;ValidationAttribute&#39; sınıfından türetilmiştir ve doğrulama sırasında özelleştirilmiş bir URI işleme mantığı sağlar.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class Validation_UrlHttpAttribute : ValidationAttribute
    {
        /// <summary>
        /// Varsayılan kurucu metod.
        /// </summary>
        public Validation_UrlHttpAttribute() { }
        /// <summary>
        /// Belirtilen değerin geçerli bir &#39;http&#39; veya &#39;https&#39; URI&#39;si olup olmadığını doğrular.
        /// <list type="bullet">
        /// <item>
        /// Eğer değer geçerli bir URI ise, doğrulama başarılı olur ve işlenen URI değeri &#39;ValidationContext&#39; içinde saklanır.
        /// </item>
        /// <item>
        /// Eğer değer boş bir dize ise ve &#39;[Required]&#39; özelliği belirtilmemişse, doğrulama başarılı olarak kabul edilir.
        /// </item>
        /// <item>
        /// Geçersiz bir URI olduğunda, hata mesajı ile birlikte doğrulama başarısız olur.
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="value">Doğrulanacak nesne.</param>
        /// <param name="validationContext">Doğrulama sırasında ek bağlam bilgilerini sağlayan nesne.</param>
        /// <returns>
        /// Geçerli bir değer için &#39;ValidationResult.Success&#39;, aksi halde hata mesajı içeren bir &#39;ValidationResult&#39;.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var uri = value.ToStringOrEmpty();
            if (_try.TryUri(uri, out Uri _uri))
            {
                validationContext.SetValidatePropertyValue(_uri.ToString().TrimEnd('/'));
                return ValidationResult.Success;
            }
            if (uri == "" && !validationContext.IsRequiredAttribute())
            {
                validationContext.SetValidatePropertyValue(null);
                return ValidationResult.Success;
            }
            return new ValidationResult(this.ErrorMessage ?? $"{validationContext.DisplayName}, geçerli bir \"http, https\" protokollerine uygun Uri adresi olmalıdır!", new List<string> { validationContext.MemberName });
        }
    }
}