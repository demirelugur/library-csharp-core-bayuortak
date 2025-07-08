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