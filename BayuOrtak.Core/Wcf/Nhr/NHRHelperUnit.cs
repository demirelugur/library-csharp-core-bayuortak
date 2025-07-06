namespace BayuOrtak.Core.Wcf.Nhr
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Interface;
    using System;
    using System.ServiceModel;
    using Wcf_Nhr_unit;
    public interface INHRHelperUnit : IConnectionStatus
    {
        UnitServiceClient client { get; }
        Task<wsnode[]> birimagacibytypeAsync_akademik();
        Task<wsnode[]> birimagacibytypeAsync_idari();
        Task<wsnode[]> birimagacibytypeAsync_diger();
        Task<wsunvan[]> unvanlistAsync();
    }
    public sealed class NHRHelperUnit : INHRHelperUnit, IDisposable
    {
        private readonly string username, password;
        private UnitServiceClient _Client;
        public void Dispose() { GC.SuppressFinalize(this); }
        public NHRHelperUnit(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
        public UnitServiceClient client
        {
            get
            {
                if (_Client == null)
                {
                    _Client = new UnitServiceClient(new BasicHttpBinding(BasicHttpSecurityMode.Transport)
                    {
                        MaxReceivedMessageSize = 104857600
                    }, new EndpointAddress("https://nhr.bayburt.edu.tr/webservices/unit?wsdl"));
                }
                return _Client;
            }
        }
        public async Task<bool> IsConnectionStatusAsync(TimeSpan timeout, CancellationToken cancellationToken = default) => !(await this.client.Endpoint.Address.Uri.IsConnectionStatusAsync(timeout, cancellationToken)).statuswarning;
        private async Task<wsnode[]> birimagacibytypeAsync(int typeid) => (await this.client.birimagacibytypeAsync(this.username, this.password, typeid)).@return;
        public Task<wsnode[]> birimagacibytypeAsync_akademik() => this.birimagacibytypeAsync(1);
        public Task<wsnode[]> birimagacibytypeAsync_idari() => this.birimagacibytypeAsync(2);
        public Task<wsnode[]> birimagacibytypeAsync_diger() => this.birimagacibytypeAsync(3);
        public async Task<wsunvan[]> unvanlistAsync() => (await this.client.unvanlistAsync(this.username, this.password)).@return;
    }
}