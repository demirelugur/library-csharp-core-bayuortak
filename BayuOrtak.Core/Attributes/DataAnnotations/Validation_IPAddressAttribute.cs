namespace BayuOrtak.Core.Attributes.DataAnnotations
{
    using BayuOrtak.Core.Extensions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    /// <summary>
    /// Bu sınıf, bir IP adresi doğrulama işlemi gerçekleştiren özel bir doğrulama niteliğidir.
    /// Girilen değerin geçerli bir IP adresi (IPv4) olup olmadığını kontrol eder.
    /// Geçerli bir IP adresi girilirse, değer otomatik olarak IPv4 biçimine dönüştürülür ve nesneye atanır.
    /// Zorunlu olmayan bir özellik için boş değer girişi kabul edilir.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class Validation_IPAddressAttribute : ValidationAttribute
    {
        /// <summary>
        /// Varsayılan kurucu metod.
        /// </summary>
        public Validation_IPAddressAttribute() { }
        /// <summary>
        /// Girilen değerin geçerli bir IP adresi (IPv4) olup olmadığını kontrol eder ve doğrulama sonucunu döndürür.
        /// Eğer geçerli bir IPv4 adresi sağlanmışsa, nesneye atanır.
        /// </summary>
        /// <param name="value">Doğrulaması yapılacak IP adresi değeri.</param>
        /// <param name="validationContext">Doğrulama bağlamını içeren nesne.</param>
        /// <returns>Geçerli bir IP adresi ise başarı sonucu, aksi takdirde hata mesajı içeren bir <see cref="ValidationResult"/> döner.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var ip = value.ToStringOrEmpty();
            if (IPAddress.TryParse(ip, out IPAddress ipaddress))
            {
                validationContext.SetValidatePropertyValue(ipaddress.MapToIPv4().ToString());
                return ValidationResult.Success;
            }
            if (ip == "" && !validationContext.IsRequiredAttribute())
            {
                validationContext.SetValidatePropertyValue(null);
                return ValidationResult.Success;
            }
            return new ValidationResult(this.ErrorMessage ?? $"{validationContext.DisplayName}, geçerli bir IP adresi(IPv4 türünde) olmalıdır!", new List<string> { validationContext.MemberName });
        }
    }
}