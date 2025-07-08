namespace BayuOrtak.Core.Wcf.Portal.Dto
{
    using System;
    public class DepNameInformation : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }
        public int depid { get; set; }
        public byte dil { get; set; }
        public string adi { get; set; }
        public string adiuzun { get; set; }
        public string dilcode => (this.dil == 1 ? "tr" : (this.dil == 2 ? "en" : ""));
        public DepNameInformation() { }
    }
}