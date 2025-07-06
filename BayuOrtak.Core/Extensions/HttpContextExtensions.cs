namespace BayuOrtak.Core.Extensions
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Linq;
    using System.Net;
    public static class HttpContextExtensions
    {
        /// <summary>
        /// İstemcinin mobil bir cihaz olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="context">HttpContext nesnesi.</param>
        /// <returns>Mobil bir cihaz ise <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
        public static bool IsMobileDevice(this HttpContext context)
        {
            var useragent = context.Request.Headers.UserAgent.ToStringOrEmpty().ToLower();
            if (useragent == "") { return false; }
            return (useragent.Contains("android") || useragent.Contains("iphone") || useragent.Contains("ipad") || useragent.Contains("mobile"));
        }
        /// <summary>
        /// Geçerli HTTP isteğine ait tam URL&#39;yi (scheme ve host dahil) döndürür.
        /// İsteğe bağlı olarak, URL&#39;ye yol (path) ve sorgu stringi (query string) de dahil edilebilir.
        /// </summary>
        /// <param name="context">Geçerli HTTP isteğini içeren HttpContext.</param>
        /// <param name="includesPathAndQueryString">
        /// true ise, URL&#39;nin path ve query string bölümleri de dahil edilir; 
        /// false ise, yalnızca scheme ve host kısmı döndürülür.
        /// </param>
        /// <returns>Geçerli isteğe ait tam bir Uri nesnesi.</returns>
        public static Uri ToAbsoluteUri(this HttpContext context, bool includesPathAndQueryString)
        {
            var r = context.Request;
            if (includesPathAndQueryString) { return new Uri($"{r.Scheme}://{r.Host.Value}{(r.Path.Value)}{(r.QueryString.Value)}"); }
            return new Uri($"{r.Scheme}://{r.Host.Value}");
        }
        /// <summary>
        /// Bearer token&#39;ı HttpContext&#39;den alır.
        /// </summary>
        /// <param name="context">HttpContext nesnesi.</param>
        /// <returns>Bearer token.</returns>
        public static string GetToken(this HttpContext context) => context.Request.Headers.Authorization.ToString().Replace("Bearer ", "").ToStringOrEmpty();
        /// <summary>
        /// İstemcinin IP adresini alır.
        /// </summary>
        /// <param name="context">HttpContext nesnesi.</param>
        /// <returns>İstemcinin IP adresi.</returns>
        public static IPAddress GetIPAddress(this HttpContext context)
        {
            if (IPAddress.TryParse(context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? "", out IPAddress _ip)) { return _ip.MapToIPv4(); }
            _ip = context.Connection.RemoteIpAddress;
            if (_ip == null) { return IPAddress.Any; }
            return _ip.MapToIPv4();
        }
    }
}