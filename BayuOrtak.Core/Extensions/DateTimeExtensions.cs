namespace BayuOrtak.Core.Extensions
{
    using System;
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Belirtilen <see cref="DateTime"/> nesnesini yalnızca tarih bilgisini içeren bir <see cref="DateOnly"/> nesnesine dönüştürür.
        /// </summary>
        /// <param name="datetime">Dönüştürülecek <see cref="DateTime"/> nesnesi.</param>
        /// <returns>Yalnızca tarih bilgisini içeren bir <see cref="DateOnly"/> nesnesi.</returns>
        public static DateOnly ToDateOnly(this DateTime datetime) => DateOnly.FromDateTime(datetime);
        /// <summary>
        /// Verilen DateTime nesnesini SQL tarih/saat biçiminde dönüştürür.
        /// </summary>
        /// <param name="datetime">Dönüştürülecek tarih/saat.</param>
        /// <param name="isdatetime">Tarih mi yoksa sadece saat mi olduğunu belirtir.</param>
        /// <returns>SQL biçiminde tarih/saat dizesi.</returns>
        public static string ToSqlDateTime(this DateTime datetime, bool isdatetime)
        {
            if (isdatetime) { return $"CONVERT(DATETIME, '{datetime.ToString("yyyy-MM-dd HH:mm:ss:fff")}')"; }
            return $"CONVERT(DATE, '{datetime.ToString("yyyy-MM-dd")}')";
        }
        /// <summary>
        /// Verilen DateTime nesnesini OADate tamsayı biçimine dönüştürür.
        /// </summary>
        /// <param name="datetime">Dönüştürülecek tarih/saat.</param>
        /// <returns>OADate tamsayı değeri.</returns>
        public static int ToOADateInteger(this DateTime datetime) => Convert.ToInt32(datetime.Date.ToOADate());
        /// <summary>
        /// Verilen DateTime nesnesini ISO 8601 biçiminde dizeye dönüştürür.
        /// </summary>
        /// <param name="datetime">Dönüştürülecek tarih/saat.</param>
        /// <returns>ISO 8601 biçiminde dize.</returns>
        public static string ToISO8601(this DateTime datetime) => datetime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        /// <summary>
        /// Geçerli saatten itibaren geçen saniyeleri döndürür.
        /// </summary>
        /// <param name="datetime">Zaman nesnesi.</param>
        /// <returns>Geçen saniye sayısı.</returns>
        public static int ToSecondsSinceMidnight(this DateTime datetime) => ((datetime.Hour * 3600) + (datetime.Minute * 60) + datetime.Second);
        /// <summary>
        /// Verilen DateTime nesnesini Unix zaman damgası (milisaniye) biçiminde döndürür.
        /// </summary>
        /// <param name="datetime">Dönüştürülecek tarih/saat.</param>
        /// <returns>Unix zaman damgası milisaniye değeri.</returns>
        public static long ToUnixTimestampMilliseconds(this DateTime datetime) => ((datetime.ToUniversalTime().Ticks - DateTime.UnixEpoch.Ticks) / TimeSpan.TicksPerMillisecond);
        /// <summary>
        /// Verilen DateTime nesnesinin ait olduğu ayın ilk gününü döndürür.
        /// </summary>
        /// <param name="datetime">İlgili tarih.</param>
        /// <returns>Ayın ilk günü.</returns>
        public static DateTime GetFirstDayOfMonth(this DateTime datetime) => new DateTime(datetime.Year, datetime.Month, 1);
        /// <summary>
        /// Verilen DateTime nesnesinin ait olduğu ayın son gününü döndürür.
        /// </summary>
        /// <param name="datetime">İlgili tarih.</param>
        /// <returns>Ayın son günü.</returns>
        public static DateTime GetLastDayOfMonth(this DateTime datetime) => new DateTime(datetime.Year, datetime.Month, DateTime.DaysInMonth(datetime.Year, datetime.Month));
    }
}