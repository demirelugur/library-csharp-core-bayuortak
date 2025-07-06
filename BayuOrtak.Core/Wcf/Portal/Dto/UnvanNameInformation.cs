namespace BayuOrtak.Core.Wcf.Portal.Dto
{
    using System;
    public class UnvanNameInformation : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }
        public string unvanid { get; set; }
        public Guid uid { get; set; }
        public int dil { get; set; }
        public string adi { get; set; }
        public string adikisa { get; set; }
        public string dilcode => (this.dil == 1 ? "tr" : (this.dil == 2 ? "en" : ""));
        public UnvanNameInformation() { }
    }
}