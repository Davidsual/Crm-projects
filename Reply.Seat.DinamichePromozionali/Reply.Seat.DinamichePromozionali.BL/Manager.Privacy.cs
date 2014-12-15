using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reply.Seat.DinamichePromozionali.DataAccess.Model;
//using Reply.Seat.DinamichePromozionali.CrmAdapter.CrmServiceWsdl;
using Reply.Seat.DinamichePromozionali.CrmAdapter.CrmServicePerformance;
using Reply.Seat.DinamichePromozionali.DataAccess;
using System.Web;
using System.Globalization;

namespace Reply.Seat.DinamichePromozionali.BL
{
    /// <summary>
    /// Gestione della privacy
    /// </summary>
    public partial class Manager : BaseManager, IDisposable
    {
        #region PRIVATE MEMBERS
        private const string PRIVACY_YES = "SI";
        private const string PRIVACY_NO = "NO";
        private const string PRIVACY_UNDEFINED = "NON SO";
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Set della privacy
        /// </summary>
        /// <param name="idChiamanteCampagna"></param>
        /// <param name="statusPrivacy"></param>
        /// <returns></returns>
        public SetPrivacyResult SetPrivacy(Guid idChiamanteCampagna, string statusPrivacy,string cryptedCode, string phoneNumber, HttpContext wsContext)
        {
            try
            {
                NewMessage(LogLevel.Message, "------------------------------------------------------------------------------------");
                NewMessage(LogLevel.Message, "Inizio SetPrivacy (richiesta ricevuta da " + wsContext.User.Identity.Name + "@" + wsContext.Request.UserHostAddress + ") con parametri "
                                             + idChiamanteCampagna + ";" + statusPrivacy + ";" + cryptedCode + ";" + phoneNumber);

                ///Controllo di obbligatorietà dei campi
                if (idChiamanteCampagna == Guid.Empty || string.IsNullOrEmpty(statusPrivacy))
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_MISS_PARAMETERS, StatusTranscoding.STATUS_DESC_MISS_PARAMETERS, idChiamanteCampagna, null);
                if (statusPrivacy.ToUpper() == PRIVACY_YES && (string.IsNullOrEmpty(cryptedCode) || string.IsNullOrEmpty(phoneNumber)))
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_MISS_PARAMETERS, StatusTranscoding.STATUS_DESC_MISS_PARAMETERS, idChiamanteCampagna, null);

                SetPrivacyResult _result = new SetPrivacyResult();
                ///Stato privacy undefined
                if (statusPrivacy.ToUpper() == PRIVACY_UNDEFINED)
                {
                    NewMessage(LogLevel.Message, "Valore Privacy non definito esco");
                    _result.IsSuccessfull = true;
                    _result.StatusCode = StatusTranscoding.STATUS_CODE_OK;
                    _result.StatusDescription = StatusTranscoding.STATUS_DESC_OK;
                }
                else if (statusPrivacy.ToUpper() == PRIVACY_NO)
                {
                    NewMessage(LogLevel.Message, "Valore Privacy definito NO");
                    New_chiamantecampagna chiamanteCampagna = this.CurrentAccessLayer.GetChiamanteCampagnaById(idChiamanteCampagna);
                    if (chiamanteCampagna == null)
                        throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_CHIAMANTE_CAMPAGNA_NOT_FOUND, StatusTranscoding.STATUS_DESC_CHIAMANTE_CAMPAGNA_NOT_FOUND, idChiamanteCampagna, null);
                    ///AGGIORNO IL CHIAMANTE CAMPAGNA CON NO A PRIVACY
                    new_chiamantecampagna chiamanteCampagnaCrm = new new_chiamantecampagna();
                    chiamanteCampagnaCrm.new_chiamantecampagnaid = new Key() { Value = chiamanteCampagna.New_chiamantecampagnaId };
                    ///chiamanteCampagnaCrm.new_partecipanteid = base.CreaLookup(EntityName.new_chiamantecampagna.ToString(), idChiamanteCampagna);
                    chiamanteCampagnaCrm.new_privacy = new Picklist() { Value = (int)DataAccessLayer.Privacy.No };
                    ///Imposto lo stato partecipante a RIFIUTO PRIVACY
                    chiamanteCampagnaCrm.new_statopartecipante = new Picklist() { Value = (int)DataAccessLayer.StatoPartecipanteInChiamanteCampagna.RifiutoPrivacy};
                    this.CurrentCrmAdapter.UpdateChiamanteCampagna(chiamanteCampagnaCrm);

