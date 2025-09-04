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
        /// İki tarih arasındaki farkı hesaplar. Fark, yıl, ay, gün ve zaman aralığı olarak döner.
        /// </summary>
        public (int yil, int ay, int gun, TimeSpan ts) CalculateDateDifference()
        {
            if (this.basdate == this.bitdate || Math.Abs(this.bitdate.Ticks - this.basdate.Ticks) < TimeSpan.TicksPerMillisecond) { return (0, 0, 0, TimeSpan.Zero); }
            int _yil = this.bitdate.Year - this.basdate.Year, _ay = this.bitdate.Month - this.basdate.Month, _gun = this.bitdate.Day - this.basdate.Day;
            var _ts = (this.bitdate.TimeOfDay - this.basdate.TimeOfDay);
            var _tsisnegative = _ts < TimeSpan.Zero;
            if (_tsisnegative) { _gun--; }
            if (_gun < 0)
            {
                if (_yil == 0 && _ay == 0) { _gun = 0; }
                else
                {
                    _ay--;
                    _gun = (DateTime.DaysInMonth(this.basdate.Year, this.basdate.Month) - this.basdate.Day + this.bitdate.Day);
                }
            }
            if (_ay < 0)
            {
                _yil--;
                _ay = 12 + _ay;
            }
            return (_yil, _ay, _gun, ((this.bitdate > this.basdate && _tsisnegative) ? new TimeSpan(TimeSpan.TicksPerDay - _ts.Negate().Ticks) : _ts));
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
            var _r = new List<string>();
            var _v = this.CalculateDateDifference();
            if (_v.yil > 0) { _r.Add(String.Join(" ", _v.yil.ToString(), p0)); }
            if (_v.ay > 0) { _r.Add(String.Join(" ", _v.ay.ToString(), p1)); }
            if (_v.gun > 0) { _r.Add(String.Join(" ", _v.gun.ToString(), p2)); }
            return (_r.Count > 0 ? String.Join(", ", _r) : "");
        }
    }
}