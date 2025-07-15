namespace BayuOrtak.Core.Wcf.Portal.Dto
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Wcf.Nvi.Enums;
    using System;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;
    using static BayuOrtak.Core.Helper.OrtakTools;
    using static BayuOrtak.Core.Wcf.Nhr.Enums.CNhr_UnvanTypes;
    public class PersonelInformationBasic : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }
        public int uyeid { get; set; }
        public string seo { get; set; }
        public string ad { get; set; }
        public string soyad { get; set; }
        public string kuladi { get; set; }
        public string kuladiuzem { get; set; }
        public long tckn { get; set; }
        public string sicilno { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public string nms => String.Concat(this.ad, " ", this.soyad);
        [JsonIgnore]
        [IgnoreDataMember]
        public string nameshort => _get.GetNameShort(this.ad, this.soyad);
        public PersonelInformationBasic() : this(default, "", "", "", "", "", default, "") { }
        public PersonelInformationBasic(int uyeid, string seo, string ad, string soyad, string kuladi, string kuladiuzem, long tckn, string sicilno)
        {
            this.uyeid = uyeid;
            this.seo = seo;
            this.ad = ad;
            this.soyad = soyad;
            this.kuladi = kuladi;
            this.kuladiuzem = kuladiuzem;
            this.tckn = tckn;
            this.sicilno = sicilno;
        }
    }
    public sealed class PersonelInformation : PersonelInformationBasic
    {
        public bool sa { get; set; }
        public int depid { get; set; }
        public string deptreecode { get; set; }
        public string imagetype { get; set; }
        public string imagepartialpath { get; set; }
        public string unvanid { get; set; }
        public decimal unvansirano { get; set; }
        public DateTime basdate { get; set; }
        public DateTime? bitdate { get; set; }
        public bool durum { get; set; }
        public string tel { get; set; }
        public short? dahili { get; set; }
        public string gender { get; set; }
        public bool ismadde35ilegiden { get; set; }
        public bool isidarigorevligelen { get; set; }
        public bool isyabanciuyruklutamzamanli { get; set; }
        public bool isengelli { get; set; }
        public DateTime nhr_dogumtarihi { get; set; }
        public Nvi_KimlikTypes? nhr_nvikimliktipi { get; set; }
        public string nhr_cuzdanserino { get; set; }
        public string nhr_jobrecordalttipi { get; set; }
        public short? nhr_dahili { get; set; }
        public Nhr_UnvanTypes nhr_unvantip { get; set; }
        public int nhr_unvanid { get; set; }
        public string nhr_unvanadi { get; set; }
        public string nhr_unvankisa { get; set; }
        public int nhr_birimid { get; set; }
        public string nhr_birimadi { get; set; }
        public int? nhr_fiilibirimid { get; set; }
        public string nhr_fiilibirimadi { get; set; }
        public YokInformation? yokinfo { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public Uri? imagefullpath
        {
            get
            {
                if (this.imagepartialpath.IsNullOrEmpty_string()) { return null; }
                if (this.imagepartialpath[0] != '/') { this.imagepartialpath = String.Concat("/", this.imagepartialpath); }
                return new Uri($"https://apifile.bayburt.edu.tr/file/portal{this.imagepartialpath}");
            }
        }
        [JsonIgnore]
        [IgnoreDataMember]
        public Uri? profilpage => (this.seo.IsNullOrEmpty_string() ? null : new Uri($"https://bayburt.edu.tr/personel/{this.seo}"));
        public PersonelInformation() : this(default, "", "", "", "", "", default, "", default, default, "", "", "", "", default, default, default, default, "", default, "", default, default, default, default, default, default, "", "", default, default, default, "", "", default, "", default, "", default) { }
        public PersonelInformation(int uyeid, string seo, string ad, string soyad, string kuladi, string kuladiuzem, long tckn, string sicilno, bool sa, int depid, string deptreecode, string imagetype, string imagepartialpath, string unvanid, decimal unvansirano, DateTime basdate, DateTime? bitdate, bool durum, string tel, short? dahili, string gender, bool ismadde35ilegiden, bool isidarigorevligelen, bool isyabanciuyruklutamzamanli, bool isengelli, DateTime nhr_dogumtarihi, Nvi_KimlikTypes? nhr_nvikimliktipi, string nhr_cuzdanserino, string nhr_jobrecordalttipi, short? nhr_dahili, Nhr_UnvanTypes nhr_unvantip, int nhr_unvanid, string nhr_unvanadi, string nhr_unvankisa, int nhr_birimid, string nhr_birimadi, int? nhr_fiilibirimid, string nhr_fiilibirimadi, YokInformation? yokinfo) : base(uyeid, seo, ad, soyad, kuladi, kuladiuzem, tckn, sicilno)
        {
            this.sa = sa;
            this.depid = depid;
            this.deptreecode = deptreecode;
            this.imagetype = imagetype;
            this.imagepartialpath = imagepartialpath;
            this.unvanid = unvanid;
            this.unvansirano = unvansirano;
            this.basdate = basdate;
            this.bitdate = bitdate;
            this.durum = durum;
            this.tel = tel;
            this.dahili = dahili;
            this.gender = gender;
            this.ismadde35ilegiden = ismadde35ilegiden;
            this.isidarigorevligelen = isidarigorevligelen;
            this.isyabanciuyruklutamzamanli = isyabanciuyruklutamzamanli;
            this.isengelli = isengelli;
            this.nhr_dogumtarihi = nhr_dogumtarihi;
            this.nhr_nvikimliktipi = nhr_nvikimliktipi;
            this.nhr_cuzdanserino = nhr_cuzdanserino;
            this.nhr_jobrecordalttipi = nhr_jobrecordalttipi;
            this.nhr_dahili = nhr_dahili;
            this.nhr_unvantip = nhr_unvantip;
            this.nhr_unvanid = nhr_unvanid;
            this.nhr_unvanadi = nhr_unvanadi;
            this.nhr_unvankisa = nhr_unvankisa;
            this.nhr_birimid = nhr_birimid;
            this.nhr_birimadi = nhr_birimadi;
            this.nhr_fiilibirimid = nhr_fiilibirimid;
            this.nhr_fiilibirimadi = nhr_fiilibirimadi;
            this.yokinfo = yokinfo;
        }
    }
    public class YokInformation : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }
        public bool durumkadro { get; set; }
        public bool durumbaglanti { get; set; }
        public int? fakid { get; set; }
        public string fakadi { get; set; }
        public int? bolid { get; set; }
        public string boladi { get; set; }
        public int? abdid { get; set; }
        public string abdadi { get; set; }
        public string akademiksrc { get; set; }
        public string orcidsrc { get; set; }
        public int? arastirmaciid { get; set; }
        public string uaktemel { get; set; }
        public string uakbilim { get; set; }
        public string uakkel1 { get; set; }
        public string uakkel2 { get; set; }
        public string uakkel3 { get; set; }
        public YokInformation() : this(default, default, default, "", default, "", default, "", "", "", default, "", "", "", "", "") { }
        public YokInformation(bool durumkadro, bool durumbaglanti, int? fakid, string fakadi, int? bolid, string boladi, int? abdid, string abdadi, string akademiksrc, string orcidsrc, int? arastirmaciid, string uaktemel, string uakbilim, string uakkel1, string uakkel2, string uakkel3)
        {
            this.durumkadro = durumkadro;
            this.durumbaglanti = durumbaglanti;
            this.fakid = fakid;
            this.fakadi = fakadi;
            this.bolid = bolid;
            this.boladi = boladi;
            this.abdid = abdid;
            this.abdadi = abdadi;
            this.akademiksrc = akademiksrc;
            this.orcidsrc = orcidsrc;
            this.arastirmaciid = arastirmaciid;
            this.uaktemel = uaktemel;
            this.uakbilim = uakbilim;
            this.uakkel1 = uakkel1;
            this.uakkel2 = uakkel2;
            this.uakkel3 = uakkel3;
        }
        internal static YokInformation ToEntityFromDynamic(dynamic item) => new YokInformation((bool)item.durumkadro, (bool)item.durumbaglanti, (int?)item.fakid, (string)item.fakadi, (int?)item.bolid, (string)item.boladi, (int?)item.abdid, (string)item.abdadi, (string)item.akademiksrc, (string)item.orcidsrc, (int?)item.arastirmaciid, (string)item.uaktemel, (string)item.uakbilim, (string)item.uakkel1, (string)item.uakkel2, (string)item.uakkel3);
    }
}