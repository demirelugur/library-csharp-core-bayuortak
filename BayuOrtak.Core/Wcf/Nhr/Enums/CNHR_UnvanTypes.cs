namespace BayuOrtak.Core.Wcf.Nhr.Enums
{
    using BayuOrtak.Core.Base;
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Wcf.Nhr.Helper;
    using System;
    using System.ComponentModel;
    using Wcf_Nhr_personelinfo;
    using static BayuOrtak.Core.Helper.OrtakTools;
    /// <summary>
    /// Personel Otomasyonu sisteminde CNHR_UnvanTypes sınıfı, personelin unvan türlerini temsil eden bir enum içermektedir.
    /// Bu sınıf, personelin hangi unvanlara sahip olduğunu belirlemek ve bu unvanları
    /// yönetmek için gerekli yöntemleri sağlar.
    /// </summary>
    public class CNhr_UnvanTypes : BaseEnum<CNhr_UnvanTypes.Nhr_UnvanTypes>
    {
        /// <summary>
        /// Personel unvan türlerini temsil eden bit alanı (flags) enumudur.
        /// Bu enum, farklı unvanların bir arada kullanılabilmesine olanak tanır.
        /// </summary>
        [Flags]
        public enum Nhr_UnvanTypes : byte
        {
            /// <summary>
            /// Akademik unvanı temsil eder.
            /// </summary>
            [Description("Akademik")]
            aka = 1,
            /// <summary>
            /// İdari unvanı temsil eder.
            /// </summary>
            [Description("İdari")]
            ida = 2,
            /// <summary>
            /// Sözleşmeli unvanı temsil eder.
            /// </summary>
            [Description("Sözleşmeli")]
            soz = 4,
            /// <summary>
            /// Diğer unvanları temsil eder.
            /// </summary>
            [Description("Diğer")]
            dig = 8
        }
        /// <summary>
        /// Verilen personel nesnesine göre unvan türlerini döndüren bir yöntem.
        /// </summary>
        /// <param name="wspersonel">Unvan türlerinin alınacağı personel nesnesi.</param>
        /// <returns>Personelin unvan türlerini temsil eden enum değerini döndürür.</returns>
        public static Nhr_UnvanTypes GetUnvanTypes(wspersonel wspersonel) => wspersonel.GetUnvanTypes();
        /// <summary>
        /// Verilen unvan türünün açıklama metnini yerel dilde döndüren bir yöntem.
        /// </summary>
        /// <param name="value">Açıklaması alınacak unvan türü.</param>
        /// <param name="dil">İstenilen dil kodu (örneğin: "tr" veya "en").</param>
        /// <returns>Unvan türünün açıklama metnini döndürür.</returns>
        /// <exception cref="NotSupportedException">Desteklenmeyen bir enum değeri için fırlatılır.</exception>
        /// <exception cref="InvalidOperationException">Desteklenmeyen bir dil kodu için fırlatılır.</exception>
        public static string GetDescriptionLocalizationValue(Nhr_UnvanTypes value, string dil)
        {
            Guard.UnSupportLanguage(dil, nameof(dil));
            if (dil == "tr") { return value.GetDescription(); }
            switch (value)
            {
                case Nhr_UnvanTypes.aka: return "Academic";
                case Nhr_UnvanTypes.ida: return "Administrative";
                case Nhr_UnvanTypes.soz: return "Contract";
                case Nhr_UnvanTypes.dig: return "Other";
                default: throw _other.ThrowNotSupportedForEnum<Nhr_UnvanTypes>();
            }
        }
    }
}