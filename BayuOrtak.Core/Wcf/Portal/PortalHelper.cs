namespace BayuOrtak.Core.Wcf.Portal
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Wcf.Portal.Dto;
    using Microsoft.Data.SqlClient;
    using System.Data;
    using System.Threading.Tasks;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    public interface IPortalHelper // TODO: Yeni Web Sitesinden sonra yapılandırılmalı!
    {
        Task<PersonelInformation[]> PersonelinformationAllAsync(params int[] portalids);
        Task<PersonelInformation> PersonelinformationAsync_byportalid(int portalid);
        Task<PersonelInformation> PersonelinformationAsync_bykuladi(string kuladi);
        Task<PersonelInformation> PersonelinformationAsync_bytckn(long tckn);
        Task<PersonelInformation> PersonelinformationAsync_bysicilno(string sicilno);
        Task<bool> HasValueAsync_byportalid(int portalid);
        Task<bool> HasValueAsync_bykuladi(string kuladi);
        Task<bool> HasValueAsync_bytckn(long tckn);
        Task<bool> HasValueAsync_bysicilno(string sicilno);
        Task<DepInformation[]> DepinformationAllAsync(bool? isaktif = true);
        Task<DepInformation> DepinformationAsync_bydepid(int depid);
        Task<int[]> DepustdepsAsync(string code);
        Task<(string tx, int vl, bool status)[]> DepinformationAsync_fakbolabd(int? birimid, int? codelength);
        Task<UnvanInformation[]> UnvaninformationAsync();
        Task<UnvanInformation> UnvaninformationAsync_byunvanid(string unvanid);
    }
    public sealed class PortalHelper : IPortalHelper
    {
        private readonly DapperHelper _dap;
        public PortalHelper(string connectionstring)
        {
            this._dap = new DapperHelper(new SqlConnection(connectionstring), null);
        }
        #region Private
        private async Task<DepInformation[]> depinfomationAsync(object parameters)
        {
            var _multi = await _dap.QueryMultipleAsync("[dbo].[sp_wcf_depinformation]", parameters, null, CommandType.StoredProcedure);
            var deps = (await _multi.ReadAsync<DepInformation>()).ToArray();
            var names = (await _multi.ReadAsync<DepNameInformation>()).ToArray();
            foreach (var item in deps) { item.names = names.Where(x => x.depid == item.id).OrderBy(x => x.dil).ToArray(); }
            names.DisposeAll();
            return deps;
        }
        private async Task<bool> hasvalueAsync(object parameters) => await _dap.ExecuteScalarAsync<bool>("[dbo].[sp_wcf_personelhasvalue]", parameters, null, CommandType.StoredProcedure);
        private async Task<PersonelInformation> getpersonelinformationAsync(object parameters)
        {
            var _multi = await _dap.QueryMultipleAsync("[dbo].[sp_wcf_personelinformation]", parameters, null, CommandType.StoredProcedure);
            var p = await _multi.ReadFirstOrDefaultAsync<PersonelInformation>();
            p.nhrinfo = await _multi.ReadFirstOrDefaultAsync<NhrInformation>();
            p.yokinfo = await _multi.ReadFirstOrDefaultAsync<YokInformation>();
            return p;
        }
        private async Task<UnvanInformation[]> unvaninformationAsync(string unvanid)
        {
            var _multi = await _dap.QueryMultipleAsync("[dbo].[sp_wcf_unvaninformation]", new
            {
                unvanid
            }, null, CommandType.StoredProcedure);
            var u = (await _multi.ReadAsync<UnvanInformation>()).ToArray();
            var names = (await _multi.ReadAsync<UnvanNameInformation>()).ToArray();
            foreach (var item in u) { item.names = names.Where(x => x.unvanid == item.unvanid).OrderBy(x => x.dil).ToArray(); }
            names.DisposeAll();
            return u;
        }
        #endregion
        #region Personel
        public async Task<PersonelInformation[]> PersonelinformationAllAsync(params int[] portalids)
        {
            if (portalids.IsNullOrEmpty_collection()) { return Array.Empty<PersonelInformation>(); }
            var _multi = await _dap.QueryMultipleAsync("[dbo].[sp_wcf_personelinformationall]", new
            {
                portalids = String.Join(";", portalids)
            }, null, CommandType.StoredProcedure);
            var p = (await _multi.ReadAsync<PersonelInformation>()).ToArray();
            var nhrinfoes = (await _multi.ReadAsync<NhrInformation>()).ToArray();
            var yokinfoes = (await _multi.ReadAsync<YokInformation>()).ToArray();
            foreach (var item in p)
            {
                item.nhrinfo = nhrinfoes.Where(x => x.uyeid == item.id).FirstOrDefault();
                item.yokinfo = yokinfoes.Where(x => x.uyeid == item.id).FirstOrDefault();
            }
            nhrinfoes.DisposeAll();
            yokinfoes.DisposeAll();
            return p;
        }
        public Task<PersonelInformation> PersonelinformationAsync_byportalid(int portalid) => getpersonelinformationAsync(new
        {
            sector = nameof(portalid),
            sectorvalue = (portalid > 0 ? portalid : 0).ToString()
        });
        public Task<PersonelInformation> PersonelinformationAsync_bykuladi(string kuladi) => getpersonelinformationAsync(new
        {
            sector = nameof(kuladi),
            sectorvalue = kuladi
        });
        public Task<PersonelInformation> PersonelinformationAsync_bytckn(long tckn) => getpersonelinformationAsync(new
        {
            sector = nameof(tckn),
            sectorvalue = (tckn > 0 ? tckn : 0).ToString()
        });
        public Task<PersonelInformation> PersonelinformationAsync_bysicilno(string sicilno) => getpersonelinformationAsync(new
        {
            sector = nameof(sicilno),
            sectorvalue = sicilno
        });
        public Task<bool> HasValueAsync_byportalid(int portalid) => hasvalueAsync(new
        {
            sector = nameof(portalid),
            sectorvalue = (portalid > 0 ? portalid : 0).ToString()
        });
        public Task<bool> HasValueAsync_bykuladi(string kuladi) => hasvalueAsync(new
        {
            sector = nameof(kuladi),
            sectorvalue = kuladi
        });
        public Task<bool> HasValueAsync_bytckn(long tckn) => hasvalueAsync(new
        {
            sector = nameof(tckn),
            sectorvalue = (tckn > 0 ? tckn : 0).ToString()
        });
        public Task<bool> HasValueAsync_bysicilno(string sicilno) => hasvalueAsync(new
        {
            sector = nameof(sicilno),
            sectorvalue = sicilno.ToString()
        });
        #endregion
        #region Birim
        public Task<DepInformation[]> DepinformationAllAsync(bool? isaktif = true) => depinfomationAsync(new
        {
            depid = -1,
            durum = (isaktif.HasValue ? (isaktif.Value ? "aktif" : "pasif") : "tumu")
        });
        public async Task<DepInformation> DepinformationAsync_bydepid(int depid) => (await depinfomationAsync(new
        {
            depid = (depid > 0 ? depid : 0),
            durum = ""
        })).FirstOrDefault();
        public async Task<int[]> DepustdepsAsync(string code)
        {
            if (code.IsNullOrEmpty_string()) { return Array.Empty<int>(); }
            return (await _dap.QueryDynamicAsync("[dbo].[sp_wcf_depustdeps]", new
            {
                code
            }, null, CommandType.StoredProcedure)).Select(x => (int)x.depid).ToArray();
        }
        public async Task<(string tx, int vl, bool status)[]> DepinformationAsync_fakbolabd(int? birimid, int? codelength)
        {
            var _data = (await this.DepinformationAllAsync()).Select(x => new
            {
                x.ustdepid,
                x.codelength,
                x.isuygarsmrkz,
                tx = x.names.Where(y => y.dilcode == "tr").Select(y => y.adiuzun).FirstOrDefault(),
                vl = x.id,
                status = !x.bitdate.HasValue
            });
            if (birimid.HasValue)
            {
                if (birimid.Value == 0 || !codelength.HasValue) { return Array.Empty<(string tx, int vl, bool status)>(); }
                Guard.CheckNotIncludes(nameof(codelength), codelength.Value, _portal.codelength_birim, _portal.codelength_abd);
                return _data.Where(x => x.ustdepid == birimid && x.codelength == codelength.Value).OrderBy(x => x.tx).Select(x => (x.tx, x.vl, x.status)).ToArray();
            }
            return _data.Where(x => x.codelength == _portal.codelength_fakulte && !x.isuygarsmrkz).OrderBy(x => x.tx).Select(x => (x.tx, x.vl, x.status)).ToArray();
        }
        #endregion
        #region Unvan
        public Task<UnvanInformation[]> UnvaninformationAsync() => unvaninformationAsync("");
        public async Task<UnvanInformation> UnvaninformationAsync_byunvanid(string unvanid)
        {
            if (unvanid.IsNullOrEmpty_string()) { return new UnvanInformation(); }
            return (await unvaninformationAsync(unvanid)).FirstOrDefault();
        }
        #endregion
    }
}