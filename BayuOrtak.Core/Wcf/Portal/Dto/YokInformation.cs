namespace BayuOrtak.Core.Wcf.Portal.Dto
{
    using System;
    public class YokInformation : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }
        public int uyeid { get; set; }
        public bool durumkadro { get; set; }
        public bool durumconnection { get; set; }
        public int? fakid { get; set; }
        public int? bolid { get; set; }
        public int? abdid { get; set; }
        public string authorid { get; set; }
        public string orcid { get; set; }
        public int? arastirmaciid { get; set; }
        public string uaktemel { get; set; }
        public string uakbilim { get; set; }
        public string uakkel1 { get; set; }
        public string uakkel2 { get; set; }
        public string uakkel3 { get; set; }
        public short? atifyil { get; set; }
    }
}