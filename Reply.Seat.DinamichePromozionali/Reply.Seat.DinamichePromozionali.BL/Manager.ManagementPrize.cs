using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reply.Seat.DinamichePromozionali.DataAccess.Model;
using Reply.Seat.DinamichePromozionali.DataAccess;
//using Reply.Seat.DinamichePromozionali.CrmAdapter.CrmServiceWsdl;
using Reply.Seat.DinamichePromozionali.CrmAdapter.CrmServicePerformance;
using System.Web;

namespace Reply.Seat.DinamichePromozionali.BL
{
    /// <summary>
    /// Gestisce il flusso delle richiesta campagna
    /// </summary>
    public partial class Manager : BaseManager, IDisposable
    {
        #region PUBLIC METHODS
        /// <summary>
        /// Gestione del premio in caso di modalità manuale.
        /// Metodo che deve essere invocato dal WS in caso di modalità manuale
        /// </summary>
        /// <param name="idChiamanteCampagna"></param>
        /// <param name="cryptedCode"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="wsContext"></param>
        public void GetManagementPrize(Guid idChiamanteCampagna, string cryptedCode, string phoneNumber, HttpContext wsContext)
        {
            try
            {
                NewMessage(LogLevel.Message, "------------------------------------------------------------------------------------");
                NewMessage(LogLevel.Message, "Inizio GetManagementPrize (richiesta ricevuta da " + wsContext.User.Identity.Name + "@" + wsContext.Request.UserHostAddress + ") con parametri "
                                             + idChiamanteCampagna + ";" + cryptedCode + ";" + phoneNumber);
                
                ///Ottengo il codice cifrato a partire dal numero di telefono
                string cipherText = this.CurrentAccessLayer.GetCryptedCode(phoneNumber);
                //NewMessage(LogLevel.Message, "Numero " + phoneNumber + " cifrato in " + cipherText);
                ///Gestione premio
                //this.GetManagementPrize(idChiamanteCampagna, cryptedCode, phoneNumber);
                this.GetManagementPrize(idChiamanteCampagna, cipherText, phoneNumber);
            }
            catch (DinamichePromozionaliException dex)
            {
                throw;
            }
            catch(Exception ex)
            {
                NewMessage(LogLevel.Message, "Errore imprevisto sul metodo GetManagementPrize: " + ex.Message + " - StackTrace: " + ex.StackTrace);
                throw;
            }
            finally
            {

                NewMessage(LogLevel.Message, "Fine GetManagementPrize (richiesta ricevuta da " + wsContext.User.Identity.Name + "@" + wsContext.Request.UserHostAddress + ") con parametri "
                                             + idChiamanteCampagna + ";" + cryptedCode + ";" + phoneNumber);
                NewMessage(LogLevel.Message, "");
            }
        }
        /// <summary>
        /// Gestione del premio in caso di modalità manuale.
        /// Metodo che deve essere invocato dal WS in caso di modalità manuale
        /// </summary>
        /// <param name="idChiamanteCampagna"></param>
        /// <param name="cryptedCode"></param>
        /// <param name="phoneNumber"></param>
        public void GetManagementPrize(Guid idChiamanteCampagna, string cryptedCode, string phoneNumber)
        {
            if (idChiamanteCampagna == Guid.Empty ||
                string.IsNullOrEmpty(cryptedCode) ||
                string.IsNullOrEmpty(phoneNumber))
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_MISS_PARAMETERS, StatusTranscoding.STATUS_DESC_MISS_PARAMETERS, idChiamanteCampagna, null);
            try
            {
                ///Se non lo possiedo recupero il chiamante campagna
                if (this.CurrentChiamanteCampagna == null)
                    this.CurrentChiamanteCampagna = this.CurrentAccessLayer.GetChiamanteCampagnaById(idChiamanteCampagna);
                ///CONTROLLO OBBLIGATORIETA SU CHIAMANTE CAMPAGNA
                if (this.CurrentChiamanteCampagna == null || !this.CurrentChiamanteCampagna.New_CampagnaId.HasValue)
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_MISS_CHIAMANTE_CAMPAGNA, StatusTranscoding.STATUS_DESC_MISS_CHIAMANTE_CAMPAGNA, idChiamanteCampagna, null);
                ///Se non la possiedo recupero la campagna
                if (this.CurrentCampaign == null)
                    this.CurrentCampaign = this.CurrentAccessLayer.GetCampaignById(this.CurrentChiamanteCampagna.New_CampagnaId.Value);
                ///Controllo obbligatorietà campagna
                if (this.CurrentCampaign == null)
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_MISS_CAMPAIGN, StatusTranscoding.STATUS_DESC_MISS_CAMPAIGN, idChiamanteCampagna, null);
                ///Recupero il template Premio
                New_premio premio = this.CurrentAccessLayer.GetPremioByIdCampaign(this.CurrentCampaign.CampaignId);
                if (premio == null)
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_MISS_TEMPLATE_PREMIO, StatusTranscoding.STATUS_DESC_MISS_TEMPLATE_PREMIO, idChiamanteCampagna, this.CurrentCampaign);

                ///Check sul tipo di premio vinto
                if (premio.New_TipologiaPremio == (int)DataAccessLayer.TemplatePremioTipologiaPremio.Codice)
                {
                    NewMessage(LogLevel.Message, "Tipologia premio vinta CODICE");
                    ///Codice in offerta
                    this.ManagmentCode(premio, this.CurrentChiamanteCampagna, cryptedCode, phoneNumber);
                    ///Dopo aver rilasciato il premio aggiorno le soglie
                    this.UpdateChiamanteCampagnaSoglieVincite(this.CurrentCampaign, this.CurrentChiamanteCampagna);
                }
                else if (premio.New_TipologiaPremio == (int)DataAccessLayer.TemplatePremioTipologiaPremio.Chiamate)
                {
                    NewMessage(LogLevel.Message, "Tipologia premio vinta CHIAMANTE GRATIS");
                    ///Chiamate gratis
                    this.ManagmentCalls(premio, this.CurrentChiamanteCampagna, cryptedCode, phoneNumber);
                    ///Dopo aver rilasciato il premio aggiorno le soglie
                    this.UpdateChiamanteCampagnaSoglieVincite(this.CurrentCampaign, this.CurrentChiamanteCampagna);
                }
                else if (premio.New_TipologiaPremio == (int)DataAccessLayer.TemplatePremioTipologiaPremio.Oggetto)
                {
                    NewMessage(LogLevel.Message, "Tipologia premio vinta OGGETTO (non gestito)");
                    ///Dopo aver rilasciato il premio aggiorno le soglie
                    this.UpdateChiamanteCampagnaSoglieVincite(this.CurrentCampaign, this.CurrentChiamanteCampagna);
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_TIPOLOGIA_PREMIO_NON_GESTITA, StatusTranscoding.STATUS_DESC_TIPOLOGIA_PREMIO_NON_GESTITA, idChiamanteCampagna, this.CurrentCampaign);
                }
                else
                {
                    NewMessage(LogLevel.Message, "Tipologia premio vinta CONCORSO (non gestito)");
                    ///Dopo aver rilasciato il premio aggiorno le soglie
                    this.UpdateChiamanteCampagnaSoglieVincite(this.CurrentCampaign, this.CurrentChiamanteCampagna);
                    ///Restanti due casi da vedere con andrea
                    ///esco con il messaggio
                    ///conconrso esco con ok
                    ///oggetto non gestito
                    //throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_PREMIO_CONCORSO, StatusTranscoding.STATUS_DESC_PREMIO_CONCORSO, idChiamanteCampagna, this.CurrentCampaign);
                }
                NewMessage(LogLevel.Message, "Esco dal flusso del premio con il codice: " + StatusTranscoding.STATUS_CODE_PREMIO_CONCORSO);
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_PREMIO_CONCORSO, StatusTranscoding.STATUS_DESC_PREMIO_CONCORSO, idChiamanteCampagna, this.CurrentCampaign);
                //return new GetManagementPrizeResult() { IsSuccessfull = true, StatusCode = "00123", StatusDescription = "Elaborazione terminata con successo" };
            }
            catch (DinamichePromozionaliException dex)
            {
                throw;
            }
            catch (Exception ex)
            {
                NewMessage(LogLevel.Message, "Errore imprevisto sul metodo GetManagementPrize: " + ex.Message + " - StackTrace: " + ex.StackTrace);
                throw;
            }
        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Gestione del premio tipo codice
        /// </summary>
        private void ManagmentCode(New_premio premio, New_chiamantecampagna chiamanteCampagna, string cryptedCode, string phoneNumber)
        {
            ///Recupero il codice promozione
            New_codicepromozionale _codPromozionale = this.CurrentAccessLayer.GetCodicePromozionaleAttivoByIdTemplatePremio(premio.New_premioId);
            if (_codPromozionale == null)
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_MISS_CODICE_PROMOZIONALE, StatusTranscoding.STATUS_DESC_MISS_CODICE_PROMOZIONALE, chiamanteCampagna.New_chiamantecampagnaId, null);
            ///Recupero la campagna
            Campaign _campaign = this.CurrentAccessLayer.GetCampaignById(chiamanteCampagna.New_CampagnaId.Value);
            if (_campaign == null)
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_MISS_CAMPAIGN, StatusTranscoding.STATUS_DESC_MISS_CAMPAIGN, chiamanteCampagna.New_chiamantecampagnaId, null);

            ///Aggiorno il codice promozione mettendolo ad usa
            new_codicepromozionale codPromozionaleCrm = new new_codicepromozionale();
            codPromozionaleCrm.new_codicepromozionaleid = new Key() { Value = _codPromozionale.New_codicepromozionaleId };
            codPromozionaleCrm.new_chiamantecampagnaid = base.CreaLookup(EntityName.new_chiamantecampagna.ToString(), chiamanteCampagna.New_chiamantecampagnaId);
            codPromozionaleCrm.new_usato = new CrmBoolean() { Value = true };
            ///Aggiorno il codice promozionale
            this.CurrentCrmAdapter.UpdateCodicePromozionale(codPromozionaleCrm);
            NewMessage(LogLevel.Message, "Recupero un codice promozionale e lo invalido mettendo a usato = TRUE, codice usato: " + _codPromozionale.New_codice);
            ///Controllo sul template premio se deve o non devo spedire SMS
            if (premio.New_mcv_sms.HasValue && premio.New_mcv_sms.Value)
            {
                ///Inserisco una linea nella tabella SMS
                new_sms _sms = new new_sms();
                _sms.new_chiamantecampagnaid = base.CreaLookup(EntityName.new_chiamantecampagna.ToString(), chiamanteCampagna.New_chiamantecampagnaId);
                _sms.new_testosms = TagTranscoding.GetTranscoding(_campaign.New_MessaggioVincitaClienteSMS, this.CurrentCampaign, chiamanteCampagna,premio, _codPromozionale.New_codice); ///TODO: REPLACE TAG ALL'INTERNO DEL TESTO DELL SMS CON I DETTAGLI DELLA CAMPAGNA E IL CODICE TROVATO
                _sms.new_statoinvio = new Picklist() { Value = (int)DataAccessLayer.SmsStatoInvio.DaInviare };
                _sms.new_numerodestinatario = cryptedCode;//phoneNumber;

                ///INSERISCO DATA IMMEDIATA O DATA FUTURA DAL TEMPLATE DEL PREMIO
                if (premio.New_Dataattivazionepremio == (int)DataAccessLayer.TemplatePremioDataAttivazione.Immediata)
                {
                    ////immediata
                    _sms.new_datainvio = base.CreaDateTime(DateTime.Now);
                }
                else
                {
                    ///futura
                    _sms.new_datainvio = base.CreaDateTime(new DateTime(premio.New_Dataattivazione.Value.Year,
                        premio.New_Dataattivazione.Value.Month, premio.New_Dataattivazione.Value.Day,
                        8, 0, 0, 0));
                }
                ///inserisco SMS
                this.CurrentCrmAdapter.CreaSMS(_sms, this.CurrentOwner(_campaign));
                NewMessage(LogLevel.Message, "Creo un SMS passando il codice promozionale trovato");
            }
        }
        /// <summary>
        /// Gestione del premio tipo chiamate
        /// </summary>
        private void ManagmentCalls(New_premio premio, New_chiamantecampagna chiamanteCampagna, string cryptedCode, string phoneNumber)
        {
            int quantita;
            if (premio.New_quantita.HasValue)
                quantita = premio.New_quantita.Value;
            else
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NULL_QUANTITA, StatusTranscoding.STATUS_DESC_NULL_QUANTITA, chiamanteCampagna.New_chiamantecampagnaId, this.CurrentCampaign);
            ///Recupero la campagna
            Campaign _campaign = this.CurrentAccessLayer.GetCampaignById(chiamanteCampagna.New_CampagnaId.Value);
            if (_campaign == null)
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_MISS_CAMPAIGN, StatusTranscoding.STATUS_DESC_MISS_CAMPAIGN, chiamanteCampagna.New_chiamantecampagnaId, null);

            NewMessage(LogLevel.Message, "Creo la call gratuita, impostando i giorni di valità in cui parte il servizio");
            ///inserisco una call gratuita
            new_callgratuita _callGratuita = new new_callgratuita();
            _callGratuita.new_chiamantecampagnaid = base.CreaLookup(EntityName.new_chiamantecampagna.ToString(), chiamanteCampagna.New_chiamantecampagnaId);
            _callGratuita.new_numerocalls = new CrmNumber() { Value = quantita };
            _callGratuita.new_templatepremioid = base.CreaLookup(EntityName.new_premio.ToString(), premio.New_premioId);
            _callGratuita.new_codice = chiamanteCampagna.New_name;// cryptedCode;
            _callGratuita.new_cleartel = this.CurrentAccessLayer.GetDecryptedCode(chiamanteCampagna.New_name);// phoneNumber; //inserisco il numero in chiaro
            NewMessage(LogLevel.Message, "Imposto la data di inizio della promozione");
            DateTime dataInizio;
            ///Recupero le date inizio e fine promozione chiamate gratuite
            if(!premio.New_Dataattivazionepremio.HasValue)
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NULL_DATA_ATTIVAZIONE_PREMIO, StatusTranscoding.STATUS_DESC_NULL_DATA_ATTIVAZIONE_PREMIO, chiamanteCampagna.New_chiamantecampagnaId, this.CurrentCampaign);
            ///imposto data inizio
            if (premio.New_Dataattivazionepremio == (int)DataAccessLayer.TemplatePremioDataAttivazione.Immediata)
            {
                dataInizio = DateTime.Now.AddDays(1);
                _callGratuita.new_datainiziopromozione = base.CreaDateTime(dataInizio);
            }
            else
            {
                if(!premio.New_Dataattivazione.HasValue)
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NULL_DATA_ATTIVAZIONE, StatusTranscoding.STATUS_DESC_NULL_DATA_ATTIVAZIONE, chiamanteCampagna.New_chiamantecampagnaId, this.CurrentCampaign);
                dataInizio = premio.New_Dataattivazione.Value;
                _callGratuita.new_datainiziopromozione = base.CreaDateTime(dataInizio);
            }

            ///imposto data fine
            if (premio.New_datascadenza.HasValue)
                _callGratuita.new_datafinepromozione = base.CreaDateTime(premio.New_datascadenza.Value);
            else
                if (premio.New_GiorniValidita.HasValue)
                    _callGratuita.new_datafinepromozione = base.CreaDateTime(dataInizio.AddDays(premio.New_GiorniValidita.Value));
                else
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NULL_GIORNI_VALIDITA, StatusTranscoding.STATUS_DESC_NULL_GIORNI_VALIDITA, chiamanteCampagna.New_chiamantecampagnaId, this.CurrentCampaign);
            ///inserisco call gratuita
            _callGratuita.ownerid = this.CurrentOwner(this.CurrentCampaign);
            this.CurrentCrmAdapter.CreaChiamataGratuita(_callGratuita, this.CurrentOwner(this.CurrentCampaign));
            ///Inserisco nell'SMS
            ///Inserisco una linea nella tabella SMS
            new_sms _sms = new new_sms();
            _sms.new_chiamantecampagnaid = base.CreaLookup(EntityName.new_chiamantecampagna.ToString(), chiamanteCampagna.New_chiamantecampagnaId);
            _sms.new_testosms = TagTranscoding.GetTranscoding(_campaign.New_MessaggioVincitaClienteSMS, this.CurrentCampaign, chiamanteCampagna, premio, string.Empty);//TagTranscoding.GetTranscoding(_campaign.New_MessaggioVincitaClienteSMS, this.CurrentCampaign, chiamanteCampagna, _codPromozionale.New_codice); ///TODO: REPLACE TAG ALL'INTERNO DEL TESTO DELL SMS CON I DETTAGLI DELLA CAMPAGNA E IL CODICE TROVATO
            _sms.new_statoinvio = new Picklist() { Value = (int)DataAccessLayer.SmsStatoInvio.DaInviare };
            _sms.new_numerodestinatario = cryptedCode;//phoneNumber;
            DateTime _data = DateTime.Parse(_callGratuita.new_datainiziopromozione.Value);
            _sms.new_datainvio = base.CreaDateTime(new DateTime(_data.Year, _data.Month, _data.Day, 8, 0, 0));
            ///inserisco SMS
            this.CurrentCrmAdapter.CreaSMS(_sms, this.CurrentOwner(_campaign));
            NewMessage(LogLevel.Message, "Creazione SMS che per la chiamata gratuita completata");
        }
        /// <summary>
        /// Aggiorna le soglie delle vincite dopo aver elargito i premi
        /// </summary>
        private void UpdateChiamanteCampagnaSoglieVincite(Campaign campaign, New_chiamantecampagna chiamanteCampagna)
        {
            ///Se non la possiedo recupero la campagna
            //if (this.CurrentCampaign == null)
            //    this.CurrentCampaign = this.CurrentAccessLayer.GetCampaignById(chiamanteCampagna.New_CampagnaId.Value);
            ///Ma se sono arrivato a -1 è perchè c'è un errore di integrità dei dati: sono arrivato a 0, quindi dovrei già essere vincitore e invece non lo sono
            //if (chiamanteCampagna.New_VinciteRimanenti.Value - 1 < 0)
            //    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_VINCITE_RIMANENTI_NEGATIVO, StatusTranscoding.STATUS_DESC_VINCITE_RIMANENTI_NEGATIVO, chiamanteCampagna.New_chiamantecampagnaId, this.CurrentCampaign);

            ///Decremento le vincite rimanenti
            new_chiamantecampagna chiamanteCampagnaCrm = new new_chiamantecampagna();
            chiamanteCampagnaCrm.new_chiamantecampagnaid = new Key() { Value = chiamanteCampagna.New_chiamantecampagnaId };
            ///GESTIONE VITTORIA
            ///Decremento le vincite rimanenti
            chiamanteCampagnaCrm.new_vinciterimanenti = new CrmNumber() { Value = (chiamanteCampagna.New_VinciteRimanenti.Value - 1) };
            ///Se il premio rimanente è == 0 imposto a lo stato a vincitore
            ///altrimenti no
            if (chiamanteCampagnaCrm.new_vinciterimanenti.Value <= 0)
            {
                ///Imposto a vincitore
                ///HO FINITO I PREMI
                chiamanteCampagnaCrm.new_statopartecipante = new Picklist() { Value = (int)DataAccessLayer.StatoPartecipanteInChiamanteCampagna.Vincitore };
            }
            else
            {
                ///Chiamate residue impostate con il default            
                chiamanteCampagnaCrm.new_chiamateresidue = new CrmNumber() { Value = campaign.New_NumeroChiamateSoglia.Value };
            }
            ///Aggiorno il chiamante campagna
            this.CurrentCrmAdapter.UpdateChiamanteCampagna(chiamanteCampagnaCrm);
            NewMessage(LogLevel.Message, "Aggiorno il chiamante campagna aggiornando il suo stato (se vincitore) oppure aggiornando le chiamante residue");
        }
        #endregion
    }
}
