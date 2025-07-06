namespace BayuOrtak.Core.Wcf.Nhr.Objects
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Wcf.Nhr.Helper;
    using BayuOrtak.Core.Wcf.Nvi.Enums;
    using System;
    using System.Net.Mail;
    using Wcf_Nhr_personelinfo;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    using static BayuOrtak.Core.Helper.OrtakTools;
    using static BayuOrtak.Core.Wcf.Nhr.Enums.CNHR_DurumTypes;
    using static BayuOrtak.Core.Wcf.Nhr.Enums.CNHR_UnvanTypes;
    /// <summary>
    /// UyeInfoResult sınıfı, personel bilgilerini tutan bir veri yapısıdır.
    /// Personel adı, soyadı, unvanı, sicil numarası gibi bilgileri içerir ve çeşitli personel durumları hakkında bilgi sağlar.
    /// Ayrıca, ilgili özelliklerdeki maksimum uzunlukları sınırlandırır.
    /// </summary>
    public sealed class UyeInfoResult
    {
        /// <summary>Personelin adı.</summary>
        public string ad { get; }
        /// <summary>Personelin soyadı.</summary>
        public string soyad { get; }
        /// <summary>Personelin kurumsal e-Posta kullanıcı adı.</summary>
        public string kuladi { get; }
        /// <summary>Personelin T.C. Kimlik Numarası.</summary>
        public long tckn { get; }
        /// <summary>Personelin kurum sicil numarası.</summary>
        public string sicilno { get; }
        /// <summary>Personelin cinsiyeti (&#39;E&#39;: Erkek, &#39;K&#39;: Kadın).</summary>
        public char gender { get; }
        /// <summary>Personelin unvan bilgisi (tip, id, adı, kısa adı).</summary>
        public (NHR_UnvanTypes tip, int id, string adi, string adi_kisa) unvan { get; }
        /// <summary>Personelin durumu (aktif, pasif, vs.).</summary>
        public NHR_DurumTypes? durumtip { get; }
        /// <summary>Personelin iş kayıt alt tipi.</summary>
        public string jobrecordalttipi { get; }
        /// <summary>Personelin işe başlama tarihi.</summary>
        public DateTime basdate { get; }
        /// <summary>Personelin işten ayrılma tarihi (varsa).</summary>
        public DateTime? bitdate { get; }
        /// <summary>Personelin dahili telefon numarası.</summary>
        public short? dahili { get; }
        /// <summary>Personelin cep telefonu numarası.</summary>
        public string tel { get; }
        /// <summary>Personelin çalıştığı birim bilgisi (id, adı).</summary>
        public (int id, string adi) birim { get; }
        /// <summary>Personelin fiili görevde olduğu birim bilgisi (id, adı) (varsa).</summary>
        public (int? id, string adi) birim_fiiligorev { get; }
        /// <summary>Personelin doğum tarihi.</summary>
        public DateTime dogumtarihi { get; }
        /// <summary>Personelin NVI kimlik bilgileri (tip, seri numarası).</summary>
        public (NVIKimlikTypes? tip, string serino) NVI_kimlikinfo { get; }
        /// <summary>Personelin resmi (resim verisi).</summary>
        public byte[] img { get; }
        /// <summary>Personelin tam adı (ad ve soyad birleştirilir).</summary>
        public string nms => String.Concat(this.ad, " ", this.soyad);
        /// <summary>SEO uyumlu ad-soyad ifadesi.</summary>
        public string seo => this.nms.ToSeoFriendly();
        /// <summary>Personelin AKA unvanına sahip olup olmadığını belirler.</summary>
        public bool isaka => this.unvan.tip == NHR_UnvanTypes.aka;
        /// <summary>Personelin aktif durumda olup olmadığını belirler.</summary>
        public bool isaktif => !this.bitdate.HasValue;
        /// <summary>Personelin bir resmi olup olmadığını kontrol eder.</summary>
        public bool hasimg => this.img.Length > 0;
        /// <summary>
        /// Varsayılan yapıcı method
        /// </summary>
        public UyeInfoResult() : this(default) { }
        /// <summary>
        /// wspersonel nesnesine dayalı olarak UyeInfoResult nesnesi oluşturur.
        /// Personel bilgilerini alır, maksimum uzunluk sınırlarını uygular ve gerekli dönüşümleri yapar.
        /// </summary>
        /// <param name="wspersonel">Personel bilgilerini içeren nesne.</param>
        public UyeInfoResult(wspersonel wspersonel)
        {
            wspersonel = wspersonel ?? new wspersonel();
            this.ad = wspersonel.personelAd.SubstringUpToLength(_nhr.ad).ToTitleCase(true, new char[] { '.' });
            this.soyad = wspersonel.personelSoyad.SubstringUpToLength(_nhr.soyad).ToUpper();
            this.kuladi = ((_try.TryMailAddress(wspersonel.personelKurumemail, out MailAddress _ma) && _ma.IsBayburtUniEPosta()) ? _ma.User : "");
            this.tckn = _try.TryTCKimlikNo(wspersonel.personelTckimlikno.ToString(), out long _tc) ? _tc : 0;
            this.sicilno = (NHRTools.IsSicilNo(wspersonel.personelKurumsicilno) ? wspersonel.personelKurumsicilno.ToUpper() : "");
            this.gender = wspersonel.personelCinsiyet.ToSeoFriendly().ToEnumerable().Select(x => x == "erkek" ? 'E' : (x == "kadin" ? 'K' : Char.MinValue)).FirstOrDefault();
            this.unvan = (wspersonel.GetUnvanTypes(), wspersonel.personelUnvanid, new TitleCaseConfiguration().Execute(wspersonel.personelUnvan).SubstringUpToLength(_nhr.unvanadi), wspersonel.personelUnvanKisaltma.SubstringUpToLength(_nhr.unvanadi)); // unvankisa boş gelebilir!
            this.durumtip = wspersonel.GetDurumTypes();
            this.jobrecordalttipi = wspersonel.personelJobrecordalttipi.SubstringUpToLength(_nhr.jobrecordalttipi).ToUpper();
            this.basdate = wspersonel.personelUnivbastar;
            this.bitdate = (wspersonel.IsActivePersonel() ? null : wspersonel.personelAyrilmatar);
            this.dahili = (Int16.TryParse(wspersonel.personelDahili, out short _da) && _da > 0 && wspersonel.personelDahili.Length == 4) ? _da : null;
            this.tel = _try.TryPhoneNumberTR(wspersonel.personelCeptel1, out string _te) ? _te : "";
            this.birim = (wspersonel.personelBirimid.ToInt32(), wspersonel.personelBirim.SubstringUpToLength(_nhr.birimadi).ToUpper());
            this.birim_fiiligorev = ((wspersonel.fiiligorevlist ?? Array.Empty<wsfiiligorev>()).GetAktifFiiliGorev(this.birim.id)).ToEnumerable().Select(x => x.fiiligorevBirimid > 0 ? ((int?)x.fiiligorevBirimid, x.fiiligorevMasterbirim.SubstringUpToLength(_nhr.birimadi).ToUpper()) : (null, "")).FirstOrDefault();
            this.dogumtarihi = wspersonel.personelKpsdogumtar;
            this.NVI_kimlikinfo = wspersonel.GetKimlikInfo();
            this.img = wspersonel.personelResim ?? Array.Empty<byte>();
        }
        /// <summary>
        /// Nesnedeki geçersiz veya eksik bilgileri kontrol eder ve uyarıları döner.
        /// Boş bırakılan veya hatalı formatlanan alanları listeler.
        /// </summary>
        /// <param name="_warnings">Geçersiz alanlarla ilgili uyarı mesajları listesi.</param>
        /// <returns>Herhangi bir uyarı varsa <see langword="true"/>, yoksa <see langword="false"/> döner.</returns>
        public bool TryIsWarning(out string[] _warnings)
        {
            var r = new List<string>();
            if (this.ad.IsNullOrEmpty_string()) { r.Add($"NHR \"{nameof(wspersonel.personelAd)}\" property değeri boş geçilemez!"); }
            if (this.soyad.IsNullOrEmpty_string()) { r.Add($"NHR \"{nameof(wspersonel.personelSoyad)}\" property değeri boş geçilemez!"); }
            if (this.kuladi.IsNullOrEmpty_string()) { r.Add($"NHR \"{nameof(wspersonel.personelKurumemail)}\" property değeri bayburt.edu.tr uzantılı bir e-Posta olmalıdır!"); }
            if (this.kuladi.Length > _nhr.kuladi) { r.Add($"NHR \"{nameof(wspersonel.personelKurumemail)}\" kullanıcı adı property değeri maksimum {_nhr.kuladi.ToString()} karakter uzunluğa olabilir!"); }
            if (this.tckn == 0) { r.Add($"NHR \"{nameof(wspersonel.personelTckimlikno)}\" property değeri T.C. Kimlik Numarasına uygun biçiminde değildir!"); }
            if (this.sicilno == "") { r.Add($"NHR \"{nameof(wspersonel.personelKurumsicilno)}\" property değeri T.C. {_title.name_bayburtuniversitesi} Kurum Sicil Numarasına uygun biçiminde değildir!"); }
            if (!this.gender.Includes('E', 'K')) { r.Add($"NHR \"{nameof(wspersonel.personelCinsiyet)}\" property değeri uygun biçimde değildir!"); }
            if (this.unvan.id == 0) { r.Add($"NHR \"{nameof(wspersonel.personelUnvanid)}\" property değeri 0'dan büyük bir sayı olmalıdır!"); }
            if (this.unvan.adi.IsNullOrEmpty_string()) { r.Add($"NHR \"{nameof(wspersonel.personelUnvan)}\" property değeri boş geçilemez!"); }
            if (this.jobrecordalttipi.IsNullOrEmpty_string()) { r.Add($"NHR \"{nameof(wspersonel.personelJobrecordalttipi)}\" property değeri boş geçilemez!"); }
            if (this.birim.id == 0) { r.Add($"NHR \"{nameof(wspersonel.personelBirimid)}\" property değeri 0'dan büyük bir sayı olmalıdır!"); }
            if (this.birim.adi.IsNullOrEmpty_string()) { r.Add($"NHR \"{nameof(wspersonel.personelBirim)}\" property değeri boş geçilemez!"); }
            if (this.birim_fiiligorev.id.HasValue)
            {
                if (this.birim_fiiligorev.id.Value == 0) { r.Add($"NHR \"{nameof(wsfiiligorev.fiiligorevBirimid)}\" property değeri 0'dan büyük bir sayı olmalıdır!"); }
                if (this.birim_fiiligorev.adi.IsNullOrEmpty_string()) { r.Add($"NHR \"{nameof(wsfiiligorev.fiiligorevMasterbirim)}\" property değeri boş geçilemez!"); }
            }
            _warnings = r.ToArray();
            return r.Count > 0;
        }
    }
}