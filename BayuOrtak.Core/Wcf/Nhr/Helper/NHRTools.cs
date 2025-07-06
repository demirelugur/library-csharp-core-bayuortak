namespace BayuOrtak.Core.Wcf.Nhr.Helper
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using System;
    using System.Drawing;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    using static BayuOrtak.Core.Wcf.Nhr.Enums.CNHR_DurumTypes;
    using static BayuOrtak.Core.Wcf.Nhr.Enums.CNHR_UnvanTypes;
    /// <summary>
    /// NHRTools sınıfı, personel iş kayıtları ve özelliklerine dair çeşitli yardımcı metotlar sağlar.
    /// Bu sınıf, personel unvanları, durum tipleri ve özel durum kontrolleri gibi işlevleri yerine getirir.
    /// </summary>
    public sealed class NHRTools
    {
        /// <summary>
        /// Kullanıcı resmi için varsayılan boyutu temsil eder. Bu boyut 200x280 piksel olarak ayarlanmıştır. <code>new Size(200, 280);</code>
        /// </summary>
        public static readonly Size UserImageSize = new Size(200, 280);
        /// <summary>
        /// Belirtilen T.C. Kimlik Numarası için Personel Otomasyon Sisteminde (NHR) kayıt bulunamadığında hata mesajı döner.
        /// Kullanıcının tercih ettiği dile göre mesaj oluşturur (Türkçe veya İngilizce).
        /// </summary>
        /// <param name="tckn">Kayıt aranacak T.C. Kimlik Numarası.</param>
        /// <param name="dil">Mesajın döneceği dil (tr: Türkçe, en: İngilizce).</param>
        /// <returns>Uygun dilde "Kayıt bulunamadı" mesajı.</returns>
        /// <exception cref="NotSupportedException">Desteklenmeyen bir dil seçildiğinde fırlatılır.</exception>
        public static string TcknNotFound(long tckn, string dil)
        {
            Guard.UnSupportLanguage(dil, nameof(dil));
            if (dil == "tr") { return $"Personel Otomasyonunda(NHR) \"{tckn}\" T.C. Kimlik Numarası üzerine kayıt bulunamadı. ${_title.name_personeldb} veya ${_title.name_bidb} ile iletişime geçiniz!"; }
            return $"No record was found for the Türkiye Republic Identity Number \"{tckn}\" in the Personnel Automation System (NHR). Please contact the Personnel Department or the Information Technology Department!";
        }
        /// <summary>
        /// Personelin 657 Sayılı Kanun 4/B kapsamında sözleşmeli personel olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="personelJobrecordalttipi">Personelin iş kayıt alt tipi</param>
        /// <returns>Sözleşmeli personel ise <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
        public static bool IsSozlesmeliPersonel(string personelJobrecordalttipi) => personelJobrecordalttipi.ToSeoFriendly() == "657-sk-4b-sozlesmeli-personel"; // 657 S.K. 4/B - Sözleşmeli Personel
        /// <summary>
        /// Personelin 35. madde kapsamında olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="personelJobrecordalttipi">Personelin iş kayıt alt tipi</param>
        /// <returns>35. madde kapsamında ise <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
        public static bool IsMadde35ileGiden(string personelJobrecordalttipi) => personelJobrecordalttipi.ToSeoFriendly() == "35-ile-giden"; // 35 ile giden
        /// <summary>
        /// Personelin idari görevli olarak gelip gelmediğini kontrol eder.
        /// </summary>
        /// <param name="personelJobrecordalttipi">Personelin iş kayıt alt tipi</param>
        /// <returns>İdari görevli olarak gelmişse <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
        public static bool IsIdariGorevliGelen(string personelJobrecordalttipi) => personelJobrecordalttipi.ToSeoFriendly() == "idari-gorevli-gelen"; // İdari Görevli Gelen
        /// <summary>
        /// Personelin yabancı uyruklu tam zamanlı olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="personelJobrecordalttipi">Personelin iş kayıt alt tipi</param>
        /// <returns>Yabancı uyruklu tam zamanlı ise <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
        public static bool IsYabanciUyrukluTamZamanli(string personelJobrecordalttipi) => personelJobrecordalttipi.ToSeoFriendly() == "yabanci-uyruklu-tam-zamanli"; // Yabancı Uyruklu - Tam Zamanlı
        /// <summary>
        /// Personelin engelli olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="personelOzurorani">Personelin özür oranı</param>
        /// <param name="personelOzurdurumu">Personelin özür durumu</param>
        /// <returns>Engelli personel ise <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
        public static bool IsEngelliPersonel(string personelOzurorani, string personelOzurdurumu) => (!personelOzurorani.IsNullOrEmpty_string() && !personelOzurdurumu.ToSeoFriendly().Includes("", "engelli-degil"));
        /// <summary>
        /// Personelin unvan tipini belirler.
        /// </summary>
        /// <param name="personelJobrecordtipi">Personelin iş kayıt tipi</param>
        /// <param name="personelJobrecordalttipi">Personelin iş kayıt alt tipi</param>
        /// <returns>Personelin unvan tipini NHR_UnvanTypes türünde döner.</returns>
        public static NHR_UnvanTypes GetUnvanTypes(string personelJobrecordtipi, string personelJobrecordalttipi)
        {
            var v = personelJobrecordtipi.ToSeoFriendly();
            if (v == "akademik") { return NHR_UnvanTypes.aka; }
            if (v == "idari") { return NHR_UnvanTypes.ida; }
            return (IsSozlesmeliPersonel(personelJobrecordalttipi) ? NHR_UnvanTypes.soz : NHR_UnvanTypes.dig);
        }
        /// <summary>
        /// Personelin durum tipini belirler.
        /// </summary>
        /// <param name="personelJobrecordalttipi">Personelin iş kayıt alt tipi</param>
        /// <param name="personelOzurorani">Personelin özür oranı</param>
        /// <param name="personelOzurdurumu">Personelin özür durumu</param>
        /// <returns>Personelin durum tipini NHR_DurumTypes türünde döner, bulunamazsa null döner.</returns>
        public static NHR_DurumTypes? GetDurumTypes(string personelJobrecordalttipi, string personelOzurorani, string personelOzurdurumu)
        {
            var r = 0;
            if (IsMadde35ileGiden(personelJobrecordalttipi)) { r = (int)NHR_DurumTypes.madde35ilegiden; }
            else if (IsIdariGorevliGelen(personelJobrecordalttipi)) { r = (int)NHR_DurumTypes.idarigorevgelen; }
            else if (IsYabanciUyrukluTamZamanli(personelJobrecordalttipi)) { r = (int)NHR_DurumTypes.yabanciuyruklutamzamanli; }
            if (IsEngelliPersonel(personelOzurorani, personelOzurdurumu)) { r = r + (int)NHR_DurumTypes.engelli; }
            return (r > 0 ? (NHR_DurumTypes?)r : null);
        }
        /// <summary>
        /// Sicil numarasının geçerli olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="sicilno">Sicil numarası</param>
        /// <returns>Geçerli sicil numarası ise <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
        public static bool IsSicilNo(string sicilno)
        {
            sicilno = sicilno.ToStringOrEmpty().ToLower();
            return (isnumeric_private(sicilno) || (sicilno.Length == _nhr.sicilno && sicilno.StartsWith("i-") && isnumeric_private(sicilno.Substring(2, 4))));
        }
        private static bool isnumeric_private(string value) => (Int32.TryParse(value, out int _i) && _i > 0 && _i < 10000);
    }
}