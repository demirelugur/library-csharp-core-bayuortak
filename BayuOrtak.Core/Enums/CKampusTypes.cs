namespace BayuOrtak.Core.Enums
{
    using BayuOrtak.Core.Base;
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using System.ComponentModel;
    /// <summary>
    /// T.C. Bayburt Üniversitesi Kampüs türlerini temsil eden sınıf.
    /// </summary>
    public sealed class CKampusTypes : BaseEnum<CKampusTypes.KampusTypes> // TODO: Yeni Web Sitesinden sonra iptal edilmelidir!
    {
        /// <summary>
        /// Kampüs türlerini tanımlayan enum.
        /// </summary>
        public enum KampusTypes : byte
        {
            [Description("Dede Korkut Külliyesi")]
            dedekorkut = 1,
            [Description("Baberti Külliyesi")]
            baberti,
            [Description("Aydıntepe")]
            aydintepe,
            [Description("Demirözü")]
            demirozu
        }
        /// <summary>
        /// Verilen <see cref="KampusTypes"/> değerini ve dili baz alarak, kampüs adını döndürür.
        /// </summary>
        /// <param name="value">Kampüs türünü temsil eden <see cref="KampusTypes"/> enum değeri.</param>
        /// <param name="dil">Çeviri yapılacak dil kodu. &quot;tr&quot; Türkçe için, &quot;en&quot; İngilizce için kullanılır.</param>
        /// <returns>Belirtilen dilde kampüs adını döndürür.</returns>
        /// <exception cref="NotSupportedException">
        /// Eğer <paramref name="value"/> geçerli bir <see cref="KampusTypes"/> değeri değilse 
        /// veya desteklenmeyen bir dil girildiyse fırlatılır.
        /// </exception>
        public static string GetDescriptionLocalizationValue(KampusTypes value, string dil)
        {
            Guard.UnSupportLanguage(dil, nameof(dil));
            if (dil == "tr") { return value.GetDescription(); }
            if (value == KampusTypes.dedekorkut) { return "Dede Korkut Campus"; }
            if (value == KampusTypes.baberti) { return "Baberti Campus"; }
            return GetDescriptionLocalizationValue(value, "tr");
        }
    }
}