namespace BayuOrtak.Core.Wcf.Portal.Dto
{
    using BayuOrtak.Core.Extensions;
    using System;
    using static BayuOrtak.Core.Enums.CKampusTypes;
    public class DepInformation : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }
        public int id { get; set; }
        public int? ustdepid { get; set; }
        public string seoid { get; set; }
        public int tip_id { get; set; }
        public string tip { get; set; }
        public KampusTypes kampus_id { get; set; }
        public string code { get; set; }
        public DateTime? bitdate { get; set; }
        public bool isdbk { get; set; }
        public bool isuygarsmrkz { get; set; }
        public DepNameInformation[] names { get; set; }
        public int codelength => this.code.ToStringOrEmpty().Length;
        public DepInformation() { }
    }
}