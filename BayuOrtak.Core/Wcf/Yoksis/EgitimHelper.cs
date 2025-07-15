namespace BayuOrtak.Core.Wcf.Yoksis
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Interface;
    using BayuOrtak.Core.Wcf.Yoksis.Helper;
    using System;
    using System.ServiceModel;
    using Wcf_Yoksis_egitim;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    public interface IEgitimHelper : IConnectionStatus
    {
        YuksekOgretimEgitimBilgisiPortClient client { get; }
        Task<BirimResponse> BirimAsync(long birimid);
        Task<BirimTumAltResponse> BirimTumAltAsync(long birimid);
        Task<EgitimBilgisiMezunResponse> EgitimBilgisiMezunAsync(long tckn);
        Task<DenklikBilgisiResponse> DenklikBilgisiAsync(long tckn);
    }
    public sealed class EgitimHelper : IEgitimHelper, IDisposable
    {
        private readonly string password;
        private YuksekOgretimEgitimBilgisiPortClient _Client;
        public void Dispose() { GC.SuppressFinalize(this); }
        public EgitimHelper(string password)
        {
            this.password = password;
        }
        public YuksekOgretimEgitimBilgisiPortClient client
        {
            get
            {
                if (_Client == null)
                {
                    _Client = new YuksekOgretimEgitimBilgisiPortClient(YoksisTools.basicHttpBinding, new EndpointAddress("http://servisler.yok.gov.tr/ws/yuksekogretim/egitim?wsdl")); // Not: EndpointAddress uri yolu http ile başlamalıdır!
                    _Client.ClientCredentials.UserName.UserName = yoksis_UNI_code.ToString();
                    _Client.ClientCredentials.UserName.Password = this.password;
                }
                return _Client;
            }
        }
        public async Task<(bool statuswarning, string error)> IsConnectionStatusAsync(TimeSpan timeout, string dil, CancellationToken cancellationToken)
        {
            var _t = await this.client.Endpoint.Address.Uri.IsConnectionStatusAsync(timeout, cancellationToken);
            return (_t.statuswarning, _t.statuswarning ? GlobalConstants.webservice_connectionwarning(dil, "YÖKSİS, Egitim") : "");
        }
        public async Task<BirimResponse> BirimAsync(long birimid) => (await this.client.BirimAsync(new BirimRequestType
        {
            BirimId = birimid
        })).BirimResponse;
        public async Task<BirimTumAltResponse> BirimTumAltAsync(long birimid) => (await this.client.BirimTumAltAsync(new BirimRequestType
        {
            BirimId = birimid
        })).BirimTumAltResponse;
        public async Task<EgitimBilgisiMezunResponse> EgitimBilgisiMezunAsync(long tckn) => (await this.client.EgitimBilgisiMezunAsync(new EgitimBilgisiMezunRequestType
        {
            TcKimlikNo = tckn
        })).EgitimBilgisiMezunResponse;
        public async Task<DenklikBilgisiResponse> DenklikBilgisiAsync(long tckn) => (await this.client.DenklikBilgisiAsync(new DenklikBilgisiRequestType
        {
            TcKimlikNo = tckn
        })).DenklikBilgisiResponse; // Örnek veri: Ali BULUT
    }
}