namespace BayuOrtak.Core.Wcf.Yoksis.Enums
{
    using BayuOrtak.Core.Base;
    using System.ComponentModel;
    /// <summary>
    /// YÖKSİS CBirim_TurTypes sınıfı, birim türlerinin tanımlandığı enum&#39;ı temsil eder.
    /// </summary>
    public class CBirim_TurTypes : BaseEnum<CBirim_TurTypes.Birim_TurTypes>
    {
        public enum Birim_TurTypes : byte
        {
            [Description("Üniversite")]
            universite = 1,
            [Description("Fakülte")]
            fakulte = 2,
            [Description("Enstitü")]
            enstitu = 4,
            [Description("Yüksekokul")]
            yuksekokul = 5,
            [Description("Meslek Yüksekokulu")]
            meslekyuksekokul = 6,
            [Description("Uygulama ve Araştırma Merkezi")]
            uygulamavearastirmamerkezi = 8,
            [Description("Rektörlük")]
            rektorluk = 9,
            [Description("Bölüm")]
            bolum = 10,
            [Description("Ana Bilim Dalı")]
            anabilimdali = 11,
            [Description("Bilim Dalı")]
            bilimdali = 12,
            [Description("Lisans")]
            lisans = 13,
            [Description("Ana Sanat Dalı")]
            anasanatdali = 15,
            [Description("Yüksek Lisans Programı")]
            yukseklisansprogrami = 16,
            [Description("Doktora Programı")]
            doktoraprogrami = 17,
            [Description("Önlisans")]
            onlisans = 20,
            [Description("Disiplinlerarası Anabilim Dalı")]
            disiplinlerarasianabilimdali = 21,
            [Description("Disiplinlerarası Yüksek Lisans Programı")]
            disiplinlerarasiyukseklisansprogrami = 22,
            [Description("Tezsiz Yüksek Lisans Programı")]
            tezsizyukseklisansprogrami = 29,
            [Description("Disiplinlerarası Tezsiz Yüksek Lisans Programı")]
            disiplinlerarasitezsizyukseklisansprogrami = 30
        }
    }
}