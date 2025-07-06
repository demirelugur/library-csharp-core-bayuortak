namespace BayuOrtak.Core.Wcf.Portal.Dto
{
    using BayuOrtak.Core.Extensions;
    using System;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;
    using static BayuOrtak.Core.Helper.OrtakTools;
    using static BayuOrtak.Core.Wcf.Nhr.Enums.CNHR_UnvanTypes;
    public class PersonelInformation : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }
        public int id { get; set; }
        public bool sa { get; set; }
        public string seo { get; set; }
        public int depid { get; set; }
        public string depcode { get; set; }
        public string img { get; set; }
        public NHR_UnvanTypes unvantip { get; set; }
        public string unvanid { get; set; }
        public int unvansirano { get; set; }
        public DateTime basdate { get; set; }
        public DateTime? bitdate { get; set; }
        public string ad { get; set; }
        public string soyad { get; set; }
        public string kuladi { get; set; }
        public long tckn { get; set; }
        public string tel { get; set; }
        public short? dahili { get; set; }
        public NhrInformation nhrinfo { get; set; }
        public YokInformation yokinfo { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public bool isaktif => !this.bitdate.HasValue;
        [JsonIgnore]
        [IgnoreDataMember]
        public string nms => String.Concat(this.ad, " ", this.soyad);
        [JsonIgnore]
        [IgnoreDataMember]
        public string nameshort => _get.GetNameShort(this.ad, this.soyad);
        [JsonIgnore]
        [IgnoreDataMember]
        public string imgfullpath => (this.img.IsNullOrEmpty_string() ? "" : $"https://bayburt.edu.tr/uploads/.users/{(this.img[0] == '/' ? this.img.Substring(1) : this.img)}");
        public PersonelInformation() { }
    }
}