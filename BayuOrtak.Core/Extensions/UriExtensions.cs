namespace BayuOrtak.Core.Extensions
{
    using BayuOrtak.Core.Helper;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    public static class UriExtensions
    {
        /// <summary>
        /// Verilen dizeye &quot;v&quot; parametresiyle bir sürüm numarası ekleyerek yeni bir URL oluşturur.
        /// </summary>
        /// <param name="uri">Temel URL.</param>
        /// <returns>Sürüm numarası eklenmiş URL.</returns>
        public static string GenerateVersionedUrl(this Uri uri)
        {
            Guard.CheckNull(uri, nameof(uri));
            var _separator = (uri.Query.Length > 0 ? "&" : "?");
            return $"{uri.ToString().TrimEnd('/')}{_separator}v={DateTime.Now.Ticks}";
        }
        /// <summary>
        /// Verilen URI&#39;nin bir YouTube gömme(embed) bağlantısı olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="uri">Kontrol edilecek URI.</param>
        /// <returns>URI bir YouTube gömme bağlantısı ise <see langword="true"/>, aksi halde <see langword="false"/> döner.</returns>
        public static bool IsYouTubeEmbedLink(this Uri uri) => (uri != null && uri.Host.Contains("youtube.com") && uri.AbsolutePath.StartsWith("/embed/"));
        /// <summary>
        /// Verilen URI&#39;nin bağlantı durumunu kontrol eder.
        /// </summary>
        public static async Task<(bool statuswarning, Uri requesturi)> IsConnectionStatusAsync(this Uri uri, TimeSpan timeout, CancellationToken cancellationtoken)
        {
            if (uri == null) { return (true, default); }
            using (var client = new HttpClient
            {
                Timeout = timeout
            })
            {
                try
                {
                    var _response = await client.GetAsync(uri, cancellationtoken);
                    if (_response.IsSuccessStatusCode) { return (false, _response.RequestMessage.RequestUri); }
                    return (true, default);
                }
                catch { return (true, default); }
            }
        }
        /// <summary>
        /// Belirtilen <see cref="Uri"/> adresinden byte[] veri almaya çalışır.
        /// </summary>
        public static async Task<(bool statuswarning, byte[] databinary, string mimetype, Exception? ex)> GetBinaryDataAsync(this Uri uri, TimeSpan timeout, CancellationToken cancellationtoken)
        {
            Guard.CheckNull(uri, nameof(uri));
            using (var client = new HttpClient
            {
                Timeout = timeout
            })
            {
                try
                {
                    var _response = await client.GetAsync(uri, cancellationtoken);
                    _response.EnsureSuccessStatusCode();
                    return (false, await _response.Content.ReadAsByteArrayAsync(cancellationtoken), _response.Content.Headers.ContentType?.MediaType.CoalesceOrDefault("application/octet-stream"), null);
                }
                catch (Exception ex) { return (true, default, "", ex); }
            }
        }
        /// <summary>
        /// Verilen URI&#39;nin şemasını &#39;https&#39; olarak ayarlar ve &#39;www.&#39; önekini kaldırır.
        /// <list type="bullet">
        /// <item>
        /// Eğer URI&#39;nin host kısmı &#39;www.&#39; ile başlıyorsa, bu önek kaldırılır.
        /// </item>
        /// <item>
        /// URI&#39;nin şeması otomatik olarak &#39;https&#39; olarak ayarlanır.
        /// </item>
        /// <item>
        /// Varsayılan port numarası (&#39;-1&#39;) kullanılır, yani URI&#39;deki port bilgisi kaldırılır.
        /// </item>
        /// <item>
        /// Sonuç olarak, düzenlenmiş URI bir dize olarak döndürülür ve sondaki &#39;/&#39; işareti kaldırılır.
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="uri">Düzenlenecek URI nesnesi.</param>
        /// <returns>Düzenlenmiş ve &#39;https&#39; şemasına sahip URI&#39;nin bir dize temsili.</returns>
        /// <exception cref="ArgumentNullException">Eğer URI null ise bir hata fırlatılır.</exception>
        public static string SetHttpsAndRemoveWww(this Uri uri)
        {
            Guard.CheckNull(uri, nameof(uri));
            var _host = uri.Host;
            if (_host.StartsWith("www.", StringComparison.OrdinalIgnoreCase)) { _host = _host.Substring(4); }
            return new UriBuilder(uri)
            {
                Scheme = Uri.UriSchemeHttps,
                Host = _host,
                Port = -1
            }.Uri.ToString().TrimEnd('/');
        }
    }
}