using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reply.Seat.DinamichePromozionali.DataAccess.Model;
using Reply.Seat.DinamichePromozionali.DataAccess;

namespace Reply.Seat.DinamichePromozionali.BL
{
    /// <summary>
    /// Errore sollevato dall'applicazione utilizzato come output del WS
    /// </summary>
    public class DinamichePromozionaliException : Exception
    {
        #region PUBLIC PROPERTY
        public string StatusCode { get; private set; }
        public string StatusDescription { get; private set; }
        public Guid IdChiamanteCampagna { get; private set; }
        public Campaign Campaign { get; private set; }

        public string UrlLogoBanner { get; private set; }
        public string UrlLogoPrivacy { get; private set; }
        public string UrlLogoCampagnaPush { get; private set; }
        public string UrlLogoCampagnaVincita { get; private set; }

        public string TestoMessaggioVincitaAPFisso { get; private set; }
        public string TestoMessaggioVincitaAPMobile { get; private set; }
        public string TestoMessaggioPushAP { get; private set; }
        public string TestoMessaggioRichiestaPrivacy { get; private set; }

        public string StatoChiamanteCampagna { get; private set; }
        public string PrivacyChiamanteCampagna { get; private set; }
        public string ModalitaComunicazionePremio { get; private set; }
        #endregion

