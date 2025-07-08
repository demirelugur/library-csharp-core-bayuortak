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
    /// Personel Otomasyonu sisteminde CNHR_DurumTypes sınıfı, personel özel durum türlerini temsil eden bir enum içermektedir.
    /// Bu sınıf, personelin hangi durumlarda olduğunu belirlemek ve bu durumları
    /// yönetmek için gerekli yöntemleri sağlar.
    /// </summary>
    public class CNHR_DurumTypes : BaseEnum<CNHR_DurumTypes.NHR_DurumTypes> // Not: 1) Enum değerleri [dbo].[sp_wcf_personelinformationall] içerisinde değerler tanımlanmaktadır.
    {
        /// <summary>
        /// Personel özel durum türlerini temsil eden bit alanı (flags) enumudur.
        /// Bu enum, farklı durumların bir arada kullanılabilmesine olanak tanır.
        /// </summary>
        [Flags]
        public enum NHR_DurumTypes : byte
        {
            /// <summary>
            /// 35. madde ile görevlendirilmiş durumu.
            /// </summary>
            [Description("35. Madde ile görevlendirilmiş")]
            madde35ilegiden = 1,
            /// <summary>
            /// İdari görevli gelen durumu.
            /// </summary>
            [Description("İdari Görevli Gelen")]
            idarigorevgelen = 2,
            /// <summary>
            /// Yabancı uyruklu ve tam zamanlı olan durumu.
            /// </summary>
            [Description("Yabancı Uyruklu - Tam Zamanlı")]
            yabanciuyruklutamzamanli = 4,
            /// <summary>
            /// Engelli durumu.
            /// </summary>
            [Description("Engelli")]
            engelli = 8
        }
        /// <summary>
        /// Verilen personel nesnesine göre durum türlerini döndüren bir yöntem.
        /// </summary>
        /// <param name="wspersonel">Durum türlerinin alınacağı personel nesnesi.</param>
        /// <returns>Personelin durum türlerini temsil eden enum değerini döndürür.</returns>
        public static NHR_DurumTypes? GetDurumTypes(wspersonel wspersonel) => wspersonel.GetDurumTypes();
        /// <summary>
        /// Verilen durum türünün açıklama metnini yerel dilde döndüren bir yöntem.
        /// </summary>
        /// <param name="value">Açıklaması alınacak durum türü.</param>
        /// <param name="dil">İstenilen dil kodu (örneğin: &quot;tr&quot; veya &quot;en&quot;).</param>
        /// <returns>Durum türünün açıklama metnini döndürür.</returns>
        /// <exception cref="NotSupportedException">Desteklenmeyen bir enum değeri için fırlatılır.</exception>
        /// <exception cref="InvalidOperationException">Desteklenmeyen bir dil kodu için fırlatılır.</exception>
        public static string GetDescriptionLocalizationValue(NHR_DurumTypes value, string dil)
        {
            Guard.UnSupportLanguage(dil, nameof(dil));
            if (dil == "tr") { return value.GetDescription(); }
            switch (value)
            {
                case NHR_DurumTypes.madde35ilegiden: return "Assigned under Article 35";
                case NHR_DurumTypes.idarigorevgelen: return "Administrative Staff Incoming";
                case NHR_DurumTypes.yabanciuyruklutamzamanli: return "Foreign National - Full Time";
                case NHR_DurumTypes.engelli: return "Disabled";
                default: throw _other.ThrowNotSupportedForEnum<NHR_DurumTypes>();
            }
        }
    }
}