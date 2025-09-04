namespace BayuOrtak.Core.Wcf.Yoksis.Helper
{
    using BayuOrtak.Core.Extensions;
    using System.ServiceModel;
    using System.ServiceModel.Security;
    using System.Text.RegularExpressions;
    using Wcf_Yoksis_OzgecmisV2;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    public sealed class YoksisTools
    {
        internal static readonly BasicHttpBinding basichttpbinding = new BasicHttpBinding
        {
            MaxReceivedMessageSize = 104857600,
            Security = new BasicHttpSecurity
            {
                Mode = BasicHttpSecurityMode.TransportCredentialOnly,
                Transport = new HttpTransportSecurity
                {
                    ClientCredentialType = HttpClientCredentialType.Basic,
                    ProxyCredentialType = HttpProxyCredentialType.Basic
                },
                Message = new BasicHttpMessageSecurity
                {
                    ClientCredentialType = BasicHttpMessageCredentialType.UserName,
                    AlgorithmSuite = SecurityAlgorithmSuite.Default
                }
            }
        };
        public static bool IsKayitBulunmadi(SonucBilgiTip? sonuc) => (sonuc == null || (sonuc.SonucKod == 0 && sonuc.SonucMesaj.ToSeoFriendly() == "kayit-bulunmadi"));
        /// <summary>
        /// Verilen <paramref name="authorid"/> değerini kontrol ederek YÖK Akademik Arama sistemi üzerinde ilgili akademisyenin profil sayfasına ait URI bilgisini üretir.
        /// <para>Örnek: https://akademik.yok.gov.tr/AkademikArama/AkademisyenGorevOgrenimBilgileri?islem=direct&amp;authorId={<paramref name="authorid"/>}</para>
        /// </summary>
        public static Uri? ToAkademikProfileUri(string authorid)
        {
            authorid = authorid.ToStringOrEmpty().ToUpper();
            if (authorid.Length == _maximumlength.yoksis_authorid || Regex.IsMatch(authorid, $"^[0-9A-Fa-f]{_maximumlength.yoksis_authorid}$")) { return new Uri($"https://akademik.yok.gov.tr/AkademikArama/AkademisyenGorevOgrenimBilgileri?islem=direct&authorId={authorid}"); }
            return null;
        }
        /// <summary>
        /// Verilen ORCID bilgisini kullanarak <see href="https://orcid.org/">ORCID</see> üzerindeki araştırmacı profil sayfasına ait <see cref="Uri"/> bilgisini döndürür.
        /// </summary>
        public static Uri? ToOrcidUri(string orcid)
        {
            orcid = orcid.ToStringOrEmpty().ToUpper();
            if (orcid == "") { return null; }
            if (orcid[0] == '/') { orcid = orcid.Substring(1); }
            return new Uri($"https://orcid.org/{orcid}");
        }
    }
}