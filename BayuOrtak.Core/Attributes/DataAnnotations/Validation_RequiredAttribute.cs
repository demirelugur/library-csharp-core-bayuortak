namespace BayuOrtak.Core.Attributes.DataAnnotations
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    /// <summary>
    /// Gerekli alan doğrulaması yapan <see cref="RequiredAttribute"/> sınıfıdır. Bu sınıf, boş dizeleri kabul etmez ve gerekli olan bir değerin sağlanıp sağlanmadığını doğrular.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class Validation_RequiredAttribute : RequiredAttribute
    {
        /// <summary>
        /// Yeni bir <see cref="Validation_RequiredAttribute"/> örneği oluşturur. Boş dizelerin geçerli olmadığını belirler ve hata mesajını ayarlar.
        /// </summary>
        public Validation_RequiredAttribute()
        {
            this.AllowEmptyStrings = false;
            this.ErrorMessage = _validationerrormessage.required;
        }
    }
}