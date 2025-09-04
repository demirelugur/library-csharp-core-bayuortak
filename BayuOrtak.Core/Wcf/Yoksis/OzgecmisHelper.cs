namespace BayuOrtak.Core.Wcf.Yoksis
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Interface;
    using BayuOrtak.Core.Wcf.Yoksis.Helper;
    using System;
    using System.Linq;
    using System.ServiceModel;
    using Wcf_Yoksis_OzgecmisV2;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    public interface IOzgecmisHelper : IConnectionStatus
    {
        OzgecmisV1PortClient client { get; }
        Task<getirOgrenimBilgisiListesiResponse> Get_OgrenimBilgisiListesiAsync(long tckn);
        Task<getirAkademikGorevListesiResponse> Get_AkademikGorevListesiAsync(long tckn);
        Task<getKitapBilgisiV1Response> Get_KitapBilgisiV1Async(long tckn);
        Task<getKitapBilgisiDetayV1Response> Get_KitapBilgisiDetayV1Async(long tckn, string eserid);
        Task<getMakaleBilgisiV1Response> Get_MakaleBilgisiV1Async(long tckn);
        Task<getMakaleBilgisiDetayV1Response> Get_MakaleBilgisiDetayV1Async(long tckn, string eserid);
        Task<getBildiriBilgisiV1Response> Get_BildiriBilgisiV1Async(long tckn);
        Task<getBildiriBilgisiDetayV1Response> Get_BildiriBilgisiDetayV1Async(long tckn, string eserid);
        Task<getirProjeListesiResponse> Get_ProjeListesiAsync(long tckn);
        Task<getirProjeListesiDetayResponse> Get_ProjeListesiDetayAsync(long tckn, string eserid);
        Task<getirProjeEkipListesiResponse> Get_ProjeEkipListesiAsync(long projeid, long formid);
        Task<getirDersListesiResponse> Get_DersListesiAsync(long tckn);
        Task<getirTezDanismanListesiResponse> Get_TezDanismanListesiAsync(long tckn);
        Task<getOdulListesiV1Response> Get_OdulListesiV1Async(long tckn);
        Task<getPatentBilgisiV1Response> Get_PatentBilgisiV1Async(long tckn);
        Task<getPatentBilgisiDetayV1Response> Get_PatentBilgisiDetayV1Async(long tckn, string eserid);
        Task<getirUyelikListesiResponse> Get_UyelikListesiAsync(long tckn);
        Task<getSanatsalFaalV1Response> Get_SanatsalFaalV1Async(long tckn);
        Task<getirIdariGorevListesiResponse> Get_IdariGorevListesiAsync(long tckn);
        Task<getirUnvDisiDeneyimListesiResponse> Get_UnvDisiDeneyimListesiAsync(long tckn);
        Task<getEditorlukBilgisiV1Response> Get_EditorlukBilgisiV1Async(long tckn);
        Task<getirYabanciDilListesiResponse> Get_YabanciDilListesiAsync(long tckn);
        Task<getArastirmaSertifkaBilgisiV1Response> Get_ArastirmaSertifkaBilgisiV1Async(long tckn);
        Task<getTasarimBilgisiV1Response> Get_TasarimBilgisiV1Async(long tckn);
        Task<getPersonelLinkV1Response> Get_PersonelLinkV1Async(long tckn);
        Task<getTemelAlanBilgisiV1Response> Get_TemelAlanBilgisiV1Async(long tckn);
        Task<OzgecmisPersonelInfoResult> Get_PersonelInfoAsync(long tckn);
    }
    public sealed class OzgecmisHelper : IOzgecmisHelper, IDisposable
    {
        #region Privates
        private readonly string password, userpassword;
        private readonly long usertckn;
        private OzgecmisV1PortClient _Client;
        private parametre createParametre(long tckn) => new parametre
        {
            P_KULLANICI_ID = this.usertckn.ToString(),
            P_SIFRE = this.userpassword,
            P_TC_KIMLIK_NO = tckn
        };
        private parametreDetay createParametreDetay(long tckn, string eserid) => new parametreDetay
        {
            P_KULLANICI_ID = this.usertckn.ToString(),
            P_SIFRE = this.userpassword,
            P_TC_KIMLIK_NO = tckn,
            P_ESER_ID = eserid
        };
        #endregion
        public void Dispose() { GC.SuppressFinalize(this); }
        public OzgecmisHelper(string password, long usertckn, string userpassword)
        {
            this.password = password;
            this.usertckn = usertckn;
            this.userpassword = userpassword;
        }
        public OzgecmisV1PortClient client
        {
            get
            {
                if (_Client == null)
                {
                    _Client = new OzgecmisV1PortClient(YoksisTools.basichttpbinding, new EndpointAddress("http://servisler.yok.gov.tr/ws/OzgecmisV2?wsdl")); // Not: EndpointAddress uri yolu http ile başlamalıdır!
                    _Client.ClientCredentials.UserName.UserName = yoksis_UNI_code.ToString();
                    _Client.ClientCredentials.UserName.Password = this.password;
                }
                return _Client;
            }
        }
        public async Task<(bool statuswarning, string error)> IsConnectionStatusAsync(TimeSpan timeout, string dil, CancellationToken cancellationtoken)
        {
            var _t = await this.client.Endpoint.Address.Uri.IsConnectionStatusAsync(timeout, cancellationtoken);
            return (_t.statuswarning, _t.statuswarning ? GlobalConstants.webservice_connectionwarning(dil, "YÖKSİS, OzgecmisV2") : "");
        }
        public async Task<getirOgrenimBilgisiListesiResponse> Get_OgrenimBilgisiListesiAsync(long tckn) => (await this.client.getirOgrenimBilgisiListesiAsync(new getirOgrenimBilgisiListesiRequestType()
        {
            parametre = createParametre(tckn)
        })).getirOgrenimBilgisiListesiResponse;
        public async Task<getirAkademikGorevListesiResponse> Get_AkademikGorevListesiAsync(long tckn) => (await this.client.getirAkademikGorevListesiAsync(new getirAkademikGorevListesiRequestType()
        {
            parametre = createParametre(tckn)
        })).getirAkademikGorevListesiResponse;
        public async Task<getKitapBilgisiV1Response> Get_KitapBilgisiV1Async(long tckn) => (await this.client.getKitapBilgisiV1Async(new getKitapBilgisiV1RequestType
        {
            parametre = createParametre(tckn)
        })).getKitapBilgisiV1Response;
        public async Task<getKitapBilgisiDetayV1Response> Get_KitapBilgisiDetayV1Async(long tckn, string eserid) => (await this.client.getKitapBilgisiDetayV1Async(new getKitapBilgisiDetayV1RequestType
        {
            parametre = createParametreDetay(tckn, eserid)
        })).getKitapBilgisiDetayV1Response;
        public async Task<getMakaleBilgisiV1Response> Get_MakaleBilgisiV1Async(long tckn) => (await this.client.getMakaleBilgisiV1Async(new getMakaleBilgisiV1RequestType
        {
            parametre = createParametre(tckn)
        })).getMakaleBilgisiV1Response;
        public async Task<getMakaleBilgisiDetayV1Response> Get_MakaleBilgisiDetayV1Async(long tckn, string eserid) => (await this.client.getMakaleBilgisiDetayV1Async(new getMakaleBilgisiDetayV1RequestType
        {
            parametre = createParametreDetay(tckn, eserid)
        })).getMakaleBilgisiDetayV1Response;
        public async Task<getBildiriBilgisiV1Response> Get_BildiriBilgisiV1Async(long tckn) => (await this.client.getBildiriBilgisiV1Async(new getBildiriBilgisiV1RequestType
        {
            parametre = createParametre(tckn)
        })).getBildiriBilgisiV1Response;
        public async Task<getBildiriBilgisiDetayV1Response> Get_BildiriBilgisiDetayV1Async(long tckn, string eserid) => (await this.client.getBildiriBilgisiDetayV1Async(new getBildiriBilgisiDetayV1RequestType
        {
            parametre = createParametreDetay(tckn, eserid)
        })).getBildiriBilgisiDetayV1Response;
        public async Task<getirProjeListesiResponse> Get_ProjeListesiAsync(long tckn) => (await this.client.getirProjeListesiAsync(new getirProjeListesiRequestType
        {
            parametre = createParametre(tckn)
        })).getirProjeListesiResponse;
        public async Task<getirProjeListesiDetayResponse> Get_ProjeListesiDetayAsync(long tckn, string eserid) => (await this.client.getirProjeListesiDetayAsync(new getirProjeListesiDetayRequestType
        {
            parametre = createParametreDetay(tckn, eserid)
        })).getirProjeListesiDetayResponse;
        public async Task<getirProjeEkipListesiResponse> Get_ProjeEkipListesiAsync(long projeid, long formid) => (await this.client.getirProjeEkipListesiAsync(new getirProjeEkipListesiRequestType
        {
            parametre = new parametreEkip
            {
                P_KULLANICI_ID = this.usertckn.ToString(),
                P_SIFRE = this.userpassword,
                P_PROJE_ID = projeid,
                P_PROJE_FORM_ID = formid
            }
        })).getirProjeEkipListesiResponse;
        public async Task<getirDersListesiResponse> Get_DersListesiAsync(long tckn) => (await this.client.getirDersListesiAsync(new getirDersListesiRequestType
        {
            parametre = createParametre(tckn)
        })).getirDersListesiResponse;
        public async Task<getirTezDanismanListesiResponse> Get_TezDanismanListesiAsync(long tckn) => (await this.client.getirTezDanismanListesiAsync(new getirTezDanismanListesiRequestType
        {
            parametre = createParametre(tckn)
        })).getirTezDanismanListesiResponse;
        public async Task<getOdulListesiV1Response> Get_OdulListesiV1Async(long tckn) => (await this.client.getOdulListesiV1Async(new getOdulListesiV1RequestType
        {
            parametre = createParametre(tckn)
        })).getOdulListesiV1Response;
        public async Task<getPatentBilgisiV1Response> Get_PatentBilgisiV1Async(long tckn) => (await this.client.getPatentBilgisiV1Async(new getPatentBilgisiV1RequestType
        {
            parametre = createParametre(tckn)
        })).getPatentBilgisiV1Response;
        public async Task<getPatentBilgisiDetayV1Response> Get_PatentBilgisiDetayV1Async(long tckn, string eserid) => (await this.client.getPatentBilgisiDetayV1Async(new getPatentBilgisiDetayV1RequestType
        {
            parametre = createParametreDetay(tckn, eserid)
        })).getPatentBilgisiDetayV1Response;
        public async Task<getirUyelikListesiResponse> Get_UyelikListesiAsync(long tckn) => (await this.client.getirUyelikListesiAsync(new getirUyelikListesiRequestType
        {
            parametre = createParametre(tckn)
        })).getirUyelikListesiResponse;
        public async Task<getSanatsalFaalV1Response> Get_SanatsalFaalV1Async(long tckn) => (await this.client.getSanatsalFaalV1Async(new getSanatsalFaalV1RequestType
        {
            parametre = createParametre(tckn)
        })).getSanatsalFaalV1Response;
        public async Task<getirIdariGorevListesiResponse> Get_IdariGorevListesiAsync(long tckn) => (await this.client.getirIdariGorevListesiAsync(new getirIdariGorevListesiRequestType
        {
            parametre = createParametre(tckn)
        })).getirIdariGorevListesiResponse;
        public async Task<getirUnvDisiDeneyimListesiResponse> Get_UnvDisiDeneyimListesiAsync(long tckn) => (await this.client.getirUnvDisiDeneyimListesiAsync(new getirUnvDisiDeneyimListesiRequestType
        {
            parametre = createParametre(tckn)
        })).getirUnvDisiDeneyimListesiResponse;
        public async Task<getEditorlukBilgisiV1Response> Get_EditorlukBilgisiV1Async(long tckn) => (await this.client.getEditorlukBilgisiV1Async(new getEditorlukBilgisiV1RequestType
        {
            parametre = createParametre(tckn)
        })).getEditorlukBilgisiV1Response;
        public async Task<getirYabanciDilListesiResponse> Get_YabanciDilListesiAsync(long tckn) => (await this.client.getirYabanciDilListesiAsync(new getirYabanciDilListesiRequestType
        {
            parametre = createParametre(tckn)
        })).getirYabanciDilListesiResponse;
        public async Task<getArastirmaSertifkaBilgisiV1Response> Get_ArastirmaSertifkaBilgisiV1Async(long tckn) => (await this.client.getArastirmaSertifkaBilgisiV1Async(new getArastirmaSertifkaBilgisiV1RequestType
        {
            parametre = createParametre(tckn)
        })).getArastirmaSertifkaBilgisiV1Response;
        public async Task<getTasarimBilgisiV1Response> Get_TasarimBilgisiV1Async(long tckn) => (await this.client.getTasarimBilgisiV1Async(new getTasarimBilgisiV1RequestType
        {
            parametre = createParametre(tckn)
        })).getTasarimBilgisiV1Response;
        public async Task<getPersonelLinkV1Response> Get_PersonelLinkV1Async(long tckn) => (await this.client.getPersonelLinkV1Async(new getPersonelLinkV1RequestType
        {
            parametre = createParametre(tckn)
        })).getPersonelLinkV1Response;
        public async Task<getTemelAlanBilgisiV1Response> Get_TemelAlanBilgisiV1Async(long tckn) => (await this.client.getTemelAlanBilgisiV1Async(new getTemelAlanBilgisiV1RequestType
        {
            parametre = createParametre(tckn)
        })).getTemelAlanBilgisiV1Response;
        public async Task<OzgecmisPersonelInfoResult> Get_PersonelInfoAsync(long tckn)
        {
            try // <Sonuc><SonucKod>0</SonucKod><SonucMesaj>AKADEMISYEN UNIVERSITENIZ KADROSUNDA BULUNMAMAKTADIR.</SonucMesaj></Sonuc>
            {
                var _iletisim = await Get_PersonelLinkV1Async(tckn);
                var _notbaglanti = (_iletisim == null || (_iletisim.Sonuc.SonucKod == 0 && _iletisim.Sonuc.SonucMesaj.ToSeoFriendly() != "akademisyen-universiteniz-kadrosunda-bulunmamaktadir"));
                docTemelAlan? _temel = null;
                if (!_notbaglanti)
                {
                    var _ta = await Get_TemelAlanBilgisiV1Async(tckn);
                    if (_ta != null && _ta.Sonuc.SonucKod == 1) { _temel = _ta.temelAlanListe.Where(x => x.AKTIF_PASIF == "1").FirstOrDefault(); }
                }
                return new OzgecmisPersonelInfoResult(true, !_notbaglanti, (_notbaglanti ? null : _iletisim.personelLinkListe.FirstOrDefault()), _temel);
            }
            catch { return new OzgecmisPersonelInfoResult(); }
        }
    }
}