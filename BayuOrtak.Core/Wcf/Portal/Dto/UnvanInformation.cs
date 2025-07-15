namespace BayuOrtak.Core.Wcf.Portal.Dto
{
    using BayuOrtak.Core.Wcf.Portal.Enums;
    using System;
    public sealed class UnvanInformation : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }
        public string unvanid { get; set; }
        public int tipid { get; set; }
        public string tip { get; set; }
        public decimal sirano { get; set; }
        public Dictionary<Portal_DilTypes, (string adi, string adikisa)> unvannames { get; set; }
        public UnvanInformation() : this("", default, "", default, default) { }
        public UnvanInformation(string unvanid, int tipid, string tip, decimal sirano, Dictionary<Portal_DilTypes, (string, string)> unvannames)
        {
            this.unvanid = unvanid;
            this.tipid = tipid;
            this.tip = tip;
            this.sirano = sirano;
            this.unvannames = unvannames ?? new Dictionary<Portal_DilTypes, (string, string)>();
        }
    }
}