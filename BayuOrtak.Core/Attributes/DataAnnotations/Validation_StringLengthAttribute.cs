namespace BayuOrtak.Core.Attributes.DataAnnotations
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    /// <summary>
    /// Bu sınıf, bir dize (string) özelliğinin uzunluğunu doğrulamak için kullanılan özel bir doğrulama niteliğidir. Uzunluk sınırlamaları, maksimum ve minimum uzunluk değerleri ile tanımlanır. Hata mesajları, belirtilen uzunluk sınırlarına göre özelleştirilmiştir.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class Validation_StringLengthAttribute : StringLengthAttribute
    {
        /// <summary>
        /// Maksimum uzunluğu belirten yeni bir örnek oluşturur.
        /// </summary>
        /// <param name="maximumlength">Dizinin alabileceği maksimum uzunluk.</param>
        public Validation_StringLengthAttribute(int maximumlength) : base(maximumlength)
        {
            this.ErrorMessage = _validationerrormessage.stringlength_max;
        }
        /// <summary>
        /// Maksimum ve minimum uzunluk değerlerini belirten yeni bir örnek oluşturur.
        /// </summary>
        /// <param name="maximumlength">Dizinin alabileceği maksimum uzunluk.</param>
        /// <param name="minimumlength">Dizinin alabileceği minimum uzunluk.</param>
        public Validation_StringLengthAttribute(int maximumlength, int minimumlength) : base(maximumlength)
        {
            this.MinimumLength = minimumlength;
            this.ErrorMessage = (maximumlength == minimumlength ? _validationerrormessage.stringlength_maxminequal : _validationerrormessage.stringlength_maxmin);
        }
    }
}