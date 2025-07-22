namespace BayuOrtak.Core.Wcf.Nhr.Helper
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using System;
    using System.Drawing;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    using static BayuOrtak.Core.Wcf.Nhr.Enums.CNhr_DurumTypes;
    using static BayuOrtak.Core.Wcf.Nhr.Enums.CNhr_UnvanTypes;
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
            if (dil == "en") { return $"No record was found for the Türkiye Republic Identity Number \"{tckn}\" in the Personnel Automation System (NHR). Please contact the Personnel Department or the Information Technology Department!"; }
            return $"Personel Otomasyonunda(NHR) \"{tckn}\" T.C. Kimlik Numarası üzerine kayıt bulunamadı. ${_title.name_personeldb} veya ${_title.name_bidb} ile iletişime geçiniz!";
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
        public static Nhr_UnvanTypes GetUnvanTypes(string personelJobrecordtipi, string personelJobrecordalttipi)
        {
            var _r = personelJobrecordtipi.ToSeoFriendly();
            if (_r == "akademik") { return Nhr_UnvanTypes.aka; }
            if (_r == "idari") { return Nhr_UnvanTypes.ida; }
            return (IsSozlesmeliPersonel(personelJobrecordalttipi) ? Nhr_UnvanTypes.soz : Nhr_UnvanTypes.dig);
        }
        /// <summary>
        /// Personelin durum tipini belirler.
        /// </summary>
        /// <param name="personelJobrecordalttipi">Personelin iş kayıt alt tipi</param>
        /// <param name="personelOzurorani">Personelin özür oranı</param>
        /// <param name="personelOzurdurumu">Personelin özür durumu</param>
        /// <returns>Personelin durum tipini NHR_DurumTypes türünde döner, bulunamazsa null döner.</returns>
        public static Nhr_DurumTypes? GetDurumTypes(string personelJobrecordalttipi, string personelOzurorani, string personelOzurdurumu)
        {
            var _r = 0;
            if (IsMadde35ileGiden(personelJobrecordalttipi)) { _r = (int)Nhr_DurumTypes.madde35ilegiden; }
            else if (IsIdariGorevliGelen(personelJobrecordalttipi)) { _r = (int)Nhr_DurumTypes.idarigorevgelen; }
            else if (IsYabanciUyrukluTamZamanli(personelJobrecordalttipi)) { _r = (int)Nhr_DurumTypes.yabanciuyruklutamzamanli; }
            if (IsEngelliPersonel(personelOzurorani, personelOzurdurumu)) { _r = _r + (int)Nhr_DurumTypes.engelli; } // Not: else if yazılmamalıdır!
            return (_r > 0 ? (Nhr_DurumTypes?)_r : null);
        }
        /// <summary>
        /// Verilen bir sicil numarasını kontrol eder ve belirtilen formata uygun şekilde düzenler. Sicil numarası &quot;İ-&quot; ile başlayabilir ve maksimum 6 karakter olabilir (örn. İ-1234). Alternatif olarak, sadece 4 haneli bir sayı olabilir (örn. 0939). Sayı 1000&#39;den küçükse, 4 haneli olacak şekilde başına sıfır eklenir (örn. 0012). Sayı 1 ile 9999 arasında olmalıdır.
        /// </summary>
        /// <param name="value">Kontrol edilecek sicil numarası (örn. &quot;İ-1&quot;, &quot;12&quot;, &quot;5001&quot;).</param>
        /// <param name="outvalue">Geçerli bir sicil numarası ise formatlanmış çıktı (örn. &quot;İ-0001&quot;, &quot;0012&quot;). Aksi takdirde boş string.</param>
        /// <returns>Sicil numarası geçerliyse <see langword="true"/>, aksi takdirde <see langword="false"/> döner.</returns>
        /// <remarks>
        /// - &quot;İ-&quot; ile başlayan girişler için maksimum uzunluk 6 karakterdir.
        /// - &quot;İ-&quot; olmadan sadece sayı girişleri için maksimum uzunluk 4 karakterdir.
        /// - Geçersiz girişler (örn. harf içeren, 9999&#39;dan büyük sayılar) için <see langword="false"/> döner.
        /// </remarks>
        public static bool TrySicilNo(string value, out string outvalue)
        {
            value = value.ToStringOrEmpty().ToUpper();
            outvalue = "";
            if (value == "") { return false; }
            var _startsI = value.StartsWith("İ-");
            if (value.Length > (_startsI ? _nhr.sicilno : 4)) { return false; }
            if (!Int32.TryParse((_startsI ? value.Substring(2) : value), out int number) || number < 1 || number > 9999) { return false; }
            outvalue = String.Concat(_startsI ? "İ-" : "", number.ToString().Replicate(4));
            return true;
        }
    }
}