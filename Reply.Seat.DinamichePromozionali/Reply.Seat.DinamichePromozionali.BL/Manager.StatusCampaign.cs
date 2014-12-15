using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reply.Seat.DinamichePromozionali.DataAccess;
using Reply.Seat.DinamichePromozionali.DataAccess.Model;
//using Reply.Seat.DinamichePromozionali.CrmAdapter.CrmServiceWsdl;
using Reply.Seat.DinamichePromozionali.CrmAdapter.CrmServicePerformance;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Globalization;
[assembly: CLSCompliant(true)]
namespace Reply.Seat.DinamichePromozionali.BL
{
    /// <summary>
    /// Gestisce il flusso delle richiesta campagna
    /// </summary>
    public partial class Manager : BaseManager, IDisposable
    {
        #region PRIVATE PROPERTY
        /// <summary>
        /// Current Owner
        /// </summary>
        private Func<Campaign, Owner> CurrentOwner
        {
            get { return _currentOwner; }
        }
        /// <summary>
        /// Campagna corrente
        /// </summary>
        private Campaign CurrentCampaign { get; set; }
        /// <summary>
        /// Chiamante campagna corrente
        /// </summary>
        private New_chiamantecampagna CurrentChiamanteCampagna { get; set; }
        /// <summary>
        /// Lead corrente associato alla campagna
        /// </summary>
        private Lead CurrentLead { get; set; }
        /// <summary>
        /// Call Type corrente
        /// </summary>
        private New_calltype CurrentCallType { get; set; }
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Richiesta arrivata dal webservice
        /// </summary>
        /// <param name="idCall"></param>
        /// <param name="callType"></param>
        /// <param name="cryptedCode"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="idOperator"></param>
        public void GetStatusCampaign(string idCall, string callType, string cryptedCode, string phoneNumber, string idOperator, HttpContext wsContext)
        {
            NewMessage(LogLevel.Message, "------------------------------------------------------------------------------------");
            NewMessage(LogLevel.Message, "Inizio GetStatusCampaign (richiesta ricevuta da " + wsContext.User.Identity.Name + "@" + wsContext.Request.UserHostAddress + ") con parametri "
                                         + idCall + ";" + callType + ";" + cryptedCode + ";" + phoneNumber + ";" + idOperator);
            if (string.IsNullOrEmpty(idCall) ||
                string.IsNullOrEmpty(callType) ||
                string.IsNullOrEmpty(cryptedCode) ||
                string.IsNullOrEmpty(phoneNumber) ||
                string.IsNullOrEmpty(idOperator))
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_MISS_PARAMETERS, StatusTranscoding.STATUS_DESC_MISS_PARAMETERS, Guid.Empty, null);
            try
            {
                ///Controllo se esiste una campagna con il crypetedCode
                this.CurrentCampaign = this.CurrentAccessLayer.GetCampaignActiveByCallType(callType);
                if (this.CurrentCampaign == null)
                {
                    NewMessage(LogLevel.Message, "Campagna == null per calltype " + callType);
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_MISS_CAMPAIGN, StatusTranscoding.STATUS_DESC_MISS_CAMPAIGN, Guid.Empty, null);
                }
                ///Controllo che passi i controlli di validità
                if (!this.CheckCurrentCampaign(this.CurrentCampaign))
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_CAMPAGNA_NOT_ACTIVE, StatusTranscoding.STATUS_DESC_CAMPAGNA_NOT_ACTIVE, Guid.Empty, null);
                ///Controllo che la campagna possieda il tipo target
                if (!this.CurrentCampaign.New_TipoTarget.HasValue)
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_TIPO_TARGET_NOT_FOUND, StatusTranscoding.STATUS_DESC_TIPO_TARGET_NOT_FOUND, Guid.Empty, null);
                ///Estraggo il call type corrente
                this.CurrentCallType = this.CurrentAccessLayer.GetCallTypeByCallTypeName(callType);
                if (this.CurrentCallType == null)
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_MISS_CALLTYPE, StatusTranscoding.STATUS_DESC_MISS_CALLTYPE, Guid.Empty, this.CurrentCampaign);

                ///Estraggo il template premio e faccio il check sui numeri telefonici
                New_premio _template = this.CurrentAccessLayer.GetPremioByIdCampaign(this.CurrentCampaign.CampaignId);
                if (_template == null)
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_MISS_TEMPLATE_PREMIO, StatusTranscoding.STATUS_DESC_MISS_TEMPLATE_PREMIO, Guid.Empty, this.CurrentCampaign);
                ///Controllo che il numero di telefono sia ammesso
                if (!this.CheckTemplatePremio(_template, phoneNumber))
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_PHONENUMBER_NON_PERMESSO, StatusTranscoding.STATUS_DESC_PHONENUMBER_NON_PERMESSO, Guid.Empty, this.CurrentCampaign);

                ///Controllo se esiste già una chiamata sulla tabella chiamata campagna per il cryptocode passato
                New_chiamantecampagna _chiamanteCampagna = this.CurrentAccessLayer.GetChiamanteCampagnaByCryptedCodeInExistingCampaign(callType, cryptedCode);
                ///Controllo se esiste un chiamamente campagna
                ///Se esiste aggiorno le statistiche altrimenti creo una nuova riga
                if (_chiamanteCampagna != null)
                {
                    if (_chiamanteCampagna.New_Privacy == (int)DataAccessLayer.Privacy.No)
                    {
                        NewMessage(LogLevel.Message, "Privacy del chiamante campagna impostato su NO. Esco");
                        this.CurrentChiamanteCampagna = _chiamanteCampagna;
                        throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NO_PRIVACY_CHIAMANTE_CAMPAGNA, StatusTranscoding.STATUS_DESC_NO_PRIVACY_CHIAMANTE_CAMPAGNA, _chiamanteCampagna.New_chiamantecampagnaId, this.CurrentCampaign);
                    }
                    NewMessage(LogLevel.Message, "Esiste già un chiamante campagna ID: " + _chiamanteCampagna.New_chiamantecampagnaId.ToString() + " vado in aggiornamento");
                    ///Aggiorno la tabella chiamante campagna e tutti le sue tabelle derivate
                    ///Vincitore
                    this.UpdateChiamanteCampagna(_chiamanteCampagna, cryptedCode, phoneNumber);
                }
                else
                {
                    NewMessage(LogLevel.Message, "Nessun chiamante campagna trovato per il calltype: " + callType + " vado ad inserire un nuovo chiamante campagna");
                    ///Inserisco un nuovo chiamante campagna e tutte i suoi derivati
                    this.InsertChiamanteCampagna(callType, cryptedCode, phoneNumber);
                }
                New_chiamantecampagna chiamanteCampagnaNew = this.CurrentAccessLayer.GetChiamanteCampagnaById(_chiamanteCampagna.New_chiamantecampagnaId);
                New_premio premioNew = this.CurrentAccessLayer.GetPremioByIdCampaign(chiamanteCampagnaNew.New_CampagnaId.Value);
                ///fine tutto flusso
                throw new DinamichePromozionaliException("888", "OK", chiamanteCampagnaNew.New_chiamantecampagnaId, this.CurrentCampaign);
            }
            catch (DinamichePromozionaliException)
            {
                NewMessage(LogLevel.Message, "Esco con una DinamicException");
                throw;
            }
            catch (Exception ex)
            {
                NewMessage(LogLevel.Message, "GetStatusCampaign generic Error: " + ex.Message + " - StackTrace: " + ex.StackTrace);
                throw;
            }
            finally
            {
                try
                {
                    if (this.CurrentChiamanteCampagna != null && this.CurrentChiamanteCampagna.New_Privacy != (int)DataAccessLayer.Privacy.No)
                    {
                        NewMessage(LogLevel.Message, "Creo una riga per segnalare che ho effetuato una chiamata");
                        ///Inserisco un Call Log
                        this.CreateCall(this.CurrentChiamanteCampagna.New_chiamantecampagnaId, idCall, idOperator, callType);

                        NewMessage(LogLevel.Message, "Aggiorno il campo chiamanteeffettuate su chiamantecampagna");
                        ///Aggiorno il campo chiamanteeffettuate su chiamantecampagna
                        if (this.CurrentCallType != null && this.CurrentCallType.New_CallTypeTrasferito.HasValue && CurrentCallType.New_CallTypeTrasferito.Value)
                        {
                            NewMessage(LogLevel.Message, "Non aggiorno il chiamante campagna in quanto il calltype è di tipo trasferito");
                        }
                        else
                        {
                            NewMessage(LogLevel.Message, "Aggiorno il chiamante campagna in quanto il calltype non è di tipo trasferito");
                            ///Serve appunto per tenere il conto di quante Chiamate sono state effettuate da quel chiamante per quella campagna
                            new_chiamantecampagna _chiamCamp = new new_chiamantecampagna();
                            _chiamCamp.new_chiamateeffettuate = new CrmNumber() { Value = (this.CurrentChiamanteCampagna.New_ChiamateEffettuate.HasValue) ? this.CurrentChiamanteCampagna.New_ChiamateEffettuate.Value + 1 : 1 };
                            _chiamCamp.new_chiamantecampagnaid = new Key() { Value = this.CurrentChiamanteCampagna.New_chiamantecampagnaId };

                            NewMessage(LogLevel.Message, "Aggiorno il il chiamanteCampagna dopo aver incrementato il New_ChiamateEffettuate");
                            this.CurrentCrmAdapter.UpdateChiamanteCampagna(_chiamCamp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ///Loggo eventuali exception
                    NewMessage(LogLevel.Message, "Errore durante la creazione della call-log/Chiamate effettuate " + ex.Message);
                }
                NewMessage(LogLevel.Message, "Fine GetStatusCampaign (richiesta ricevuta da " + wsContext.User.Identity.Name + "@" + wsContext.Request.UserHostAddress + ") con parametri "
                                             + idCall + ";" + callType + ";" + cryptedCode + ";" + phoneNumber + ";" + idOperator);
                NewMessage(LogLevel.Message, "");
            }
        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Controllo se l'utente ha passato un numero di telefono in lineaa con quello previsto dal template premio
        /// </summary>
        /// <param name="template"></param>
        /// <param name="telephone"></param>
        /// <returns></returns>
        private bool CheckTemplatePremio(New_premio template, string telephone)
        {
            if (template.New_TipoNumeriche == (int)DataAccessLayer.TemplatePremioTipoTelefono.Fisso && telephone.StartsWith("0"))
                return true;
            else if (template.New_TipoNumeriche == (int)DataAccessLayer.TemplatePremioTipoTelefono.Mobile && telephone.StartsWith("3"))
                return true;
            else if (template.New_TipoNumeriche == (int)DataAccessLayer.TemplatePremioTipoTelefono.Tutti)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Aggiorna sulla tabella un chiamante campagna già esistente facendo tutte le considerazioni sugli indici
        /// </summary>
        /// <param name="chiamanteCampagna"></param>
        private void UpdateChiamanteCampagna(New_chiamantecampagna chiamanteCampagna, string cryptedCode, string phoneNumber)
        {
            NewMessage(LogLevel.Message, "Aggiornamento del chiamante campagna");
            ///controllo  stato partecipante
            new_chiamantecampagna chiamanteCampagnaCrm = new new_chiamantecampagna();
            chiamanteCampagnaCrm.new_chiamantecampagnaid = new Key() { Value = chiamanteCampagna.New_chiamantecampagnaId };
            
            if (!chiamanteCampagna.New_StatoPartecipante.HasValue)
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NULL_STATO_PARTECIPANTE, StatusTranscoding.STATUS_DESC_NULL_STATO_PARTECIPANTE, chiamanteCampagna.New_chiamantecampagnaId, this.CurrentCampaign);
            
            ///Controllo il rifiuto privacy
            if (chiamanteCampagna.New_StatoPartecipante.Value == (int)DataAccessLayer.StatoPartecipanteInChiamanteCampagna.RifiutoPrivacy)
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_RIFIUTO_PRIVACY_CHIAMANTE_CAMPAGNA, StatusTranscoding.STATUS_DESC_RIFIUTO_PRIVACY_CHIAMANTE_CAMPAGNA, chiamanteCampagna.New_chiamantecampagnaId, this.CurrentCampaign);
            
            ///Controllo se è vincitore della campagna
            if (chiamanteCampagna.New_StatoPartecipante.Value == (int)DataAccessLayer.StatoPartecipanteInChiamanteCampagna.Vincitore)
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_GIA_VINCITORE_CAMPAGNA, StatusTranscoding.STATUS_DESC_GIA_VINCITORE_CAMPAGNA, chiamanteCampagna.New_chiamantecampagnaId, this.CurrentCampaign);
            
            ///Decremento le chiamate residue se su campagna decrementa trasferimento è a true
            ///oppure sul call type il calltype trasferimento è a true
            if (this.CurrentCampaign.New_DecrementaTrasferimento.HasValue && this.CurrentCampaign.New_DecrementaTrasferimento.Value)
                chiamanteCampagnaCrm.new_chiamateresidue = new CrmNumber() { Value = chiamanteCampagna.New_ChiamateResidue.Value - 1 };
            else if (this.CurrentCallType.New_CallTypeTrasferito.HasValue && !this.CurrentCallType.New_CallTypeTrasferito.Value)
                chiamanteCampagnaCrm.new_chiamateresidue = new CrmNumber() { Value = chiamanteCampagna.New_ChiamateResidue.Value - 1 };
            

            if (!chiamanteCampagna.New_Privacy.HasValue)
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NULL_PRIVACY_CHIAMANTE_CAMPAGNA, StatusTranscoding.STATUS_DESC_NULL_PRIVACY_CHIAMANTE_CAMPAGNA, chiamanteCampagna.New_chiamantecampagnaId, this.CurrentCampaign);
            NewMessage(LogLevel.Message, "Aggiornamento del chiamante campagna Pre update");
            ///Aggiorno chiamante campagna
            this.CurrentCrmAdapter.UpdateChiamanteCampagna(chiamanteCampagnaCrm);

            NewMessage(LogLevel.Message, "Ho aggiornato il chiamante campagna ID: " + chiamanteCampagnaCrm.new_chiamantecampagnaid.Value.ToString());
            ///Recupero il valore del chiamante campagna
            this.CurrentChiamanteCampagna = this.CurrentAccessLayer.GetChiamanteCampagnaById(chiamanteCampagnaCrm.new_chiamantecampagnaid.Value);

            ///CONTROLLO OBBLIGATORIETA SU CHIAMANTE CAMPAGNA
            if (this.CurrentChiamanteCampagna == null || !this.CurrentChiamanteCampagna.New_CampagnaId.HasValue)
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_MISS_CHIAMANTE_CAMPAGNA, StatusTranscoding.STATUS_DESC_MISS_CHIAMANTE_CAMPAGNA, chiamanteCampagnaCrm.new_chiamantecampagnaid.Value, null);

            ///da ora in poi si fà update
            if (this.CurrentChiamanteCampagna.New_Privacy.Value == (int)DataAccessLayer.Privacy.NonSo)
            {
                NewMessage(LogLevel.Message, "Valore della privacy su chiamante campagna impostato a NON SO, esco con codice " + StatusTranscoding.STATUS_CODE_NONSO_PRIVACY_CHIAMANTE_CAMPAGNA);
                ///esco codice non sò
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NONSO_PRIVACY_CHIAMANTE_CAMPAGNA, StatusTranscoding.STATUS_DESC_NONSO_PRIVACY_CHIAMANTE_CAMPAGNA, this.CurrentChiamanteCampagna.New_chiamantecampagnaId, this.CurrentCampaign);
            }
            NewMessage(LogLevel.Message, "Controllo le chiamate residue " + this.CurrentChiamanteCampagna.New_ChiamateResidue.HasValue.ToString());
            ///Controllo se ha vinto
            if (this.CurrentChiamanteCampagna.New_ChiamateResidue.Value <= 0)
            {
                NewMessage(LogLevel.Message, "Valore delle chiamate residue sul chiamante campagna <= 0 vado a gestire i premi");
                ///Ho vinto
                this.CheckPrize(this.CurrentChiamanteCampagna, cryptedCode, phoneNumber);
            }
            else
            {
                NewMessage(LogLevel.Message, "Esco dal flusso come potenziale vincitore con codice: " + StatusTranscoding.STATUS_CODE_POTENZIALE_VINCITORE_PREMIO_CONCORSO);
                ///potenziale vincitore uscire con mressaggio
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_POTENZIALE_VINCITORE_PREMIO_CONCORSO, StatusTranscoding.STATUS_DESC_POTENZIALE_VINCITORE_PREMIO_CONCORSO, this.CurrentChiamanteCampagna.New_chiamantecampagnaId, this.CurrentCampaign);
                //this.CurrentCrmAdapter.UpdateChiamanteCampagna(chiamanteCampagna);
                //throw new Exception("aggiorno ed esco con un messaggio");
                //si aggiorna e si esce
            }
        }
        /// <summary>
        /// Inserisce un nuovo chiamante campagn
        /// </summary>
        /// <param name="idCall"></param>
        /// <param name="callType"></param>
        /// <param name="cryptedCode"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="idOperator"></param>
        private void InsertChiamanteCampagna(string callType, string cryptedCode, string phoneNumber)
        {
            int vinciteRimanenti;
            bool privacy = false;
            int chiamateResidue;

            NewMessage(LogLevel.Message, "InsertChiamanteCampagna 1 creo o recupero il lead");
            ///Estraggo il lead esistente o creando un lead nuovo
            this.CurrentLead = this.GetNewOrExistingLead(cryptedCode, phoneNumber, this.CurrentOwner(this.CurrentCampaign));

            NewMessage(LogLevel.Message, "Recupero il Lead IDLEAD: " + this.CurrentLead.LeadId.ToString());
            ///Recupero il premio della campagna
            New_premio premio = this.CurrentAccessLayer.GetPremioByIdCampaign(this.CurrentCampaign.CampaignId);
            if (premio == null)
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_MISS_TEMPLATE_PREMIO, StatusTranscoding.STATUS_DESC_MISS_TEMPLATE_PREMIO, Guid.Empty, this.CurrentCampaign);

            ///Recupero le vincite rimanenti dal premio
            if (premio.New_PremioRicorrente.HasValue)
                vinciteRimanenti = Convert.ToInt32(premio.New_PremioRicorrente.Value);
            else
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NULL_PREMIO_RICORRENTE, StatusTranscoding.STATUS_DESC_NULL_PREMIO_RICORRENTE, Guid.Empty, this.CurrentCampaign);

            ///Recupero il valore della privacy
            ///A SECONDA DEL TARGET DELLA CAMPAGNA IMPOSTO LA PRIVACY PER IL CHIAMANTE CAMPAGNA PRENDENDO DAL LEAD
            if (this.CurrentCampaign.New_TipoTarget == (int)DataAccessLayer.CampaignTipoTarget.T892424)
            {
                if (this.CurrentLead.New_Privacy.HasValue)
                    privacy = this.CurrentLead.New_Privacy.Value;
                else
                {
                    
                    privacy = false;
                }
                //{
                //    privacy = this.CurrentLead.New_Privacy.Value;
                //}
                //else
                //    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NULL_PRIVACY_LEAD, StatusTranscoding.STATUS_DESC_NULL_PRIVACY_LEAD, Guid.Empty, this.CurrentCampaign);
            }
            else if (this.CurrentCampaign.New_TipoTarget == (int)DataAccessLayer.CampaignTipoTarget.T1240)
            {
                if (this.CurrentLead.New_Privacy12.HasValue)
                    privacy = this.CurrentLead.New_Privacy12.Value;
                else
                    privacy = false;
                //{
                //    privacy = this.CurrentLead.New_Privacy12.Value;
                //}
                //else
                //    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NULL_PRIVACY_LEAD, StatusTranscoding.STATUS_DESC_NULL_PRIVACY_LEAD, Guid.Empty, this.CurrentCampaign);
            }
            else
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NULL_PRIVACY_LEAD, StatusTranscoding.STATUS_DESC_NULL_PRIVACY_LEAD, Guid.Empty, this.CurrentCampaign);
            ///Recupero il numero di chiamate
            if (this.CurrentCampaign.New_NumeroChiamateSoglia.HasValue)
                chiamateResidue = this.CurrentCampaign.New_NumeroChiamateSoglia.Value;
            else
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NULL_CHIAMATE_SOGLIA, StatusTranscoding.STATUS_DESC_NULL_CHIAMATE_SOGLIA, Guid.Empty, this.CurrentCampaign);
            ///Popolo oggetto chiamante campagna da storare
            new_chiamantecampagna chiamanteCampagna = new new_chiamantecampagna()
            {
                new_campagnaid = base.CreaLookup(EntityName.campaign.ToString(), this.CurrentCampaign.CampaignId),
                new_partecipanteid = base.CreaLookup(EntityName.lead.ToString(), this.CurrentLead.LeadId),
                new_statopartecipante = new Picklist() { Value = (int)DataAccessLayer.StatoPartecipanteInChiamanteCampagna.PotenzialeVincitore },
                new_name = cryptedCode.ToUpper(),
                new_vinciterimanenti = new CrmNumber() { Value = vinciteRimanenti },
                new_privacy = privacy ? new Picklist() { Value = (int)DataAccessLayer.Privacy.Si } : new Picklist() { Value = (int)DataAccessLayer.Privacy.NonSo },
                new_chiamateresidue = new CrmNumber() { Value = chiamateResidue - 1 }//,
                //new_numerotelefono = (this.CurrentCallType.New_Cifratura.HasValue && !this.CurrentCallType.New_Cifratura.Value) ? phoneNumber : string.Empty
            };

            NewMessage(LogLevel.Message, "InsertChiamanteCampagna 2 creo il chiamante campagna");
            ///Prendo il risultato della create
            Guid idChiamanteCampagna = this.CurrentCrmAdapter.CreateChiamanteCampagna(chiamanteCampagna, this.CurrentOwner(this.CurrentCampaign));
            if (idChiamanteCampagna != Guid.Empty)
                this.CurrentChiamanteCampagna = this.CurrentAccessLayer.GetChiamanteCampagnaById(idChiamanteCampagna);
            else
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_CREATE_CHIAMANTE_CAMPAGNA, StatusTranscoding.STATUS_DESC_CREATE_CHIAMANTE_CAMPAGNA, Guid.Empty, this.CurrentCampaign);
            NewMessage(LogLevel.Message, "Ho creato il nuovo chiamante campagna ID: " + this.CurrentChiamanteCampagna.New_chiamantecampagnaId.ToString());
            ///todo: 3 COSE ID/CRYPTED/PRIVACY CONTROLLO SE IL FLAG PRIVACY è == NON SO ESCO CON  UN CODICE
            ///ALTRIMENTI CONTINUO
            ///Se la privacy è impostata a NON SO ed esco con il codice errore
            if (this.CurrentChiamanteCampagna.New_Privacy.Value == (int)DataAccessLayer.Privacy.NonSo)
            {
                NewMessage(LogLevel.Message, "Valore della privacy su chiamante campagna impostato a NON SO, esco con codice " + StatusTranscoding.STATUS_CODE_NONSO_PRIVACY_CHIAMANTE_CAMPAGNA);
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NONSO_PRIVACY_CHIAMANTE_CAMPAGNA, StatusTranscoding.STATUS_DESC_NONSO_PRIVACY_CHIAMANTE_CAMPAGNA, this.CurrentChiamanteCampagna.New_chiamantecampagnaId, this.CurrentCampaign);
            }
            ///CONTROLLO LA PRIVACY????
            /////////da ora in poi si fà update 
            //////if (chiamanteCampagna.New_Privacy.Value == (int)DataAccessLayer.Privacy.NonSo)
            //////{
            //////    NewMessage(LogLevel.Message, "Valore della privacy su chiamante campagna impostato a NON SO, esco con codice " + StatusTranscoding.STATUS_CODE_NONSO_PRIVACY_CHIAMANTE_CAMPAGNA);
            //////    ///esco codice non sò
            //////    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NONSO_PRIVACY_CHIAMANTE_CAMPAGNA, StatusTranscoding.STATUS_DESC_NONSO_PRIVACY_CHIAMANTE_CAMPAGNA, chiamanteCampagna.New_chiamantecampagnaId, this.CurrentCampaign);
            //////}
            ///Controllo se ha vinto
            if (chiamanteCampagna.new_chiamateresidue.Value <= 0)
            {
                NewMessage(LogLevel.Message, "Ho vinto il premio e vado alla gestione del premio");
                this.CheckPrize(this.CurrentChiamanteCampagna, cryptedCode, phoneNumber);
            }
            else
            {
                NewMessage(LogLevel.Message, "Ho creato il chiamante campagna ed esco come potenziale vincitore");
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_POTENZIALE_VINCITORE_PREMIO_CONCORSO, StatusTranscoding.STATUS_DESC_POTENZIALE_VINCITORE_PREMIO_CONCORSO, this.CurrentChiamanteCampagna.New_chiamantecampagnaId, this.CurrentCampaign);
            }
            ///DEVO AGGIORNARE QUI????
            //else
            //{
            //    //this.CurrentCrmAdapter.UpdateChiamanteCampagna(chiamanteCampagna);
            //    throw new Exception("aggiorno ed esco con un messaggio");
            //    //si aggiorna e si esce
            //}
        }
        /// <summary>
        /// Inserisce una nuova 
        /// </summary>
        private void CreateCall(Guid idChiamante, string phoneCallId, string idOperatore, string callType)
        {
            ///Popolo oggetto call
            new_call call = new new_call()
            {
                new_chiamantecampagnaid = base.CreaLookup(EntityName.new_chiamantecampagna.ToString(), idChiamante),
                new_name = phoneCallId,
                new_operatorid = idOperatore,
                new_calltypeid = base.CreaLookup(EntityName.new_calltype.ToString(), this.CurrentAccessLayer.GetCallTypeByCallTypeName(callType).New_calltypeId)
                
            };
            NewMessage(LogLevel.Message, "Creo una riga sulla tabella Call");
            ///Creo la call
            Guid _guid = this.CurrentCrmAdapter.CreateCall(call, this.CurrentOwner(this.CurrentCampaign));
            ///Errore durante la creazione della call
            if (_guid == Guid.Empty)
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_CREATE_CALL, StatusTranscoding.STATUS_DESC_CREATE_CALL, idChiamante, this.CurrentCampaign);

        }
        /// <summary>
        /// Restituisce un lead esistente in base al cryptedcode o ne crea uno nuovo
        /// </summary>
        /// <param name="cryptedCode"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        private Lead GetNewOrExistingLead(string cryptedCode, string phoneNumber, Owner owner)
        {
            ///Recupero l'eventuale lead associato alla campagna
            Lead _lead = this.CurrentAccessLayer.GetLeadByCryptedCode(cryptedCode);
            if (_lead == null)
            {
                NewMessage(LogLevel.Message, "Pre creazione Lead");
                ///Creazione di un lead nuovo
                lead _crmLead = new lead()
                {
                    new_codicecifratopartecipante = cryptedCode,
                    //telephone1 = (this.CurrentCallType.New_Cifratura.HasValue && !this.CurrentCallType.New_Cifratura.Value) ? phoneNumber : string.Empty, //in base al flag del call type solo se 0 new_cifratura OK
                    new_calltypeid = base.CreaLookup(EntityName.new_calltype.ToString(), this.CurrentCallType.New_calltypeId)
                    //New_Privacy del calltype se 1 = true else 0                    
                };
                _crmLead.new_privacy = new CrmBoolean() { IsNull = true, IsNullSpecified = true };
                _crmLead.new_privacy12 = new CrmBoolean() { IsNull = true, IsNullSpecified = true };
                ///Controllo quale flag privacy aggiornare in base al target della campagna
                if (this.CurrentCampaign.New_TipoTarget.Value == (int)DataAccessLayer.CampaignTipoTarget.T892424)
                {
                    _crmLead.new_privacy = (this.CurrentCallType.New_Privacy.HasValue && this.CurrentCallType.New_Privacy.Value) ? new CrmBoolean() { Value = true } : new CrmBoolean() { Value = false };
                }
                else if (this.CurrentCampaign.New_TipoTarget == (int)DataAccessLayer.CampaignTipoTarget.T1240)
                {
                    _crmLead.new_privacy12 = (this.CurrentCallType.New_Privacy.HasValue && this.CurrentCallType.New_Privacy.Value) ? new CrmBoolean() { Value = true } : new CrmBoolean() { Value = false };
                }
                ///Creazione del nuovo lead
                Guid _idLead = this.CurrentCrmAdapter.CreateLead(_crmLead, owner);
                ///Ritorno il lead appena creato
                return this.CurrentAccessLayer.GetLeadById(_idLead);
            }
            else
            {
                NewMessage(LogLevel.Message, "Lead Trovato ID: " + _lead.LeadId.ToString());
                ///LEAD TROVATO
                ///Controllo quale flag privacy aggiornare in base al target della campagna
                if (this.CurrentCampaign.New_TipoTarget.Value == (int)DataAccessLayer.CampaignTipoTarget.T892424)
                {
                    ///Aggiorno la privacy del lead trovato!
                    if (!_lead.New_Privacy.HasValue)
                    {
                        NewMessage(LogLevel.Message, "Lead nell campagna 862424 e valore di New_Privacy a NULL quindi aggiorno la privacy con il valore del calltype");
                        lead _crmLead = new lead()
                        {
                            leadid = new Key() {Value = _lead.LeadId},
                            new_privacy = (this.CurrentCallType.New_Privacy.HasValue && this.CurrentCallType.New_Privacy.Value) ? new CrmBoolean() { Value = true } : new CrmBoolean() { Value = false }

                        };
                        NewMessage(LogLevel.Message, "Aggiorno il lead con i valori di default della privacy");
                        //update Lead
                        this.CurrentCrmAdapter.UpdateLead(_crmLead);
                        ///Aggiorno la privacy
                        _lead = this.CurrentAccessLayer.GetLeadById(_lead.LeadId);
                    }
                }
                else if (this.CurrentCampaign.New_TipoTarget == (int)DataAccessLayer.CampaignTipoTarget.T1240)
                {
                    ///Aggiorno la privacy del lead trovato!
                    if (!_lead.New_Privacy12.HasValue)
                    {
                        NewMessage(LogLevel.Message, "Lead nell campagna 1240 e valore di New_Privacy12 a NULL quindi aggiorno la privacy con il valore del calltype");
                        lead _crmLead = new lead()
                        {
                            leadid = new Key() { Value = _lead.LeadId },
                            new_privacy12 = (this.CurrentCallType.New_Privacy.HasValue && this.CurrentCallType.New_Privacy.Value) ? new CrmBoolean() { Value = true } : new CrmBoolean() { Value = false }

                        };
                        NewMessage(LogLevel.Message, "Aggiorno il lead con i valori di default della privacy12");
                        //update Lead
                        this.CurrentCrmAdapter.UpdateLead(_crmLead);
                        ///Aggiorno la privacy
                        _lead = this.CurrentAccessLayer.GetLeadById(_lead.LeadId); 
                    }
                }
            }
            ///Lead esistente
            return _lead;
        }
        /// <summary>
        /// Controlla se la campagna è attiva in base alla data e fascia oraria
        /// </summary>
        /// <param name="currentCampaign"></param>
        /// <returns></returns>
        private bool CheckCurrentCampaign(Campaign currentCampaign)
        {
            ///Campagna corrente....
            ///giorno attuale == giorno sulla campagna attivo = true
            ///verificare ora attuale == prendo dalla campagna le fascie attive...
            ///e confronto che l'ora attuale sia compresa nelle fasce attive...
            ///se nessuna è attiva si considera valida.
            /// Pensare ad un messaggio parlante
            ///Controllo che il giorno attuale appartiene ad una giorno della campagna attivo;

            DateTime dataInizio, dataFine;
            if (currentCampaign.ActualStart.HasValue)
                dataInizio = currentCampaign.ActualStart.Value;
            else
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NULL_DATA_ACTUALSTART, StatusTranscoding.STATUS_DESC_NULL_DATA_ACTUALSTART, Guid.Empty, null);
            if (currentCampaign.ActualEnd.HasValue)
                dataFine = currentCampaign.ActualEnd.Value;
            else
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NULL_DATA_ACTUALEND, StatusTranscoding.STATUS_DESC_NULL_DATA_ACTUALEND, Guid.Empty, null);
            ///Controllo che la data ora attuale sia compresa nel range
            if (dataInizio.ToLocalTime() > DateTime.Now || dataFine.ToLocalTime() < DateTime.Now)
            {
                NewMessage(LogLevel.Message, "Campagna non risulta attiva nell'istante attuale");
                return false;
            }

            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    if (!currentCampaign.New_Venerdi.HasValue || !currentCampaign.New_Venerdi.Value)
                    {
                        NewMessage(LogLevel.Message, "Campagna non trovata Venerdi");
                        return false;
                    }
                    break;
                case DayOfWeek.Monday:
                    if (!currentCampaign.New_Lunedi.HasValue || !currentCampaign.New_Lunedi.Value)
                    {
                        NewMessage(LogLevel.Message, "Campagna non trovata Lunedi");
                        return false;
                    }
                    break;
                case DayOfWeek.Saturday:
                    if (!currentCampaign.New_Sabato.HasValue || !currentCampaign.New_Sabato.Value)
                    {
                        NewMessage(LogLevel.Message, "Campagna non trovata Sabato");
                        return false;
                    }
                    break;
                case DayOfWeek.Sunday:
                    if (!currentCampaign.New_Domenica.HasValue || !currentCampaign.New_Domenica.Value)
                    {
                        NewMessage(LogLevel.Message, "Campagna non trovata Domenica");
                        return false;
                    }
                    break;
                case DayOfWeek.Thursday:
                    if (!currentCampaign.New_Giovedi.HasValue || !currentCampaign.New_Giovedi.Value)
                    {
                        NewMessage(LogLevel.Message, "Campagna non trovata Giovedi");
                        return false;
                    }
                    break;
                case DayOfWeek.Tuesday:
                    if (!currentCampaign.New_Martedi.HasValue || !currentCampaign.New_Martedi.Value)
                    {
                        NewMessage(LogLevel.Message, "Campagna non trovata Martedi");
                        return false;
                    }
                    break;
                case DayOfWeek.Wednesday:
                    if (!currentCampaign.New_Mercoledi.HasValue || !currentCampaign.New_Mercoledi.Value)                  
                    {
                        NewMessage(LogLevel.Message, "Campagna non trovata Mercoledi");
                        return false;
                    }                    
                    break;
                default:
                    break;
            }
            bool fascia1Always, fascia2Always, fascia3Always;
            bool fascia1Ok, fascia2Ok, fascia3Ok;
            fascia1Ok = fascia2Ok = fascia3Ok = false;
            ///Se una fascia ha valore true, vuol dire che non è sempre attiva ma che bisogna controllare gli orari (tratto null o false come "sempre attiva")
            fascia1Always = !(currentCampaign.New_Fascia1.HasValue && currentCampaign.New_Fascia1.Value);
            fascia2Always = !(currentCampaign.New_Fascia2.HasValue && currentCampaign.New_Fascia2.Value);
            fascia3Always = !(currentCampaign.New_Fascia3.HasValue && currentCampaign.New_Fascia3.Value);
            ///Se sono tutte sempre attive (non sono settate le fasce), la campagna è attiva
            if (fascia1Always && fascia2Always && fascia3Always)
                return true;
            ///Se non sono tutte sempre attive controllo che l'ora attuale rientri almeno in una delle fasce attive
            if (!fascia1Always)
            {
                if (!currentCampaign.New_DataIniziofascia1.HasValue)
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NULL_DATA_INIZIO_FASCIA1, StatusTranscoding.STATUS_DESC_NULL_DATA_INIZIO_FASCIA1, Guid.Empty, this.CurrentCampaign);
                if (!currentCampaign.New_DataFinefascia1.HasValue)
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NULL_DATA_FINE_FASCIA1, StatusTranscoding.STATUS_DESC_NULL_DATA_FINE_FASCIA1, Guid.Empty, this.CurrentCampaign);
                //Se l'ora attuale è compresa tra l'ora di inizio e quella di fine fascia (l'ora del CRM deve essere portata al fuso orario locale per matchare con l'ora attuale)
                fascia1Ok = currentCampaign.New_DataIniziofascia1.Value.ToLocalTime().TimeOfDay <= DateTime.Now.TimeOfDay && DateTime.Now.TimeOfDay <= currentCampaign.New_DataFinefascia1.Value.ToLocalTime().TimeOfDay;
            }
            if (!fascia2Always)
            {
                if (!currentCampaign.New_DataIniziofascia2.HasValue)
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NULL_DATA_INIZIO_FASCIA2, StatusTranscoding.STATUS_DESC_NULL_DATA_INIZIO_FASCIA2, Guid.Empty, this.CurrentCampaign);
                if (!currentCampaign.New_DataFinefascia2.HasValue)
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NULL_DATA_FINE_FASCIA2, StatusTranscoding.STATUS_DESC_NULL_DATA_FINE_FASCIA2, Guid.Empty, this.CurrentCampaign);
                //Se l'ora attuale è compresa tra l'ora di inizio e quella di fine fascia (l'ora del CRM deve essere portata al fuso orario locale per matchare con l'ora attuale)
                fascia2Ok = currentCampaign.New_DataIniziofascia2.Value.ToLocalTime().TimeOfDay <= DateTime.Now.TimeOfDay && DateTime.Now.TimeOfDay <= currentCampaign.New_DataFinefascia2.Value.ToLocalTime().TimeOfDay;
            }
            if (!fascia3Always)
            {
                if (!currentCampaign.New_DataIniziofascia3.HasValue)
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NULL_DATA_INIZIO_FASCIA3, StatusTranscoding.STATUS_DESC_NULL_DATA_INIZIO_FASCIA3, Guid.Empty, this.CurrentCampaign);
                if (!currentCampaign.New_DataFinefascia3.HasValue)
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NULL_DATA_FINE_FASCIA3, StatusTranscoding.STATUS_DESC_NULL_DATA_FINE_FASCIA3, Guid.Empty, this.CurrentCampaign);
                //Se l'ora attuale è compresa tra l'ora di inizio e quella di fine fascia (l'ora del CRM deve essere portata al fuso orario locale per matchare con l'ora attuale)
                fascia3Ok = currentCampaign.New_DataIniziofascia3.Value.ToLocalTime().TimeOfDay <= DateTime.Now.TimeOfDay && DateTime.Now.TimeOfDay <= currentCampaign.New_DataFinefascia3.Value.ToLocalTime().TimeOfDay;
            }

            NewMessage(LogLevel.Message, "Fascio1: " + fascia1Ok + " fascia2: " + fascia2Ok + " fascia3: " + fascia3Ok);
            //La campagna è attiva se l'ora ricade in almeno una delle tre fasce (possono rimanere a false nel caso in cui le fascie non siano proprio attivate)
            return fascia1Ok || fascia2Ok || fascia3Ok;
        }

        /// <summary>
        /// Gestisce lo stato della vittoria
        /// </summary>
        /// <returns></returns>
        private void CheckPrize(New_chiamantecampagna chiamanteCampagna, string cryptedCode, string phoneNumber)
        {

            ///Recupero il template Premio
            New_premio premio = this.CurrentAccessLayer.GetPremioByIdCampaign(this.CurrentCampaign.CampaignId);
            if (premio == null)
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_MISS_TEMPLATE_PREMIO, StatusTranscoding.STATUS_DESC_MISS_TEMPLATE_PREMIO, Guid.Empty, this.CurrentCampaign);

            ///Se automatico continuo nella gestione Premio vincita
            ///altrimenti esco con un messaggio aspettandomi che venga richiamato un'altro WS
            if (premio.New_ModalitaComPremio == (int)DataAccessLayer.TemplatePremioModalitaComunicazionePremio.Automatica)
            {
                //if numero inizia con 0
                if (phoneNumber.StartsWith("0", true, CultureInfo.InvariantCulture))
                {
                    ///Gestisco come se fosse manuale
                    NewMessage(LogLevel.Message, "Il numero di telefono inzia per 0 e quindi lo tratto come se fosse MANUALE tel number: " + phoneNumber);
                    throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_VINCITORE_MANUALE, StatusTranscoding.STATUS_DESC_VINCITORE_MANUALE, chiamanteCampagna.New_chiamantecampagnaId, this.CurrentCampaign);
                }
                ///continuo con la gestione manuale
                NewMessage(LogLevel.Message, "Gestione Premio AUTOMATICA - vado a discriminare sul premio");
                ///Continuo su gestione del premio in quanto non ho bisogno di interagire per ottenere il numero di telefono
                this.GetManagementPrize(chiamanteCampagna.New_chiamantecampagnaId, cryptedCode, phoneNumber);
            }
            else
            {
                NewMessage(LogLevel.Message, "Gestione Premio MANUALE - ritorno lo stato di vincitore manuale " + StatusTranscoding.STATUS_CODE_VINCITORE_MANUALE);
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_VINCITORE_MANUALE, StatusTranscoding.STATUS_DESC_VINCITORE_MANUALE, chiamanteCampagna.New_chiamantecampagnaId, this.CurrentCampaign);
            }
        }
        
        #endregion
    }
}
