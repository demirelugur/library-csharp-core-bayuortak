namespace BayuOrtak.Core.Wcf.Portal.Dto
{
    using System;
    public sealed class UnvanInformation : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }
        public string unvanid { get; set; }
        public int tip_id { get; set; }
        public string tip { get; set; }
        public decimal sirano { get; set; }
        public UnvanNameInformation[] names { get; set; }
        public UnvanInformation() { }
    }
}