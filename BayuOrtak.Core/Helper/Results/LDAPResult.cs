namespace BayuOrtak.Core.Helper.Results
{
    using BayuOrtak.Core.Enums;
    using BayuOrtak.Core.Extensions;
    using System;
    using System.Collections.Generic;
    public class LDAPResult : IEquatable<LDAPResult>
    {
        #region Equals
        public override bool Equals(object other) => this.Equals(other as LDAPResult);
        public override int GetHashCode() => HashCode.Combine(this.useraccountcontrol, this.sicilno, this.ad, this.soyad, this.tckn, this.attributes);
        public bool Equals(LDAPResult other) => (other != null && this.useraccountcontrol == other.useraccountcontrol && this.sicilno == other.sicilno && this.ad == other.ad && this.soyad == other.soyad && this.tckn == other.tckn && this.attributes.IsEqual(other.attributes));
        #endregion
        public ADUserAccountControl useraccountcontrol { get; set; }
        public string sicilno { get; set; }
        public string ad { get; set; }
        public string soyad { get; set; }
        public long tckn { get; set; }
        public Dictionary<string, object> attributes { get; set; }
        public string nms => String.Concat(this.ad, " ", this.soyad).Trim();
        public bool isaccountdisabled => this.useraccountcontrol.HasFlag(ADUserAccountControl.ACCOUNTDISABLE);
        public DateOnly? accountexpiresUTC
        {
            get
            {
                var _d = ((this.attributes.TryGetValue("accountexpires", out object _v) && Int64.TryParse(_v.ToStringOrEmpty(), out long _f)) ? _f.ToFileTimeUTC() : null);
                return (_d.HasValue ? _d.Value.Date.ToDateOnly() : null);
            }
        }
        public bool isaccountexpiresactive()
        {
            var _ae = this.accountexpiresUTC;
            return (!_ae.HasValue || _ae.Value >= DateTime.UtcNow.ToDateOnly());
        }
        public bool isaccountstatus => (!this.isaccountdisabled && this.isaccountexpiresactive());
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
        internal bool trytckncheckwarning(LDAPTip ldaptip, string dil, out Exception outvalue)
        {
            if (LDAPHelper.IsTCKimlikNo(ldaptip, this.tckn))
            {
                outvalue = null;
                return false;
            }
            else
            {
                outvalue = new Exception(String.Concat((dil == "en" ? "No record found on Republic of Türkiye ID number" : "T.C. Kimlik numarası üzerine kayıt bulunamadı"), ", TCKNO, DESCRIPTION"));
                return true;
            }
        }
    }
}