namespace BayuOrtak.Core.Wcf.Yoksis.Helper
{
    using BayuOrtak.Core.Extensions;
    using System.ServiceModel;
    using System.ServiceModel.Security;
    using Wcf_Yoksis_OzgecmisV2;
    public sealed class YoksisTools
    {
        internal static readonly BasicHttpBinding basicHttpBinding = new BasicHttpBinding
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
        /// Verilen ORCID değerini bir URI formatına dönüştürür. 
        /// Eğer ORCID boş bir string ise null döner. 
        /// ORCID başında &#39;/&#39; karakteri içeriyorsa, bu karakteri kaldırır ve 
        /// &quot;https://orcid.org/&quot; temel adresiyle birleştirerek bir Uri nesnesi oluşturur.
        /// </summary>
        /// <param name="orcid">Dönüştürülecek ORCID string değeri</param>
        /// <returns>ORCID&#39;e karşılık gelen Uri nesnesi veya null</returns>
        public static Uri? ToOrcidUri(string orcid)
        {
            orcid = orcid.ToStringOrEmpty();
            if (orcid == "") { return null; }
            if (orcid[0] == '/') { orcid = orcid.Substring(1); }
            return new Uri($"https://orcid.org/{orcid}");
        }
    }
}