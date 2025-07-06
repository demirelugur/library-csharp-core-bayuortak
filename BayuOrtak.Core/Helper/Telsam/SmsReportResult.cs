namespace BayuOrtak.Core.Helper.Telsam
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper.Telsam.Enums;
    using SmsApiNode.Operations;
    public sealed class SmsReportResult
    {
        private string _Error;
        private StateTypes _State;
        private string _Target;
        private SetStateTypes _Setstate;
        /// <summary>
        /// Hata mesajını alır veya ayarlar.
        /// Eğer hata yoksa, boş bir değer döner.
        /// </summary>
        public string error { get { return _Error; } set { _Error = value; } }
        /// <summary>
        /// SMS&#39;in durumunu alır veya ayarlar.
        /// Durum, <see cref="StateTypes"/> enum türü kullanılarak tanımlanmıştır.
        /// </summary>
        public StateTypes state { get { return _State; } set { _State = value; } }
        /// <summary>
        /// SMS&#39;in gönderildiği hedef numarayı alır veya ayarlar.
        /// </summary>
        public string target { get { return _Target; } set { _Target = value; } }
        /// <summary>
        /// SMS&#39;in ayar durumunu alır veya ayarlar.
        /// Durum, <see cref="SetStateTypes"/> enum türü kullanılarak tanımlanmıştır.
        /// </summary>
        public SetStateTypes setstate { get { return _Setstate; } set { _Setstate = value; } }
        /// <summary>
        /// Yeni bir <see cref="SmsReportResult"/> örneği oluşturur.
        /// Varsayılan değerlerle başlatır.
        /// </summary>
        public SmsReportResult() : this(default) { }
        /// <summary>
        /// Yeni bir <see cref="SmsReportResult"/> örneği oluşturur.
        /// Verilen rapor detayını kullanarak başlatır.
        /// </summary>
        /// <param name="item">Rapor detaylarını içeren <see cref="ReportDetailItem"/> nesnesi.</param>
        public SmsReportResult(ReportDetailItem item)
        {
            item = item ?? new ReportDetailItem();
            this.error = item.Error.ToStringOrEmpty();
            this.state = ((Int32.TryParse(item.State, out int _i) && Enum.IsDefined((StateTypes)_i)) ? (StateTypes)_i : default);
            this.target = item.Target.ToStringOrEmpty();
            this.setstate = (Enum.IsDefined((SetStateTypes)item.SetState) ? (SetStateTypes)item.SetState : default);
        }
    }
}