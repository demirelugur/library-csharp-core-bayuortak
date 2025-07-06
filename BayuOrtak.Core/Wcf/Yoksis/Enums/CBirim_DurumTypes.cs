namespace BayuOrtak.Core.Wcf.Yoksis.Enums
{
    using BayuOrtak.Core.Base;
    using System.ComponentModel;
    /// <summary>
    /// YÖKSİS CBirim_DurumTypes sınıfı, birim durumlarının tanımlandığı enum&#39;ı temsil eder.
    /// </summary>
    public class CBirim_DurumTypes : BaseEnum<CBirim_DurumTypes.Birim_DurumTypes>
    {
        public enum Birim_DurumTypes : byte
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