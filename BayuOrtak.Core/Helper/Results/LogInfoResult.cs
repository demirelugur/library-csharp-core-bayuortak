namespace BayuOrtak.Core.Helper.Results
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
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
        public bool Equals(LogInfoResult other)
        {
            if (other is LogInfoResult _lir) { return this.portalid == _lir.portalid && this.sicilno == _lir.sicilno && this.ogrencino == _lir.ogrencino && this.eposta == _lir.eposta && this.ad == _lir.ad && this.soyad == _lir.soyad && this.sa == _lir.sa && this.hash == _lir.hash && this.sessionid == _lir.sessionid; }
            return false;
        }
        #endregion
        /// <summary>
        /// Portal kimliği.
        /// </summary>
        public int portalid { get; set; }
        /// <summary>
        /// Sicil numarası.
        /// </summary>
        public string sicilno { get; set; }
        /// <summary>
        /// Öğrenci numarası.
        /// </summary>
        public string ogrencino { get; set; }
        /// <summary>
        /// Kullanıcı e-Posta adresi, eğer tanımlıysa, &quot;example@bayburt.edu.tr&quot; biçiminde bir değer alır.
        /// </summary>
        public string eposta { get; set; }
        /// <summary>
        /// Kullanıcı adı
        /// </summary>
        public string ad { get; set; }
        /// <summary>
        /// Kullanıcı soyadı
        /// </summary>
        public string soyad { get; set; }
        /// <summary>
        /// Kullanıcının ana yetkili olup olmadığını belirten property.
        /// </summary>
        public bool sa { get; set; }
        /// <summary>
        /// Kullanıcının hash bilgisi
        /// </summary>
        public string hash { get; set; }
        /// <summary>
        /// Oturum kimliği.
        /// </summary>
        public Guid sessionid { get; set; }
        /// <summary>
        /// Kullanıcın ad ve soyad bilgisi
        /// </summary>
        [JsonIgnore]
        [IgnoreDataMember]
        public string nms => String.Concat(this.ad, " ", this.soyad).Trim();
        /// <summary>
        /// Kullanıcı ad ve soyad bilgileriyle oluşan kısaltma
        /// </summary>
        [JsonIgnore]
        [IgnoreDataMember]
        public string nameshort => _get.GetNameShort(this.ad, this.soyad);
        /// <summary>
        /// e-Posta adresinin kullanıcı adı
        /// </summary>
        [JsonIgnore]
        [IgnoreDataMember]
        public string kuladi => (_try.TryMailAddress(this.eposta, out MailAddress _ma) ? _ma.User : "");
        /// <summary>
        /// Varsayılan yapıcı metod.
        /// </summary>
        public LogInfoResult() : this(default, "", "", default, "", "", default, "", default) { }
        /// <summary>
        /// LogInfoResult nesnesini başlatan bir yapıcı metod.
        /// </summary>
        /// <param name="portalid">Portal kimliği.</param>
        /// <param name="sicilno">Sicil numarası.</param>
        /// <param name="ogrencino">Öğrenci numarası.</param>
        /// <param name="eposta">e-Posta adresi nesnesi. String veya MailAddress türünde olmalıdır!</param>
        /// <param name="ad">Adı</param>
        /// <param name="soyad">Soyadı</param>
        /// <param name="sa">Ana Yetkili durumu</param>
        /// <param name="hash">Hash bilgisi</param>
        /// <param name="sessionid">Oturum kimliği.</param>
        public LogInfoResult(int portalid, string sicilno, string ogrencino, object eposta, string ad, string soyad, bool sa, string hash, Guid sessionid)
        {
            this.portalid = (portalid > 0 ? portalid : 0);
            this.sicilno = sicilno.ToStringOrEmpty().ToUpper();
            this.ogrencino = ogrencino.ToStringOrEmpty().ToUpper();
            this.eposta = setEMail(eposta);
            this.ad = ad.ToTitleCase(true, new char[] { '.' });
            this.soyad = soyad.ToStringOrEmpty().ToUpper();
            this.sa = sa;
            this.hash = hash.ToStringOrEmpty();
            this.sessionid = sessionid;
        }
        private string setEMail(object eposta)
        {
            if (eposta is MailAddress _m) { return _m.Address; }
            if (eposta is String _s && _try.TryMailAddress(_s, out _m)) { return _m.Address; }
            return "example@bayburt.edu.tr";
        }
        /// <summary>
        /// Nesne içerisindeki belirli alanların doğrulamalarını kontrol eder.
        /// </summary>
        public void GuardValidation()
        {
            if (this.portalid > 0) { Guard.CheckZeroOrNegative(this.portalid, nameof(this.portalid)); }
            Guard.CheckOutOfLength(this.sicilno, _nhr.sicilno, nameof(this.sicilno));
            Guard.CheckOutOfLength(this.ogrencino, _maximumlength.ogrencino, nameof(this.ogrencino));
            if (!this.eposta.IsNullOrEmpty_string()) { Guard.CheckOutOfLength(this.eposta, _maximumlength.eposta, nameof(this.eposta)); }
            Guard.CheckOutOfLength(this.ad, _nhr.ad, nameof(this.ad));
            Guard.CheckOutOfLength(this.soyad, _nhr.soyad, nameof(this.soyad));
            Guard.CheckNotIncludes(nameof(this.hash), this.hash.Length, 0, _maximumlength.hash);
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
                    ad = _form.ToKeyValueParseOrDefault_formcollection<string>(nameof(ad)) ?? "",
                    soyad = _form.ToKeyValueParseOrDefault_formcollection<string>(nameof(soyad)) ?? "",
                    sa = _form.ToKeyValueParseOrDefault_formcollection<bool>(nameof(sa)),
                    hash = _form.ToKeyValueParseOrDefault_formcollection<string>(nameof(hash)) ?? "",
                    sessionid = _form.ToKeyValueParseOrDefault_formcollection<Guid>(nameof(sessionid))
                });
            }
            return value.ToEnumerable().Select(x => x.ToDynamic()).Select(x => new LogInfoResult((int)x.portalid, (string)x.sicilno, (string)x.ogrencino, (object)x.eposta, (string)x.ad, (string)x.soyad, (bool)x.sa, (string)x.hash, (Guid)x.sessionid)).FirstOrDefault();
        }
    }
}