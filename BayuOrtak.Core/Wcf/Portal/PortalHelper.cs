namespace BayuOrtak.Core.Wcf.Portal
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Wcf.Portal.Dto;
    using BayuOrtak.Core.Wcf.Portal.Enums;
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using static Dapper.SqlMapper;
    public interface IPortalHelper
    {
        Task<DepInformation[]> DepInformationAsync_getall(CancellationToken cancellationtoken, Portal_DurumTypes? durum = Portal_DurumTypes.aktif);
        Task<DepInformation[]> DepInformationAsync_getalldepids(CancellationToken cancellationtoken, params int[] depids);
        Task<DepInformation?> DepInformationAsync_getbydepid(int depid, CancellationToken cancellationtoken);
        Task<DepInformation?> DepInformationAsync_getbyseo(string seo, CancellationToken cancellationtoken);
        Task<DepInformation?> DepInformationAsync_getbytreecode(string treecode, CancellationToken cancellationtoken);
        Task<int[]> DepUstDepsAsync(bool isonlyparents, string treecode, CancellationToken cancellationtoken);
        Task<PersonelInformationBasic[]> PersonelSuperAdminAsync_getall(CancellationToken cancellationtoken);
        Task<PersonelInformation[]> PersonelInformationAsync_getall(CancellationToken cancellationtoken, Portal_DurumTypes? durum = Portal_DurumTypes.aktif);
        Task<PersonelInformation[]> PersonelInformationAsync_getallportalids(CancellationToken cancellationtoken, params int[] portalids);
        Task<PersonelInformation?> PersonelInformationAsync_getbyportalid(int portalid, CancellationToken cancellationtoken);
        Task<PersonelInformation?> PersonelInformationAsync_getbykuladi(string kuladi, CancellationToken cancellationtoken);
        Task<PersonelInformation?> PersonelInformationAsync_getbytckn(long tckn, CancellationToken cancellationtoken);
        Task<PersonelInformation?> PersonelInformationAsync_getbysicilno(string sicilno, CancellationToken cancellationtoken);
        Task<PersonelInformation?> PersonelInformationAsync_getbyseo(string seo, CancellationToken cancellationtoken);
        Task<bool> HasValueAsync_byportalid(int portalid, CancellationToken cancellationtoken);
        Task<bool> HasValueAsync_bykuladi(string kuladi, CancellationToken cancellationtoken);
        Task<bool> HasValueAsync_bytckn(long tckn, CancellationToken cancellationtoken);
        Task<bool> HasValueAsync_bysicilno(string sicilno, CancellationToken cancellationtoken);
        Task<bool> HasValueAsync_byseo(string seo, CancellationToken cancellationtoken);
        Task<UnvanInformation[]> UnvanInformationAsync_getall(CancellationToken cancellationtoken);
        Task<UnvanInformation[]> UnvanInformationAsync_getallunvanids(CancellationToken cancellationtoken, params string[] unvanids);
        Task<UnvanInformation?> UnvanInformationAsync_byunvanid(string unvanid, CancellationToken cancellationtoken);
    }
    public sealed class PortalHelper : IPortalHelper
    {
        #region Private
        private readonly IDapperHelper dap;
        private string setDurum(Portal_DurumTypes? durum) => (durum.HasValue ? durum.Value.ToString("g") : "tumu");
        private async Task<DepInformation[]> depinfomationDataAsync(GridReader multi)
        {
            var _deps = (await multi.ReadAsync<DepInformation>()).ToArray();
            if (_deps.Length > 0)
            {
                var _names = (await multi.ReadAsync()).Select(x => new
                {
                    depid = (int)x.depid,
                    dil = (Portal_DilTypes)x.dil,
                    adi = (string)x.adi,
                    adiuzun = (string)x.adiuzun
                }).ToArray();
                var _kampus = (await multi.ReadAsync()).Select(x => new
                {
                    kampusid = (int)x.kampusid,
                    dil = (Portal_DilTypes)x.dil,
                    adi = (string)x.adi
                }).ToArray();
                foreach (var item in _deps)
                {
                    item.depnames = _names.Where(x => x.depid == item.depid).OrderBy(x => x.dil).ToDictionary(x => x.dil, x => (x.adi, x.adiuzun));
                    item.kampusnames = _kampus.Where(x => x.kampusid == item.kampusid).OrderBy(x => x.dil).ToDictionary(x => x.dil, x => x.adi);
                }
            }
            return _deps;
        }
        private async Task<DepInformation[]> depinfomationAllAsync(object parameters, CancellationToken cancellationtoken) => await this.depinfomationDataAsync(await this.dap.QueryMultipleAsync("[dbo].[sp_wcf_depinformationall]", parameters, null, CommandType.StoredProcedure, cancellationtoken));
        private async Task<DepInformation?> depinfomationAsync(object parameters, CancellationToken cancellationtoken)
        {
            var _deps = await this.depinfomationDataAsync(await this.dap.QueryMultipleAsync("[dbo].[sp_wcf_depinformation]", parameters, null, CommandType.StoredProcedure, cancellationtoken));
            if (_deps.IsNullOrEmpty_collection()) { return null; }
            return _deps[0];
        }
        private async Task<PersonelInformation[]> personelinformationDataAsync(GridReader multi)
        {
            var _personels = (await multi.ReadAsync<PersonelInformation>()).ToArray();
            if (_personels.Length > 0)
            {
                var _yokinfoes = (await multi.ReadAsync()).Select(item => new
                {
                    uyeid = (int)item.uyeid,
                    item
                }).ToArray();
                foreach (var item in _personels) { item.yokinfo = _yokinfoes.Where(x => x.uyeid == item.uyeid).Select(x => YokInformation.ToEntityFromDynamic(x.item)).FirstOrDefault(); }
            }
            return _personels;
        }
        private async Task<PersonelInformation[]> personelinfomationAllAsync(object parameters, CancellationToken cancellationtoken) => await this.personelinformationDataAsync(await this.dap.QueryMultipleAsync("[dbo].[sp_wcf_personelinformationall]", parameters, null, CommandType.StoredProcedure, cancellationtoken));
        private async Task<PersonelInformation?> personelinformationAsync(object parameters, CancellationToken cancellationtoken)
        {
            var _personels = await this.personelinformationDataAsync(await dap.QueryMultipleAsync("[dbo].[sp_wcf_personelinformation]", parameters, null, CommandType.StoredProcedure, cancellationtoken));
            if (_personels.IsNullOrEmpty_collection()) { return null; }
            return _personels[0];
        }
        private Task<bool> personelhasvalueAsync(object parameters, CancellationToken cancellationtoken) => this.dap.ExecuteScalarAsync<bool>("[dbo].[sp_wcf_personelhasvalue]", parameters, null, CommandType.StoredProcedure, cancellationtoken);
        private async Task<UnvanInformation[]> unvaninformationAsync(string unvanids, CancellationToken cancellationtoken)
        {
            var _multi = await dap.QueryMultipleAsync("[dbo].[sp_wcf_unvaninformationall]", new
            {
                unvanids
            }, null, CommandType.StoredProcedure, cancellationtoken);
            var _unvans = (await _multi.ReadAsync<UnvanInformation>()).ToArray();
            if (_unvans.Length > 0)
            {
                var _names = (await _multi.ReadAsync()).Select(x => new
                {
                    unvanid = (string)x.unvanid,
                    dil = (Portal_DilTypes)x.dil,
                    adi = (string)x.adi,
                    adikisa = (string)x.adikisa
                }).ToArray();
                foreach (var item in _unvans) { item.unvannames = _names.Where(x => x.unvanid == item.unvanid).OrderBy(x => x.dil).ToDictionary(x => x.dil, x => (x.adi, x.adikisa)); }
            }
            return _unvans;
        }
        #endregion
        public PortalHelper(IDapperHelper dap)
        {
            this.dap = dap;
        }
        public Task<DepInformation[]> DepInformationAsync_getall(CancellationToken cancellationtoken, Portal_DurumTypes? durum = Portal_DurumTypes.aktif) => this.depinfomationAllAsync(new
        {
            depids = "_all",
            durum = this.setDurum(durum)
        }, cancellationtoken);
        public Task<DepInformation[]> DepInformationAsync_getalldepids(CancellationToken cancellationtoken, params int[] depids)
        {
            depids = (depids ?? Array.Empty<int>()).Distinct().OrderBy(x => x).ToArray();
            if (depids.Length == 0) { return Task.FromResult(Array.Empty<DepInformation>()); }
            return this.depinfomationAllAsync(new
            {
                depids = String.Join(",", depids),
                durum = this.setDurum(null)
            }, cancellationtoken);
        }
        public Task<DepInformation?> DepInformationAsync_getbydepid(int depid, CancellationToken cancellationtoken) => this.depinfomationAsync(new
        {
            sector = nameof(depid),
            sectorvalue = (depid > 0 ? depid : 0).ToString()
        }, cancellationtoken);
        public Task<DepInformation?> DepInformationAsync_getbyseo(string seo, CancellationToken cancellationtoken) => this.depinfomationAsync(new
        {
            sector = nameof(seo),
            sectorvalue = seo.ToStringOrEmpty()
        }, cancellationtoken);
        public Task<DepInformation?> DepInformationAsync_getbytreecode(string treecode, CancellationToken cancellationtoken) => this.depinfomationAsync(new
        {
            sector = nameof(treecode),
            sectorvalue = treecode.ToStringOrEmpty()
        }, cancellationtoken);
        public async Task<int[]> DepUstDepsAsync(bool isonlyparents, string treecode, CancellationToken cancellationtoken)
        {
            treecode = treecode.ToStringOrEmpty();
            if (treecode == "") { return Array.Empty<int>(); }
            return (await dap.QueryDynamicAsync("[dbo].[sp_wcf_depustdeps]", new
            {
                isonlyparents,
                treecode
            }, null, CommandType.StoredProcedure, cancellationtoken)).Select(x => (int)x.depid).ToArray();
        }
        public async Task<PersonelInformationBasic[]> PersonelSuperAdminAsync_getall(CancellationToken cancellationtoken) => (await dap.QueryAsync<PersonelInformationBasic>("SELECT * FROM [dbo].[vw_wcf_personelsaall]", null, null, CommandType.Text, cancellationtoken)).ToArray();
        public Task<PersonelInformation[]> PersonelInformationAsync_getall(CancellationToken cancellationtoken, Portal_DurumTypes? durum = Portal_DurumTypes.aktif) => this.personelinfomationAllAsync(new
        {
            portalids = "_all",
            durum = this.setDurum(durum)
        }, cancellationtoken);
        public Task<PersonelInformation[]> PersonelInformationAsync_getallportalids(CancellationToken cancellationtoken, params int[] portalids)
        {
            portalids = (portalids ?? Array.Empty<int>()).Distinct().OrderBy(x => x).ToArray();
            if (portalids.Length == 0) { return Task.FromResult(Array.Empty<PersonelInformation>()); }
            return this.personelinfomationAllAsync(new
            {
                portalids = String.Join(",", portalids),
                durum = this.setDurum(null)
            }, cancellationtoken);
        }
        public Task<PersonelInformation?> PersonelInformationAsync_getbyportalid(int portalid, CancellationToken cancellationtoken) => this.personelinformationAsync(new
        {
            sector = nameof(portalid),
            sectorvalue = (portalid > 0 ? portalid : 0).ToString()
        }, cancellationtoken);
        public Task<PersonelInformation?> PersonelInformationAsync_getbykuladi(string kuladi, CancellationToken cancellationtoken) => this.personelinformationAsync(new
        {
            sector = nameof(kuladi),
            sectorvalue = kuladi.ToStringOrEmpty().ToLower()
        }, cancellationtoken);
        public Task<PersonelInformation?> PersonelInformationAsync_getbytckn(long tckn, CancellationToken cancellationtoken) => this.personelinformationAsync(new
        {
            sector = nameof(tckn),
            sectorvalue = (tckn > 0 ? tckn : 0).ToString()
        }, cancellationtoken);
        public Task<PersonelInformation?> PersonelInformationAsync_getbysicilno(string sicilno, CancellationToken cancellationtoken) => this.personelinformationAsync(new
        {
            sector = nameof(sicilno),
            sectorvalue = sicilno.ToStringOrEmpty().ToUpper()
        }, cancellationtoken);
        public Task<PersonelInformation?> PersonelInformationAsync_getbyseo(string seo, CancellationToken cancellationtoken) => this.personelinformationAsync(new
        {
            sector = nameof(seo),
            sectorvalue = seo.ToStringOrEmpty()
        }, cancellationtoken);
        public Task<bool> HasValueAsync_byportalid(int portalid, CancellationToken cancellationtoken) => this.personelhasvalueAsync(new
        {
            sector = nameof(portalid),
            sectorvalue = (portalid > 0 ? portalid : 0).ToString()
        }, cancellationtoken);
        public Task<bool> HasValueAsync_bykuladi(string kuladi, CancellationToken cancellationtoken) => this.personelhasvalueAsync(new
        {
            sector = nameof(kuladi),
            sectorvalue = kuladi.ToStringOrEmpty().ToLower()
        }, cancellationtoken);
        public Task<bool> HasValueAsync_bytckn(long tckn, CancellationToken cancellationtoken) => this.personelhasvalueAsync(new
        {
            sector = nameof(tckn),
            sectorvalue = (tckn > 0 ? tckn : 0).ToStringOrEmpty()
        }, cancellationtoken);
        public Task<bool> HasValueAsync_bysicilno(string sicilno, CancellationToken cancellationtoken) => this.personelhasvalueAsync(new
        {
            sector = nameof(sicilno),
            sectorvalue = sicilno.ToStringOrEmpty().ToUpper()
        }, cancellationtoken);
        public Task<bool> HasValueAsync_byseo(string seo, CancellationToken cancellationtoken) => this.personelhasvalueAsync(new
        {
            sector = nameof(seo),
            sectorvalue = seo.ToStringOrEmpty()
        }, cancellationtoken);
        public Task<UnvanInformation[]> UnvanInformationAsync_getall(CancellationToken cancellationtoken) => this.unvaninformationAsync("_all", cancellationtoken);
        public Task<UnvanInformation[]> UnvanInformationAsync_getallunvanids(CancellationToken cancellationtoken, params string[] unvanids)
        {
            unvanids = (unvanids ?? Array.Empty<string>()).Distinct().OrderBy(x => x).ToArray();
            if (unvanids.Length == 0) { return Task.FromResult(Array.Empty<UnvanInformation>()); }
            return this.unvaninformationAsync(String.Join(",", unvanids), cancellationtoken);
        }
        public async Task<UnvanInformation?> UnvanInformationAsync_byunvanid(string unvanid, CancellationToken cancellationtoken)
        {
            var _t = await this.unvaninformationAsync(unvanid, cancellationtoken);
            if (_t.IsNullOrEmpty_collection()) { return null; }
            return _t[0];
        }
    }
}