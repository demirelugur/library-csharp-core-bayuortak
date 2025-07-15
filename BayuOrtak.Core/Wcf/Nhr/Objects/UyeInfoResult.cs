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
    using static BayuOrtak.Core.Wcf.Nhr.Enums.CNhr_DurumTypes;
    using static BayuOrtak.Core.Wcf.Nhr.Enums.CNhr_UnvanTypes;
    public sealed class UyeInfoResult
    {
        public string ad { get; }
        public string soyad { get; }
        public string kuladi { get; }
        public long tckn { get; }
        public string sicilno { get; }
        public char gender { get; }
        public (Nhr_UnvanTypes tip, int id, string adi, string adi_kisa) unvan { get; }
        public Nhr_DurumTypes? durumtip { get; }
        public string jobrecordalttipi { get; }
        public DateTime basdate { get; }
        public DateTime? bitdate { get; }
        public short? dahili { get; }
        public string tel { get; }
        public (int id, string adi) birim { get; }
        public (int? id, string adi) birim_fiiligorev { get; }
        public DateTime dogumtarihi { get; }
        public (Nvi_KimlikTypes? tip, string serino) nvi_kimlikinfo { get; }
        public byte[] img { get; }
        public string nms => String.Concat(this.ad, " ", this.soyad);
        public string seo => this.nms.ToSeoFriendly();
        public bool isaka => this.unvan.tip == Nhr_UnvanTypes.aka;
        public bool isaktif => !this.bitdate.HasValue;
        public bool hasimg => this.img.Length > 0;
        public UyeInfoResult() : this(default) { }
        public UyeInfoResult(wspersonel wspersonel)
        {
            wspersonel = wspersonel ?? new wspersonel();
            this.ad = wspersonel.personelAd.SubstringUpToLength(_nhr.ad).ToTitleCase(true, new char[] { '.' });
            this.soyad = wspersonel.personelSoyad.SubstringUpToLength(_nhr.soyad).ToUpper();
            this.kuladi = ((_try.TryMailAddress(wspersonel.personelKurumemail, out MailAddress _ma) && _ma.IsBayburtUniEPosta()) ? _ma.User : "");
            this.tckn = _try.TryTCKimlikNo(wspersonel.personelTckimlikno.ToString(), out long _tckn) ? _tckn : 0;
            this.sicilno = NHRTools.TrySicilNo(wspersonel.personelKurumsicilno, out string _sicilno) ? _sicilno : "";
            this.gender = wspersonel.personelCinsiyet.ToSeoFriendly().ToEnumerable().Select(x => x == "erkek" ? 'E' : (x == "kadin" ? 'K' : Char.MinValue)).FirstOrDefault();
            this.unvan = (wspersonel.GetUnvanTypes(), wspersonel.personelUnvanid, new TitleCaseConfiguration().Execute(wspersonel.personelUnvan).SubstringUpToLength(_nhr.unvanadi), wspersonel.personelUnvanKisaltma.SubstringUpToLength(_nhr.unvanadi)); // unvankisa boş gelebilir!
            this.durumtip = wspersonel.GetDurumTypes();
            this.jobrecordalttipi = wspersonel.personelJobrecordalttipi.SubstringUpToLength(_nhr.jobrecordalttipi).ToUpper();
            this.basdate = wspersonel.personelUnivbastar;
            this.bitdate = (wspersonel.IsActivePersonel() ? null : wspersonel.personelAyrilmatar);
            this.dahili = (Int16.TryParse(wspersonel.personelDahili, out short _dahili) && _dahili > 0 && wspersonel.personelDahili.Length == 4) ? _dahili : null;
            this.tel = _try.TryPhoneNumberTR(wspersonel.personelCeptel1, out string _tel) ? _tel : "";
            this.birim = (wspersonel.personelBirimid.ToInt32(), wspersonel.personelBirim.SubstringUpToLength(_nhr.birimadi).ToUpper());
            this.birim_fiiligorev = ((wspersonel.fiiligorevlist ?? Array.Empty<wsfiiligorev>()).GetAktifFiiliGorev(this.birim.id)).ToEnumerable().Select(x => x.fiiligorevBirimid > 0 ? ((int?)x.fiiligorevBirimid, x.fiiligorevMasterbirim.SubstringUpToLength(_nhr.birimadi).ToUpper()) : (null, "")).FirstOrDefault();
            this.dogumtarihi = wspersonel.personelKpsdogumtar;
            this.nvi_kimlikinfo = wspersonel.GetKimlikInfo();
            this.img = wspersonel.personelResim ?? Array.Empty<byte>();
        }
        /// <summary>
        /// Web servisinden gelen personel verilerinin doğruluğunu kontrol eder ve hata mesajlarını belirtilen dilde döndürür.
        /// </summary>
        /// <param name="dil">Hata mesajlarının döndürüleceği dil. Desteklenen değerler: &quot;tr&quot; (Türkçe) veya &quot;en&quot; (İngilizce).</param>
        /// <param name="_warnings">Doğrulama sırasında tespit edilen hata mesajlarının dizisi.</param>
        /// <returns>Doğrulama sırasında hata bulunursa <see langword="true"/>, aksi takdirde <see langword="false"/> döner.</returns>
        public bool TryIsWarning(string dil, out string[] _warnings)
        {
            Guard.UnSupportLanguage(dil, nameof(dil));
            var _messages = new Dictionary<string, Dictionary<string, string>>
            {
                ["tr"] = new Dictionary<string, string>
                {
                    ["ad_empty"] = $"NHR \"{nameof(wspersonel.personelAd)}\" property değeri boş geçilemez!",
                    ["soyad_empty"] = $"NHR \"{nameof(wspersonel.personelSoyad)}\" property değeri boş geçilemez!",
                    ["kuladi_empty"] = $"NHR \"{nameof(wspersonel.personelKurumemail)}\" property değeri bayburt.edu.tr uzantılı bir e-Posta olmalıdır!",
                    ["kuladi_length"] = $"NHR \"{nameof(wspersonel.personelKurumemail)}\" kullanıcı adı property değeri maksimum {_nhr.kuladi.ToString()} karakter uzunluğa olabilir!",
                    ["tckn_invalid"] = $"NHR \"{nameof(wspersonel.personelTckimlikno)}\" property değeri T.C. Kimlik Numarasına uygun biçiminde değildir!",
                    ["sicilno_invalid"] = $"NHR \"{nameof(wspersonel.personelKurumsicilno)}\" property değeri T.C. {_title.name_bayburtuniversitesi} Kurum Sicil Numarasına uygun biçiminde değildir!",
                    ["gender_invalid"] = $"NHR \"{nameof(wspersonel.personelCinsiyet)}\" property değeri uygun biçimde değildir!",
                    ["unvan_id_invalid"] = $"NHR \"{nameof(wspersonel.personelUnvanid)}\" property değeri 0'dan büyük bir sayı olmalıdır!",
                    ["unvan_empty"] = $"NHR \"{nameof(wspersonel.personelUnvan)}\" property değeri boş geçilemez!",
                    ["jobrecord_empty"] = $"NHR \"{nameof(wspersonel.personelJobrecordalttipi)}\" property değeri boş geçilemez!",
                    ["birim_id_invalid"] = $"NHR \"{nameof(wspersonel.personelBirimid)}\" property değeri 0'dan büyük bir sayı olmalıdır!",
                    ["birim_empty"] = $"NHR \"{nameof(wspersonel.personelBirim)}\" property değeri boş geçilemez!",
                    ["fiiligorev_id_invalid"] = $"NHR \"{nameof(wsfiiligorev.fiiligorevBirimid)}\" property değeri 0'dan büyük bir sayı olmalıdır!",
                    ["fiiligorev_empty"] = $"NHR \"{nameof(wsfiiligorev.fiiligorevMasterbirim)}\" property değeri boş geçilemez!"
                },
                ["en"] = new Dictionary<string, string>
                {
                    ["ad_empty"] = $"NHR \"{nameof(wspersonel.personelAd)}\" property cannot be empty!",
                    ["soyad_empty"] = $"NHR \"{nameof(wspersonel.personelSoyad)}\" property cannot be empty!",
                    ["kuladi_empty"] = $"NHR \"{nameof(wspersonel.personelKurumemail)}\" property must be an email with bayburt.edu.tr domain!",
                    ["kuladi_length"] = $"NHR \"{nameof(wspersonel.personelKurumemail)}\" username property can be maximum {_nhr.kuladi.ToString()} characters long!",
                    ["tckn_invalid"] = $"NHR \"{nameof(wspersonel.personelTckimlikno)}\" property is not in a valid T.C. Identity Number format!",
                    ["sicilno_invalid"] = $"NHR \"{nameof(wspersonel.personelKurumsicilno)}\" property is not in a valid Bayburt University Institutional Registration Number format!",
                    ["gender_invalid"] = $"NHR \"{nameof(wspersonel.personelCinsiyet)}\" property is not in a valid format!",
                    ["unvan_id_invalid"] = $"NHR \"{nameof(wspersonel.personelUnvanid)}\" property must be a number greater than 0!",
                    ["unvan_empty"] = $"NHR \"{nameof(wspersonel.personelUnvan)}\" property cannot be empty!",
                    ["jobrecord_empty"] = $"NHR \"{nameof(wspersonel.personelJobrecordalttipi)}\" property cannot be empty!",
                    ["birim_id_invalid"] = $"NHR \"{nameof(wspersonel.personelBirimid)}\" property must be a number greater than 0!",
                    ["birim_empty"] = $"NHR \"{nameof(wspersonel.personelBirim)}\" property cannot be empty!",
                    ["fiiligorev_id_invalid"] = $"NHR \"{nameof(wsfiiligorev.fiiligorevBirimid)}\" property must be a number greater than 0!",
                    ["fiiligorev_empty"] = $"NHR \"{nameof(wsfiiligorev.fiiligorevMasterbirim)}\" property cannot be empty!"
                }
            };
            var _r = new List<string>();
            if (this.ad.IsNullOrEmpty_string()) { _r.Add(_messages[dil]["ad_empty"]); }
            if (this.soyad.IsNullOrEmpty_string()) { _r.Add(_messages[dil]["soyad_empty"]); }
            if (this.kuladi.IsNullOrEmpty_string()) { _r.Add(_messages[dil]["kuladi_empty"]); }
            if (this.kuladi.Length > _nhr.kuladi) { _r.Add(_messages[dil]["kuladi_length"]); }
            if (this.tckn == 0) { _r.Add(_messages[dil]["tckn_invalid"]); }
            if (this.sicilno == "") { _r.Add(_messages[dil]["sicilno_invalid"]); }
            if (!this.gender.Includes('E', 'K')) { _r.Add(_messages[dil]["gender_invalid"]); }
            if (this.unvan.id == 0) { _r.Add(_messages[dil]["unvan_id_invalid"]); }
            if (this.unvan.adi.IsNullOrEmpty_string()) { _r.Add(_messages[dil]["unvan_empty"]); }
            if (this.jobrecordalttipi.IsNullOrEmpty_string()) { _r.Add(_messages[dil]["jobrecord_empty"]); }
            if (this.birim.id == 0) { _r.Add(_messages[dil]["birim_id_invalid"]); }
            if (this.birim.adi.IsNullOrEmpty_string()) { _r.Add(_messages[dil]["birim_empty"]); }
            if (this.birim_fiiligorev.id.HasValue)
            {
                if (this.birim_fiiligorev.id.Value == 0) { _r.Add(_messages[dil]["fiiligorev_id_invalid"]); }
                if (this.birim_fiiligorev.adi.IsNullOrEmpty_string()) { _r.Add(_messages[dil]["fiiligorev_empty"]); }
            }
            _warnings = _r.ToArray();
            return _r.Count > 0;
        }
    }
}