                    NewMessage(LogLevel.Message, "Aggiorno il chiamante campagna con valore Privacy NO ed esco");
                    ///Popolo oggetto di ritorn
                    _result.IsSuccessfull = true;
                    _result.StatusCode = StatusTranscoding.STATUS_CODE_OK;
                    _result.StatusDescription = StatusTranscoding.STATUS_DESC_OK;
                }
                else if (statusPrivacy.ToUpper() == PRIVACY_YES)
                {
                    NewMessage(LogLevel.Message, "Valore Privacy definito SI");
                    New_chiamantecampagna chiamanteCampagna = this.CurrentAccessLayer.GetChiamanteCampagnaById(idChiamanteCampagna);
                    if (chiamanteCampagna == null)
                        throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_CHIAMANTE_CAMPAGNA_NOT_FOUND, StatusTranscoding.STATUS_DESC_CHIAMANTE_CAMPAGNA_NOT_FOUND, idChiamanteCampagna, null);
                    NewMessage(LogLevel.Message, "Ho recuperato il chiamante campagna - New_campagnaId = " + chiamanteCampagna.New_CampagnaId.Value.ToString());
                    ///Se non la possiedo recupero la campagna
                    if (this.CurrentCampaign == null)
                        this.CurrentCampaign = this.CurrentAccessLayer.GetCampaignById(chiamanteCampagna.New_CampagnaId.Value);
                    NewMessage(LogLevel.Message, "Ho recuperato la campagna - New_PartecipanteId = " + chiamanteCampagna.New_PartecipanteId.Value.ToString());
                    Lead _lead = this.CurrentAccessLayer.GetLeadById(chiamanteCampagna.New_PartecipanteId.Value);
                    if (_lead == null)
                        throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_LEAD_NOT_FOUND, StatusTranscoding.STATUS_DESC_LEAD_NOT_FOUND, idChiamanteCampagna, null);
                    NewMessage(LogLevel.Message, "Ho recuperato il LEAD");
                    ///Controllo che abbia il tipo target
                    if(!this.CurrentCampaign.New_TipoTarget.HasValue)
                        throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_TIPO_TARGET_NOT_FOUND, StatusTranscoding.STATUS_DESC_TIPO_TARGET_NOT_FOUND, idChiamanteCampagna, null);

                    ///Aggiorno il chiamante campagna
                    new_chiamantecampagna chiamanteCampagnaCrm = new new_chiamantecampagna();
                    chiamanteCampagnaCrm.new_chiamantecampagnaid = new Key() { Value = idChiamanteCampagna };
                    chiamanteCampagnaCrm.new_privacy = new Picklist() { Value = (int)DataAccessLayer.Privacy.Si };
                    chiamanteCampagnaCrm.new_statopartecipante = new Picklist() { Value = (int)DataAccessLayer.StatoPartecipanteInChiamanteCampagna.PotenzialeVincitore };
                    ///aGGIORNO CHIAMANTE CAMPAGNA
                    this.CurrentCrmAdapter.UpdateChiamanteCampagna(chiamanteCampagnaCrm);

                    NewMessage(LogLevel.Message, "Aggiorno il chiamante campagna con valore Privacy SI");
                    //TODO: E' possibile che in caso di privacy = SI, anche il numero di telefono in chiaro debba essere aggiunto al lead
                    lead leadCrm = new lead()
                    {
                        leadid = new Key() { Value = _lead.LeadId }
                    };
                    ///A SECONDA DEL TARGET DELLA CAMPAGNA IMPOSTO LA PRIVACY SUI VARI CAMPI
                    if(this.CurrentCampaign.New_TipoTarget == (int)DataAccessLayer.CampaignTipoTarget.T892424)
                        leadCrm.new_privacy = new CrmBoolean() { Value = true };
                    else if (this.CurrentCampaign.New_TipoTarget == (int)DataAccessLayer.CampaignTipoTarget.T1240)
                        leadCrm.new_privacy12 = new CrmBoolean() { Value = true };
                    //Update lead
                    this.CurrentCrmAdapter.UpdateLead(leadCrm);
                    NewMessage(LogLevel.Message, "Aggiorno il lead con valore Privacy SI");
                    ///POTREMMO AVER GIà VINTO
                    //////Controllo se è vincitore della campagna
                    //if (chiamanteCampagna.New_StatoPartecipante.Value == (int)DataAccessLayer.StatoPartecipanteInChiamanteCampagna.Vincitore)
                    //    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_VINCITORE_CAMPAGNA, StatusTranscoding.STATUS_DESC_VINCITORE_CAMPAGNA, chiamanteCampagna.New_chiamantecampagnaId, this.CurrentCampaign);
                    /////DOVREI FARE LA CHECKPRICE
                    //this.CheckPrize(this.CurrentAccessLayer.GetChiamante
                    this.CurrentChiamanteCampagna = this.CurrentAccessLayer.GetChiamanteCampagnaById(idChiamanteCampagna);
                    if (this.CurrentChiamanteCampagna.New_ChiamateResidue.Value <= 0)
                    {
                        /////DECREMENTO IL NUMERO DI PREMI VINTI
                        /////Decremento le vincite rimanenti
                        //chiamanteCampagnaCrm = new new_chiamantecampagna();
                        //chiamanteCampagnaCrm.new_chiamantecampagnaid = new Key() { Value = chiamanteCampagna.New_chiamantecampagnaId };
                        /////GESTIONE VITTORIA
                        /////Decremento le vincite rimanenti
                        //chiamanteCampagnaCrm.new_vinciterimanenti = new CrmNumber() { Value = (chiamanteCampagna.New_VinciteRimanenti.Value - 1) };
                        /////Se il premio rimanente è == 0 imposto a lo stato a vincitore
                        /////altrimenti no
                        //if (chiamanteCampagnaCrm.new_vinciterimanenti.Value <= 0)
                        //{
                        //    ///Imposto a vincitore
                        //    ///HO FINITO I PREMI
                        //    chiamanteCampagnaCrm.new_statopartecipante = new Picklist() { Value = (int)DataAccessLayer.StatoPartecipanteInChiamanteCampagna.Vincitore };
                        //}
                        //else
                        //{
                        //    ///Chiamate residue impostate con il default            
                        //    chiamanteCampagnaCrm.new_chiamateresidue = new CrmNumber() { Value = this.CurrentCampaign.New_NumeroChiamateSoglia.Value };
                        //}
                        /////Aggiorno il chiamante campagna
                        //this.CurrentCrmAdapter.UpdateChiamanteCampagna(chiamanteCampagnaCrm);

                        NewMessage(LogLevel.Message, "Il valore delle chiamate residue è <= 0 quindi ha vinto");
                        New_premio premio = this.CurrentAccessLayer.GetPremioByIdCampaign(this.CurrentChiamanteCampagna.New_CampagnaId.Value);
                        if (premio == null)
                            throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_MISS_TEMPLATE_PREMIO, StatusTranscoding.STATUS_DESC_MISS_TEMPLATE_PREMIO, Guid.Empty, this.CurrentCampaign);

                        if (premio.New_ModalitaComPremio == (int)DataAccessLayer.TemplatePremioModalitaComunicazionePremio.Automatica)
                        {
                            //if numero inizia con 0
                            if (phoneNumber.StartsWith("0", true, CultureInfo.InvariantCulture))
                            {
                                ///Gestisco come se fosse manuale
                                NewMessage(LogLevel.Message, "Il numero di telefono inzia per 0 e quindi lo tratto come se fosse MANUALE tel number: " + phoneNumber);
                                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_VINCITORE_MANUALE, StatusTranscoding.STATUS_DESC_VINCITORE_MANUALE, chiamanteCampagnaCrm.new_chiamantecampagnaid.Value, this.CurrentCampaign);
                            }
                            NewMessage(LogLevel.Message, "Gestione Premio AUTOMATICA - vado a discriminare sul premio");
                            ///Continuo su gestione del premio in quanto non ho bisogno di interagire per ottenere il numero di telefono
                            this.GetManagementPrize(this.CurrentChiamanteCampagna.New_chiamantecampagnaId, cryptedCode, phoneNumber);
                        }
                        else
                        {
                            NewMessage(LogLevel.Message, "Gestione Premio MANUALE - ritorno lo stato di vincitore manuale " + StatusTranscoding.STATUS_CODE_VINCITORE_MANUALE);
                            throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_VINCITORE_MANUALE, StatusTranscoding.STATUS_DESC_VINCITORE_MANUALE, chiamanteCampagnaCrm.new_chiamantecampagnaid.Value, this.CurrentCampaign);
                        }
                    }
                    else
                    {
                        NewMessage(LogLevel.Message, "Esco dal flusso come potenziale vincitore con codice: " + StatusTranscoding.STATUS_CODE_POTENZIALE_VINCITORE_PREMIO_CONCORSO);
                        ///potenziale vincitore uscire con mressaggio
                        throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_POTENZIALE_VINCITORE_PREMIO_CONCORSO, StatusTranscoding.STATUS_DESC_POTENZIALE_VINCITORE_PREMIO_CONCORSO, this.CurrentChiamanteCampagna.New_chiamantecampagnaId, this.CurrentCampaign);
                    }
                    ///Popolo oggetto di ritorn
                    _result.IsSuccessfull = true;
                    _result.StatusCode = StatusTranscoding.STATUS_CODE_OK;
                    _result.StatusDescription = StatusTranscoding.STATUS_DESC_OK;
                }

                return _result;
            }
            catch (DinamichePromozionaliException dex)
            {
                throw;
            }
            catch (Exception ex)
            {
                NewMessage(LogLevel.Message, "Errore imprevisto sul metodo SetPrivacy: " + ex.Message + " - StackTrace: " + ex.StackTrace);
                throw;
            }
            finally
            {
                NewMessage(LogLevel.Message, "Fine SetPrivacy (richiesta ricevuta da " + wsContext.User.Identity.Name + "@" + wsContext.Request.UserHostAddress + ") con parametri "
                                             + idChiamanteCampagna + ";" + statusPrivacy + ";" + cryptedCode + ";" + phoneNumber);
                NewMessage(LogLevel.Message, "");
            }
        }
        /// <summary>
        /// Aggiorna solo il valore della privacy ed esce
        /// </summary>
        /// <param name="idChiamanteCampagna"></param>
        /// <param name="statusPrivacy"></param>
        /// <param name="wsContext"></param>
        /// <returns></returns>
        public SetOnlyPrivacyResult SetOnlyPrivacy(Guid idChiamanteCampagna, string statusPrivacy, HttpContext wsContext)
        {
            try
            {
                NewMessage(LogLevel.Message, "------------------------------------------------------------------------------------");
                NewMessage(LogLevel.Message, "Inizio SetOnlyPrivacy (richiesta ricevuta da " + wsContext.User.Identity.Name + "@" + wsContext.Request.UserHostAddress + ") con parametri "
                                             + idChiamanteCampagna + ";" + statusPrivacy);

                ///Controllo di obbligatorietà dei campi
                if (idChiamanteCampagna == Guid.Empty || string.IsNullOrEmpty(statusPrivacy))
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_MISS_PARAMETERS, StatusTranscoding.STATUS_DESC_MISS_PARAMETERS, idChiamanteCampagna, null);

                SetOnlyPrivacyResult _result = new SetOnlyPrivacyResult();
                ///Stato privacy undefined
                if (statusPrivacy.ToUpper() == PRIVACY_UNDEFINED)
                {
                    NewMessage(LogLevel.Message, "Valore Privacy non definito esco");
                    _result.IsSuccessfull = true;
                    _result.StatusCode = StatusTranscoding.STATUS_CODE_OK;
                    _result.StatusDescription = StatusTranscoding.STATUS_DESC_OK;
                }
                else if (statusPrivacy.ToUpper() == PRIVACY_NO)
                {
                    NewMessage(LogLevel.Message, "Valore Privacy definito NO");
                    New_chiamantecampagna chiamanteCampagna = this.CurrentAccessLayer.GetChiamanteCampagnaById(idChiamanteCampagna);
                    if (chiamanteCampagna == null)
                        throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_CHIAMANTE_CAMPAGNA_NOT_FOUND, StatusTranscoding.STATUS_DESC_CHIAMANTE_CAMPAGNA_NOT_FOUND, idChiamanteCampagna, null);
                    ///AGGIORNO IL CHIAMANTE CAMPAGNA CON NO A PRIVACY
                    new_chiamantecampagna chiamanteCampagnaCrm = new new_chiamantecampagna();
                    chiamanteCampagnaCrm.new_chiamantecampagnaid = new Key() { Value = chiamanteCampagna.New_chiamantecampagnaId };
                    ///chiamanteCampagnaCrm.new_partecipanteid = base.CreaLookup(EntityName.new_chiamantecampagna.ToString(), idChiamanteCampagna);
                    chiamanteCampagnaCrm.new_privacy = new Picklist() { Value = (int)DataAccessLayer.Privacy.No };
                    ///Imposto lo stato partecipante a RIFIUTO PRIVACY
                    chiamanteCampagnaCrm.new_statopartecipante = new Picklist() { Value = (int)DataAccessLayer.StatoPartecipanteInChiamanteCampagna.RifiutoPrivacy };
                    this.CurrentCrmAdapter.UpdateChiamanteCampagna(chiamanteCampagnaCrm);

                    NewMessage(LogLevel.Message, "Aggiorno il chiamante campagna con valore Privacy NO ed esco");
                    ///Popolo oggetto di ritorn
                    _result.IsSuccessfull = true;
                    _result.StatusCode = StatusTranscoding.STATUS_CODE_OK;
                    _result.StatusDescription = StatusTranscoding.STATUS_DESC_OK;
                }
                else if (statusPrivacy.ToUpper() == PRIVACY_YES)
                {
                    NewMessage(LogLevel.Message, "Valore Privacy definito SI");
                    New_chiamantecampagna chiamanteCampagna = this.CurrentAccessLayer.GetChiamanteCampagnaById(idChiamanteCampagna);
                    if (chiamanteCampagna == null)
                        throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_CHIAMANTE_CAMPAGNA_NOT_FOUND, StatusTranscoding.STATUS_DESC_CHIAMANTE_CAMPAGNA_NOT_FOUND, idChiamanteCampagna, null);

                    ///Se non la possiedo recupero la campagna
                    if (this.CurrentCampaign == null)
                        this.CurrentCampaign = this.CurrentAccessLayer.GetCampaignById(chiamanteCampagna.New_CampagnaId.Value);

                    Lead _lead = this.CurrentAccessLayer.GetLeadById(chiamanteCampagna.New_PartecipanteId.Value);
                    if (_lead == null)
                        throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_LEAD_NOT_FOUND, StatusTranscoding.STATUS_DESC_LEAD_NOT_FOUND, idChiamanteCampagna, null);
                    ///Controllo che abbia il tipo target
                    if (!this.CurrentCampaign.New_TipoTarget.HasValue)
                        throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_TIPO_TARGET_NOT_FOUND, StatusTranscoding.STATUS_DESC_TIPO_TARGET_NOT_FOUND, idChiamanteCampagna, null);

                    ///Aggiorno il chiamante campagna
                    new_chiamantecampagna chiamanteCampagnaCrm = new new_chiamantecampagna();
                    chiamanteCampagnaCrm.new_chiamantecampagnaid = new Key() { Value = idChiamanteCampagna };
                    chiamanteCampagnaCrm.new_privacy = new Picklist() { Value = (int)DataAccessLayer.Privacy.Si };
                    chiamanteCampagnaCrm.new_statopartecipante = new Picklist() { Value = (int)DataAccessLayer.StatoPartecipanteInChiamanteCampagna.PotenzialeVincitore };
                    ///aGGIORNO CHIAMANTE CAMPAGNA
                    this.CurrentCrmAdapter.UpdateChiamanteCampagna(chiamanteCampagnaCrm);

                    NewMessage(LogLevel.Message, "Aggiorno il chiamante campagna con valore Privacy SI");
                    //TODO: E' possibile che in caso di privacy = SI, anche il numero di telefono in chiaro debba essere aggiunto al lead
                    lead leadCrm = new lead()
                    {
                        leadid = new Key() { Value = _lead.LeadId }
                    };
                    ///A SECONDA DEL TARGET DELLA CAMPAGNA IMPOSTO LA PRIVACY SUI VARI CAMPI
                    if (this.CurrentCampaign.New_TipoTarget == (int)DataAccessLayer.CampaignTipoTarget.T892424)
                        leadCrm.new_privacy = new CrmBoolean() { Value = true };
                    else if (this.CurrentCampaign.New_TipoTarget == (int)DataAccessLayer.CampaignTipoTarget.T1240)
                        leadCrm.new_privacy12 = new CrmBoolean() { Value = true };
                    //Update lead
                    this.CurrentCrmAdapter.UpdateLead(leadCrm);
                    NewMessage(LogLevel.Message, "Aggiorno il lead con valore Privacy SI");


                    ///Popolo oggetto di ritorn
                    _result.IsSuccessfull = true;
                    _result.StatusCode = StatusTranscoding.STATUS_CODE_OK;
                    _result.StatusDescription = StatusTranscoding.STATUS_DESC_OK;
                }

                return _result;
            }
            catch (DinamichePromozionaliException dex)
            {
                throw;
            }
            catch (Exception ex)
            {
                NewMessage(LogLevel.Message, "Errore imprevisto sul metodo SetOnlyPrivacy: " + ex.Message + " - StackTrace: " + ex.StackTrace);
                throw;
            }
            finally
            {
                NewMessage(LogLevel.Message, "Fine SetOnlyPrivacy (richiesta ricevuta da " + wsContext.User.Identity.Name + "@" + wsContext.Request.UserHostAddress + ") con parametri "
                                             + idChiamanteCampagna + ";" + statusPrivacy);
                NewMessage(LogLevel.Message, "");
            }
        }
        #endregion

    }
}