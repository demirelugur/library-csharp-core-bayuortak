namespace BayuOrtak.Core.Wcf.Nhr.Helper
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Wcf.Nvi;
    using BayuOrtak.Core.Wcf.Nvi.Enums;
    using System;
    using System.Linq;
    using Wcf_Nhr_personelinfo;
    using static BayuOrtak.Core.Wcf.Nhr.Enums.CNHR_DurumTypes;
    using static BayuOrtak.Core.Wcf.Nhr.Enums.CNHR_UnvanTypes;
    /// <summary>
    /// Personel Otomasyonu için NHRExtensions sınıfı, wspersonel objesine ve ilgili türlere çeşitli yardımcı ve kontrol metotları sağlar.
    /// Bu sınıf, personel objeleri üzerinde null kontrolü, aktiflik durumu, unvan ve durum tipleri gibi 
    /// çeşitli bilgiler elde etmeye yönelik genişletme (extension) metotları içerir.
    /// </summary>
    public static class NHRExtensions
    {
        /// <summary>
        /// Personelin aktif olup olmadığını kontrol eder. Personelin durumunun &quot;aktif&quot; olması gerekir.
        /// </summary>
        /// <param name="wspersonel">Kontrol edilecek personel objesi</param>
        /// <returns>Personel aktifse <see langword="true"/>, değilse <see langword="false"/></returns>
        public static bool IsActivePersonel(this wspersonel wspersonel) => (wspersonel != null && wspersonel.personelJobrecorddurum.ToSeoFriendly() == "aktif");
        /// <summary>
        /// Personelin sözleşmeli personel olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="wspersonel">Kontrol edilecek personel objesi</param>
        /// <returns>Sözleşmeli personelse <see langword="true"/>, değilse <see langword="false"/></returns>
        public static bool IsSozlesmeliPersonel(this wspersonel wspersonel) => (wspersonel != null && NHRTools.IsSozlesmeliPersonel(wspersonel.personelJobrecordalttipi));
        /// <summary>
        /// Personelin 35. madde ile görevlendirilmiş olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="wspersonel">Kontrol edilecek personel objesi</param>
        /// <returns>35. madde ile görevlendirilmişse <see langword="true"/>, değilse <see langword="false"/></returns>
        public static bool IsMadde35ileGiden(this wspersonel wspersonel) => (wspersonel != null && NHRTools.IsMadde35ileGiden(wspersonel.personelJobrecordalttipi));
        /// <summary>
        /// Personelin idari görevli olarak görevlendirilmiş olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="wspersonel">Kontrol edilecek personel objesi</param>
        /// <returns>Idari görevliyse <see langword="true"/>, değilse <see langword="false"/></returns>
        public static bool IsIdariGorevliGelen(this wspersonel wspersonel) => (wspersonel != null && NHRTools.IsIdariGorevliGelen(wspersonel.personelJobrecordalttipi));
        /// <summary>
        /// Personelin yabancı uyruklu tam zamanlı olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="wspersonel">Kontrol edilecek personel objesi</param>
        /// <returns>Yabancı uyruklu tam zamanlı ise <see langword="true"/>, değilse <see langword="false"/></returns>
        public static bool IsYabanciUyrukluTamZamanli(this wspersonel wspersonel) => (wspersonel != null && NHRTools.IsYabanciUyrukluTamZamanli(wspersonel.personelJobrecordalttipi));
        /// <summary>
        /// Personelin engelli olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="wspersonel">Kontrol edilecek personel objesi</param>
        /// <returns>Engelli ise <see langword="true"/>, değilse <see langword="false"/></returns>
        public static bool IsEngelliPersonel(this wspersonel wspersonel) => (wspersonel != null && NHRTools.IsEngelliPersonel(wspersonel.personelOzurorani, wspersonel.personelOzurdurumu));
        /// <summary>
        /// Personelin unvan tipini döner.
        /// </summary>
        /// <param name="wspersonel">Kontrol edilecek personel objesi</param>
        /// <returns>Unvan tipini döner</returns>
        public static NHR_UnvanTypes GetUnvanTypes(this wspersonel wspersonel) => NHRTools.GetUnvanTypes(wspersonel.personelJobrecordtipi, wspersonel.personelJobrecordalttipi);
        /// <summary>
        /// Personelin durum tipini döner.
        /// </summary>
        /// <param name="wspersonel">Kontrol edilecek personel objesi</param>
        /// <returns>Durum tipini döner</returns>
        public static NHR_DurumTypes? GetDurumTypes(this wspersonel wspersonel) => NHRTools.GetDurumTypes(wspersonel.personelJobrecordtipi, wspersonel.personelOzurorani, wspersonel.personelOzurdurumu);
        /// <summary>
        /// Personelin kimlik bilgilerini döner. Yeni kimlik veya eski kimlik bilgilerine göre kontrol yapılır.
        /// </summary>
        /// <param name="wspersonel">Kimlik bilgisi kontrol edilecek personel objesi</param>
        /// <returns>Kimlik tipi ve seri numarasını döner</returns>
        public static (NVIKimlikTypes? tip, string serino) GetKimlikInfo(this wspersonel wspersonel)
        {
            var _k = String.Concat(wspersonel.personelKpscuzdanseri.ToStringOrEmpty(), wspersonel.personelKpscuzdanno.ToStringOrEmpty()).ToUpper();
            if (NVIHelperTR.TryValidate_YeniKimlikSeriNo(_k, out _)) { return (NVIKimlikTypes.yeni, _k); }
            if (NVIHelperTR.TryValidate_EskiNufusCuzdaniSeriNo(_k, out _, out _)) { return (NVIKimlikTypes.eski, _k); }
            return (null, "");
        }
        /// <summary>
        /// Personelin aktif fiili görevini döner. Personelin birim ID&#39;sine göre geçerli fiili görev aranır.
        /// </summary>
        /// <param name="wsfiiligorevs">Personelin fiili görevleri</param>
        /// <param name="personelBirimid">Personelin bağlı olduğu birim ID&#39;si</param>
        /// <returns>Aktif fiili görev objesi</returns>
        public static wsfiiligorev GetAktifFiiliGorev(this wsfiiligorev[] wsfiiligorevs, int personelBirimid) => (wsfiiligorevs ?? Array.Empty<wsfiiligorev>()).Where(x => x.fiiligorevMasterbirimid > 0 && x.fiiligorevMasterbirimid != personelBirimid && DateTime.Today > x.fiiligorevBastar.Date && (x.fiiligorevBittar == DateTime.MinValue || x.fiiligorevBittar.Date > DateTime.Today)).OrderByDescending(x => x.fiiligorevBastar).FirstOrDefault() ?? new wsfiiligorev { };
    }
}