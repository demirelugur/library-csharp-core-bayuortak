# BayuOrtak.Core

[T.C. Bayburt Üniversitesi](https://www.bayburt.edu.tr) için geliştirilmiş, kurumsal projelerde tekrar kullanılabilir, doğrulama attribute'ları, yardımcı sınıflar, extension method'lar ve servis entegrasyonları içeren bir .NET Core kütüphanesidir. Local NuGet paketi olarak dağıtılır.

## Özellikler

- **Gelişmiş Doğrulama Attribute'ları:** TCKN, Kurum Sicil No, e-Posta, ISBN, IP, MAC, Telefon, Tarih, URL, Pozitif Sayı vb. için hazır validasyonlar.
- **Yardımcı Sınıflar:** e-Posta Gönderimi, Şifreleme, Dosya İşlemleri, LDAP, Dapper, Parola Üretimi, SMTP Ayarları vb.
- **Extension Method'lar:** Koleksiyon, String, DateTime, IQueryable, Object vb. gibi temel tipler için pratik uzantılar.
- **WCF Servis Entegrasyonları:** Üniversiteye özel NHR(Personel Özlük Sistemi), OGR(Öğrenci Web Servisi), [NVI](https://www.nvi.gov.tr), [YÖKSİS](https://www.yok.gov.tr) servislerine kolay erişim.
- **Enum ve Sonuç Tipleri:** Standart hata yönetimi ve veri modellemesi için Enum ve Result tipleri.

## Kullanım

### 1. Doğrulama Attribute'ları

```csharp
using BayuOrtak.Attributes.DataAnnotations;
using static BayuOrtak.Helper.GlobalConstants;
public class PersonelModel
{
    [Validation_Tckn]
    [DefaultValue(null)]
    public long? TCKN { get; set; }

    [Validation_StringLength(_nhr.sicilno)]
    [Validation_BayburtUniSicilNo]
    [DefaultValue(null)]
    public string? SicilNo { get; set; }

    [Validation_Required]
    [EnumDataType(typeof(KampusTypes), ErrorMessage = _validationerrormessage.enumdatatype)]
    [DefaultValue(KampusTypes.baberti)]
    public KampusTypes kampustip { get; set; }

    [Validation_MinDateOnly]
    [DefaultValue(null)]
    public DateOnly? tarih { get; set; }

    [Validation_Required]
    [Validation_Includes<string>(true, "tumu", "basarili", "basarisiz")]
    [DefaultValue("tumu")]
    public string durum { get; set; }

    [Validation_StringLength(_maximumlength.uri)]
    [Validation_UrlHttp]
    [DefaultValue("")]
    public string? src_tr { get; set; }
}
```

### 2. Yardımcı Sınıflar

```csharp
using BayuOrtak.Helper;

var password = PasswordGenerator.Generate(12, true, true, true, true);
var isvalid = StrongPasswordValid.TryIsWarning(password, null, "Ad", "SOYAD", "tr", out string[] _warnings);
```

### 3. Extension Method'lar

```csharp
using BayuOrtak.Extensions;

var list = new List<int> { 1, 2, 3 };
var hasitems = !list.IsNullOrEmpty_collection();
```

### 4. WCF Servis Kullanımı

```csharp
using BayuOrtak.Wcf.Nhr;

var nhr = new NHRHelper("###", "###", "###", "###");
var result = await nhr.bytckimliknoAsync("12345678901");
```

## Katkı ve Geri Bildirim

- Katkıda bulunmak için lütfen bir pull request açın veya issue oluşturun.
- Hatalar ve öneriler için: 📧 [ugurdemirel@bayburt.edu.tr](mailto:ugurdemirel@bayburt.edu.tr)

## Lisans

MIT Lisansı (MIT)

Tüm hakları saklıdır. 2024 [Uğur DEMİREL](https://www.bayburt.edu.tr/ugur-demirel)

Bu yazılımı ve ilişkilendirilmiş dokümantasyon dosyalarını (bundan böyle "BayuOrtak.Core" olarak anılacaktır) 
edinen herhangi bir kişiye, bu Yazılım ile sınırsız bir şekilde işlem yapma izni verilmektedir. 
Bu, kopyalama, değiştirme, birleştirme, yayınlama, dağıtma, alt lisans verme ve/veya Yazılım'ın kopyalarını 
satma haklarını içermektedir. Yazılım'ı sağlanan kişilerin de aynı şekilde kullanmasına izin verilmiştir; 
ancak aşağıdaki koşullara uyulması gerekmektedir:

Yukarıdaki telif hakkı bildirimi ve bu izin bildirimi, Yazılım'ın tüm kopyalarında 
veya önemli kısımlarında yer almalıdır.

YAZILIM, "OLDUĞU GİBİ" SUNULMAKTADIR, HERHANGİ BİR TÜR GARANTİ VERİLMEMEKTEDİR, AÇIK YA DA ZIMNİ, 
TİCARETELİK, BELİRLİ BİR AMACA UYGUNLUK VEYA İHLAL ETMEME GARANTİLERİ DAHİL, ANCAK BUNLARLA 
SINIRLI OLMAYAN GARANTİLERİ KAPSAMAKTADIR. YAZARLAR YA DA TELİF HAKKI SAHİPLERİ, YAZILIM'IN 
KULLANIMINDAN YA DA DİĞER ŞEKİLLERDEKİ İŞLEMLERDEN KAYNAKLANAN HERHANGİ BİR TALEP, 
ZARAR VEYA DİĞER YÜKÜMLÜLÜKLERDEN, SÖZLEŞME, HUKUKSUZ EYLEM YA DA BAŞKA BİR YOLLA 
BAĞLANTILI OLARAK SORUMLU TUTULAMAZ