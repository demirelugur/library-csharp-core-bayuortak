namespace BayuOrtak.Core.Wcf.Ogr.Enums
{
    using System.ComponentModel;
    public enum Ogr_DurumTypes
    {
        [Description("Kayıtlanmayan")]
        kayitlanmayan = 905001,
        [Description("Aktif")]
        aktif = 905002,
        [Description("Aktif Değil")]
        aktifdegil = 905003,
        [Description("Mezun")]
        mezun = 905004,
        [Description("İlişkisi Kesilmiş")]
        ilisigikesilmis = 905005
    }
}