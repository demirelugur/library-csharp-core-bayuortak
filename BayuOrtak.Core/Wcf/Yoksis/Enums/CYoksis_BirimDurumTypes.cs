namespace BayuOrtak.Core.Wcf.Yoksis.Enums
{
    using BayuOrtak.Core.Base;
    using System.ComponentModel;
    public class CYoksis_BirimDurumTypes : BaseEnum<CYoksis_BirimDurumTypes.Yoksis_BirimDurumTypes>
    {
        public enum Yoksis_BirimDurumTypes : byte
        {
            [Description("Pasif")]
            pasif = 0,
            [Description("Aktif")]
            aktif = 1,
            [Description("Yarı Pasif")]
            yari = 2,
            [Description("Kapatılmış")]
            closed = 3
        }
    }
}