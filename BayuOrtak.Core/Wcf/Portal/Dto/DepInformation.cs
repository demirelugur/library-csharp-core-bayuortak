namespace BayuOrtak.Core.Wcf.Portal.Dto
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Wcf.Portal.Enums;
    using System;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public sealed class DepInformation : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }
        public int depid { get; set; }
        public int? maindepid { get; set; }
        public string seo { get; set; }
        public string domain { get; set; }
        public int tipid { get; set; }
        public string tip { get; set; }
        public int kampusid { get; set; }
        public string treecode { get; set; }
        public bool durum { get; set; }
        public bool isdbk { get; set; }
        public bool isuygarsmrkz { get; set; }
        public Dictionary<Portal_DilTypes, (string adi, string adiuzun)> depnames { get; set; }
        public Dictionary<Portal_DilTypes, string> kampusnames { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public int treecodelength => this.treecode.ToStringOrEmpty().Length;
        [JsonIgnore]
        [IgnoreDataMember]
        public Uri? profilpage
        {
            get
            {
                if (_try.TryUri(this.domain, out Uri _u)) { return _u; }
                if (!this.seo.IsNullOrEmpty_string()) { return new Uri($"https://bayburt.edu.tr/birim/{this.seo}"); }
                return null;
            }
        }
        public DepInformation() : this(default, default, "", "", default, "", default, "", default, default, default, default, default) { }
        public DepInformation(int depid, int? maindepid, string seo, string domain, int tipid, string tip, int kampusid, string treecode, bool durum, bool isdbk, bool isuygarsmrkz, Dictionary<Portal_DilTypes, (string, string)> depnames, Dictionary<Portal_DilTypes, string> kampusnames)
        {
            this.depid = depid;
            this.maindepid = maindepid;
            this.seo = seo;
            this.domain = domain;
            this.tipid = tipid;
            this.tip = tip;
            this.kampusid = kampusid;
            this.treecode = treecode;
            this.durum = durum;
            this.isdbk = isdbk;
            this.isuygarsmrkz = isuygarsmrkz;
            this.depnames = depnames ?? new Dictionary<Portal_DilTypes, (string, string)>();
            this.kampusnames = kampusnames ?? new Dictionary<Portal_DilTypes, string>();
        }
    }
}