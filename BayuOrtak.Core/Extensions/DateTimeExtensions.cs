namespace BayuOrtak.Core.Extensions
{
    using System;
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Belirtilen <see cref="DateTime"/> nesnesini yalnızca tarih bilgisini içeren bir <see cref="DateOnly"/> nesnesine dönüştürür.
        /// </summary>
        /// <param name="dateTime">Dönüştürülecek <see cref="DateTime"/> nesnesi.</param>
        /// <returns>Yalnızca tarih bilgisini içeren bir <see cref="DateOnly"/> nesnesi.</returns>
        public static DateOnly ToDateOnly(this DateTime dateTime) => DateOnly.FromDateTime(dateTime);
        /// <summary>
        /// Verilen DateTime nesnesini SQL tarih/saat biçiminde dönüştürür.
        /// </summary>
        /// <param name="dateTime">Dönüştürülecek tarih/saat.</param>
        /// <param name="isDateTime">Tarih mi yoksa sadece saat mi olduğunu belirtir.</param>
        /// <returns>SQL biçiminde tarih/saat dizesi.</returns>
        public static string ToSqlDateTime(this DateTime dateTime, bool isDateTime) => $"CONVERT(DATE{(isDateTime ? "TIME" : "")}, '{dateTime.ToString(String.Concat("yyyy-MM-dd", (isDateTime ? " HH:mm:ss:fff" : "")))}')";
        /// <summary>
        /// Verilen DateTime nesnesini OADate tamsayı biçimine dönüştürür.
        /// </summary>
        /// <param name="dateTime">Dönüştürülecek tarih/saat.</param>
        /// <returns>OADate tamsayı değeri.</returns>
        public static int ToOADateInteger(this DateTime dateTime) => Convert.ToInt32(dateTime.Date.ToOADate());
        /// <summary>
        /// Verilen DateTime nesnesini ISO 8601 biçiminde dizeye dönüştürür.
        /// </summary>
        /// <param name="dateTime">Dönüştürülecek tarih/saat.</param>
        /// <returns>ISO 8601 biçiminde dize.</returns>
        public static string ToISO8601(this DateTime dateTime) => dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        /// <summary>
        /// Geçerli saatten itibaren geçen saniyeleri döndürür.
        /// </summary>
        /// <param name="dateTime">Zaman nesnesi.</param>
        /// <returns>Geçen saniye sayısı.</returns>
        public static int ToSecondsSinceMidnight(this DateTime dateTime) => ((dateTime.Hour * 3600) + (dateTime.Minute * 60) + dateTime.Second);
        /// <summary>
        /// Verilen DateTime nesnesini Unix zaman damgası (milisaniye) biçiminde döndürür.
        /// </summary>
        /// <param name="dateTime">Dönüştürülecek tarih/saat.</param>
        /// <returns>Unix zaman damgası milisaniye değeri.</returns>
        public static long ToUnixTimestampMilliseconds(this DateTime dateTime) => ((dateTime.ToUniversalTime().Ticks - DateTime.UnixEpoch.Ticks) / TimeSpan.TicksPerMillisecond);
        /// <summary>
        /// Verilen DateTime nesnesinin ait olduğu ayın ilk gününü döndürür.
        /// </summary>
        /// <param name="dateTime">İlgili tarih.</param>
        /// <returns>Ayın ilk günü.</returns>
        public static DateTime GetFirstDayOfMonth(this DateTime dateTime) => new DateTime(dateTime.Year, dateTime.Month, 1);
        /// <summary>
        /// Verilen DateTime nesnesinin ait olduğu ayın son gününü döndürür.
        /// </summary>
        /// <param name="dateTime">İlgili tarih.</param>
        /// <returns>Ayın son günü.</returns>
        public static DateTime GetLastDayOfMonth(this DateTime dateTime) => new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
    }
}