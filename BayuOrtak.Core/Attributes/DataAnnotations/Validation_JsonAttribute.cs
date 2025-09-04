namespace BayuOrtak.Core.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Extensions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.ComponentModel.DataAnnotations;
    using static BayuOrtak.Core.Helper.OrtakTools;
    /// <summary>
    /// JSON verilerini doğrulamak için bir <see cref="ValidationAttribute"/> sınıfıdır.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class Validation_JsonAttribute : ValidationAttribute
    {
        private readonly JTokenType jtokentype;
        /// <summary>
        /// Yeni bir <see cref="Validation_JsonAttribute"/> örneği oluşturur.
        /// </summary>
        /// <param name="jtokentype">Geçerli JSON token türünü belirtir.</param>
        public Validation_JsonAttribute(JTokenType jtokentype) { this.jtokentype = jtokentype; }
        /// <summary>
        /// Verilen değerin geçerli bir JSON biçiminde olup olmadığını doğrular.
        /// </summary>
        /// <param name="value">Doğrulanacak değer.</param>
        /// <param name="validationContext">Doğrulama bağlamı bilgileri.</param>
        /// <returns>Başarı durumu veya hata sonucu.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _jsondata = value.ToStringOrEmpty();
            var _r = validationContext.IsRequiredAttribute();
            if (_try.TryJson(_jsondata, this.jtokentype, out JToken _jt))
            {
                if (_jt.Children().Any())
                {
                    validationContext.SetValidatePropertyValue(_jt.ToString(Formatting.None));
                    return ValidationResult.Success;
                }
                if (!_r)
                {
                    validationContext.SetValidatePropertyValue(null);
                    return ValidationResult.Success;
                }
            }
            if (_jsondata == "" && !_r)
            {
                validationContext.SetValidatePropertyValue(null);
                return ValidationResult.Success;
            }
            return new ValidationResult(this.ErrorMessage ?? $"{validationContext.DisplayName}, JSON biçimine ({this.jtokentype.ToString("g")}) uygun olmalıdır!", new List<string> { validationContext.MemberName });
        }
    }
}