namespace BayuOrtak.Core.Helper.Telsam.Enums
{
    using System.ComponentModel;
    /// <summary>
    /// StateTypes Enum değerleri
    /// </summary>
    /// <remarks>Not: Son Kontrol Tarihi: 16.11.2023 14:19 </remarks>
    public enum StateTypes
    {
        [Description("İptal Edildi")]
        iptal = -2,
        [Description("Operatör Tarafından Reddedildi")]
        operatorred = -1,
        [Description("Beklemede")]
        beklemede = 0,
        [Description("Gönderiliyor")]
        gonderiliyor = 1,
        [Description("Gönderildi")]
        gonderildi = 2,
        [Description("Numaraya İletildi")]
        iletildi = 3,
        [Description("Numaraya İletilemedi")]
        iletilmedi = 4,
        [Description("Zamanaşımına Uğradı")]
        zamanasimi = 5
    }
}