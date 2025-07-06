namespace BayuOrtak.Core.Helper
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Globalization;
    public sealed class GlobalConstants
    {
        public const long yoksis_UNI_code = 104922;
        public static readonly string[] filesizeunits = new string[] { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public static readonly char[] turkishcharacters = new char[] { 'Ç', 'ç', 'Ğ', 'ğ', 'İ', 'ı', 'Ö', 'ö', 'Ş', 'ş', 'Ü', 'ü' };
        public static readonly JsonSerializerSettings jsonserializersettings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            NullValueHandling = NullValueHandling.Include,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            }
        };
        /// <summary>
        /// image/* uzantı değerleri:
        /// <para><code>new string[] { &quot;.apng&quot;, &quot;.avif&quot;, &quot;.bmp&quot;, &quot;.dib&quot;, &quot;.gif&quot;, &quot;.heic&quot;, &quot;.heif&quot;, &quot;.ico&quot;, &quot;.jfif&quot;, &quot;.jpeg&quot;, &quot;.jpg&quot;, &quot;.pbm&quot;, &quot;.pgm&quot;, &quot;.pjp&quot;, &quot;.pjpeg&quot;, &quot;.png&quot;, &quot;.ppm&quot;, &quot;.svg&quot;, &quot;.svgz&quot;, &quot;.tif&quot;, &quot;.tiff&quot;, &quot;.webp&quot;, &quot;.xbm&quot; };</code></para>
        /// </summary>
        public static readonly string[] imageextensions = new string[] { ".apng", ".avif", ".bmp", ".dib", ".gif", ".heic", ".heif", ".ico", ".jfif", ".jpeg", ".jpg", ".pbm", ".pgm", ".pjp", ".pjpeg", ".png", ".ppm", ".svg", ".svgz", ".tif", ".tiff", ".webp", ".xbm" };
        public sealed class _date
        {
            public const string ddMMyyyy_HHmmss = "dd.MM.yyyy HH:mm:ss";
            public const string ddMMyyyy_HHmm = "dd.MM.yyyy HH:mm";
            public const string ddMMyyyy = "dd.MM.yyyy";
            public const string full_datetime = "dddd, dd MMMM yyyy HH:mm:ss";
            public const string yyyyMMdd_HHmmss = "yyyy-MM-dd HH:mm:ss";
            public const string yyyyMMdd_HHmm = "yyyy-MM-dd HH:mm";
            public const string yyyyMMdd = "yyyy-MM-dd";
            /// <summary>
            /// T.C. Bayburt Üniversitesi Resmi Kuruluş Tarihi 
            /// <para>new DateTime(2008, 6, 1);</para>
            /// <para>new Date(1212267600000);</para>
            /// <para>2008-05-31T21:00:00.000Z</para>
            /// </summary>
            public static readonly DateTime bayburtUniversityFoundationDate = new DateTime(2008, 6, 1);
            /// <summary>
            /// OLE Automation için başlangıç tarihi
            /// <para>new DateTime(1899, 12, 30)</para>
            /// </summary>
            public static readonly DateTime beginOfOLEAutomation = new DateTime(1899, 12, 30);
        }
        public sealed class _domain
        {
            public const string bayburtuni = "https://bayburt.edu.tr";
            public const string bilgiislemdb = "https://bayburt.edu.tr/bilgi-islem";
            public const string example = "https://example.com";
            public const string personeldb = "https://bayburt.edu.tr/personel";
            public const string ogrencidb = "https://bayburt.edu.tr/ogrenci-isleri";
            public const string sifredegistir_ogrenci = "https://sorgula.bayburt.edu.tr/ogrenci-sifre-degistir";
            public const string sifredegistir_personel = "https://sorgula.bayburt.edu.tr/personel-kurum-sifre-degistir";
        }
        public sealed class _maximumlength
        {
            public const int cuzdanserino = 9;
            public const int ebyscode = 21;
            public const int eposta = 50;
            public const int hash = 64;
            public const int ipaddress = 15;
            public const int mac = 17;
            public const int ogrencino = 15;
            public const int proje = 50;
            public const int tckn = 11;
            public const int uri = 255;
            public const int vkn = 10;
            public const int sifre = 256;
            public const int sms_singlemaxlength = 150;
            public const int sms_messagelength = 4000;
        }
        public sealed class _nhr
        {
            public const int ad = 30;
            public const int bidb_diger = 617;
            public const int bidb_idari = 301;
            public const int birimadi = 255;
            public const int eposta = 35;
            public const int jobrecordalttipi = 50;
            public const int kuladi = 20;
            public const int sicilno = 6;
            public const int soyad = 40;
            public const int unvanadi = 128;
            /// <summary>
            /// Öğretim Üyelerine ait unvan ID&#39;lerini içeren dizi.
            /// <list type="bullet">
            /// <item>
            /// <term>Profesör</term>: 1
            /// </item>
            /// <item>
            /// <term>Doçent</term>: 2
            /// </item>
            /// <item>
            /// <term>Doktor Öğretim Üyesi</term>: 10000
            /// </item>
            /// </list>
            /// </summary>
            public static readonly int[] ogretimuye_unvanids = new int[] { 1, 2, 10000 };
        }
        public sealed class _portal // TODO: Yeni Web Sitesinden sonra yapılandırılmalı!
        {
            public const int codelength_fakulte = 13;
            public const int codelength_birim = 20;
            public const int codelength_abd = 27;
            /// <summary>
            /// Sekreter Unvan Kodları
            /// <list type="bullet">
            /// <item>
            /// <term>Enstitü</term>: IDY0040
            /// </item>
            /// <item>
            /// <term>Fakülte</term>: IDY0045
            /// </item>
            /// <item>
            /// <term>Meslek Yüksekokulu</term>: IDY0090
            /// </item>
            /// <item>
            /// <term>Yüksekokul Sekreter</term>: IDY0145
            /// </item>
            /// </list>
            /// </summary>
            public static readonly Dictionary<string, string> sekreterunvans = new Dictionary<string, string> {
                { "IDY0040", "Enstitü Sekreteri" },
                { "IDY0045", "Fakülte Sekreteri" },
                { "IDY0090", "Meslek Yüksekokulu Sekreteri" },
                { "IDY0145", "Yüksekokul Sekreteri" }
            };
        }
        public sealed class _title
        {
            public const string koordinat_bayburtuniversitesi = "40.250230335582955,40.23025617808133";
            public const string ebys = "EBYS(Elektronik Belge Yönetim Sistemi)";
            public const string iletisim_bilgiislem = "Bilgi İşlem Daire Başkanlığı ile iletişime geçiniz!";
            public const string iletisim_bilgiislem_sistem = "Bilgi İşlem Daire Başkanlığı Sistem ekibi ile iletişime geçiniz!";
            public const string iletisim_bilgiislem_webyazilim = "Bilgi İşlem Daire Başkanlığı Web - Yazılım ekibi ile iletişime geçiniz!";
            public const string iletisim_personeldb = "Personel Daire Başkanlığı ile iletişime geçiniz!";
            public const string iletisim_ogrencidb = "Öğrenci İşleri Daire Başkanlığı ile iletişime geçiniz!";
            public const string isbn = "ISBN(Uluslararası Standart Kitap Numarası)";
            public const string mac = "MAC(Media Access Control)";
            public const string name_bayburtuniversitesi = "Bayburt Üniversitesi";
            public const string name_bidb = "Bilgi İşlem Daire Başkanlığı";
            public const string name_personeldb = "Personel Daire Başkanlığı";
            public const string name_ogrencidb = "Öğrenci İşleri Daire Başkanlığı";
            public const string xss = "XSS(Cross - Site Scripting)";
        }
        public sealed class _validationerrormessage
        {
            public const string array_minlength = "{0} boş geçilemez! En az {1} adet eleman içermelidir.";  // {1} -> minimum
            public const string email = "{0}, geçerli bir e-Posta adresi olmalıdır!";
            public const string enumdatatype = "{0}, geçerli değildir!";
            public const string range = "{0}, [{1} - {2}] arasında olmalıdır!"; // {1} -> minimum, {2} -> maksimum
            public const string rangepositive = "{0}, sıfırdan büyük bir değer olmalıdır!";
            public const string required = "{0}, boş geçilemez!";
            public const string stringlength_max = "{0}, en fazla {1} karakter uzunluğunda olmalıdır."; // {1} -> maksimum
            public const string stringlength_maxmin = "{0}, en az {2}, en fazla {1} karakter uzunluğunda olmalıdır!";  // {1} -> maksimum, {2} -> minimum
            public const string stringlength_maxminequal = "{0}, tam olarak {1} karakter uzunluğunda olmalıdır!"; // {1} -> maksimum
        }
    }
}