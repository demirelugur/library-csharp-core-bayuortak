namespace BayuOrtak.Core.Wcf.Nvi
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Interface;
    using BayuOrtak.Core.Wcf.Nvi.Enums;
    using System;
    using System.ServiceModel;
    using System.Text.RegularExpressions;
    using Wcf_Nvi_kpspublicv2;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public interface INVIHelperTR : IConnectionStatus
    {
        KPSPublicV2SoapClient client { get; }
        Task<bool> KisiVeCuzdanDogrulaAsync(Nvi_KimlikTypes tip, long tckn, string ad, string soyad, DateOnly dogumtarih, string cuzdanserino);
    }
    public sealed class NVIHelperTR : INVIHelperTR, IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }
        public NVIHelperTR() { }
        private KPSPublicV2SoapClient _Client;
        public KPSPublicV2SoapClient client
        {
            get
            {
                if (_Client == null)
                {
                    _Client = new KPSPublicV2SoapClient(new BasicHttpBinding(BasicHttpSecurityMode.Transport)
                    {
                        MaxReceivedMessageSize = 104857600
                    }, new EndpointAddress("https://tckimlik.nvi.gov.tr/Service/KPSPublicV2.asmx?wsdl"));
                }
                return _Client;
            }
        }
        private async Task<bool> yenikimlikAsync(long tckn, string ad, string soyad, DateOnly dogumtarih, string yenitckkserino) => (await this.client.KisiVeCuzdanDogrulaAsync(tckn, ad.ToUpper(), soyad.ToUpper(), true, dogumtarih.Day, true, dogumtarih.Month, true, dogumtarih.Year, null, null, yenitckkserino.ToUpper())).Body.KisiVeCuzdanDogrulaResult;
        private async Task<bool> eskikimlikAsync(long tckn, string ad, string soyad, DateOnly dogumtarih, string eskicuzdanseri, int eskicuzdanno) => (await this.client.KisiVeCuzdanDogrulaAsync(tckn, ad.ToUpper(), soyad.ToUpper(), true, dogumtarih.Day, true, dogumtarih.Month, true, dogumtarih.Year, eskicuzdanseri.ToUpper(), eskicuzdanno, null)).Body.KisiVeCuzdanDogrulaResult;
        public async Task<(bool statuswarning, string error)> IsConnectionStatusAsync(TimeSpan timeout, string dil, CancellationToken cancellationtoken)
        {
            var _t = await this.client.Endpoint.Address.Uri.IsConnectionStatusAsync(timeout, cancellationtoken);
            return (_t.statuswarning, _t.statuswarning ? webservice_connectionwarning(dil, "NVI KPSPublicV2") : "");
        }
        public Task<bool> KisiVeCuzdanDogrulaAsync(Nvi_KimlikTypes tip, long tckn, string ad, string soyad, DateOnly dogumtarih, string cuzdanserino)
        {
            Guard.CheckZeroOrNegative(tckn, nameof(tckn));
            Guard.CheckEmpty(ad, nameof(ad));
            Guard.CheckEmpty(soyad, nameof(soyad));
            Guard.CheckEmpty(cuzdanserino, nameof(cuzdanserino));
            if (tip == Nvi_KimlikTypes.yeni) { return this.yenikimlikAsync(tckn, ad, soyad, dogumtarih, cuzdanserino); }
            if (tip == Nvi_KimlikTypes.eski) { return (TryValidateEskiNufusCuzdaniSeriNo(cuzdanserino, out string _serino, out int _no) ? this.eskikimlikAsync(tckn, ad, soyad, dogumtarih, _serino, _no) : Task.FromResult(false)); }
            throw _other.ThrowNotSupportedForEnum<Nvi_KimlikTypes>();
        }
        public static bool IsValidate(ref string value, Nvi_KimlikTypes tip)
        {
            value = value.ToStringOrEmpty().Replace(" ", "").Replace("-", "").ToUpper();
            if (value.Length == _maximumlength.cuzdanserino)
            {
                if (tip == Nvi_KimlikTypes.yeni && Regex.IsMatch(value, @"^[A-Z][0-9]{2}[A-Z][0-9]{5}$")) { return true; } //A12I34567
                if (tip == Nvi_KimlikTypes.eski && Regex.IsMatch(value, @"^[A-Z][0-9]{8}$")) { return true; } // A12345678
            }
            return false;
        }
        public static bool TryValidateEskiNufusCuzdaniSeriNo(string value, out string serino, out int no)
        {
            if (IsValidate(ref value, Nvi_KimlikTypes.eski))
            {
                serino = value.Substring(0, 3);
                no = Convert.ToInt32(value.Substring(3, 6));
                return true;
            }
            else
            {
                serino = "";
                no = 0;
                return false;
            }
        }
    }
}