namespace BayuOrtak.Core.Helper.Results
{
    using static BayuOrtak.Core.Helper.OrtakTools;
    public sealed class DateDiffResult
    {
        private readonly DateTime basdate;
        private readonly DateTime bitdate;
        /// <summary>
        /// Belirtilen başlangıç ve bitiş tarihlerini kullanarak bir tarih farkı hesaplama nesnesi oluşturur.
        /// </summary>
        /// <param name="basdate">Başlangıç tarihi. Tarih olarak dönüştürülebilen bir nesne verilmelidir. Eğer dönüştürülemezse varsayılan tarih kullanılır.</param>
        /// <param name="bitdate">Bitiş tarihi. Tarih olarak dönüştürülebilen bir nesne verilmelidir. Eğer null verilirse, bugünün tarihi (DateTime.Today) kullanılır.</param>
        public DateDiffResult(object basdate, object bitdate)
        {
            this.basdate = _to.ToDateTimeFromObject(basdate, default);
            this.bitdate = (bitdate == null ? DateTime.Today : _to.ToDateTimeFromObject(bitdate, default));
        }
        /// <summary>
        /// İki tarih arasındaki farkı hesaplar.
        /// Fark, yıl, ay, gün ve zaman aralığı olarak döner.
        /// </summary>
        public (int yil, int ay, int gun, TimeSpan ts) CalculateDateDifference()
        {
            if (this.basdate == this.bitdate || Math.Abs(this.bitdate.Ticks - this.basdate.Ticks) < TimeSpan.TicksPerMillisecond) { return (0, 0, 0, TimeSpan.Zero); }
            int yil = this.bitdate.Year - this.basdate.Year, ay = this.bitdate.Month - this.basdate.Month, gun = this.bitdate.Day - this.basdate.Day;
            var ts = (this.bitdate.TimeOfDay - this.basdate.TimeOfDay);
            var tsIsNegative = ts < TimeSpan.Zero;
            if (tsIsNegative) { gun--; }
            if (gun < 0)
            {
                if (yil == 0 && ay == 0) { gun = 0; }
                else
                {
                    ay--;
                    gun = (DateTime.DaysInMonth(this.basdate.Year, this.basdate.Month) - this.basdate.Day + this.bitdate.Day);
                }
            }
            if (ay < 0)
            {
                yil--;
                ay = 12 + ay;
            }
            return (yil, ay, gun, ((this.bitdate > this.basdate && tsIsNegative) ? new TimeSpan(TimeSpan.TicksPerDay - ts.Negate().Ticks) : ts));
        }
        /// <summary>
        /// Tarih farkını yıl, ay ve gün olarak Türkçe formatta birleştirir.
        /// </summary>
        /// <param name="p0">Yıl için kullanılacak metin.</param>
        /// <param name="p1">Ay için kullanılacak metin</param>
        /// <param name="p2">Gün için kullanılacak metin</param>
        /// <returns>Tarih farkını yıl, ay ve gün cinsinden birleştirilmiş string olarak döndürür. Fark yoksa boş string döner.</returns>
        public string FormatDateDifference(string p0 = "yıl", string p1 = "ay", string p2 = "gün")
        {
            Guard.CheckEmpty(p0, nameof(p0));
            Guard.CheckEmpty(p1, nameof(p1));
            Guard.CheckEmpty(p2, nameof(p2));
            var r = new List<string>();
            var _v = this.CalculateDateDifference();
            if (_v.yil > 0) { r.Add(String.Join(" ", _v.yil.ToString(), p0)); }
            if (_v.ay > 0) { r.Add(String.Join(" ", _v.ay.ToString(), p1)); }
            if (_v.gun > 0) { r.Add(String.Join(" ", _v.gun.ToString(), p2)); }
            return (r.Count > 0 ? String.Join(", ", r) : "");
        }
    }
}