        #region CTOR
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="statusDescription"></param>
        /// <param name="idChiamanteCampagna"></param>
        /// <param name="campaign"></param>
        public DinamichePromozionaliException(string statusCode, string statusDescription, Guid idChiamanteCampagna, Campaign campaign)
        {
            try
            {
                New_chiamantecampagna chiamanteCampagna = null;
                New_codicepromozionale _codPromozionale = null;

                this.StatusCode = statusCode;
                this.StatusDescription = statusDescription;
                this.IdChiamanteCampagna = idChiamanteCampagna;
                this.Campaign = campaign;
                this.UrlLogoBanner = "";
                this.UrlLogoPrivacy = "";
                this.UrlLogoCampagnaPush = "";
                this.UrlLogoCampagnaVincita = "";
                this.TestoMessaggioVincitaAPFisso = "";
                this.TestoMessaggioVincitaAPMobile = "";
                this.TestoMessaggioPushAP = "";
                this.TestoMessaggioRichiestaPrivacy = "";
                if (campaign != null)
                {
                    this.UrlLogoBanner = campaign.New_LogoBanner;
                    this.UrlLogoPrivacy = campaign.New_LogoPrivacy;
                    this.UrlLogoCampagnaPush = campaign.New_LogoCampagnaPush;
                    this.UrlLogoCampagnaVincita = campaign.New_LogoCampagnaVincita;
                    this.TestoMessaggioVincitaAPFisso = campaign.New_VincitaAPFisso;
                    this.TestoMessaggioVincitaAPMobile = campaign.New_VincitaAPMobile;
                    this.TestoMessaggioPushAP = campaign.New_MessaggioPushAPStatusconPrivacy;
                    this.TestoMessaggioRichiestaPrivacy = campaign.New_MessaggioRichiestaPrivacy;
                    this.StatoChiamanteCampagna = "";
                    this.PrivacyChiamanteCampagna = "";
                    this.ModalitaComunicazionePremio = "";
                    using (DataAccessLayer model = new DataAccessLayer())
                    {
                        if (idChiamanteCampagna != Guid.Empty)
                        {

                            chiamanteCampagna = model.GetChiamanteCampagnaById(idChiamanteCampagna);
                            if (chiamanteCampagna != null)
                            {
                                if (chiamanteCampagna.New_StatoPartecipante.HasValue)
                                    this.StatoChiamanteCampagna = ((DataAccessLayer.StatoPartecipanteInChiamanteCampagna)Enum.ToObject(typeof(DataAccessLayer.StatoPartecipanteInChiamanteCampagna), chiamanteCampagna.New_StatoPartecipante.Value)).ToString().ToUpper();
                                if (chiamanteCampagna.New_Privacy.HasValue)
                                    this.PrivacyChiamanteCampagna = ((DataAccessLayer.Privacy)Enum.ToObject(typeof(DataAccessLayer.Privacy), chiamanteCampagna.New_Privacy.Value)).ToString().ToUpper();

                                _codPromozionale = model.GetLastCodicePromozionaleUsatoByIdChiamanteCampagna(idChiamanteCampagna);
                            }
                        }
                        New_premio premio = model.GetPremioByIdCampaign(campaign.CampaignId);
                        if (premio != null)
                        {
                            if (premio.New_ModalitaComPremio.HasValue)
                                this.ModalitaComunicazionePremio = ((DataAccessLayer.TemplatePremioModalitaComunicazionePremio)Enum.ToObject(typeof(DataAccessLayer.TemplatePremioModalitaComunicazionePremio), premio.New_ModalitaComPremio.Value)).ToString().ToUpper();
                        }


                        this.TestoMessaggioVincitaAPFisso = TagTranscoding.GetTranscoding(campaign.New_VincitaAPFisso, campaign, chiamanteCampagna, premio, (_codPromozionale != null)?_codPromozionale.New_codice:string.Empty);
                        this.TestoMessaggioVincitaAPMobile = TagTranscoding.GetTranscoding(campaign.New_VincitaAPMobile, campaign, chiamanteCampagna, premio, (_codPromozionale != null) ? _codPromozionale.New_codice : string.Empty);
                        this.TestoMessaggioPushAP = TagTranscoding.GetTranscoding(campaign.New_MessaggioPushAPStatusconPrivacy, campaign, chiamanteCampagna, premio, (_codPromozionale != null) ? _codPromozionale.New_codice : string.Empty);
                        this.TestoMessaggioRichiestaPrivacy = TagTranscoding.GetTranscoding(campaign.New_MessaggioRichiestaPrivacy, campaign, chiamanteCampagna, premio, (_codPromozionale != null) ? _codPromozionale.New_codice : string.Empty);
                    }
                }
                ///Log risposta
                this.LogError();
            }
            catch (Exception ex)
            {
                Manager _manager = new Manager();
                _manager.NewMessage(LogLevel.Message,"Errore interno " + ex.Message);
            }
        }
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void LogError()
        {
            Manager _manager = new Manager();
            StringBuilder _strBuilder = new StringBuilder();
            _strBuilder.Append("StatusCode:" + this.StatusCode + Environment.NewLine);
            _strBuilder.Append("StatusDescription: " + this.StatusDescription + Environment.NewLine);
            _strBuilder.Append("IdChiamanteCampagna: " + this.IdChiamanteCampagna + Environment.NewLine);
            _strBuilder.Append("UrlLogoBanner: " + this.UrlLogoBanner + Environment.NewLine);
            _strBuilder.Append("UrlLogoPrivacy: " + this.UrlLogoPrivacy + Environment.NewLine);
            _strBuilder.Append("UrlLogoCampagnaPush: " + this.UrlLogoCampagnaPush + Environment.NewLine);
            _strBuilder.Append("UrlLogoCampagnaVincita: " + this.UrlLogoCampagnaVincita + Environment.NewLine);
            _strBuilder.Append("TestoMessaggioVincitaAPFisso: " + this.TestoMessaggioVincitaAPFisso + Environment.NewLine);
            _strBuilder.Append("TestoMessaggioVincitaAPMobile: " + this.TestoMessaggioVincitaAPMobile + Environment.NewLine);
            _strBuilder.Append("TestoMessaggioPushAP: " + this.TestoMessaggioPushAP + Environment.NewLine);
            _strBuilder.Append("TestoMessaggioRichiestaPrivacy: " + this.TestoMessaggioRichiestaPrivacy + Environment.NewLine);
            _strBuilder.Append("StatoChiamanteCampagna: " + this.StatoChiamanteCampagna + Environment.NewLine);
            _strBuilder.Append("PrivacyChiamanteCampagna: " + this.PrivacyChiamanteCampagna + Environment.NewLine);
            _strBuilder.Append("ModalitaComunicazionePremio: " + this.ModalitaComunicazionePremio + Environment.NewLine);
            _manager.NewMessage(LogLevel.Message, "Log Exception:  " + _strBuilder.ToString());
        }
        #endregion
    }
}
