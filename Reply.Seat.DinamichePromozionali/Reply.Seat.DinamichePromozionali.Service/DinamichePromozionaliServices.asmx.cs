using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Reply.Seat.DinamichePromozionali.BL;

[assembly: CLSCompliant(true)]
namespace Reply.Seat.DinamichePromozionali.Service
{
    /// <summary>
    /// Interfaccia a servizi per ottenere lo stato della campagna, il settaggio della privacy e la 
    /// gestione dei premi vinti.
    /// Il WS risponde sempre anche in caso di eccezzione con un oggetto di ritorno, che al suo interno
    /// contiene la property issuccessfull che definisce se l'elaborazione è finita con errore piuttosto che 
    /// correttamente
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class DinamichePromozionaliServices : System.Web.Services.WebService
    {
        #region PUBLIC METHODS
        /// <summary>
        /// Gestione del premio
        /// </summary>
        /// <param name="idChiamanteCampagna"></param>
        /// <param name="cryptedCode"></param>
        /// <param name="phoneNumber"></param>
        [WebMethod]
        public GetManagementPrizeResult GetManagementPrize(Guid idChiamanteCampagna, string cryptedCode, string phoneNumber)
        {
            try
            {
                //return new GetManagementPrizeResult() { IsSuccessfull = true, StatusCode = "00123", StatusDescription = "Elaborazione terminata con successo" };
                using (Manager _statusCampaign = new Manager(Context))
                {
                    _statusCampaign.GetManagementPrize(idChiamanteCampagna, cryptedCode, phoneNumber, this.Context);
                }
                return new GetManagementPrizeResult() { IsSuccessfull = true, StatusCode = StatusTranscoding.STATUS_CODE_OK, StatusDescription = StatusTranscoding.STATUS_DESC_OK };
            }
            catch (DinamichePromozionaliException dinamicheException)
            {
                return new GetManagementPrizeResult() { IsSuccessfull = true, StatusCode = dinamicheException.StatusCode, StatusDescription = dinamicheException.StatusDescription };
            }
            catch (Exception ex)
            {
                return new GetManagementPrizeResult()
                {
                    IsSuccessfull = false,
                    StatusCode = StatusTranscoding.STATUS_CODE_UNEXPECTED_ERROR,
                    StatusDescription = ex.Message + " - " + ex.StackTrace + " - " + ((ex.InnerException != null) ? ex.InnerException.Message : string.Empty)
                };
            }
        }
        
        /// <summary>
        /// Restituisce lo stato della campagna e dei suoi attributi
        /// </summary>
        /// <param name="idCall"></param>
        /// <param name="callType"></param>
        /// <param name="cryptedCode"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="idOperator"></param>
        [WebMethod]
        public GetStatusCampaignResult GetStatusCampaign(string idCall, string callType, string cryptedCode, string phoneNumber, string idOperator)
        {
            try
            {
                using (Manager _statusCampaign = new Manager(Context))//Manager.Instance)
                {
                    _statusCampaign.GetStatusCampaign(idCall, callType, cryptedCode, phoneNumber, idOperator, this.Context);
                    return new GetStatusCampaignResult() { IsSuccessfull = false, StatusCode = "000", StatusDescription = "Ritorno non possibile" };
                }
            }
            catch (DinamichePromozionaliException dinamicheException)
            {
                return new GetStatusCampaignResult()
                {
                    IsSuccessfull = true,
                    StatusCode = dinamicheException.StatusCode,
                    StatusDescription = dinamicheException.StatusDescription,
                    TestoMessaggioVincitaAPFisso = dinamicheException.TestoMessaggioVincitaAPFisso,
                    TestoMessaggioVincitaAPMobile = dinamicheException.TestoMessaggioVincitaAPMobile,
                    TestoMessaggioPushAP = dinamicheException.TestoMessaggioPushAP,
                    TestoMessaggioRichiestaPrivacy = dinamicheException.TestoMessaggioRichiestaPrivacy,
                    UrlLogoPrivacy = dinamicheException.UrlLogoPrivacy,
                    UrlLogoBanner = dinamicheException.UrlLogoBanner,
                    UrlLogoCampagnaPush = dinamicheException.UrlLogoCampagnaPush,
                    UrlLogoCampagnaVincita = dinamicheException.UrlLogoCampagnaVincita,
                    ChiamanteCampagnaId = dinamicheException.IdChiamanteCampagna,
                    PrivacyChiamanteCampagna = dinamicheException.PrivacyChiamanteCampagna,
                    StatoChiamanteCampagna = dinamicheException.StatoChiamanteCampagna,
                    ModalitaComunicazionePremio = dinamicheException.ModalitaComunicazionePremio

                };
            }
            catch (Exception ex)
            {
                return new GetStatusCampaignResult()
                {
                    IsSuccessfull = false,
                    StatusCode = StatusTranscoding.STATUS_CODE_UNEXPECTED_ERROR,
                    StatusDescription = ex.Message + " - " + ex.StackTrace + " - " + ((ex.InnerException != null) ? ex.InnerException.Message : string.Empty)
                };
            }
        }
        
        /// <summary>
        /// Setta il consenso o meno della privacy per la campagna
        /// (SI: per sempre NO: campagna in corso NON SO: campagna in corso)
        /// </summary>
        /// <param name="cryptedCode"></param>
        /// <param name="flagPrivacy"></param>
        [WebMethod]
        public SetPrivacyResult SetPrivacy(Guid idChiamanteCampagna, string statusPrivacy, string cryptedCode, string phoneNumber)
        {
            //return new SetPrivacyResult() { IsSuccessfull = false, StatusCode = string.Empty, StatusDescription = "TEST" };
            try
            {
                using (Manager _managerPrivacy = new Manager(Context))
                {
                    return _managerPrivacy.SetPrivacy(idChiamanteCampagna, statusPrivacy, cryptedCode, phoneNumber, this.Context);
                }
            }
            catch (DinamichePromozionaliException dinamicheException)
            {
                return new SetPrivacyResult() { IsSuccessfull = true, StatusCode = dinamicheException.StatusCode, StatusDescription = dinamicheException.StatusDescription };
            }
            catch (Exception ex)
            {
                return new SetPrivacyResult()
                {
                    IsSuccessfull = false,
                    StatusCode = StatusTranscoding.STATUS_CODE_UNEXPECTED_ERROR,
                    StatusDescription = ex.Message + " - " + ex.StackTrace + " - " + ((ex.InnerException != null) ? ex.InnerException.Message : string.Empty)
                };
            }
        }

        /// <summary>
        /// Setta solo il valore della privacy a SI / NO / NON SO
        /// </summary>
        /// <param name="idChiamanteCampagna"></param>
        /// <param name="statusPrivacy"></param>
        /// <returns></returns>
        [WebMethod]
        public SetOnlyPrivacyResult SetOnlyPrivacy(Guid idChiamanteCampagna, string statusPrivacy)
        {
            //return new SetPrivacyResult() { IsSuccessfull = false, StatusCode = string.Empty, StatusDescription = "TEST" };
            try
            {
                using (Manager _managerPrivacy = new Manager(Context))
                {
                    return _managerPrivacy.SetOnlyPrivacy(idChiamanteCampagna, statusPrivacy,this.Context);
                }
            }
            catch (DinamichePromozionaliException dinamicheException)
            {
                return new SetOnlyPrivacyResult() { IsSuccessfull = true, StatusCode = dinamicheException.StatusCode, StatusDescription = dinamicheException.StatusDescription };
            }
            catch (Exception ex)
            {
                return new SetOnlyPrivacyResult()
                {
                    IsSuccessfull = false,
                    StatusCode = StatusTranscoding.STATUS_CODE_UNEXPECTED_ERROR,
                    StatusDescription = ex.Message + " - " + ex.StackTrace + " - " + ((ex.InnerException != null) ? ex.InnerException.Message : string.Empty)
                };
            }
        }

        /// <summary>
        /// Ritorna il chiamante campagna cone le sue informazioni
        /// </summary>
        [WebMethod]
        public GetChiamanteCampagnaResult GetChiamanteCampagna(Guid idChiamanteCampagna)
        {
            try
            {
                using (Manager _managerChiamanteCampagna = new Manager())
                {
                    ///Ritorno lo stato del chiamante campagna in base al suo ID
                    return _managerChiamanteCampagna.GetChiamanteCampagna(idChiamanteCampagna, this.Context);
                }
            }
            catch (Exception ex)
            {
                ///Gestisco l'eventuale eccezzione riportanto il messaggio di fallimento
                return new GetChiamanteCampagnaResult()
                {
                    IsSuccessfull = false,
                    StatusCode = StatusTranscoding.STATUS_CODE_UNEXPECTED_ERROR,
                    StatusDescription = ex.Message + " - " + ex.StackTrace + " - " + ((ex.InnerException != null) ? ex.InnerException.Message : string.Empty)
                };
            }
        }
        #endregion

    }
}
