namespace BayuOrtak.Core.Wcf.Yoksis
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Interface;
    using BayuOrtak.Core.Wcf.Yoksis.Helper;
    using System;
    using System.ServiceModel;
    using System.Threading.Tasks;
    using Wcf_Yoksis_DocentlikBilgi;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    public interface IDocentlikBilgiHelper : IConnectionStatus
    {
        DocentlikBilgiPortClient client { get; }
        Task<DocentlikBilgiResponse> DocentlikBilgiAsync(long tckn);
    }
    public sealed class DocentlikBilgiHelper : IDocentlikBilgiHelper, IDisposable
    {
        private readonly string password;
        public void Dispose() { GC.SuppressFinalize(this); }
        private DocentlikBilgiPortClient _Client;
        public DocentlikBilgiHelper(string password)
        {
            this.password = password;
        }
        public DocentlikBilgiPortClient client
        {
            get
            {
                if (_Client == null)
                {
                    _Client = new DocentlikBilgiPortClient(YoksisTools.basichttpbinding, new EndpointAddress("http://servisler.yok.gov.tr/ws/DocentlikBilgi?wsdl")); // Not: EndpointAddress uri yolu http ile başlamalıdır!
                    _Client.ClientCredentials.UserName.UserName = yoksis_UNI_code.ToString();
                    _Client.ClientCredentials.UserName.Password = this.password;
                }
                return _Client;
            }
        }
        public async Task<(bool statuswarning, string error)> IsConnectionStatusAsync(TimeSpan timeout, string dil, CancellationToken cancellationtoken)
        {
            var _t = await this.client.Endpoint.Address.Uri.IsConnectionStatusAsync(timeout, cancellationtoken);
            return (_t.statuswarning, _t.statuswarning ? GlobalConstants.webservice_connectionwarning(dil, "YÖKSİS, DocentlikBilgi") : "");
        }
        public async Task<DocentlikBilgiResponse> DocentlikBilgiAsync(long tckn) => (await this.client.DocentlikBilgiAsync(new DocentlikBilgiRequestType
        {
            TcKimlikNo = tckn
        })).DocentlikBilgiResponse;
    }
}