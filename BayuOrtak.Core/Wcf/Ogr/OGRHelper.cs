namespace BayuOrtak.Core.Wcf.Ogr
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Interface;
    using System;
    using System.ServiceModel;
    using Wcf_Ogr_ogrservice;
    public interface IOGRHelper : IConnectionStatus
    {
        OgrServiceClient client { get; }
        Task<OgrImage_Response> GetByOgrImage_ogridAsync(int ogrid);
        Task<OgrImage_Response> GetByOgrImage_ogrnoAsync(string ogrno);
        Task<OgrContact_Response> GetByOgrContact_ogridAsync(int ogrid);
        Task<OgrContact_Response> GetByOgrContact_ogrnoAsync(string ogrno);
        Task<OgrContactUnipaMasterDB_Response> GetByOgrContactUnipaMasterDBAsync(string ogrno);
        Task<OgrAddress_Response> GetByOgrAddress_ogridAsync(int ogrid);
        Task<OgrAddress_Response> GetByOgrAddress_ogrnoAsync(string ogrno);
        Task<OgrCiftYandal_Response> GetByOgrCiftYanDalAsync(int ciftyandalid);
        Task<OgrMktr_Response> GetByOgrMktrAsync();
        Task<OgrOgretimYiliDonemi_Response> GetByOgretimYiliDonemiAsync(string ogrno);
        Task<OgrOgretimYiliDonemiDetay_Response> GetByOgretimYiliDonemiDetayAsync(string ogrno, int ogretimyili, int ogretimdonemiid);
        Task<OgrInformationOgrNo_Response> GetByOgrNoAsync(string ogrno);
        Task<OgrInformation_Response> GetByTcknAsync(string tckn);
        Task<OgrInformation_Response> GetByOgrDataListAsync(string[] values, bool istckn = true);
        Task<UyrukInformation_Response> GetUyrukListAsync();
        Task<SehirIlceInformation_Response> GetSehirListAsync();
        Task<SehirIlceInformation_Response> GetIlceListAsync(int ilkodu, bool isplaka = true);
        Task<ValidateOgrNo_Response> ValidateOgrNoAsync(string ogrno);
        Task<ValidateTckn_Response> ValidateTcknAsync(string tckn);
    }
    public sealed class OGRHelper : IOGRHelper, IDisposable
    {
        private readonly string username, password;
        private OgrServiceClient _Client;
        public void Dispose() { GC.SuppressFinalize(this); }
        public OGRHelper(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
        public OgrServiceClient client
        {
            get
            {
                if (_Client == null)
                {
                    _Client = new OgrServiceClient(new BasicHttpBinding(BasicHttpSecurityMode.Transport)
                    {
                        MaxReceivedMessageSize = 104857600
                    }, new EndpointAddress("https://ogrencisorgula.bayburt.edu.tr/OgrService.svc?wsdl"));
                }
                return _Client;
            }
        }
        public async Task<(bool statuswarning, string error)> IsConnectionStatusAsync(TimeSpan timeout, string dil, CancellationToken cancellationtoken)
        {
            var _t = await this.client.Endpoint.Address.Uri.IsConnectionStatusAsync(timeout, cancellationtoken);
            return (_t.statuswarning, _t.statuswarning ? GlobalConstants.webservice_connectionwarning(dil, "OgrService") : "");
        }
        public Task<OgrImage_Response> GetByOgrImage_ogridAsync(int ogrid) => this.client.GetByOgrImageAsync(this.username, this.password, ogrid.ToString(), false);
        public Task<OgrImage_Response> GetByOgrImage_ogrnoAsync(string ogrno) => this.client.GetByOgrImageAsync(this.username, this.password, ogrno, true);
        public Task<OgrContact_Response> GetByOgrContact_ogridAsync(int ogrid) => this.client.GetByOgrContactAsync(this.username, this.password, ogrid.ToString(), false);
        public Task<OgrContact_Response> GetByOgrContact_ogrnoAsync(string ogrno) => this.client.GetByOgrContactAsync(this.username, this.password, ogrno, true);
        public Task<OgrContactUnipaMasterDB_Response> GetByOgrContactUnipaMasterDBAsync(string ogrno) => this.client.GetByOgrContactUnipaMasterDBAsync(this.username, this.password, ogrno);
        public Task<OgrAddress_Response> GetByOgrAddress_ogridAsync(int ogrid) => this.client.GetByOgrAddressAsync(this.username, this.password, ogrid.ToString(), false);
        public Task<OgrAddress_Response> GetByOgrAddress_ogrnoAsync(string ogrno) => this.client.GetByOgrAddressAsync(this.username, this.password, ogrno, true);
        public Task<OgrCiftYandal_Response> GetByOgrCiftYanDalAsync(int ciftyandalid) => this.client.GetByOgrCiftYanDalAsync(this.username, this.password, ciftyandalid);
        public Task<OgrMktr_Response> GetByOgrMktrAsync() => this.client.GetByOgrMktrAsync(this.username, this.password);
        public Task<OgrOgretimYiliDonemi_Response> GetByOgretimYiliDonemiAsync(string ogrno) => this.client.GetByOgretimYiliDonemiAsync(this.username, this.password, ogrno);
        public Task<OgrOgretimYiliDonemiDetay_Response> GetByOgretimYiliDonemiDetayAsync(string ogrno, int ogretimyili, int ogretimdonemiid) => this.client.GetByOgretimYiliDonemiDetayAsync(this.username, this.password, ogrno, ogretimyili, ogretimdonemiid);
        public Task<OgrInformationOgrNo_Response> GetByOgrNoAsync(string ogrno) => this.client.GetByOgrNoAsync(this.username, this.password, ogrno);
        public Task<OgrInformation_Response> GetByTcknAsync(string tckn) => this.client.GetByTcknAsync(this.username, this.password, tckn);
        public Task<OgrInformation_Response> GetByOgrDataListAsync(string[] values, bool istckn = true) => this.client.GetByOgrDataListAsync(this.username, this.password, istckn, String.Join(";", values));
        public Task<UyrukInformation_Response> GetUyrukListAsync() => this.client.GetUyrukListAsync(this.username, this.password);
        public Task<SehirIlceInformation_Response> GetSehirListAsync() => this.client.GetSehirListAsync(this.username, this.password);
        public Task<SehirIlceInformation_Response> GetIlceListAsync(int ilkodu, bool isplaka = true) => this.client.GetIlceListAsync(this.username, this.password, ilkodu, isplaka);
        public Task<ValidateOgrNo_Response> ValidateOgrNoAsync(string ogrno) => this.client.ValidateOgrNoAsync(this.username, this.password, ogrno);
        public Task<ValidateTckn_Response> ValidateTcknAsync(string tckn) => this.client.ValidateTcknAsync(this.username, this.password, tckn);
    }
}