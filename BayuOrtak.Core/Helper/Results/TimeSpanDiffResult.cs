namespace BayuOrtak.Core.Helper.Results
{
    using BayuOrtak.Core.Extensions;
    public sealed class TimeSpanDiffResult
    {
        private readonly TimeSpan ts;
        /// <summary>
        /// <see cref="TimeSpanDiffResult"/> sınıfının bir örneğini oluşturur.
        /// </summary>
        /// <param name="ts">Zaman aralığını temsil eden <see cref="TimeSpan"/> nesnesi.</param>
        /// <remarks>
        /// Bu yapıcı, verilen <see cref="TimeSpan"/> nesnesini alarak sınıfın bir örneğini başlatır.
        /// </remarks>
        public TimeSpanDiffResult(TimeSpan ts)
        {
            this.ts = ts;
        }
        /// <summary>
        /// <see cref="TimeSpanDiffResult"/> sınıfının bir örneğini oluşturur.
        /// </summary>
        /// <param name="to">Zamanı temsil eden <see cref="TimeOnly"/> nesnesi.</param>
        /// <remarks>
        /// Bu yapıcı, verilen <see cref="TimeOnly"/> nesnesini <see cref="TimeSpan"/>&#39;e çevirerek sınıfın bir örneğini başlatır.
        /// </remarks>
        public TimeSpanDiffResult(TimeOnly to) : this(to.ToTimeSpan()) { }
        /// <summary>
        /// Zaman aralığını saat, dakika, saniye ve milisaniye cinsinden dekompoze eden özellik.
        /// </summary>
        /// <returns>
        /// Zaman aralığını oluşturan toplam saat, dakika, saniye ve milisaniye değerlerini döner.
        /// </returns>
        public (double totalhours, int minutes, int seconds, int milliseconds) DecomposeTimeSpan => (Math.Truncate(ts.TotalHours), ts.Minutes, ts.Seconds, ts.Milliseconds);
        /// <summary>
        /// Bir <see cref="TimeSpan"/> değerini saat, dakika ve saniye cinsinden okunabilir bir metin formatına dönüştürür.
        /// </summary>
        /// <param name="p0">Saniye birimi metni</param>
        /// <param name="p1">Dakika birimi metni</param>
        /// <param name="p2">Saat birimi metni</param>
        /// <returns>Süreyi insan okunabilir formatta bir metin olarak döner.</returns>
        public string FormatTimeSpan(string p0 = "sn.", string p1 = "dk.", string p2 = "saat")
        {
            Guard.CheckEmpty(p0, nameof(p0));
            if (this.ts == TimeSpan.Zero) { return $"0 {p0}"; }
            Guard.CheckEmpty(p1, nameof(p1));
            Guard.CheckEmpty(p2, nameof(p2));
            var _dts = this.DecomposeTimeSpan;
            var r = new List<string>();
            if (_dts.totalhours != 0) { r.Add(String.Join(" ", Math.Abs(_dts.totalhours).ToString(), p2)); }
            if (_dts.minutes != 0) { r.Add(String.Join(" ", Math.Abs(_dts.minutes).ToString(), p1)); }
            if ((_dts.seconds > 0 && _dts.milliseconds > 0) || (_dts.seconds < 0 && _dts.milliseconds < 0)) { r.Add($"{Math.Abs(_dts.seconds).ToString()},{Math.Abs(_dts.milliseconds).ToString().Replicate(3)} {p0}"); }
            else if (_dts.seconds != 0 && _dts.milliseconds == 0) { r.Add(String.Join(" ", Math.Abs(_dts.seconds).ToString(), p0)); }
            else if (_dts.milliseconds != 0) { r.Add($"0,{Math.Abs(_dts.milliseconds).ToString().Replicate(3)} {p0}"); }
            return String.Join(" ", r);
        }
    }
}