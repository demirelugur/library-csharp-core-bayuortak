namespace BayuOrtak.Core.Wcf.Ogr.Enums
{
    using System.ComponentModel;
    public enum OgretimDuzeyiTypes
    {
        [Description("Önlisans")]
        onlisans = 900001,
        [Description("Lisans")]
        lisans = 900002,
        [Description("Yüksek Lisans")]
        ylisans = 900003,
        [Description("Doktora")]
        doktora = 900004
    }
}