namespace BayuOrtak.Core.Helper.Telsam.Enums
{
    using System.ComponentModel;
    /// <summary>
    /// SetStateTypes Enum değerleri
    /// </summary>
    /// <remarks>Not: Son Kontrol Tarihi: 16.11.2023 14:19 </remarks>
    public enum SetStateTypes : byte
    {
        [Description("Beklemede")]
        beklemede = 0,
        [Description("Gönderiliyor")]
        gonderiliyor = 1,
        [Description("Gönderildi")]
        gonderildi = 2,
        [Description("Raporlanıyor")]
        rapor = 3,
        [Description("Bitti")]
        bitti = 5
    }
}