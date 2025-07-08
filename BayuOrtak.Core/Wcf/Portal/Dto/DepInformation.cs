namespace BayuOrtak.Core.Wcf.Portal.Dto
{
    using BayuOrtak.Core.Extensions;
    using System;
    public class DepInformation : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }
        public int depid { get; set; }
        public int? maindepid { get; set; }
        public string seo { get; set; }
        public string domain { get; set; }
        public int tip_id { get; set; }
        public string tip { get; set; }
        public int kampus_id { get; set; }
        public string kampus_adi { get; set; }
        public string treecode { get; set; }
        public bool durum { get; set; }
        public bool isdbk { get; set; }
        public bool isuygarsmrkz { get; set; }
        public DepNameInformation[] names { get; set; }
        public int treecodelength => this.treecode.ToStringOrEmpty().Length;
        public DepInformation() { }
    }
}