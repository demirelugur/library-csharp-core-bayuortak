namespace BayuOrtak.Core.Helper.Results
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Wcf.Nhr.Helper;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Net.Mail;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public sealed class LogInfoResult : IEquatable<LogInfoResult>
    {
        #region Equals
        public override bool Equals(object other) => this.Equals(other as LogInfoResult);
        public override int GetHashCode() => HashCode.Combine(HashCode.Combine(this.portalid, this.sicilno, this.ogrencino, this.eposta, this.ad, this.soyad, this.sa, this.hash), this.sessionid);
        public bool Equals(LogInfoResult other) => (other != null && this.portalid == other.portalid && this.sicilno == other.sicilno && this.ogrencino == other.ogrencino && this.eposta == other.eposta && this.ad == other.ad && this.soyad == other.soyad && this.sa == other.sa && this.hash == other.hash && this.sessionid == other.sessionid);
        #endregion
        public int portalid { get; set; }
        public string sicilno { get; set; }
        public string ogrencino { get; set; }
        public string eposta { get; set; }
        public string kuladi { get; set; }
        public string ad { get; set; }
        public string soyad { get; set; }
        public bool sa { get; set; }
        public string hash { get; set; }
        public Guid sessionid { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public string nms => String.Concat(this.ad, " ", this.soyad).Trim();
        [JsonIgnore]
        [IgnoreDataMember]
        public string nameshort => _get.GetNameShort(this.ad, this.soyad);
        public LogInfoResult() : this(default, "", "", default, "", "", "", default, "", default) { }
        public LogInfoResult(int portalid, string sicilno, string ogrencino, object eposta, string kuladi, string ad, string soyad, bool sa, string hash, Guid sessionid)
        {
            hash = hash.ToStringOrEmpty();
            this.portalid = (portalid > 0 ? portalid : 0);
            this.sicilno = (NHRTools.TrySicilNo(sicilno, out string _s) ? _s : "");
            this.ogrencino = ogrencino.ToStringOrEmpty().ToUpper();
            this.eposta = this.setemail(eposta);
            this.kuladi = kuladi.ToStringOrEmpty().ToLower();
            this.ad = ad.ToTitleCase(true, new char[] { '.' });
            this.soyad = soyad.ToStringOrEmpty().ToUpper();
            this.sa = sa;
            this.hash = hash.Length.Includes(0, _maximumlength.hash) ? hash : "";
            this.sessionid = sessionid;
        }
        private string setemail(object eposta)
        {
            if (eposta is MailAddress _m) { return _m.Address; }
            if (eposta is String _s && _try.TryMailAddress(_s, out _m)) { return _m.Address; }
            return "";
        }
        /// <summary>
        /// value için tanımlanan nesneler: LogInfoResult, IFormCollection, AnonymousObjectClass
        /// </summary>
        public static LogInfoResult ToEntityFromObject(object value)
        {
            if (value == null) { return new LogInfoResult(); }
            if (value is LogInfoResult _t) { return _t; }
            if (value is IFormCollection _form)
            {
                return ToEntityFromObject(new
                {
                    portalid = _form.ToKeyValueParseOrDefault_formcollection<int>(nameof(portalid)),
                    sicilno = _form.ToKeyValueParseOrDefault_formcollection<string>(nameof(sicilno)) ?? "",
                    ogrencino = _form.ToKeyValueParseOrDefault_formcollection<string>(nameof(ogrencino)) ?? "",
                    eposta = _form.ToKeyValueParseOrDefault_formcollection<string>(nameof(eposta)) ?? "",
                    kuladi = _form.ToKeyValueParseOrDefault_formcollection<string>(nameof(kuladi)) ?? "",
                    ad = _form.ToKeyValueParseOrDefault_formcollection<string>(nameof(ad)) ?? "",
                    soyad = _form.ToKeyValueParseOrDefault_formcollection<string>(nameof(soyad)) ?? "",
                    sa = _form.ToKeyValueParseOrDefault_formcollection<bool>(nameof(sa)),
                    hash = _form.ToKeyValueParseOrDefault_formcollection<string>(nameof(hash)) ?? "",
                    sessionid = _form.ToKeyValueParseOrDefault_formcollection<Guid>(nameof(sessionid))
                });
            }
            return value.ToEnumerable().Select(x => x.ToDynamic()).Select(x => new LogInfoResult((int)x.portalid, (string)x.sicilno, (string)x.ogrencino, (object)x.eposta, (string)x.kuladi, (string)x.ad, (string)x.soyad, (bool)x.sa, (string)x.hash, (Guid)x.sessionid)).FirstOrDefault();
        }
    }
}