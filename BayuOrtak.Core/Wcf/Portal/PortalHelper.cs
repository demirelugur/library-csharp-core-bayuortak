namespace BayuOrtak.Core.Wcf.Portal
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Wcf.Portal.Dto;
    using Microsoft.Data.SqlClient;
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using static BayuOrtak.Core.Wcf.Portal.PortalHelper;
    public interface IPortalHelper
    {
        Task<DepInformation[]> DepInformationAsync_getall(DurumTypes? durum = null, CancellationToken cancellationToken = default);
        Task<DepInformation[]> DepInformationAsync_getalldepids(DurumTypes? durum, CancellationToken cancellationToken, params int[] depids);
        Task<DepInformation?> DepInformationAsync_getbydepid(int depid, CancellationToken cancellationToken = default);
        Task<DepInformation?> DepInformationAsync_getbyseo(string seo, CancellationToken cancellationToken = default);
        Task<DepInformation?> DepInformationAsync_getbytreecode(string treecode, CancellationToken cancellationToken = default);
        Task<int[]> DepUstDepsAsync(bool isonlyparents, string treecode, CancellationToken cancellationToken = default);
        Task<PersonelInformation[]> PersonelInformationAsync_getall(DurumTypes? durum = null, CancellationToken cancellationToken = default);
        Task<PersonelInformation[]> PersonelInformationAsync_getallportalids(DurumTypes? durum, CancellationToken cancellationToken, params int[] portalids);
        Task<PersonelInformation?> PersonelInformationAsync_getbyportalid(int portalid, CancellationToken cancellationToken = default);
        Task<PersonelInformation?> PersonelInformationAsync_getbykuladi(string kuladi, CancellationToken cancellationToken = default);
        Task<PersonelInformation?> PersonelInformationAsync_getbytckn(long tckn, CancellationToken cancellationToken = default);
        Task<PersonelInformation?> PersonelInformationAsync_getbysicilno(string sicilno, CancellationToken cancellationToken = default);
        Task<PersonelInformation?> PersonelInformationAsync_getbyseo(string seo, CancellationToken cancellationToken = default);
        Task<bool> HasValueAsync_byportalid(int portalid, CancellationToken cancellationToken = default);
        Task<bool> HasValueAsync_bykuladi(string kuladi, CancellationToken cancellationToken = default);
        Task<bool> HasValueAsync_bytckn(long tckn, CancellationToken cancellationToken = default);
        Task<bool> HasValueAsync_bysicilno(string sicilno, CancellationToken cancellationToken = default);
        Task<bool> HasValueAsync_byseo(string seo, CancellationToken cancellationToken = default);
        Task<UnvanInformation[]> UnvanInformationAsync_getall(CancellationToken cancellationToken = default);
        Task<UnvanInformation[]> UnvanInformationAsync_getallunvanids(CancellationToken cancellationToken, params string[] unvanids);
        Task<UnvanInformation?> UnvanInformationAsync_byunvanid(string unvanid, CancellationToken cancellationToken = default);
    }
    public sealed class PortalHelper : IPortalHelper
    {
        #region Private
        private readonly IDapperHelper _dap;
        private string setDurum(DurumTypes? durum) => (durum.HasValue ? durum.Value.ToString("g") : "tumu");
        private async Task<DepInformation[]> depinfomationAllAsync(object parameters, CancellationToken cancellationToken)
        {
            var _multi = await this._dap.QueryMultipleAsync("[dbo].[sp_wcf_depinformationall]", parameters, null, CommandType.StoredProcedure, cancellationToken);
            var _deps = (await _multi.ReadAsync<DepInformation>()).ToArray();
            if (_deps.Length > 0)
            {
                var _names = (await _multi.ReadAsync<DepNameInformation>()).ToArray();
                foreach (var item in _deps) { item.names = _names.Where(x => x.depid == item.depid).OrderBy(x => x.dil).ToArray(); }
                _names.DisposeAll();
            }
            return _deps;
        }
        private async Task<DepInformation?> depinfomationAsync(object parameters, CancellationToken cancellationToken)
        {
            var _multi = await this._dap.QueryMultipleAsync("[dbo].[sp_wcf_depinformation]", parameters, null, CommandType.StoredProcedure, cancellationToken);
            var _dep = (await _multi.ReadAsync<DepInformation>()).FirstOrDefault();
            if (_dep != null) { _dep.names = (await _multi.ReadAsync<DepNameInformation>()).ToArray(); }
            return _dep;
        }
        private async Task<PersonelInformation[]> personelinfomationAllAsync(object parameters, CancellationToken cancellationToken)
        {
            var _multi = await this._dap.QueryMultipleAsync("[dbo].[sp_wcf_personelinformationall]", parameters, null, CommandType.StoredProcedure, cancellationToken);
            var _personels = (await _multi.ReadAsync<PersonelInformation>()).ToArray();
            if (_personels.Length > 0)
            {
                var _yokinfoes = (await _multi.ReadAsync<YokInformation>()).ToArray();
                foreach (var item in _personels) { item.yokinfo = _yokinfoes.Where(x => x.uyeid == item.uyeid).FirstOrDefault(); }
                _yokinfoes.DisposeAll();
            }
            return _personels;
        }
        private async Task<PersonelInformation?> personelinformationAsync(object parameters, CancellationToken cancellationToken)
        {
            var _multi = await _dap.QueryMultipleAsync("[dbo].[sp_wcf_personelinformation]", parameters, null, CommandType.StoredProcedure, cancellationToken);
            var _personel = await _multi.ReadFirstOrDefaultAsync<PersonelInformation>();
            if (_personel != null) { _personel.yokinfo = await _multi.ReadFirstOrDefaultAsync<YokInformation?>(); }
            return _personel;
        }
        private Task<bool> personelhasvalueAsync(object parameters, CancellationToken cancellationToken) => this._dap.ExecuteScalarAsync<bool>("[dbo].[sp_wcf_personelhasvalue]", parameters, null, CommandType.StoredProcedure, cancellationToken);
        private async Task<UnvanInformation[]> unvaninformationAsync(string unvanids, CancellationToken cancellationToken)
        {
            var _multi = await _dap.QueryMultipleAsync("[dbo].[sp_wcf_unvaninformation]", new
            {
                unvanids
            }, null, CommandType.StoredProcedure, cancellationToken);
            var _unvans = (await _multi.ReadAsync<UnvanInformation>()).ToArray();
            if (_unvans.Length > 0)
            {
                var _names = (await _multi.ReadAsync<UnvanNameInformation>()).ToArray();
                foreach (var item in _unvans) { item.names = _names.Where(x => x.unvanid == item.unvanid).OrderBy(x => x.dil).ToArray(); }
                _names.DisposeAll();
            }
            return _unvans;
        }
        #endregion
        public enum DurumTypes : byte
        {
            pasif = 0,
            aktif
        }
        public PortalHelper(string connectionstring)
        {
            this._dap = new DapperHelper(new SqlConnection(connectionstring), null);
        }
        public Task<DepInformation[]> DepInformationAsync_getall(DurumTypes? durum = null, CancellationToken cancellationToken = default) => this.depinfomationAllAsync(new
        {
            depids = "_all",
            durum = this.setDurum(durum)
        }, cancellationToken);
        public Task<DepInformation[]> DepInformationAsync_getalldepids(DurumTypes? durum, CancellationToken cancellationToken, params int[] depids)
        {
            depids = (depids ?? Array.Empty<int>()).Distinct().OrderBy(x => x).ToArray();
            if (depids.Length == 0) { return Task.FromResult(Array.Empty<DepInformation>()); }
            return this.depinfomationAllAsync(new
            {
                depids = String.Join(",", depids),
                durum = this.setDurum(durum)
            }, cancellationToken);
        }
        public Task<DepInformation?> DepInformationAsync_getbydepid(int depid, CancellationToken cancellationToken = default) => this.depinfomationAsync(new
        {
            sector = nameof(depid),
            sectorvalue = (depid > 0 ? depid : 0).ToString()
        }, cancellationToken);
        public Task<DepInformation?> DepInformationAsync_getbyseo(string seo, CancellationToken cancellationToken = default) => this.depinfomationAsync(new
        {
            sector = nameof(seo),
            sectorvalue = seo.ToStringOrEmpty()
        }, cancellationToken);
        public Task<DepInformation?> DepInformationAsync_getbytreecode(string treecode, CancellationToken cancellationToken = default) => this.depinfomationAsync(new
        {
            sector = nameof(treecode),
            sectorvalue = treecode.ToStringOrEmpty()
        }, cancellationToken);
        public async Task<int[]> DepUstDepsAsync(bool isonlyparents, string treecode, CancellationToken cancellationToken = default)
        {
            treecode = treecode.ToStringOrEmpty();
            if (treecode == "") { return Array.Empty<int>(); }
            return (await _dap.QueryDynamicAsync("[dbo].[sp_wcf_depustdeps]", new
            {
                isonlyparents,
                treecode
            }, null, CommandType.StoredProcedure, cancellationToken)).Select(x => (int)x.depid).ToArray();
        }
        public Task<PersonelInformation[]> PersonelInformationAsync_getall(DurumTypes? durum = null, CancellationToken cancellationToken = default) => this.personelinfomationAllAsync(new
        {
            portalids = "_all",
            durum = this.setDurum(durum)
        }, cancellationToken);
        public Task<PersonelInformation[]> PersonelInformationAsync_getallportalids(DurumTypes? durum, CancellationToken cancellationToken, params int[] portalids)
        {
            portalids = (portalids ?? Array.Empty<int>()).Distinct().OrderBy(x => x).ToArray();
            if (portalids.Length == 0) { return Task.FromResult(Array.Empty<PersonelInformation>()); }
            return this.personelinfomationAllAsync(new
            {
                portalids = String.Join(",", portalids),
                durum = this.setDurum(durum)
            }, cancellationToken);
        }
        public Task<PersonelInformation?> PersonelInformationAsync_getbyportalid(int portalid, CancellationToken cancellationToken = default) => this.personelinformationAsync(new
        {
            sector = nameof(portalid),
            sectorvalue = (portalid > 0 ? portalid : 0).ToString()
        }, cancellationToken);
        public Task<PersonelInformation?> PersonelInformationAsync_getbykuladi(string kuladi, CancellationToken cancellationToken = default) => this.personelinformationAsync(new
        {
            sector = nameof(kuladi),
            sectorvalue = kuladi.ToStringOrEmpty().ToLower()
        }, cancellationToken);
        public Task<PersonelInformation?> PersonelInformationAsync_getbytckn(long tckn, CancellationToken cancellationToken = default) => this.personelinformationAsync(new
        {
            sector = nameof(tckn),
            sectorvalue = (tckn > 0 ? tckn : 0).ToString()
        }, cancellationToken);
        public Task<PersonelInformation?> PersonelInformationAsync_getbysicilno(string sicilno, CancellationToken cancellationToken = default) => this.personelinformationAsync(new
        {
            sector = nameof(sicilno),
            sectorvalue = sicilno.ToStringOrEmpty().ToUpper()
        }, cancellationToken);
        public Task<PersonelInformation?> PersonelInformationAsync_getbyseo(string seo, CancellationToken cancellationToken = default) => this.personelinformationAsync(new
        {
            sector = nameof(seo),
            sectorvalue = seo.ToStringOrEmpty()
        }, cancellationToken);
        public Task<bool> HasValueAsync_byportalid(int portalid, CancellationToken cancellationToken = default) => this.personelhasvalueAsync(new
        {
            sector = nameof(portalid),
            sectorvalue = (portalid > 0 ? portalid : 0).ToString()
        }, cancellationToken);
        public Task<bool> HasValueAsync_bykuladi(string kuladi, CancellationToken cancellationToken = default) => this.personelhasvalueAsync(new
        {
            sector = nameof(kuladi),
            sectorvalue = kuladi.ToStringOrEmpty().ToLower()
        }, cancellationToken);
        public Task<bool> HasValueAsync_bytckn(long tckn, CancellationToken cancellationToken = default) => this.personelhasvalueAsync(new
        {
            sector = nameof(tckn),
            sectorvalue = (tckn > 0 ? tckn : 0).ToStringOrEmpty()
        }, cancellationToken);
        public Task<bool> HasValueAsync_bysicilno(string sicilno, CancellationToken cancellationToken = default) => this.personelhasvalueAsync(new
        {
            sector = nameof(sicilno),
            sectorvalue = sicilno.ToStringOrEmpty().ToUpper()
        }, cancellationToken);
        public Task<bool> HasValueAsync_byseo(string seo, CancellationToken cancellationToken = default) => this.personelhasvalueAsync(new
        {
            sector = nameof(seo),
            sectorvalue = seo.ToStringOrEmpty()
        }, cancellationToken);
        public Task<UnvanInformation[]> UnvanInformationAsync_getall(CancellationToken cancellationToken = default) => this.unvaninformationAsync("_all", cancellationToken);
        public Task<UnvanInformation[]> UnvanInformationAsync_getallunvanids(CancellationToken cancellationToken, params string[] unvanids)
        {
            unvanids = (unvanids ?? Array.Empty<string>()).Distinct().OrderBy(x => x).ToArray();
            if (unvanids.Length == 0) { return Task.FromResult(Array.Empty<UnvanInformation>()); }
            return this.unvaninformationAsync(String.Join(",", unvanids), cancellationToken);
        }
        public async Task<UnvanInformation?> UnvanInformationAsync_byunvanid(string unvanid, CancellationToken cancellationToken = default)
        {
            var _t = (await this.unvaninformationAsync(unvanid, cancellationToken));
            if (_t.Length > 0) { return _t[0]; }
            return null;
        }
    }
}