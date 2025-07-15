namespace BayuOrtak.Core.Wcf.Nhr
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Interface;
    using System.ServiceModel;
    using Wcf_Nhr_personelinfo;

    public interface INHRHelper : IConnectionStatus
    {
        PersonelinfoServiceClient client { get; }
        Task<wspersonel> bytckimliknoAsync(long tckn);
        Task<wspersonel> byemailAsync(string eposta);
        Task<wspersonel> bykurumsicilnoAsync(string sicilno);
        Task<wspersonel[]> bynodeAsync(int nodeid = 1);
        Task<bool> validateAsync(long tckn);
    }
    public sealed class NHRHelper : INHRHelper, IDisposable
    {
        private readonly string username, password, username_all, password_all;
        private PersonelinfoServiceClient _Client;
        public void Dispose() { GC.SuppressFinalize(this); }
        public NHRHelper(string username, string password, string username_all, string password_all)
        {
            this.username = username;
            this.password = password;
            this.username_all = username_all;
            this.password_all = password_all;
        }
        public PersonelinfoServiceClient client
        {
            get
            {
                if (_Client == null)
                {
                    _Client = new PersonelinfoServiceClient(new BasicHttpBinding(BasicHttpSecurityMode.Transport)
                    {
                        MaxReceivedMessageSize = 104857600
                    }, new EndpointAddress("https://nhr.bayburt.edu.tr/webservices/personelinfo?wsdl"));
                }
                return _Client;
            }
        }
        public async Task<(bool statuswarning, string error)> IsConnectionStatusAsync(TimeSpan timeout, string dil, CancellationToken cancellationToken)
        {
            var _t = await this.client.Endpoint.Address.Uri.IsConnectionStatusAsync(timeout, cancellationToken);
            return (_t.statuswarning, _t.statuswarning ? GlobalConstants.webservice_connectionwarning(dil, "NHR PersonelInfo") : "");
        }
        public async Task<wspersonel> bytckimliknoAsync(long tckn) => (await this.client.bytckimliknoAsync(this.username, this.password, tckn)).@return;
        public async Task<wspersonel> byemailAsync(string eposta) => (await this.client.byemailAsync(this.username, this.password, eposta)).@return;
        public async Task<wspersonel> bykurumsicilnoAsync(string sicilno) => (await this.client.bykurumsicilnoAsync(this.username, this.password, sicilno)).@return;
        public async Task<wspersonel[]> bynodeAsync(int nodeid = 1) => (await this.client.bynodeAsync(this.username_all, this.password_all, nodeid)).@return;
        public async Task<bool> validateAsync(long tckn) => (await this.client.validateAsync(this.username, this.password, tckn)).@return > 0;
    }
}