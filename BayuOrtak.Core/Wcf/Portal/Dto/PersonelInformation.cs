namespace BayuOrtak.Core.Wcf.Portal.Dto
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Wcf.Nvi.Enums;
    using System;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;
    using static BayuOrtak.Core.Helper.OrtakTools;
    using static BayuOrtak.Core.Wcf.Nhr.Enums.CNHR_UnvanTypes;
    public class PersonelInformation : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }
        public int uyeid { get; set; }
        public bool sa { get; set; }
        public string seo { get; set; }
        public int depid { get; set; }
        public string deptreecode { get; set; }
        public string imagetype { get; set; }
        public string imagepath { get; set; }
        public string unvanid { get; set; }
        public decimal unvansirano { get; set; }
        public DateTime basdate { get; set; }
        public DateTime? bitdate { get; set; }
        public bool durum { get; set; }
        public string ad { get; set; }
        public string soyad { get; set; }
        public string kuladi { get; set; }
        public string kuladiuzem { get; set; }
        public long tckn { get; set; }
        public string sicilno { get; set; }
        public string tel { get; set; }
        public short? dahili { get; set; }
        public string gender { get; set; }
        public bool ismadde35ilegiden { get; set; }
        public bool isidarigorevligelen { get; set; }
        public bool isyabanciuyruklutamzamanli { get; set; }
        public bool isengelli { get; set; }
        public DateTime nhr_dogumtarihi { get; set; }
        public NVIKimlikTypes? nhr_kimliktipi { get; set; }
        public string nhr_cuzdanserino { get; set; }
        public string nhr_jobrecordalttipi { get; set; }
        public short? nhr_dahili { get; set; }
        public NHR_UnvanTypes nhr_unvantip { get; set; }
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
        public string nms => String.Concat(this.ad, " ", this.soyad);
        [JsonIgnore]
        [IgnoreDataMember]
        public string nameshort => _get.GetNameShort(this.ad, this.soyad);
        [JsonIgnore]
        [IgnoreDataMember]
        public string imagefullpath
        {
            get
            {
                if (this.imagepath.IsNullOrEmpty_string()) { return ""; }
                if (this.imagepath[0] != '/') { this.imagepath = String.Concat("/", this.imagepath); }
                return $"https://apifile.bayburt.edu.tr/file/portal{this.imagepath}";
            }
        }
        public PersonelInformation() { }
    }
}