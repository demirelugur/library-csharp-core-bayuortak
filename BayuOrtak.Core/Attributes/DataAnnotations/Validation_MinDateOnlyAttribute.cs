namespace BayuOrtak.Core.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Extensions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    /// <summary>
    /// Bir tarih değerinin, belirtilen minimum tarihten (en erken tarih) büyük veya eşit olup olmadığını doğrulamak için kullanılan bir <see cref="ValidationAttribute"/> sınıfıdır.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public class Validation_MinDateOnlyAttribute : ValidationAttribute
    {
        private readonly DateOnly minDate;
        /// <summary>
        /// Yeni bir <see cref="Validation_MinDateOnlyAttribute"/> örneği oluşturur ve minimum tarihi Bayburt Üniversitesi'nin kuruluş tarihi olarak ayarlar.
        /// </summary>
        public Validation_MinDateOnlyAttribute()
        {
            this.minDate = _date.bayburtUniversityFoundationDate.ToDateOnly();
        }
        /// <summary>
        /// Yeni bir <see cref="Validation_MinDateOnlyAttribute"/> örneği oluşturur ve belirtilen tarih dizesini minimum tarih olarak ayarlar.
        /// </summary>
        /// <param name="minDate">Minimum tarih dizesi (yyyy-MM-dd biçiminde).</param>
        public Validation_MinDateOnlyAttribute(string minDate)
        {
            this.minDate = DateOnly.Parse(minDate);
        }
        /// <summary>
        /// Yeni bir <see cref="Validation_MinDateOnlyAttribute"/> örneği oluşturur ve belirtilen yıl, ay ve gün değerlerine göre minimum tarihi ayarlar.
        /// </summary>
        /// <param name="yil">Yıl değeri.</param>
        /// <param name="ay">Ay değeri.</param>
        /// <param name="gun">Gün değeri.</param>
        public Validation_MinDateOnlyAttribute(int yil, int ay, int gun)
        {
            this.minDate = new DateOnly(yil, ay, gun);
        }
        /// <summary>
        /// Verilen değerin, belirlenen minimum tarih ile karşılaştırarak geçerli olup olmadığını doğrular.
        /// </summary>
        /// <param name="value">Doğrulanacak değer.</param>
        /// <param name="validationContext">Doğrulama bağlamı bilgileri.</param>
        /// <returns>Başarı durumu veya hata sonucu.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateOnly dateOnly && dateOnly >= minDate)
            {
                validationContext.SetValidatePropertyValue(dateOnly);
                return ValidationResult.Success;
            }
            if (value is DateTime dateTime && DateOnly.FromDateTime(dateTime) >= minDate)
            {
                validationContext.SetValidatePropertyValue(dateTime);
                return ValidationResult.Success;
            }
            if (!validationContext.IsRequiredAttribute() && value.ToStringOrEmpty() == "")
            {
                validationContext.SetValidatePropertyValue(null);
                return ValidationResult.Success;
            }
            return new ValidationResult(this.ErrorMessage ?? $"{validationContext.DisplayName}, {minDate.ToShortDateString()} tarihinden ileri bir tarih olmalıdır!", new List<string> { validationContext.MemberName });
        }
    }
}