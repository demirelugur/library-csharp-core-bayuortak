namespace BayuOrtak.Core.Attributes.DataAnnotations
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    /// <summary>
    /// Pozitif tam sayılar için bir aralık doğrulaması yapan <see cref="RangeAttribute"/> sınıfıdır.
    /// Bu sınıf, 1 ile <see cref="Int32.MaxValue"/> arasında bir değerin geçerli olduğunu doğrular.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class Validation_RangePositiveInt32Attribute : RangeAttribute
    {
        /// <summary>
        /// Yeni bir <see cref="Validation_RangePositiveInt32Attribute"/> örneği oluşturur ve hata mesajını ayarlar.
        /// </summary>
        public Validation_RangePositiveInt32Attribute() : base(1, Int32.MaxValue)
        {
            this.ErrorMessage = _validationerrormessage.rangepositive;
        }
    }
}