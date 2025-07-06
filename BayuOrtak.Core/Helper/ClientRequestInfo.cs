namespace BayuOrtak.Core.Helper
{
    using BayuOrtak.Core.Attributes.DataAnnotations;
    using BayuOrtak.Core.Extensions;
    using Microsoft.AspNetCore.Http;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    public class ClientRequestInfo
    {
        [Validation_Required]
        [Display(Name = "Mobil")]
        [DefaultValue(false)]
        public bool ismobil { get; set; }
        [Validation_StringLength(_maximumlength.ipaddress)]
        [Validation_IPAddress]
        [Display(Name = "IP Adresi")]
        [DefaultValue(null)]
        public string? ipaddress { get; set; }
        public ClientRequestInfo() : this(default, default) { }
        public ClientRequestInfo(bool ismobil, object ipaddress)
        {
            this.ismobil = ismobil;
            this.ipaddress = ipaddressCast(ipaddress);
        }
        private string? ipaddressCast(object ipaddress)
        {
            if (ipaddress is IPAddress _ip) { return _ip.MapToIPv4().ToString(); }
            if (ipaddress is String _s && IPAddress.TryParse(_s, out _ip)) { return _ip.MapToIPv4().ToString(); }
            return null;
        }
        /// <summary>
        /// value için tanımlanan nesneler: ClientRequestInfo, HttpContext, IFormCollection, AnonymousObjectClass
        /// </summary>
        public static ClientRequestInfo ToEntityFromObject(object value)
        {
            if (value == null) { return new ClientRequestInfo(); }
            if (value is ClientRequestInfo _c) { return _c; }
            if (value is HttpContext context)
            {
                return ToEntityFromObject(new
                {
                    ismobil = context.IsMobileDevice(),
                    ipaddress = context.GetIPAddress()
                });
            }
            if (value is IFormCollection _form)
            {
                return ToEntityFromObject(new
                {
                    ismobil = _form.ToKeyValueParseOrDefault_formcollection<bool>(nameof(ismobil)),
                    ipaddress = _form.ToKeyValueParseOrDefault_formcollection<string>(nameof(ipaddress))
                });
            }
            return value.ToEnumerable().Select(x => x.ToDynamic()).Select(x => new ClientRequestInfo((bool)x.ismobil, (object)x.ipaddress)).FirstOrDefault();
        }
    }
}