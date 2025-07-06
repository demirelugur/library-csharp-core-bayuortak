namespace BayuOrtak.Core.Attributes.DataAnnotations
{
    using System.ComponentModel.DataAnnotations;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    /// <summary>
    /// Bir koleksiyonun veya dizinin minimum eleman uzunluğunu kontrol eden özel doğrulama özniteliği.
    /// </summary>
    /// <remarks>
    /// Eğer koleksiyonun eleman sayısı belirtilen minimum uzunluğun altındaysa hata mesajı üretir.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class Validation_ArrayMinLengthAttribute : MinLengthAttribute
    {
        /// <summary>
        /// Minimum uzunluğu 1 olarak ayarlayan varsayılan yapıcı (constructor).
        /// </summary>
        public Validation_ArrayMinLengthAttribute() : this(1) { }
        /// <summary>
        /// Minimum uzunluğu parametre olarak kabul eden yapıcı (constructor).
        /// </summary>
        /// <param name="minimumLength">Koleksiyonun sağlaması gereken minimum eleman sayısı.</param>
        public Validation_ArrayMinLengthAttribute(int minimumLength) : base(minimumLength)
        {
            this.ErrorMessage = minimumLength > 1 ? _validationerrormessage.array_minlength : _validationerrormessage.required;
        }
    }
}