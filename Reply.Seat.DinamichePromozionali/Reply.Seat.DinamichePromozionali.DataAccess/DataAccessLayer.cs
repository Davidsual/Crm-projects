using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reply.Seat.DinamichePromozionali.DataAccess.Model;
using System.Linq.Expressions;
using System.Configuration;

namespace Reply.Seat.DinamichePromozionali.DataAccess
{
    /// <summary>
    /// Classe che espone metodi get per la retrieve dei dati
    /// </summary>
    public sealed class DataAccessLayer : IDisposable
    {
        #region PRIVATE MEMBERS
        private static volatile DataAccessLayer instance = null;
        private static readonly object syncRoot = new object();
        /// <summary>
        /// Stato della campagna
        /// </summary>
        private enum StatusCampaign
        {
            Proposed = 0,
            ReadyToLaunch,
            Launched,
            Completed,
            Canceled,
            Suspended
        }
        #endregion

        #region CTOR
        /// <summary>
        /// Costruttore
        /// </summary>
        public DataAccessLayer()
        {
        }
        #endregion

        #region PUBLIC MEMBERS
        /// <summary>
        /// Tipo Target Campagna
        /// </summary>
        public enum CampaignTipoTarget
        {
            T892424 = 6,
            T1240
        }
        /// <summary>
        /// Stato partecipante in chiamnante campagna
        /// </summary>
        public enum StatoPartecipanteInChiamanteCampagna
        {
            PotenzialeVincitore = 1,
            Vincitore,
            RifiutoPrivacy
        }
        /// <summary>
        /// Privacy
        /// </summary>
        public enum Privacy
        {
            Si=1,
            No,
            NonSo
        }
        /// <summary>
        /// Template PRemio
        /// </summary>
        public enum TemplatePremioModalitaComunicazionePremio
        {
            Manuale = 1,
            Automatica
            
        }
        /// <summary>
        /// Tipologia di premio in chiamante campagna
        /// </summary>
        public enum TemplatePremioTipologiaPremio
        {
            Chiamate = 1,
            Codice,
            Oggetto,
            Concorso
        }
        /// <summary>
        /// Su template premio il tipo di telefono ammesso
        /// </summary>
        public enum TemplatePremioTipoTelefono
        {
            Fisso = 1,
            Mobile,
            Tutti
        }
        /// <summary>
        /// Data attivazione premio
        /// </summary>
        public enum TemplatePremioDataAttivazione
        {
            Immediata = 1,
            DataFutura 
        }
        /// <summary>
        /// Stato invio SMS
        /// </summary>
        public enum SmsStatoInvio
        {
            DaInviare = 1,
            Inviato ,
            Errore 
        }
        #endregion

