using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Crm.SdkTypeProxy;
using Reply.Iveco.LeadManagement.Presenter.Model;
using System.Globalization;
using Microsoft.Crm.Sdk;

namespace Reply.Iveco.LeadManagement.Presenter
{
    /// <summary>
    /// Gestione della BL
    /// </summary>
    public partial class LeadManagementController : BaseLeadManagementController, IDisposable
    {
        #region PUBLIC METHODS
        /// <summary>
        /// Test Operatore proposto DA CANCELLARE
        /// </summary>
        /// <param name="languageName"></param>
        /// <param name="countryName"></param>
        /// <param name="typeService"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public Proposal TestProposalOperator(string languageName, string countryName, DataConstant.TypeService typeService, DateTime startDate, DateTime endDate)
        {
            ///Recupero offset
            int currentOffset = base.CurrentDataAccessLayer.GetOffsetDateByCurrentUser();
            ///Ottengo il country
            New_country country = base.CurrentDataAccessLayer.GetCountryByCountryName(countryName);
            ///Ottengo la lingua
            New_language _language = base.CurrentDataAccessLayer.GetLanguageByLanguageName(languageName);
            ///Recupero il tipo di servizio
            Service _service = base.CurrentDataAccessLayer.GetServiceByTypeAndLanguage(typeService, _language.New_languageId);

            ///Data inizio / fine per ASAP
            ///Calcolo i valori in base a quelli impostati sulla language
            if (typeService == DataConstant.TypeService.ASAP)
            {
                startDate = DateTime.Now.ToUniversalTime().AddHours(country.New_ASAPStartHours.Value);
                endDate = startDate.AddDays(country.New_ASAPEndDays.Value);
            }
            ///Recupero dal servizio del CRM l'operatore libero per i parametri passati in ingresso.
            return base.CurrentDataAccessLayer.FindFreeSlotOperator(_service, _language, countryName, startDate, endDate, currentOffset);
        
        }       
        /// <summary>
        /// Set management da ASAP
        /// </summary>
        /// <param name="callbackData"></param>
        /// <param name="countryName"></param>
        public void SetAppointmentAsap(CallBackData callbackData)
        {
            try
            {
                this.SetAppointment(callbackData, callbackData.Nation, DataConstant.TypeService.ASAP,DateTime.MinValue,DateTime.MinValue,DataConstant.CallBackSource.WsAsap,Guid.Empty);
            }
            catch (SetAppointmentException appEx)
            {
                ///Aggiorno il callback data indicando il record come errore su tabella
                base.CurrentDataAccessLayer.UpdateCallBackDataStatus(appEx.IdCallBackData, DataConstant.CallBackDataState.Error, appEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                if (ex is ICustomException && ((ICustomException)ex).IdCallBackData != Guid.Empty)
                {
                    base.CurrentDataAccessLayer.UpdateCallBackDataStatus(((ICustomException)ex).IdCallBackData, DataConstant.CallBackDataState.Error, string.Format(CultureInfo.InvariantCulture, "{0} {1}", ((ICustomException)ex).Code, ((ICustomException)ex).Descr));
                }
                throw;
            }
        }
        /// <summary>
        /// Set management da Booking
        /// </summary>
        /// <param name="callbackData"></param>
        /// <param name="countryName"></param>
        public void SetAppointmentBooking(CallBackData callbackData,DateTime startDate,DateTime endDate)
        {
            try
            {
                this.SetAppointment(callbackData, callbackData.Nation, DataConstant.TypeService.BOOKING, startDate, endDate, DataConstant.CallBackSource.WsBooking, Guid.Empty);
            }
            catch (SetAppointmentException appEx)
            {
                ///Aggiorno il callback data indicando il record come errore su tabella
                base.CurrentDataAccessLayer.UpdateCallBackDataStatus(appEx.IdCallBackData, DataConstant.CallBackDataState.Error, appEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                if (ex is ICustomException && ((ICustomException)ex).IdCallBackData != Guid.Empty)
                {
                    base.CurrentDataAccessLayer.UpdateCallBackDataStatus(((ICustomException)ex).IdCallBackData, DataConstant.CallBackDataState.Error, string.Format(CultureInfo.InvariantCulture,"{0} {1}", ((ICustomException)ex).Code, ((ICustomException)ex).Descr));
                }
                throw;
            }
        }
        /// <summary>
        /// Inserimento massivo di appointment Asap
        /// </summary>
        /// <param name="callBackData"></param>
        public SetMassiveAppointmentAsapReturn SetMassiveAppointmentAsap(List<CallBackData> callBackData)
        {
            List<CallBackData> callBackDataError = new List<CallBackData>();
            Func<CallBackData, bool> InsertSingleAppointment = delegate(CallBackData item)
            {
                try
                {
                    ///Inserisce l'appuntamento
                    base.CurrentDataAccessLayer.SetCallBackData(item,DateTime.MinValue,DateTime.MinValue,DataConstant.CallBackSource.UploadFile);
                    return true;
                }
                catch (Exception ex)
                {
                    LoggingUtility.WriteEvent("Error during file upload " + ex.Message + " " + DateTime.Now.ToShortTimeString(), ControllerConstant.LEAD_MANAGEMENT_LOAD_LEAD_EVENT_NAME);
                    return false;
                }
            };
            try
            {
                bool result = false;
                callBackData.ForEach(item =>
                    {
                        ///Inserimento appuntamento
                        result = InsertSingleAppointment(item);
                        if (!result) callBackDataError.Add(item);
                    });
                return new SetMassiveAppointmentAsapReturn()
                {
                    StartNumberRecord = callBackData.Count(),
                    ErrorLoadRecord = callBackDataError.Count()
                };
            }
            catch (OperatorNotFoundException opex)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        /// <summary>
        /// Processo i file caricati (Lead) chiamando ripetutamente la set appointemt
        /// </summary>
        public void ProcessUploadedLead()
        {
            //LoggingUtility.WriteEvent("Start Process Upload Lead " + DateTime.Now.ToShortTimeString(), ControllerConstant.LEAD_MANAGEMENT_LOAD_LEAD_EVENT_NAME);
            List<New_callback> callBackDataToProcess = null;
            CallBackData callBackData = null;

            try
            {
                //LoggingUtility.WriteEvent("Entro" + DateTime.Now.ToShortTimeString(), ControllerConstant.LEAD_MANAGEMENT_LOAD_LEAD_EVENT_NAME);
                //base.CurrentDataAccessLayer.UpdateCallBackDataStatus(new Guid("72330641-524E-E011-8B8A-005056326A2D"), DataConstant.CallBackDataState.Error, string.Format(CultureInfo.InvariantCulture, "{0} {1}", "002", "Entro"));

                ///Leggo tutti i valori dalla tabella callback data
                callBackDataToProcess = base.CurrentDataAccessLayer.GetCallBackDatasBySourceAndState(DataConstant.CallBackDataState.ToCreate, DataConstant.CallBackSource.UploadFile);
                ///base.CurrentDataAccessLayer.UpdateCallBackDataStatus(new Guid("72330641-524E-E011-8B8A-005056326A2D"), DataConstant.CallBackDataState.Error, string.Format(CultureInfo.InvariantCulture, "{0} {1}", "002", "Entro"));

            }
            catch (Exception ex) 
            {
                //string lines = ex.Message;

                // Write the string to a file.
                //System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\WSLoadLead\\a.text", true);
                //file.WriteLine(lines);

                //file.Close();
                if(ex is ICustomException)
                    base.CurrentDataAccessLayer.UpdateCallBackDataStatus(((ICustomException)ex).IdCallBackData, DataConstant.CallBackDataState.Error, string.Format(CultureInfo.InvariantCulture, "{0} {1}", ((ICustomException)ex).Code, ((ICustomException)ex).Descr));

                //LoggingUtility.WriteEvent("Error during retrive call backdata " + ex.Message + " " + ex.StackTrace + " " + DateTime.Now.ToShortTimeString(), ControllerConstant.LEAD_MANAGEMENT_LOAD_LEAD_EVENT_NAME);
                return;
            }
            ///Esco se non trovo niente
            if (callBackDataToProcess == null || callBackDataToProcess.Count <= 0) return;
            
            //LoggingUtility.WriteEvent("Founded: " + callBackDataToProcess.Count.ToString() + " " + DateTime.Now.ToShortTimeString(), ControllerConstant.LEAD_MANAGEMENT_LOAD_LEAD_EVENT_NAME);
            
            ///Converto il record new_callback in callbackData
            callBackDataToProcess.ForEach(newCallBackData =>
                {
                    ///Ottengo l'oggetto callbackdata
                    callBackData = this.NewCallBackToCallBackData(newCallBackData);
                    
                    try
                    {
                        ///Per ognuno lancio la SetAppointment
                        this.SetAppointment(callBackData, callBackData.Nation, DataConstant.TypeService.ASAP, DateTime.MinValue, DateTime.MinValue, DataConstant.CallBackSource.UploadFile, newCallBackData.New_callbackId);
                        //base.CurrentDataAccessLayer.UpdateCallBackDataStatus(newCallBackData.New_callbackId, DataConstant.CallBackDataState.Done, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        if (ex is ICustomException && ((ICustomException)ex).IdCallBackData != Guid.Empty)
                        {
                            ///Aggiorno lo stato di errore
                            base.CurrentDataAccessLayer.UpdateCallBackDataStatus(((ICustomException)ex).IdCallBackData, DataConstant.CallBackDataState.Error, string.Format(CultureInfo.InvariantCulture, "{0} {1}", ((ICustomException)ex).Code, ((ICustomException)ex).Descr));
                        }
                        else if (ex is SetAppointmentException)
                        {
                            ///Aggiorno lo stato di errore
                            base.CurrentDataAccessLayer.UpdateCallBackDataStatus(((SetAppointmentException)ex).IdCallBackData, DataConstant.CallBackDataState.Error, string.Format(CultureInfo.InvariantCulture, "{0} {1}", ((SetAppointmentException)ex).Message, ((SetAppointmentException)ex).StackTrace));
                        }
                        LoggingUtility.Error("Error during Set Appointment: " + ex.Message + " " + ex.StackTrace + " " + ((ex.InnerException != null)? ex.InnerException.StackTrace : string.Empty) + " " + DateTime.Now.ToShortTimeString(), ControllerConstant.LEAD_MANAGEMENT_LOAD_LEAD_EVENT_NAME);

                    }
                });

           // LoggingUtility.WriteEvent("Finisched " + DateTime.Now.ToShortTimeString(), ControllerConstant.LEAD_MANAGEMENT_LOAD_LEAD_EVENT_NAME);
        }
        /// <summary>
        /// Restituisce la lista delle lead category
        /// </summary>
        /// <returns></returns>
        public List<PickListValue> GetPickListLeadCategoryClosed()
        {
            ///Lista di valori piclist da portare in pagina ovviamente tutto schiantata ignorantemente nel codice
            int[] arrAttributevalue = new int[] {6,7,8,9,10,11,12,13,14,15,16,17,22};
            ///Recupero i valori dalla persistenza e filtro per i valori qui sopra
            return this.CurrentDataAccessLayer.GetPickListValueByObjectTypeCodeAndPicklistName(ControllerConstant.LEAD_OBJECTTYPECODE, "new_leadcategory").Where(c => arrAttributevalue.Contains(c.AttributeValue)).ToList();
        }
        /// <summary>
        /// Restituisce la lista delle lead category
        /// </summary>
        /// <returns></returns>
        public List<PickListValue> GetPickListLeadCategoryClosedCsi()
        {
            ///Lista di valori piclist da portare in pagina ovviamente tutto schiantata ignorantemente nel codice
            int[] arrAttributevalue = new int[] {19,20,21};
            ///Recupero i valori dalla persistenza e filtro per i valori qui sopra
            return this.CurrentDataAccessLayer.GetPickListValueByObjectTypeCodeAndPicklistName(ControllerConstant.LEAD_OBJECTTYPECODE, "new_leadcategory").Where(c => arrAttributevalue.Contains(c.AttributeValue)).ToList();
        }
        /// <summary>
        /// Restituisce tutti i brand
        /// </summary>
        /// <returns></returns>
        public List<PickListValue> GetAllBrand()
        {
            return this.CurrentDataAccessLayer.GetPickListValueByObjectTypeCodeAndPicklistName(ControllerConstant.LEAD_OBJECTTYPECODE, "new_brand").ToList();
        }
        /// <summary>
        /// Restituisce tutti i channel
        /// </summary>
        /// <returns></returns>
        public List<PickListValue> GetAllChannel()
        {
            return this.CurrentDataAccessLayer.GetPickListValueByObjectTypeCodeAndPicklistName(ControllerConstant.LEAD_OBJECTTYPECODE, "new_channel").ToList();
        }
        /// <summary>
        /// Restituisce tutti i type contact
        /// </summary>
        /// <returns></returns>
        public List<PickListValue> GetAllTypeContact()
        {
            return this.CurrentDataAccessLayer.GetPickListValueByObjectTypeCodeAndPicklistName(ControllerConstant.LEAD_OBJECTTYPECODE, "new_typecontact").ToList();
        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Converto l'oggetto new call back in callbackdata
        /// </summary>
        /// <param name="newCallBack"></param>
        /// <returns></returns>
        private CallBackData NewCallBackToCallBackData(New_callback newCallBack)
        {
            try
            {
                CallBackData callBack = new CallBackData()
                {
                    Address = newCallBack.New_address + string.Empty,
                    Brand = newCallBack.New_brand + string.Empty,
                    CabType = newCallBack.New_cabtype+ string.Empty,
                    Canale = newCallBack.New_canale+ string.Empty,
                    City = newCallBack.New_city+ string.Empty,
                    CodePromotion = newCallBack.New_codepromotion+ string.Empty,
                    CommentWebForm = newCallBack.New_commentwebform+ string.Empty,
                    CompanyName = newCallBack.New_companyname+ string.Empty,
                    CustomerName = newCallBack.New_customername+ string.Empty,
                    CustomerSurname = newCallBack.New_customersurname+ string.Empty,
                    DataLeadCreation = (newCallBack.New_dataleadcreation.HasValue)? newCallBack.New_dataleadcreation.Value:DateTime.MinValue,
                    DesideredData = (newCallBack.New_desidereddata.HasValue)?newCallBack.New_desidereddata.Value:DateTime.MinValue,
                    DueDate = (newCallBack.New_duedate.HasValue)?newCallBack.New_duedate.Value:DateTime.MinValue,
                    EMail = newCallBack.New_email+ string.Empty,
                    FlagPrivacy = newCallBack.New_flagprivacy,
                    FlagPrivacyDue = newCallBack.New_flagprivacydue,
                    Fuel = newCallBack.New_fuel+ string.Empty,
                    GVW = newCallBack.New_gvw+ string.Empty,
                    IdCare = newCallBack.New_idcare+ string.Empty,
                    IdLeadSite = newCallBack.New_idleadsite+ string.Empty,
                    InitiativeSource = newCallBack.New_initiativesource+ string.Empty,
                    InitiativeSourceReport = newCallBack.New_initiativesourcereport+ string.Empty,
                    InitiativeSourceReportDetail = newCallBack.New_intiativesourcereportdetail + string.Empty,
                    Language = newCallBack.New_language + string.Empty,
                    MobilePhoneNumber = newCallBack.New_mobilephonenumber + string.Empty,
                    Model = newCallBack.New_model + string.Empty,
                    ModelOfInterest = newCallBack.New_modelofinterest + string.Empty,
                    Nation = newCallBack.New_nation + string.Empty,
                    PhoneNumber = newCallBack.New_phonenumber + string.Empty,
                    Power = newCallBack.New_power + string.Empty,
                    Province = newCallBack.New_province + string.Empty,
                    StockSearchedModel = newCallBack.New_stocksearchedmodel + string.Empty,
                    Suspension = newCallBack.New_suspension + string.Empty,
                    Title = newCallBack.New_title + string.Empty,
                    Type = newCallBack.New_type + string.Empty,
                    TypeContact = newCallBack.New_typecontact + string.Empty,
                    WheelType = newCallBack.New_wheeltype + string.Empty,
                    ZipCode = newCallBack.New_zipcode + string.Empty
                };
                return callBack;
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
        /// <summary>
        /// Controlla il lead duplicati e i relativi dossier
        /// </summary>
        /// <param name="country"></param>
        /// <param name="language"></param>
        /// <param name="service"></param>
        /// <param name="callbackData"></param>
        /// <param name="newLeadId"></param>
        private void CheckLeadDuplicati(New_country country, New_language language,Service service, CallBackData callbackData,Guid newLeadId,DataConstant.TypeService typeService)
        {
            ///Controllo se esiste già un lead con lo stesso numero di telefono
            Contact oldLead = base.CurrentDataAccessLayer.GetLeadDuplicated(callbackData.PhoneNumber, service.ServiceId, newLeadId, new TimeSpan(country.New_DuplicatedLeadDays.Value, 0, 0, 0), country.New_countryId);
            ///Se non è stato trovato nessun lead allora inserisco senza problemi
            if (oldLead == null)
                return;
            ///Recupero il lead appena inserito!! (DA VALUTARE)
            Contact newLead = base.CurrentDataAccessLayer.GetContactById(newLeadId);

            ///Controllo tra i due LEAD chi è il MASTER
            ///PER PRIMA COSA GUARDO SE QUALCUNO è IN PRONTA CONSEGNA
            ///(se c’è un lead “Pronta Consegna” ha sempre priorità e diventa quindi master)
            if (oldLead.New_gototab.Value == (int)DataConstant.TypeGoToTab.PromptDelivery && newLead.New_gototab.Value != (int)DataConstant.TypeGoToTab.PromptDelivery)
            {
                ///Gestisco il master oldLead
                this.ManageDuplicateLead(oldLead, newLead, typeService);
                return;
            }
            else if (newLead.New_gototab.Value == (int)DataConstant.TypeGoToTab.PromptDelivery && oldLead.New_gototab.Value != (int)DataConstant.TypeGoToTab.PromptDelivery)
            {
                ///Gestisco il master newLead
                this.ManageDuplicateLead(newLead, oldLead, typeService);
                return;
            }
            ///ALTRIMENTI CONTROLLO SE QUALCUNO è NUOVO 
            ///(tra un lead “nuovo” e un lead “usato” vince sempre il “nuovo”)
            if (oldLead.New_typecontact.Value == (int)DataConstant.TYPE_CONTACT.NEW && newLead.New_typecontact.Value != (int)DataConstant.TYPE_CONTACT.NEW)
            {
                ///Gestisco il master oldLead
                this.ManageDuplicateLead(oldLead, newLead, typeService);
                return;
            }
            else if (newLead.New_typecontact.Value == (int)DataConstant.TYPE_CONTACT.NEW && oldLead.New_typecontact.Value != (int)DataConstant.TYPE_CONTACT.NEW)
            {
                ///Gestisco il master newLead
                this.ManageDuplicateLead(newLead, oldLead, typeService);
                return;
            }
            ///Altrimenti controllo se quale tra i due ha la data di creazione più vecchia
            ///(tra due lead “nuovi” oppure due lead “usati” vince il lead con data di creazione più vecchia)
            ///Gestisco oldLead come MASTER
            this.ManageDuplicateLead(oldLead, newLead, typeService);
        }
        /// <summary>
        /// Gestisce la duplicazione lead e tratta i relativi dossier
        /// </summary>
        /// <param name="masterLead"></param>
        /// <param name="slaveLead"></param>
        private void ManageDuplicateLead(Contact masterLead, Contact slaveLead,DataConstant.TypeService typeService)
        {
            Service _service = null;
            ///Gestione del lead master/slave
            ///Imposto come slave il lead non master
            base.CurrentDataAccessLayer.UpdateStatusLeadAsDuplicateAndReasigned(slaveLead.ContactId, masterLead.ContactId);
            ///Recuporo per il Lead Slave tutti i lead dove hanno masterleadid uguale a se stesso e vado in aggiornamento
            ///sul quel campo impostando come masterleadid id del attuale lead MASTER
            List<Guid> listIdLeadAssociated = base.CurrentDataAccessLayer.GetLeadSlaveAssociatedByIdLead(slaveLead.ContactId);
            ///Aggiorno tutti questi lead associandoli al current LEAD MASTER
            if (listIdLeadAssociated != null && listIdLeadAssociated.Count > 0)
                listIdLeadAssociated.ForEach(leadToReassign =>
                    {
                        base.CurrentDataAccessLayer.UpdateLeadReasignedToMasterLead(leadToReassign, masterLead.ContactId);
                    });

            ///Ottengo il dossier del lead Slave
            ServiceAppointment dossierLeadSlave = base.CurrentDataAccessLayer.GetActiveDossierByIdLead(slaveLead.ContactId);
            ///Ottengo il dossier del lead MASTER
            ServiceAppointment dossierLeadMaster = base.CurrentDataAccessLayer.GetActiveDossierByIdLead(masterLead.ContactId);
            ///se non trovo i lead allora esco
            if (dossierLeadSlave == null || dossierLeadMaster == null) return;
            ///Assegno il dossier del lead slave al lead master
            base.CurrentDataAccessLayer.AssignDossierToLead(dossierLeadSlave, masterLead.ContactId);

            ///Faccio la compare delle date dossier
            int result = DateTime.Compare(dossierLeadMaster.ScheduledStart.Value, dossierLeadSlave.ScheduledStart.Value);
            if (dossierLeadSlave.New_ResultContact1.HasValue && dossierLeadSlave.New_ResultContact1.Value > 0)
            {
                ///il dossierslave vince perchè ha New_ResultContact1 > 0 e quindi è in lavorazione
                ///Cancello il dossier Slave
                base.CurrentDataAccessLayer.DeleteDossier(dossierLeadMaster.ActivityId);
                ///Recupero il servizio associato al dossier vincente
                _service = base.CurrentDataAccessLayer.GetServiceByDossierId(dossierLeadSlave.ActivityId);
                ///Aggiorno il subject del dossier vincente con il nome del lead vincente
                base.CurrentDataAccessLayer.UpdateSubjectDossier(dossierLeadSlave.ActivityId, string.Format(CultureInfo.InvariantCulture, "{0} - {1} {2} - Scheduled", _service.Name, masterLead.FirstName, masterLead.LastName));
            }
            else if (dossierLeadMaster.New_ResultContact1.HasValue && dossierLeadMaster.New_ResultContact1.Value > 0)
            {
                ///il dossier master vince perchè ha New_ResultContact1 > 0 e quindi è in lavorazione
                ///Cancello il dossier Slave
                base.CurrentDataAccessLayer.DeleteDossier(dossierLeadSlave.ActivityId);
                ///Recupero il servizio associato al dossier vincente
                _service = base.CurrentDataAccessLayer.GetServiceByDossierId(dossierLeadMaster.ActivityId);
                ///Aggiorno il subject del dossier vincente con il nome del lead vincente
                base.CurrentDataAccessLayer.UpdateSubjectDossier(dossierLeadMaster.ActivityId, string.Format(CultureInfo.InvariantCulture, "{0} - {1} {2} - Scheduled", _service.Name, masterLead.FirstName, masterLead.LastName));
            }
            else if (dossierLeadSlave.New_ServiceType.Value == (int)DataConstant.TypeService.ASAP &&
                dossierLeadMaster.New_ServiceType.Value == (int)DataConstant.TypeService.ASAP) ///identifico quale tra i due sia il dossier che deve vivere.
            {
                /// COMPARE
                /// < 0 VUOL DIRE CHE TI è PIU RECENTE DI T2
                /// = 0 VUOL DIRE CHE TI è UGUALE A T2
                /// > 0 VUOL DIRE CHE TI è SUCCESSIVA A T2
                ///Si sceglie quello che è più vicino al datetime.now
                if (result < 0)
                {
                    ///Cancello il dossier Slave
                    base.CurrentDataAccessLayer.DeleteDossier(dossierLeadSlave.ActivityId);
                    ///Recupero il servizio associato al dossier vincente
                    _service = base.CurrentDataAccessLayer.GetServiceByDossierId(dossierLeadMaster.ActivityId);
                    ///Aggiorno il subject del dossier vincente con il nome del lead vincente
                    base.CurrentDataAccessLayer.UpdateSubjectDossier(dossierLeadMaster.ActivityId, string.Format(CultureInfo.InvariantCulture, "{0} - {1} {2} - Scheduled", _service.Name, masterLead.FirstName, masterLead.LastName));
                }
                else
                {
                    ///Cancello il dossier Slave
                    base.CurrentDataAccessLayer.DeleteDossier(dossierLeadMaster.ActivityId);
                    ///Recupero il servizio associato al dossier vincente
                    _service = base.CurrentDataAccessLayer.GetServiceByDossierId(dossierLeadSlave.ActivityId);
                    ///Aggiorno il subject del dossier vincente con il nome del lead vincente
                    base.CurrentDataAccessLayer.UpdateSubjectDossier(dossierLeadSlave.ActivityId, string.Format(CultureInfo.InvariantCulture, "{0} - {1} {2} - Scheduled", _service.Name, masterLead.FirstName, masterLead.LastName));
                }
            }
            else if (dossierLeadSlave.New_ServiceType.Value == (int)DataConstant.TypeService.BOOKING &&
                dossierLeadMaster.New_ServiceType.Value == (int)DataConstant.TypeService.BOOKING)
            {
                ///Si sceglie quello più lontano al datetime.now
                if (result < 0)
                {
                    ///Cancello il dossier Slave
                    base.CurrentDataAccessLayer.DeleteDossier(dossierLeadMaster.ActivityId);
                    ///Recupero il servizio associato al dossier vincente
                    _service = base.CurrentDataAccessLayer.GetServiceByDossierId(dossierLeadSlave.ActivityId);
                    ///Aggiorno il subject del dossier vincente con il nome del lead vincente
                    base.CurrentDataAccessLayer.UpdateSubjectDossier(dossierLeadSlave.ActivityId, string.Format(CultureInfo.InvariantCulture, "{0} - {1} {2} - Scheduled", _service.Name, masterLead.FirstName, masterLead.LastName));
                }
                else
                {
                    ///Cancello il dossier Slave
                    base.CurrentDataAccessLayer.DeleteDossier(dossierLeadSlave.ActivityId);
                    ///Recupero il servizio associato al dossier vincente
                    _service = base.CurrentDataAccessLayer.GetServiceByDossierId(dossierLeadMaster.ActivityId);
                    ///Aggiorno il subject del dossier vincente con il nome del lead vincente
                    base.CurrentDataAccessLayer.UpdateSubjectDossier(dossierLeadMaster.ActivityId, string.Format(CultureInfo.InvariantCulture, "{0} - {1} {2} - Scheduled", _service.Name, masterLead.FirstName, masterLead.LastName));
                }
            }
            else
            {
                ///uno asap e uno booking
                ///Si sceglie quello più lontano al datetime.now
                if (result < 0)
                {
                    ///Cancello il dossier Slave
                    base.CurrentDataAccessLayer.DeleteDossier(dossierLeadMaster.ActivityId);
                    ///Recupero il servizio associato al dossier vincente
                    _service = base.CurrentDataAccessLayer.GetServiceByDossierId(dossierLeadSlave.ActivityId);
                    ///Aggiorno il subject del dossier vincente con il nome del lead vincente
                    base.CurrentDataAccessLayer.UpdateSubjectDossier(dossierLeadSlave.ActivityId, string.Format(CultureInfo.InvariantCulture, "{0} - {1} {2} - Scheduled", _service.Name, masterLead.FirstName, masterLead.LastName));
                }
                else
                {
                    ///Cancello il dossier Slave
                    base.CurrentDataAccessLayer.DeleteDossier(dossierLeadSlave.ActivityId);
                    ///Recupero il servizio associato al dossier vincente
                    _service = base.CurrentDataAccessLayer.GetServiceByDossierId(dossierLeadMaster.ActivityId);
                    ///Aggiorno il subject del dossier vincente con il nome del lead vincente
                    base.CurrentDataAccessLayer.UpdateSubjectDossier(dossierLeadMaster.ActivityId, string.Format(CultureInfo.InvariantCulture, "{0} - {1} {2} - Scheduled", _service.Name, masterLead.FirstName, masterLead.LastName));
                }
            }
        }
        /// <summary>
        /// Set Appointment
        /// </summary>
        /// <param name="callBackData"></param>
        /// <param name="countryName"></param>
        /// <param name="typeService"></param>
        private void SetAppointment(CallBackData callBackData, string countryName, DataConstant.TypeService typeService)
        {
            this.SetAppointment(callBackData,countryName,typeService,DateTime.MinValue,DateTime.MinValue,DataConstant.CallBackSource.WsAsap,Guid.Empty);
        }
        /// <summary>
        /// Set management booking / asap
        /// </summary>
        /// <param name="callbackData"></param>
        /// <param name="countryName"></param>
        /// <param name="typeService"></param>
        private void SetAppointment(CallBackData callBackData, string countryName, DataConstant.TypeService typeService,DateTime startDate,DateTime endDate,DataConstant.CallBackSource callBackSource,Guid callBackDataId)
        {
            Proposal _proposal = null;
            Guid idCallBackData = Guid.Empty;
            try
            {

                ///Controllo se l'id callback data mi viene passato come parametro oppure no.
                ///se non mi viene mandato vuol dire che è una chiamata da WS 
                ///se invece ho idcallbackdata allora vuol dire che è una richiesta per un import massivo
                if (callBackDataId == Guid.Empty)
                {
                    ///Inserisco nella tabella callback il dettaglio del record che mi arriva
                    idCallBackData = base.CurrentDataAccessLayer.SetCallBackData(callBackData, startDate, endDate, callBackSource);
                }
                else
                    idCallBackData = callBackDataId;

                ///controllo i parametri (obligatorietà)
                if (!this.CheckSetAppointmentParameter(callBackData, countryName, typeService, startDate, endDate))
                    throw new InvalidInputParameterException() { IdCallBackData = idCallBackData };

                ///Recupero offset in base all'utente per fixare (debug / release) che lavorano con fusorari differenti
                int currentOffset = base.CurrentDataAccessLayer.GetOffsetDateByCurrentUser();

                ///Ottengo il country
                New_country country = base.CurrentDataAccessLayer.GetCountryByCountryName(countryName);
                ///Sollevo errore se country è null
                if (country == null) throw new CountryNotFoundException() { IdCallBackData = idCallBackData };
                ///Ottengo la lingua
                New_language _language = base.CurrentDataAccessLayer.GetLanguageByLanguageNameOrDefaultNation(callBackData.Language, country);
                ///Controllo che language sia valorizzato
                if (_language == null) throw new LanguageNotFoundException() { IdCallBackData = idCallBackData };
                ///Recupero il tipo di servizio
                Service _service = base.CurrentDataAccessLayer.GetServiceByTypeAndLanguage(typeService, _language.New_languageId);
                ////Controllo se service è stato trovato
                if (_service == null) throw new ServiceTypeNotFoundException() { IdCallBackData = idCallBackData };

                ///Data inizio / fine per ASAP
                ///Calcolo i valori in base a quelli impostati sulla language
                if (typeService == DataConstant.TypeService.ASAP)
                {
                    startDate = DateTime.Now.ToUniversalTime().AddHours(country.New_ASAPStartHours.Value);
                    endDate = startDate.AddDays(country.New_ASAPEndDays.Value);
                }
                ///Recupero dal servizio del CRM l'operatore libero per i parametri passati in ingresso.
                _proposal = base.CurrentDataAccessLayer.FindFreeSlotOperator(_service, _language, countryName, startDate, endDate, currentOffset);

                ///Se il CRM non trova nessun operatore e sono nel servizio di tipo booking allora sollevo un eccezzione
                ///altrimenti se arrivo da un servizio ASAP lo assegno al tem leader
                if (_proposal == null && typeService == DataConstant.TypeService.BOOKING)
                    throw new OperatorNotFoundException() { IdCallBackData = idCallBackData };

                else if (_proposal == null && typeService == DataConstant.TypeService.ASAP)
                {
                    ///Recuper il team leader per lingua / servizio
                    ///OVERBOOKING
                    _proposal = new Proposal()
                    {
                        ServiceId = _service.ServiceId,
                        LanguageId = _language.New_languageId,
                        StartSlotUniversalTime = DateTime.MinValue,
                        StartSlotUserTime = DateTime.MinValue,
                        EndSlotUniversalTime = DateTime.MinValue,
                        EndSlotUserTime = DateTime.MinValue,
                        ProposalUser = new ProposalUser()
                        {
                            Name = string.Empty,
                            IdUser = country.New_TeamLeaderOverbookingId.Value,
                            TypeProposalUser = ProposalUser.TypeUser.TeamLeader
                        }
                    };
                }

                ///Creazione di dossier e lead in base ai dati che arrivano da sito
                ///2)Creazione del LEAD (Contact)
                Guid idLead = base.CurrentDataAccessLayer.CreateLead(callBackData, country.New_LeadAssignedToId.Value, country.New_countryId, _language.New_languageId, typeService, _proposal.StartSlotUniversalTime, currentOffset);
                ///Creo lo used vehicle model
                RelationLeadUsedVehicleModel relationLeadUsedVehicleModel = base.CurrentDataAccessLayer.CreateUsedVehicleModelByFieldName(callBackData, idLead, country.New_countryId);
                ///Aggiorno il lead
                base.CurrentDataAccessLayer.UpdateLeadWithUsedVehicleModel(relationLeadUsedVehicleModel, idLead);
                ///3)Creazione del Dossier 
                Guid idDossier = Guid.Empty;
                ///Se l'utente è uno user correttamene trovato allora metto il datetime dello start/end slot compatibile con quello che mi ha suggerito il webservice crm
                ///altrimenti metto di default la dataora di now + la duration
                if (_proposal.ProposalUser.TypeProposalUser == ProposalUser.TypeUser.User)
                    idDossier = base.CurrentDataAccessLayer.CreateDossier(callBackData, idLead, _proposal.ProposalUser.IdUser, country.New_countryId, _language.New_languageId, _service, _proposal.StartSlotUniversalTime, _proposal.EndSlotUniversalTime, typeService, currentOffset);
                else
                {
                    ///Caso in cui il dossier è assegnato al team leader del servizio
                    idDossier = base.CurrentDataAccessLayer.CreateDossier(callBackData, idLead, _proposal.ProposalUser.IdUser, country.New_countryId, _language.New_languageId, _service, DateTime.Now, DateTime.Now.AddMinutes(_service.Duration), typeService, currentOffset);
                    ///Recupero gli id queue per rifare la riassegnazione
                    Guid? idQueue = base.CurrentDataAccessLayer.GetQueueIdByObjectId(idDossier);
                    ///Recupero id della queue dove destinare il dossier da processare
                    Guid? idTeamLeaderQueue = _language.New_QueueOverbookingId;//base.CurrentDataAccessLayer.GetQueueIdByIdService(_service.ServiceId);
                    ///sE NON TROVO LA QUEUE SOLLEVO ECCEZZIONE
                    if (!idQueue.HasValue || !idTeamLeaderQueue.HasValue)
                        throw new QueueNotFoundException() { IdCallBackData = idCallBackData };
                    ///Assegno il dossier alla queue del team leader
                    base.CurrentDataAccessLayer.AssignDossierToTeamLeaderQueue(idDossier, idTeamLeaderQueue.Value, idQueue.Value);
                }
                ///Sino a qui è la normale creazione del lead / dossier
                ///Ora bisogna eseguire la gestione dei duplica lead
                this.CheckLeadDuplicati(country, _language, _service, callBackData, idLead, typeService);
                ///Aggiorno la riga come eseguita su callback come TUTTO OK
                base.CurrentDataAccessLayer.UpdateCallBackDataStatus(idCallBackData, DataConstant.CallBackDataState.Done, string.Empty);
            }
            catch (InvalidInputParameterException parmex)
            {
                throw;
            }
            catch (QueueNotFoundException queuex)
            {
                throw;
            }
            catch (LanguageNotFoundException langex)
            {
                throw;
            }
            catch (CountryNotFoundException countex)
            {
                throw;
            }
            catch (ServiceTypeNotFoundException servType)
            {
                throw;
            }
            catch (OperatorNotFoundException opex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SetAppointmentException(idCallBackData, ex.Message, ex);
            }
        }
        /// <summary>
        /// Check dei parametri
        /// </summary>
        /// <param name="callBackData"></param>
        /// <param name="countryName"></param>
        /// <param name="typeService"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private bool CheckSetAppointmentParameter(CallBackData callBackData, string countryName, DataConstant.TypeService typeService, DateTime startDate, DateTime endDate)
        {
            ///Controllo che callback data sia valorizzato
            if (callBackData == null)
                return false;
            if (string.IsNullOrEmpty(callBackData.Nation) || string.IsNullOrEmpty(callBackData.PhoneNumber))
                return false;
            if (string.IsNullOrEmpty(countryName))
                return false;
            if (typeService == DataConstant.TypeService.CSI)
                return false;
            ///Controllo che per booking le date siano popolate
            if (typeService == DataConstant.TypeService.BOOKING && (startDate == DateTime.MinValue || endDate == DateTime.MinValue))
                return false;

            return true;
        }
        #endregion
    }
}
