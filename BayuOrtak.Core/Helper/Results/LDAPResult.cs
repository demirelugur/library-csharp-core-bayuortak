namespace BayuOrtak.Core.Helper.Results
{
    using BayuOrtak.Core.Enums;
    using System;
    using System.Collections.Generic;
    public class LDAPResult
    {
        public ADUserAccountControl useraccountcontrol { get; set; }
        public string sicilno { get; set; }
        public string ad { get; set; }
        public string soyad { get; set; }
        public long tckn { get; set; }
        public Dictionary<string, object> attributes { get; set; }
        public string nms => String.Concat(this.ad, " ", this.soyad);
        public LDAPResult() : this(default, "", "", "", default, default) { }
        public LDAPResult(ADUserAccountControl useraccountcontrol, string sicilno, string ad, string soyad, long tckn, Dictionary<string, object> attributes)
        {
            this.useraccountcontrol = useraccountcontrol;
            this.sicilno = sicilno;
            this.ad = ad;
            this.soyad = soyad;
            this.tckn = tckn;
            this.attributes = attributes ?? new Dictionary<string, object>();
        }
        internal void tryiswarning(string dil, bool istcknrequired, out Exception outvalueex) => outvalueex = ((istcknrequired && this.tckn == 0) ? new Exception(String.Concat((dil == "en" ? "No record found on Republic of Türkiye ID number" : "T.C. Kimlik numarası üzerine kayıt bulunamadı"), ", Attributes: tckno, description")) : null);
    }
}