namespace BayuOrtak.Core.Enums
{
    using BayuOrtak.Core.Base;
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using System;
    using System.ComponentModel;
    using static BayuOrtak.Core.Helper.OrtakTools;
    /// <summary>
    /// Ayların Türkçe karşılığını temsil eden class.
    /// </summary>
    public sealed class CMonthTR : BaseEnum<CMonthTR.MonthTR>
    {
        /// <summary>
        /// Ayları tanımlayan enum.
        /// </summary>
        public enum MonthTR : byte
        {
            [Description("Ocak")]
            oca = 1,
            [Description("Şubat")]
            sub,
            [Description("Mart")]
            mrt,
            [Description("Nisan")]
            nis,
            [Description("Mayıs")]
            may,
            [Description("Haziran")]
            haz,
            [Description("Temmuz")]
            tem,
            [Description("Ağustos")]
            agu,
            [Description("Eylül")]
            eyu,
            [Description("Ekim")]
            eki,
            [Description("Kasım")]
            kas,
            [Description("Aralık")]
            ara
        }
        /// <summary>
        /// Verilen <see cref="MonthTR"/> değerine göre yerelleştirilmiş ay adını döndürür.
        /// </summary>
        /// <param name="value">Ayı temsil eden <see cref="MonthTR"/> enum değeri.</param>
        /// <param name="dil">Dil kodu; &quot;tr&quot; Türkçe için, &quot;en&quot; İngilizce için.</param>
        /// <returns>Belirtilen dilde ay adını döndürür.</returns>
        /// <exception cref="NotSupportedException">
        /// Eğer <paramref name="value"/> geçerli bir <see cref="MonthTR"/> değeri değilse veya desteklenmeyen bir dil girildiyse fırlatılır.
        /// </exception>
        public static string GetDescriptionLocalizationValue(MonthTR value, string dil)
        {
            Guard.UnSupportLanguage(dil, nameof(dil));
            if (dil == "en")
            {
                switch (value)
                {
                    case MonthTR.oca: return "January";
                    case MonthTR.sub: return "February";
                    case MonthTR.mrt: return "March";
                    case MonthTR.nis: return "April";
                    case MonthTR.may: return "May";
                    case MonthTR.haz: return "June";
                    case MonthTR.tem: return "July";
                    case MonthTR.agu: return "August";
                    case MonthTR.eyu: return "September";
                    case MonthTR.eki: return "October";
                    case MonthTR.kas: return "November";
                    case MonthTR.ara: return "December";
                    default: throw _other.ThrowNotSupportedForEnum<MonthTR>();
                }
            }
            return value.GetDescription();
        }
    }
}