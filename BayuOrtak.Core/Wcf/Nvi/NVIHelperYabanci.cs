namespace BayuOrtak.Core.Wcf.Nvi
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Interface;
    using System;
    using System.ServiceModel;
    using Wcf_Nvi_kpspublicyabancidogrula;
    public interface INVIHelperYabanci : IConnectionStatus
    {
        KPSPublicYabanciDogrulaSoapClient client { get; }
        Task<bool> YabanciKimlikNoDogrulaAsync(long tckn, string ad, string soyad, DateOnly dogumtarih);
    }
    public sealed class NVIHelperYabanci : INVIHelperYabanci, IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }
        public NVIHelperYabanci() { }
        private KPSPublicYabanciDogrulaSoapClient _Client;
        public KPSPublicYabanciDogrulaSoapClient client
        {
            get
            {
                if (_Client == null)
                {
                    _Client = new KPSPublicYabanciDogrulaSoapClient(new BasicHttpBinding(BasicHttpSecurityMode.Transport)
                    {
                        MaxReceivedMessageSize = 104857600
                    }, new EndpointAddress("https://tckimlik.nvi.gov.tr/Service/KPSPublicYabanciDogrula.asmx?wsdl"));
                }
                return _Client;
            }
        }
        public async Task<(bool statuswarning, string error)> IsConnectionStatusAsync(TimeSpan timeout, string dil, CancellationToken cancellationtoken)
        {
            var _t = await this.client.Endpoint.Address.Uri.IsConnectionStatusAsync(timeout, cancellationtoken);
            return (_t.statuswarning, _t.statuswarning ? GlobalConstants.webservice_connectionwarning(dil, "NVI KPSPublicYabanciDogrula") : "");
        }
        public async Task<bool> YabanciKimlikNoDogrulaAsync(long tckn, string ad, string soyad, DateOnly dogumtarih)
        {
            Guard.CheckZeroOrNegative(tckn, nameof(tckn));
            Guard.CheckEmpty(ad, nameof(ad));
            Guard.CheckEmpty(soyad, nameof(soyad));
            return (await this.client.YabanciKimlikNoDogrulaAsync(tckn, ad.ToUpper(), soyad.ToUpper(), dogumtarih.Day, dogumtarih.Month, dogumtarih.Year)).Body.YabanciKimlikNoDogrulaResult;
        }
    }
}