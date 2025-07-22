namespace BayuOrtak.Core.Enums
{
    using BayuOrtak.Core.Base;
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using System.ComponentModel;
    using static BayuOrtak.Core.Helper.OrtakTools;
    /// <summary>
    /// <see cref="DayOfWeek"/> ile uyumlu Haftanın Türkçe günlerini temsil eden sınıf.
    /// </summary>
    public sealed class CDayOfWeekTR : BaseEnum<CDayOfWeekTR.DayOfWeekTR>
    {
        /// <summary>
        /// Haftanın Türkçe günlerini temsil eden enum.
        /// </summary>
        public enum DayOfWeekTR : byte
        {
            [Description("Pazar")]
            pzr = 0,
            [Description("Pazartesi")]
            pzts,
            [Description("Salı")]
            sali,
            [Description("Çarşamba")]
            car,
            [Description("Perşembe")]
            per,
            [Description("Cuma")]
            cuma,
            [Description("Cumartesi")]
            cmrts
        }
        /// <summary>
        /// Verilen <see cref="DayOfWeekTR"/> değerini ve dili baz alarak, gün ismini döndürür.
        /// </summary>
        /// <param name="value">Türkçe gün adını temsil eden <see cref="DayOfWeekTR"/> enum değeri.</param>
        /// <param name="dil">Çeviri yapılacak dil kodu. &quot;tr&quot; Türkçe için, &quot;en&quot; İngilizce için kullanılır.</param>
        /// <returns>Gün isminin belirtilen dildeki karşılığını döndürür.</returns>
        /// <exception cref="NotSupportedException">
        /// Eğer <paramref name="value"/> geçerli bir <see cref="DayOfWeekTR"/> değeri değilse 
        /// veya desteklenmeyen bir dil girildiyse fırlatılır.
        /// </exception>
        public static string GetDescriptionLocalizationValue(DayOfWeekTR value, string dil)
        {
            Guard.UnSupportLanguage(dil, nameof(dil));
            if (dil == "en")
            {
                switch (value)
                {
                    case DayOfWeekTR.pzr: return "Sunday";
                    case DayOfWeekTR.pzts: return "Monday";
                    case DayOfWeekTR.sali: return "Tuesday";
                    case DayOfWeekTR.car: return "Wednesday";
                    case DayOfWeekTR.per: return "Thursday";
                    case DayOfWeekTR.cuma: return "Friday";
                    case DayOfWeekTR.cmrts: return "Saturday";
                    default: throw _other.ThrowNotSupportedForEnum<DayOfWeekTR>();
                }
            }
            return value.GetDescription();
        }
        /// <summary>
        /// Belirtilen <see cref="DayOfWeekTR"/> değerinin hafta içi günlerinden biri olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="value">Kontrol edilecek <see cref="DayOfWeekTR"/> enum değeri.</param>
        /// <returns>Eğer <paramref name="value"/> hafta içi günlerinden biriyse <c>true</c>, aksi takdirde <c>false</c> döner.</returns>
        public static bool IsWeekDays(DayOfWeekTR value) => value.Includes(DayOfWeekTR.pzts, DayOfWeekTR.sali, DayOfWeekTR.car, DayOfWeekTR.per, DayOfWeekTR.cuma);
    }
}