        #region PUBLIC PROPERTY
        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static DataAccessLayer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new DataAccessLayer();
                    }
                }

                return instance;
            }
        }
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Ottiene l'ultimo codice promozionale usato associato alla campagna
        /// </summary>
        /// <param name="idChiamanteCampagna"></param>
        /// <returns></returns>
        public New_codicepromozionale GetLastCodicePromozionaleUsatoByIdChiamanteCampagna(Guid idChiamanteCampagna)
        {
            using (MarketingSeatDataContext _model = new MarketingSeatDataContext())
            {
                return _model.New_codicepromozionales.Where(item => item.New_Usato == true && item.New_ChiamanteCampagnaId == idChiamanteCampagna && item.DeletionStateCode == 0).OrderByDescending(item => item.ModifiedOn).FirstOrDefault();
            }
        }
        /// <summary>
        /// Ottiene il numero di Sms Inviati dato id del chiamante campagna
        /// </summary>
        /// <param name="idChiamanteCampagna"></param>
        /// <returns></returns>
        public int GetNumSmsInviatiByIdChiamanteCampagna(Guid idChiamanteCampagna)
        {
            using (MarketingSeatDataContext _model = new MarketingSeatDataContext())
            {
                ///Totale degli sms inviati
                return (from New_sm sms in _model.New_sms
                        join New_chiamantecampagna chiamCampagna in _model.New_chiamantecampagnas on sms.New_ChiamanteCampagnaId equals chiamCampagna.New_chiamantecampagnaId
                        where chiamCampagna.New_chiamantecampagnaId == idChiamanteCampagna &&
                        sms.New_StatoInvio == (int)DataAccessLayer.SmsStatoInvio.Inviato &&
                        sms.DeletionStateCode == 0
                        select sms).Count();
            }
        }
        /// <summary>
        /// Ottiene il numero di chiamate gratuti dato id del chiamante campagna
        /// </summary>
        /// <param name="idChiamanteCampagna"></param>
        /// <returns></returns>
        public int GetNumChiamateGratuiteByIdChiamanteCampagna(Guid idChiamanteCampagna)
        {
            using (MarketingSeatDataContext _model = new MarketingSeatDataContext())
            {
                return (from New_callgratuita call in _model.New_callgratuitas
                        join New_chiamantecampagna chiamCampagna in _model.New_chiamantecampagnas on call.New_ChiamanteCampagnaId equals chiamCampagna.New_chiamantecampagnaId
                        where chiamCampagna.New_chiamantecampagnaId == idChiamanteCampagna &&
                        call.DeletionStateCode == 0
                        select call).Count();
            }
        }
        /// <summary>
        /// Ottiene il num di oggetti dato id del chiamante campagna
        /// </summary>
        /// <param name="idChiamanteCampagna"></param>
        /// <returns></returns>
        public int GetNumOggettiByIdChiamanteCampagna(Guid idChiamanteCampagna)
        {
            using (MarketingSeatDataContext _model = new MarketingSeatDataContext())
            {
                return (from New_oggetto oggetto in _model.New_oggettos
                        join New_chiamantecampagna chiamCampagna in _model.New_chiamantecampagnas on oggetto.New_ChiamanteCampagnaId equals chiamCampagna.New_chiamantecampagnaId
                        where chiamCampagna.New_chiamantecampagnaId == idChiamanteCampagna &&
                        oggetto.DeletionStateCode == 0
                        select oggetto).Count();
            }
        }
        /// <summary>
        /// Ottiene il numero di codici promozionali usati dato id chiamante campagna
        /// </summary>
        /// <param name="idChiamanteCampagna"></param>
        /// <returns></returns>
        public int GetNumCodiciPromozionaliUsatiByIdChiamanteCampagna(Guid idChiamanteCampagna)
        {
            using (MarketingSeatDataContext _model = new MarketingSeatDataContext())
            {
                return (from New_codicepromozionale codProm in _model.New_codicepromozionales
                        join New_chiamantecampagna chiamCampagna in _model.New_chiamantecampagnas on codProm.New_ChiamanteCampagnaId equals chiamCampagna.New_chiamantecampagnaId
                        where chiamCampagna.New_chiamantecampagnaId == idChiamanteCampagna &&
                        codProm.New_Usato == true &&
                        codProm.DeletionStateCode == 0
                        select codProm).Count();
            }
        }
        /// <summary>
        /// Ottiene un codice promozionale dato ID Template premio
        /// </summary>
        /// <param name="idTemplatePremio"></param>
        /// <returns></returns>
        public New_codicepromozionale GetCodicePromozionaleAttivoByIdTemplatePremio(Guid idTemplatePremio)
        {
            using (MarketingSeatDataContext _model = new MarketingSeatDataContext())
            {
                _model.DeferredLoadingEnabled = false;
                _model.ObjectTrackingEnabled = false;
                return _model.New_codicepromozionales.Where(cam => cam.New_TemplatePremioId == idTemplatePremio && cam.New_Usato == false).FirstOrDefault();
            }
        }
        /// <summary>
        /// Restituisce la campagna dato il suo ID
        /// </summary>
        /// <param name="idCampaign"></param>
        /// <returns></returns>
        public Campaign GetCampaignById(Guid idCampaign)
        {
            using (MarketingSeatDataContext _model = new MarketingSeatDataContext())
            {
                _model.DeferredLoadingEnabled = false;
                _model.ObjectTrackingEnabled = false;
                return _model.Campaigns.Where(cam => cam.CampaignId == idCampaign).SingleOrDefault();
            }
        }
        /// <summary>
        /// Restituisce la campagna che è attiva e possiede un determinato calltype
        /// </summary>
        /// <returns></returns>
        public Campaign GetCampaignActiveByCallType(string callType)
        {
            using (MarketingSeatDataContext _model = new MarketingSeatDataContext())
            {
                _model.DeferredLoadingEnabled = false;
                _model.ObjectTrackingEnabled = false;
                var query = from Campaign _camp in _model.Campaigns
                            join new_campaign_new_calltype _campaignCallType in _model.new_campaign_new_calltypes on _camp.CampaignId equals _campaignCallType.campaignid
                            join New_calltype _callType in _model.New_calltypes on _campaignCallType.new_calltypeid equals _callType.New_calltypeId
                            where _callType.New_nomecalltype == callType &&
                            _camp.StatusCode == (int)StatusCampaign.Launched &&
                            _camp.DeletionStateCode == 0
                            select _camp;
                //string queryString = query.ToString();
                //Console.WriteLine(queryString);
                return query.SingleOrDefault();
            }
        }
        /// <summary>
        /// Restituisce la campagna data la chiave primaria
        /// </summary>
        /// <returns></returns>
        public New_chiamantecampagna GetChiamanteCampagnaByCryptedCodeInExistingCampaign(string callType, string cryptedCode)
        {
            using (MarketingSeatDataContext _model = new MarketingSeatDataContext())
            {
                _model.DeferredLoadingEnabled = false;
                _model.ObjectTrackingEnabled = false;
                return (from New_chiamantecampagna _chiamante in _model.New_chiamantecampagnas
                        join Campaign _camp in _model.Campaigns on _chiamante.New_CampagnaId equals _camp.CampaignId
                        join new_campaign_new_calltype _campaignCallType in _model.new_campaign_new_calltypes on _camp.CampaignId equals _campaignCallType.campaignid
                        join New_calltype _callType in _model.New_calltypes on _campaignCallType.new_calltypeid equals _callType.New_calltypeId
                        where _callType.New_nomecalltype == callType &&
                        _camp.StatusCode == (int)StatusCampaign.Launched && 
                        _chiamante.New_name == cryptedCode &&
                        _chiamante.DeletionStateCode == 0
                        select _chiamante).SingleOrDefault();
            }
        }
        /// <summary>
        /// Estrae il template premio a partire dall'id campagna
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        public New_premio GetPremioByIdCampaign(Guid campaignId)
        {
            using (MarketingSeatDataContext _model = new MarketingSeatDataContext())
            {
                _model.DeferredLoadingEnabled = false;
                _model.ObjectTrackingEnabled = false;
                return (from New_premio _premio in _model.New_premios
                        join new_campaign_new_premio _rel in _model.new_campaign_new_premios on _premio.New_premioId equals _rel.new_premioid
                        join Campaign _camp in _model.Campaigns on _rel.campaignid equals _camp.CampaignId
                        where _camp.CampaignId == campaignId &&
                        _premio.DeletionStateCode == 0
                        select _premio).SingleOrDefault();
            }
        }

        /// <summary>
        /// Cifra il numero di telefono in chiaro
        /// </summary>
        /// <param name="plaintext"></param>
        /// <returns></returns>
        public string GetCryptedCode(string plaintext)
        {
            using (ExposedEncryptor.ExposedEncryptor _service = new ExposedEncryptor.ExposedEncryptor(ConfigurationManager.AppSettings["ExposedEncryptorUrl"]))
            {
                return _service.EncryptNumber(plaintext);
            }
        }

        /// <summary>
        /// Decifra il numero di telefono in chiaro
        /// </summary>
        /// <param name="plaintext"></param>
        /// <returns></returns>
        public string GetDecryptedCode(string ciphertext)
        {
            using (ExposedEncryptor.ExposedEncryptor _service = new ExposedEncryptor.ExposedEncryptor(ConfigurationManager.AppSettings["ExposedEncryptorUrl"]))
            {
                return _service.DecryptNumber(ciphertext);
            }
        }
        /// <summary>
        /// Restituisce il lead data la sua chiave primaria
        /// </summary>
        /// <returns></returns>
        public Lead GetLeadById(Guid idLead)
        {
            using (MarketingSeatDataContext _model = new MarketingSeatDataContext())
            {
                _model.DeferredLoadingEnabled = false;
                _model.ObjectTrackingEnabled = false;
                return _model.Leads.Where(item => item.LeadId == idLead && item.DeletionStateCode == 0).SingleOrDefault();
            }
        }
        /// <summary>
        /// Restituisce il lead associato ad una campagna
        /// </summary>
        /// <param name="idCampaign"></param>
        /// <returns></returns>
        public Lead GetLeadByCryptedCode(string cryptedCode)
        {
            using (MarketingSeatDataContext _model = new MarketingSeatDataContext())
            {
                _model.DeferredLoadingEnabled = false;
                _model.ObjectTrackingEnabled = false;
                return _model.Leads.Where(item => item.New_CodiceCifratoPartecipante == cryptedCode && item.DeletionStateCode == 0).SingleOrDefault();
            }
        }
        /// <summary>
        /// Restituisce tutte le marketing list
        /// </summary>
        /// <returns></returns>
        public New_calltype GetCallTypeByCallTypeName(string callType)
        {
            using (MarketingSeatDataContext _model = new MarketingSeatDataContext())
            {
                _model.DeferredLoadingEnabled = false;
                _model.ObjectTrackingEnabled = false;
                return _model.New_calltypes.Where(item => item.New_nomecalltype == callType && item.DeletionStateCode == 0).SingleOrDefault();
            }
        }
        /// <summary>
        /// Restituisce il chiamante campagna dato il suo ID
        /// </summary>
        /// <param name="idChiamanteCampagna"></param>
        /// <returns></returns>
        public New_chiamantecampagna GetChiamanteCampagnaById(Guid idChiamanteCampagna)
        {
            using (MarketingSeatDataContext _model = new MarketingSeatDataContext())
            {
                _model.DeferredLoadingEnabled = false;
                _model.ObjectTrackingEnabled = false;
                return _model.New_chiamantecampagnas.Where(item => item.New_chiamantecampagnaId == idChiamanteCampagna && item.DeletionStateCode == 0).SingleOrDefault();
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Rilascio delle risorse
        /// </summary>
        public void Dispose()
        {

        }

        #endregion
    }
}
