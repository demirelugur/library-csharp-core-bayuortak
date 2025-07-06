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
        Task<bool> KisiVeCuzdanDogrula_YeniKimlikAsync(long tckn, string ad, string soyad, DateOnly dogumTarih, string yeniTckkSeriNo);
        Task<bool> KisiVeCuzdanDogrula_EskiKimlikAsync(long tckn, string ad, string soyad, DateOnly dogumTarih, string eskiCuzdanSeri, int eskiCuzdanNo);
        Task<bool> KontrolAsync(long tckn, string ad, string soyad, DateOnly dogumTarih, NVIKimlikTypes tip, string yeniTckkSeriNo, string eskiCuzdanSeri, int eskiCuzdanNo);
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
        public async Task<bool> IsConnectionStatusAsync(TimeSpan timeout, CancellationToken cancellationToken = default) => !(await this.client.Endpoint.Address.Uri.IsConnectionStatusAsync(timeout, cancellationToken)).statuswarning;
        public async Task<bool> KisiVeCuzdanDogrula_YeniKimlikAsync(long tckn, string ad, string soyad, DateOnly dogumTarih, string yeniTckkSeriNo) => (await this.client.KisiVeCuzdanDogrulaAsync(tckn, ad.ToUpper(), soyad.ToUpper(), true, dogumTarih.Day, true, dogumTarih.Month, true, dogumTarih.Year, null, null, yeniTckkSeriNo.ToUpper())).Body.KisiVeCuzdanDogrulaResult;
        public async Task<bool> KisiVeCuzdanDogrula_EskiKimlikAsync(long tckn, string ad, string soyad, DateOnly dogumTarih, string eskiCuzdanSeri, int eskiCuzdanNo) => (await this.client.KisiVeCuzdanDogrulaAsync(tckn, ad.ToUpper(), soyad.ToUpper(), true, dogumTarih.Day, true, dogumTarih.Month, true, dogumTarih.Year, eskiCuzdanSeri.ToUpper(), eskiCuzdanNo, null)).Body.KisiVeCuzdanDogrulaResult;
        public Task<bool> KontrolAsync(long tckn, string ad, string soyad, DateOnly dogumTarih, NVIKimlikTypes tip, string yeniTckkSeriNo, string eskiCuzdanSeri, int eskiCuzdanNo)
        {
            Guard.CheckZeroOrNegative(tckn, nameof(tckn));
            Guard.CheckEmpty(ad, nameof(ad));
            Guard.CheckEmpty(soyad, nameof(soyad));
            if (tip == NVIKimlikTypes.yeni)
            {
                Guard.CheckEmpty(yeniTckkSeriNo, nameof(yeniTckkSeriNo));
                return this.KisiVeCuzdanDogrula_YeniKimlikAsync(tckn, ad, soyad, dogumTarih, yeniTckkSeriNo);
            }
            if (tip == NVIKimlikTypes.eski)
            {
                Guard.CheckEmpty(eskiCuzdanSeri, nameof(eskiCuzdanSeri));
                Guard.CheckZeroOrNegative(eskiCuzdanNo, nameof(eskiCuzdanNo));
                return this.KisiVeCuzdanDogrula_EskiKimlikAsync(tckn, ad, soyad, dogumTarih, eskiCuzdanSeri, eskiCuzdanNo);
            }
            throw _other.ThrowNotSupportedForEnum<NVIKimlikTypes>();
        }
        public static bool IsValidate(string value, NVIKimlikTypes tip)
        {
            if (tip == NVIKimlikTypes.yeni) { return TryValidate_YeniKimlikSeriNo(value, out _); }
            if (tip == NVIKimlikTypes.eski) { return TryValidate_EskiNufusCuzdaniSeriNo(value, out _, out _); }
            throw _other.ThrowNotSupportedForEnum<NVIKimlikTypes>();
        }
        public static bool TryValidate_YeniKimlikSeriNo(string value, out string outvalue)
        {
            outvalue = "";
            value = value.ToStringOrEmpty().Replace(" ", "").Replace("-", "").ToUpper();
            if (value.Length == _maximumlength.cuzdanserino && Regex.IsMatch(value, @"^[A-Z][0-9]{2}[A-Z][0-9]{5}$"))
            {
                outvalue = value;
                return true;
            }
            return false;
        }
        public static bool TryValidate_EskiNufusCuzdaniSeriNo(string value, out string serino, out int no)
        {
            serino = "";
            no = 0;
            value = value.ToStringOrEmpty().Replace(" ", "").Replace("-", "").ToUpper();
            if (value.Length == _maximumlength.cuzdanserino && Regex.IsMatch(value, @"^[A-Z][0-9]{8}$"))
            {
                serino = value.Substring(0, 3);
                no = Convert.ToInt32(value.Substring(3, 6));
                return true;
            }
            return false;
        }
    }
}