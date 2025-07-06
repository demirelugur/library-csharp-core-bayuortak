namespace BayuOrtak.Core.Helper
{
    using BayuOrtak.Core.Attributes.DataAnnotations;
    using BayuOrtak.Core.Extensions;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json.Linq;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Net.Mail;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public sealed class SmtpSettingsHelper
    {
        /// <summary>
        /// SMTP sunucu kullanıcı adı.
        /// </summary>
        [Validation_Required]
        [EmailAddress(ErrorMessage = _validationerrormessage.email)]
        [Validation_StringLength(_maximumlength.eposta)]
        [Display(Name = "e-Posta")]
        [DefaultValue("")]
        public string email { get; set; }
        /// <summary>
        /// SMTP sunucu parolası.
        /// </summary>s
        [Validation_Required]
        [Validation_StringLength(16, 8)]
        [Display(Name = "Şifre")]
        [DefaultValue("")]
        public string password { get; set; }
        /// <summary>
        /// SMTP sunucu adresi.
        /// </summary>
        [Validation_Required]
        [Validation_StringLength(30)]
        [Display(Name = "Host")]
        [DefaultValue("")]
        public string host { get; set; }
        /// <summary>
        /// SMTP sunucusunun port numarası.
        /// </summary>
        [Validation_Required]
        [Validation_RangePositiveInt32]
        [Display(Name = "Port")]
        [DefaultValue(25)]
        public int port { get; set; }
        /// <summary>
        /// SSL kullanım durumunu belirtir.
        /// </summary>
        [Validation_Required]
        [Display(Name = "Enable SSL")]
        [DefaultValue(false)]
        public bool enablessl { get; set; }
        /// <summary>
        /// Varsayılan kimlik bilgilerini kullanma durumunu belirtir.
        /// </summary>
        [Validation_Required]
        [Display(Name = "Use Default Credentials")]
        [DefaultValue(false)]
        public bool usedefaultcredentials { get; set; }
        /// <summary>
        /// SMTP iletim yöntemini belirtir.
        /// </summary>
        [Validation_Required]
        [EnumDataType(typeof(SmtpDeliveryMethod), ErrorMessage = _validationerrormessage.enumdatatype)]
        [Display(Name = "Delivery Method")]
        [DefaultValue(SmtpDeliveryMethod.Network)]
        public SmtpDeliveryMethod deliverymethod { get; set; }
        /// <summary>
        /// SMTP istemcisinin zaman aşım süresi.
        /// </summary>
        [Validation_Required]
        [Display(Name = "Timeout")]
        [DefaultValue(0)]
        public int timeout { get; set; }
        /// <summary>
        /// <see cref="SmtpClient"/> objesini oluşturur
        /// </summary>
        /// <returns></returns>
        public SmtpClient toSmtpClient()
        {
            var sc = new SmtpClient
            {
                Port = this.port,
                Host = this.host,
                EnableSsl = this.enablessl,
                UseDefaultCredentials = this.usedefaultcredentials,
                Credentials = new NetworkCredential(this.email, this.password),
                DeliveryMethod = this.deliverymethod
            };
            if (this.timeout > 0) { sc.Timeout = this.timeout; }
            return sc;
        }
        /// <summary>
        /// Varsayılan yapıcı metot.
        /// </summary>
        public SmtpSettingsHelper() : this("", "", "", default, default, default, default, default) { }
        /// <summary>
        /// Belirtilen parametrelerle SmtpSettingsHelper nesnesi oluşturur.
        /// </summary>
        /// <param name="email">e-Posta.</param>
        /// <param name="password">e-Posta Şifresi.</param>
        /// <param name="host">Sunucu adresi.</param>
        /// <param name="port">Port numarası.</param>
        /// <param name="enablessl">SSL durumu.</param>
        /// <param name="usedefaultcredentials">Varsayılan kimlik bilgilerini kullanma durumu.</param>
        /// <param name="deliverymethod">İletim yöntemi.</param>
        /// <param name="timeout">Zaman aşım süresi.</param>
        public SmtpSettingsHelper(string email, string password, string host, int port, bool enablessl, bool usedefaultcredentials, SmtpDeliveryMethod deliverymethod, int timeout)
        {
            this.email = email;
            this.password = password;
            this.host = host;
            this.port = port;
            this.enablessl = enablessl;
            this.usedefaultcredentials = usedefaultcredentials;
            this.deliverymethod = deliverymethod;
            this.timeout = timeout;
        }
        /// <summary>
        /// Gmail için SMTP ayarlarını oluşturur. <code>new SmtpSettingsHelper(mailAddress.Address, password, &quot;smtp.gmail.com&quot;, 587, true, true, SmtpDeliveryMethod.Network, 0);</code>
        /// </summary>
        /// <param name="mailAddress">e-Posta adresi.</param>
        /// <param name="password">Parola.</param>
        /// <returns>Oluşturulan SMTP ayarları.</returns>
        public static SmtpSettingsHelper CreateSmtpSettings_gmail(MailAddress mailAddress, string password) => new SmtpSettingsHelper(mailAddress.Address, password, "smtp.gmail.com", 587, true, true, SmtpDeliveryMethod.Network, 0);
        /// <summary>
        /// Outlook için SMTP ayarlarını oluşturur. <code>new SmtpSettingsHelper(mailAddress.Address, password, &quot;smtp.office365.com&quot;, 587, true, false, SmtpDeliveryMethod.Network, 0);</code>
        /// </summary>
        /// <param name="mailAddress">e-Posta adresi.</param>
        /// <param name="password">Parola.</param>
        /// <returns>Oluşturulan SMTP ayarları.</returns>
        public static SmtpSettingsHelper CreateSmtpSettings_outlook(MailAddress mailAddress, string password) => new SmtpSettingsHelper(mailAddress.Address, password, "smtp.office365.com", 587, true, false, SmtpDeliveryMethod.Network, 0);
        /// <summary>
        /// T.C. Bayburt Üniversitesi için SMTP ayarlarını oluşturur. <code>new SmtpSettingsHelper(mailAddress.Address, password, &quot;posta.bayburt.edu.tr&quot;, 25, false, false, SmtpDeliveryMethod.Network, 0);</code>
        /// </summary>
        /// <param name="mailAddress">e-Posta adresi.</param>
        /// <param name="password">Parola.</param>
        /// <returns>Oluşturulan SMTP ayarları.</returns>
        public static SmtpSettingsHelper CreateSmtpSettings_bayburtuni(MailAddress mailAddress, string password) => new SmtpSettingsHelper(mailAddress.Address, password, "posta.bayburt.edu.tr", 25, true, true, SmtpDeliveryMethod.Network, 0);
        /// <summary>
        /// value için tanımlanan nesneler: SmtpSettingsHelper, IFormCollection, String(JTokenType.Object), AnonymousObjectClass
        /// </summary>
        public static SmtpSettingsHelper ToEntityFromObject(object value)
        {
            if (value == null) { return new SmtpSettingsHelper(); }
            if (value is SmtpSettingsHelper _ssh) { return _ssh; }
            if (value is IFormCollection _form)
            {
                return ToEntityFromObject(new
                {
                    email = _form.ToKeyValueParseOrDefault_formcollection<string>(nameof(email)) ?? "",
                    password = _form.ToKeyValueParseOrDefault_formcollection<string>(nameof(password)) ?? "",
                    host = _form.ToKeyValueParseOrDefault_formcollection<string>(nameof(host)) ?? "",
                    port = _form.ToKeyValueParseOrDefault_formcollection<int>(nameof(port)),
                    enablessl = _form.ToKeyValueParseOrDefault_formcollection<bool>(nameof(enablessl)),
                    usedefaultcredentials = _form.ToKeyValueParseOrDefault_formcollection<bool>(nameof(usedefaultcredentials)),
                    deliverymethod = _form.ToKeyValueParseOrDefault_formcollection<SmtpDeliveryMethod>(nameof(deliverymethod)),
                    timeout = _form.ToKeyValueParseOrDefault_formcollection<int>(nameof(timeout))
                });
            }
            if (value is String _s)
            {
                try
                {
                    if (_try.TryJson(_s, JTokenType.Object, out JObject _jo) && _jo.ToObject<SmtpSettingsHelper>() is SmtpSettingsHelper _sj) { return _sj; }
                    return new SmtpSettingsHelper();
                }
                catch { return new SmtpSettingsHelper(); }
            }
            return value.ToEnumerable().Select(x => x.ToDynamic()).Select(x => new SmtpSettingsHelper((string)x.email, (string)x.password, (string)x.host, (int)x.port, (bool)x.enablessl, (bool)x.usedefaultcredentials, (SmtpDeliveryMethod)x.deliverymethod, (int)x.timeout)).FirstOrDefault();
        }
    }
}