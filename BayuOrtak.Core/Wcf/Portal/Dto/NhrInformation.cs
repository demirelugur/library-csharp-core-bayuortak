namespace BayuOrtak.Core.Wcf.Portal.Dto
{
    using BayuOrtak.Core.Wcf.Nvi.Enums;
    using System;
    public class NhrInformation : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }
        public int uyeid { get; set; }
        public string sicilno { get; set; }
        public DateTime dogumtarihi { get; set; }
        public NVIKimlikTypes? kimliktipi { get; set; }
        public string cuzdanserino { get; set; }
        public string gender { get; set; }
        public string jobrecordalttipi { get; set; }
        public short? dahili { get; set; }
        public bool isengelli { get; set; }
        public bool ismadde35ilegiden { get; set; }
        public bool isidarigorevligelen { get; set; }
        public bool isyabanciuyruklutamzamanli { get; set; }
        public int unvanid { get; set; }
        public string unvanadi { get; set; }
        public string unvankisa { get; set; }
        public int birimid { get; set; }
        public string birimadi { get; set; }
        public int? fiilibirimid { get; set; }
        public string fiilibirimadi { get; set; }
    }
}