using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Data.Linq;
using Microsoft.Crm.Sdk;
using System.Web.Services.Protocols;
using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Crm.Sdk.Query;
using System.Collections;
using System.Globalization;
using System.Configuration;

namespace Reply.Iveco.LeadManagement.Presenter.Model
{
    public partial class DataAccessLayer : BaseDataAccessLayer, IDisposable
    {
        #region PRIVATE MEMBERS
        private const int DEFAULT_NUMBER_OF_RESULT_FIND_OPERATOR = 10; 
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Restituisce i datio della callback dato i filtri stato e source
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public List<New_callback> GetCallBackDatasBySourceAndState(DataConstant.CallBackDataState state, DataConstant.CallBackSource source)
        {
            return base.CurrentDataContext.New_callbacks.Where(c => c.New_callbackstate.Value == (int)state && c.New_Source.Value == (int)source && c.DeletionStateCode == 0).ToList();
        }

        /// <summary>
        /// Ottiene l'id dell'ultimo listino attivo disponibile
        /// </summary>
        /// <param name="idCountry"></param>
        /// <returns></returns>
        public Guid GetLastActiveVehicleModelListId(Guid idCountry)
        {
            return base.CurrentDataContext.New_vehiclemodellists.Where(c => c.New_CountryId == idCountry && c.DeletionStateCode == 0).OrderByDescending(c => c.New_InsertDate).Select(c => c.New_vehiclemodellistId).FirstOrDefault();
        }
        /// <summary>
        /// Aggiorna il record cambiando stato e descrizione errore
        /// </summary>
        /// <param name="idCallBackData"></param>
        /// <param name="state"></param>
        /// <param name="errorDescription"></param>
        public void UpdateCallBackDataStatus(Guid idCallBackData, DataConstant.CallBackDataState state, string errorDescription)
        {
            if (idCallBackData == Guid.Empty)
                return;

            DynamicEntity _callBackData = new DynamicEntity("new_callback");
            _callBackData.Properties.Add(new KeyProperty("new_callbackid", new Key(idCallBackData)));
            _callBackData.Properties.Add(new PicklistProperty("new_callbackstate", new Picklist((int)state)));
            _callBackData.Properties.Add(new StringProperty("new_statusdescription", errorDescription));
            try
            {
                base.CurrentCrmService.Update(_callBackData);
            }
            catch (SoapException x)
            {
                //throw new Exception("UpdateCallBackDataStatus: " + x.Detail.InnerText);
                throw;
            }
        }
        /// <summary>
        /// Ottiene il current offset in base all'utente corrente
        /// </summary>
        /// <returns></returns>
        public int GetOffsetDateByCurrentUser()
        {
            int offset = 0;
            try
            {
                WhoAmIResponse whoIam = this.CurrentCrmService.Execute(new WhoAmIRequest()) as WhoAmIResponse;
                RetrieveUserSettingsSystemUserRequest _req = new RetrieveUserSettingsSystemUserRequest()
                {
                    ColumnSet = new ColumnSet(new string[] { "timezonebias" }),
                    EntityId = whoIam.UserId,
                    ReturnDynamicEntities = true
                };
                RetrieveUserSettingsSystemUserResponse _res = this.CurrentCrmService.Execute(_req) as RetrieveUserSettingsSystemUserResponse;
                ///Controllo se ho ottenuto un valore valido
                if (_res != null
                    && _res.BusinessEntity is DynamicEntity
                    && ((DynamicEntity)_res.BusinessEntity)["timezonebias"] != null
                    && !((CrmNumber)(((DynamicEntity)_res.BusinessEntity)["timezonebias"])).IsNull)
                    offset = (-1) * ((CrmNumber)(((DynamicEntity)_res.BusinessEntity)["timezonebias"])).Value;
            }
            catch(Exception ex)
            {

            }
            return offset;
        }
        /// <summary>
        /// Inserisce il record di callback data all'interno di una tabella di appoggio con lo stato
        /// PRE-BOOKING
        /// </summary>
        /// <param name="data"></param>
        public Guid SetCallBackData(CallBackData callBackData, DateTime startDate, DateTime endDate,DataConstant.CallBackSource callBackSource)
        {
            DynamicEntity _callBackData = new DynamicEntity("new_callback");
            ///Popolo gli attributi dell'entità
            if (callBackData.DataLeadCreation != DateTime.MinValue)
                _callBackData.Properties.Add(new CrmDateTimeProperty("new_dataleadcreation", new CrmDateTime(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:s}", callBackData.DataLeadCreation))));
            _callBackData.Properties.Add(new StringProperty("new_customername", callBackData.CustomerName));
            if(!string.IsNullOrEmpty(callBackData.FileName))
                _callBackData.Properties.Add(new StringProperty("new_name", callBackData.FileName));
            _callBackData.Properties.Add(new StringProperty("new_customersurname", callBackData.CustomerSurname));
            _callBackData.Properties.Add(new StringProperty("new_address", callBackData.Address));
            _callBackData.Properties.Add(new StringProperty("new_zipcode", callBackData.ZipCode));
            _callBackData.Properties.Add(new StringProperty("new_city", callBackData.City));
            _callBackData.Properties.Add(new StringProperty("new_province", callBackData.Province));
            _callBackData.Properties.Add(new StringProperty("new_nation", callBackData.Nation));// callBackData.Nation));
            _callBackData.Properties.Add(new StringProperty("new_email", callBackData.EMail));
            _callBackData.Properties.Add(new StringProperty("new_phonenumber", callBackData.PhoneNumber));
            _callBackData.Properties.Add(new StringProperty("new_mobilephonenumber", callBackData.MobilePhoneNumber));
            _callBackData.Properties.Add(new CrmBooleanProperty("new_flagprivacy", new CrmBoolean((callBackData.FlagPrivacy.HasValue) ? callBackData.FlagPrivacy.Value : false)));
            _callBackData.Properties.Add(new StringProperty("new_brand", callBackData.Brand));
            _callBackData.Properties.Add(new StringProperty("new_model", callBackData.Model));
            _callBackData.Properties.Add(new StringProperty("new_type", callBackData.Type));
            _callBackData.Properties.Add(new StringProperty("new_gvw", callBackData.GVW));
            _callBackData.Properties.Add(new StringProperty("new_wheeltype", callBackData.WheelType));
            _callBackData.Properties.Add(new StringProperty("new_fuel", callBackData.Fuel));
            _callBackData.Properties.Add(new StringProperty("new_initiativesource", callBackData.InitiativeSource));
            _callBackData.Properties.Add(new StringProperty("new_initiativesourcereport", callBackData.InitiativeSourceReport));
            _callBackData.Properties.Add(new StringProperty("new_intiativesourcereportdetail", callBackData.InitiativeSourceReportDetail));
            _callBackData.Properties.Add(new StringProperty("new_companyname", callBackData.CompanyName));
            _callBackData.Properties.Add(new StringProperty("new_power", callBackData.Power));
            _callBackData.Properties.Add(new StringProperty("new_cabtype", callBackData.CabType));
            _callBackData.Properties.Add(new StringProperty("new_suspension", callBackData.Suspension));
            _callBackData.Properties.Add(new StringProperty("new_idleadsite", callBackData.IdLeadSite));
            _callBackData.Properties.Add(new StringProperty("new_title", callBackData.Title));
            _callBackData.Properties.Add(new StringProperty("new_commentwebform", callBackData.CommentWebForm));
            _callBackData.Properties.Add(new StringProperty("new_language", callBackData.Language));
            _callBackData.Properties.Add(new StringProperty("new_stocksearchedmodel", callBackData.StockSearchedModel));
            _callBackData.Properties.Add(new StringProperty("new_canale", callBackData.Canale));
            if(callBackData.DueDate != DateTime.MinValue)
                _callBackData.Properties.Add(new CrmDateTimeProperty("new_duedate", new CrmDateTime(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:s}", callBackData.DueDate))));
            _callBackData.Properties.Add(new CrmBooleanProperty("new_flagprivacydue", new CrmBoolean((callBackData.FlagPrivacyDue.HasValue) ? callBackData.FlagPrivacyDue.Value : false)));
            _callBackData.Properties.Add(new StringProperty("new_idcare", callBackData.IdCare));
            _callBackData.Properties.Add(new StringProperty("new_codepromotion", callBackData.CodePromotion));
            _callBackData.Properties.Add(new StringProperty("new_modelofinterest", callBackData.ModelOfInterest));
            if(callBackData.DesideredData != DateTime.MinValue)
                _callBackData.Properties.Add(new CrmDateTimeProperty("new_desidereddata", new CrmDateTime(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:s}", callBackData.DesideredData))));
            _callBackData.Properties.Add(new StringProperty("new_typecontact", callBackData.TypeContact));
            if (startDate == DateTime.MinValue || endDate == DateTime.MinValue)
            {
                //_callBackData.Properties.Add(new CrmDateTimeProperty("new_startappointment", new CrmDateTime(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:s}", CrmDateTime.MinValue))));
                //_callBackData.Properties.Add(new CrmDateTimeProperty("new_endappointment", new CrmDateTime(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:s}", CrmDateTime.MinValue))));
            }
            else
            {
                _callBackData.Properties.Add(new CrmDateTimeProperty("new_startappointment", new CrmDateTime(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:s}", startDate))));
                _callBackData.Properties.Add(new CrmDateTimeProperty("new_endappointment", new CrmDateTime(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:s}", endDate))));
            }
            ///inserisco gli stati
            _callBackData.Properties.Add(new PicklistProperty("new_callbackstate", new Picklist((int)DataConstant.CallBackDataState.ToCreate)));
            _callBackData.Properties.Add(new PicklistProperty("new_source", new Picklist((int)callBackSource)));

            ///Creo il callbackdata
            try
            {
                return base.CurrentCrmService.Create(_callBackData);
            }
            catch (SoapException x)
            {
                string y = x.Detail.InnerText;
                //throw new Exception("SetCallBackData: " + x.Detail.InnerText);
                throw;
            }
        }
        /// <summary>
        /// Cancella il dossier logicamente deletetionstatecode = 0
        /// </summary>
        /// <param name="idDossier"></param>
        public void DeleteDossier(Guid idDossier)
        {
            try
            {
                ///Cancella il dossier logicamente deletetionstatecode = 0
                base.CurrentCrmService.Delete("serviceappointment", idDossier);
            }
            catch (SoapException x)
            {
                //throw new Exception("DeleteDossier: " + x.Detail.InnerText);
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// Ottiene il service dato l'iddossier
        /// </summary>
        /// <param name="idDossier"></param>
        /// <returns></returns>
        public Service GetServiceByDossierId(Guid idDossier)
        {
            return (from Service myService in base.CurrentDataContext.Services
                    join ServiceAppointment serviceApp in base.CurrentDataContext.ServiceAppointments
                    on myService.ServiceId equals serviceApp.ServiceId.Value
                    where serviceApp.ActivityId == idDossier
                    select myService).SingleOrDefault();
        }
        /// <summary>
        /// Aggiorna il soggetto del dossier
        /// </summary>
        /// <param name="idDossier"></param>
        /// <param name="subjectToUpdate"></param>
        public void UpdateSubjectDossier(Guid idDossier, string subjectToUpdate)
        {
            DynamicEntity _dossier = new DynamicEntity("serviceappointment");
            _dossier.Properties.Add(new KeyProperty("activityid", new Key(idDossier)));
            _dossier.Properties.Add(new StringProperty("subject", subjectToUpdate));
            try
            {
                ///Aggiorna il dossier 
                base.CurrentCrmService.Update(_dossier);
            }
            catch (SoapException x)
            {
                //throw new Exception("DeleteDossier: " + x.Detail.InnerText);
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// Cambia lo stato a cancellato del lead
        /// </summary>
        /// <param name="idDossier"></param>
        public void ChangeStatusDossierToCanceled(Guid idDossier)
        {
            DataUtility.SetState("serviceappointment", idDossier, ServiceAppointmentState.Canceled.ToString(), DataConstant.DOSSIER_STATUS_CODE_CANCELED, base.CurrentCrmService);
        }
        /// <summary>
        /// Assegno un dossier ad un lead
        /// </summary>
        /// <param name="dossier"></param>
        /// <param name="idLead"></param>
        public void AssignDossierToLead(ServiceAppointment dossier, Guid idLead)
        {
            DynamicEntity _dossier = new DynamicEntity("serviceappointment");
            ///Popolo gli attributi dell'entità
            _dossier.Properties.Add(this.GetDynamicEntityParty("customers", "contact", idLead));
            _dossier.Properties.Add(new KeyProperty("activityid", new Key(dossier.ActivityId)));

            try
            {
                // Execute the request.
                base.CurrentCrmService.Update(_dossier);
            }
            catch (SoapException x)
            {
                //throw new Exception("AssignDossierToLead: " + x.Detail.InnerText);
                throw;
            }
            catch (Exception a)
            {
                throw;
            }
        }
        /// <summary>
        /// Ottiene i dossier attivi associati ad un idlead e di un particolare tipo csi/booking/scheduled
        /// </summary>
        /// <param name="idLead"></param>
        /// <returns></returns>
        public List<ServiceAppointment> GetActviceCsiDossierByIdLeadAndServiceType(Guid idLead,DataConstant.TypeService typeService)
        {
            return (from dossier in base.CurrentDataContext.ServiceAppointments
                    join ActivityParty party in base.CurrentDataContext.ActivityParties
                    on dossier.ActivityId equals party.ActivityId
                    where party.PartyId.Value == idLead &&
                    dossier.New_ServiceType.Value == (int)typeService &&
                    dossier.DeletionStateCode == 0
                    select dossier).ToList();
        }
        /// <summary>
        /// Ottiene il dossier attivo legato ad un lead
        /// </summary>
        /// <param name="idLead"></param>
        /// <returns></returns>
        public ServiceAppointment GetActiveDossierByIdLead(Guid idLead)
        {
            ///Ottengo il dossier attivo legato ad un lead
            return (from dossier in base.CurrentDataContext.ServiceAppointments
                    join ActivityParty party in base.CurrentDataContext.ActivityParties
                    on dossier.ActivityId equals party.ActivityId
                    where party.PartyId.Value == idLead &&
                    dossier.StateCode.Value == (int)ServiceAppointmentState.Scheduled
                    select dossier).SingleOrDefault();
        }
        /// <summary>
        /// Ottiene un contact dato ID
        /// </summary>
        /// <param name="idContact"></param>
        /// <returns></returns>
        public Contact GetContactById(Guid idContact)
        {
            return base.CurrentDataContext.Contacts.Where(c => c.ContactId == idContact).SingleOrDefault();
        }
        /// <summary>
        /// Controllo se esiste a sistema già un lead con lo stesso numero di telefono e con la data minore di una certo span
        /// </summary>
        /// <param name="telephone"></param>
        /// <param name="dataDiff"></param>
        /// <returns></returns>
        public Contact GetLeadDuplicated(string telephone, Guid serviceId, Guid idLeadCompare, TimeSpan dataDiff,Guid idCountry)
        {
            return (from Contact myLead in base.CurrentDataContext.Contacts
                    join ActivityParty myParty in base.CurrentDataContext.ActivityParties
                    on myLead.ContactId equals myParty.PartyId
                    join ServiceAppointment myDossier in base.CurrentDataContext.ServiceAppointments
                    on myParty.ActivityId equals myDossier.ActivityId
                    where myLead.New_leadslave.Value == false &&
                    myLead.New_leadstatus == (int)DataConstant.LeadStatus.Open &&
                    myLead.New_countryid == idCountry &&
                    myLead.Telephone1 == telephone &&
                    myLead.ContactId != idLeadCompare &&
                    (DateTime.Now.Date - myLead.CreatedOn.Value.Date) <= dataDiff &&
                    myParty.PartyObjectTypeCode == 2 &&
                    myDossier.StateCode.Value == (int)ServiceAppointmentState.Scheduled &&
                    myDossier.New_ServiceType.Value != (int)DataConstant.TypeService.CSI
                    select myLead
                              ).SingleOrDefault<Contact>();

        }
        /// <summary>
        /// Ottiene tutti i lead associati ad un lead padre 
        /// </summary>
        /// <param name="idLead"></param>
        /// <returns></returns>
        public List<Guid> GetLeadSlaveAssociatedByIdLead(Guid idLead)
        {
            return base.CurrentDataContext.Contacts.Where(c => c.New_leadslave.Value == true && c.New_masterleadid == idLead).Select(c => c.ContactId).ToList();
        }
        /// <summary>
        /// Aggiorno il campo master lead id con il nuovo id del current lead master
        /// </summary>
        /// <param name="idLeadToReassign"></param>
        /// <param name="idLeadMaster"></param>
        public void UpdateLeadReasignedToMasterLead(Guid idLeadToReassign,Guid idLeadMaster)
        {
            DynamicEntity _lead = new DynamicEntity("contact");
            _lead.Properties.Add(new KeyProperty("contactid", new Key(idLeadToReassign)));
            _lead.Properties.Add(new LookupProperty("new_masterleadid", new Lookup("contact", idLeadMaster)));

            ///Aggiorn o il lead come dupplicato
            try
            {
                ///Aggiorno entità
                base.CurrentCrmService.Update(_lead);
            }
            catch (SoapException x)
            {
                string y = x.Detail.InnerText;
                //throw new Exception("UpdateStatusLeadAsDuplicateAndReasigned: " + x.Detail.InnerText);
                throw;
            }
        }
        /// <summary>
        /// Imposta il LEAD come duplicato
        /// </summary>
        /// <param name="idDossier"></param>
        public void UpdateStatusLeadAsDuplicateAndReasigned(Guid idLead, Guid idLeadMaster)
        {
            DynamicEntity _lead = new DynamicEntity("contact");
            _lead.Properties.Add(new KeyProperty("contactid", new Key(idLead)));
            _lead.Properties.Add(new PicklistProperty("new_leadstatus", new Picklist((int)DataConstant.LeadStatus.Closed)));
            _lead.Properties.Add(new PicklistProperty("new_leadsubstatus", new Picklist((int)DataConstant.LeadSubStatus.Duplicated)));
            _lead.Properties.Add(new PicklistProperty("new_leadcategory", new Picklist(DataConstant.LEAD_LEAD_CATEGORY_DUPLICATE)));
            ///Imposto il lead come slave
            _lead.Properties.Add(new CrmBooleanProperty("new_leadslave", new CrmBoolean(true)));
            _lead.Properties.Add(new LookupProperty("new_masterleadid", new Lookup("contact", idLeadMaster)));

            ///Aggiorn o il lead come dupplicato
            try
            {
                ///Aggiorno entità
                base.CurrentCrmService.Update(_lead);
            }
            catch (SoapException x)
            {
                string y = x.Detail.InnerText;
                //throw new Exception("UpdateStatusLeadAsDuplicateAndReasigned: " + x.Detail.InnerText);
                throw;
            }
        }
        /// <summary>
        /// Restituisce i dettagli della picklist
        /// </summary>
        /// <param name="objectTypeCode"></param>
        /// <param name="pickListValue"></param>
        /// <returns></returns>
        public List<PickListValue> GetPickListValueByObjectTypeCodeAndPicklistName(int objectTypeCode,string pickListValue)
        {
            return (from picklist in this.CurrentDataContext.PickListValues
                     where picklist.ObjectTypeCode == objectTypeCode &&
                     picklist.AttributeName == pickListValue
                    select picklist).ToList();
        }
        /// <summary>
        /// Si occupa di trovare il primo operatore libero in base a dei parametri di data-ora inizio e data-ora fine.
        /// controllo tra tutti gli operatori in servizio in quelle fascie chi è il più scarico.
        /// Vengono proposti 10 risultati in base al range di date al tipo di servizio
        /// </summary>
        /// <param name="service"></param>
        /// <param name="language"></param>
        /// <param name="countryName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="offsetDate"></param>
        /// <param name="numberOfResult"></param>
        /// <returns></returns>
        public List<Proposal> FindFreeSlotOperatorsByIdOperator(Guid operatorId,Service service, New_language language, string countryName, DateTime startDate, DateTime endDate, int offsetDate, int numberOfResult)
        {
            List<Proposal> _proposal = null;
            ///Faccio la Richiesta          
            SearchResponse searched = this.FindOperatorByParameters(service, language, countryName, startDate, endDate.AddHours(23).AddMinutes(59), offsetDate, numberOfResult, new RequiredResource[] { new RequiredResource(operatorId, service.ResourceSpecId) });
            if (searched != null &&
                searched.SearchResults != null &&
                searched.SearchResults.Proposals.Count() > 0)
            {
                _proposal = new List<Proposal>(searched.SearchResults.Proposals.Count());
                ///Ciclo per tutti i risultati trovati
                for (int i = 0; i < searched.SearchResults.Proposals.Count(); i++)
                {
                    _proposal.Add(new Proposal()
                    {
                        ServiceId = service.ServiceId,
                        LanguageId = language.New_languageId,
                        StartSlotUniversalTime = searched.SearchResults.Proposals[i].Start.UniversalTime,
                        StartSlotUserTime = searched.SearchResults.Proposals[i].Start.UserTime,
                        EndSlotUniversalTime = searched.SearchResults.Proposals[i].End.UniversalTime,
                        EndSlotUserTime = searched.SearchResults.Proposals[i].End.UserTime,
                        ProposalUser = new ProposalUser()
                        {
                            Name = searched.SearchResults.Proposals[i].ProposalParties[0].DisplayName,
                            IdUser = searched.SearchResults.Proposals[i].ProposalParties[0].ResourceId,
                            TypeProposalUser = ProposalUser.TypeUser.User
                        }
                    });
                }
            }
            ///Ritorno l'oggetto
            return _proposal;
        }
        /// <summary>
        /// Si occupa di trovare il primo operatore libero in base a dei parametri di data-ora inizio e data-ora fine.
        /// controllo tra tutti gli operatori in servizio in quelle fascie chi è il più scarico.
        /// Vengono proposti 10 risultati in base al range di date al tipo di servizio
        /// </summary>
        /// <param name="typeService"></param>
        /// <param name="languageName"></param>
        /// <param name="countryName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public Proposal FindFreeSlotOperator(Service service, New_language language, string countryName, DateTime startDate, DateTime endDate, int offsetDate)
        {
            Proposal _proposal = null;
            ///Find Operator
            SearchResponse searched = this.FindOperatorByParameters(service, language, countryName, startDate, endDate, offsetDate, DEFAULT_NUMBER_OF_RESULT_FIND_OPERATOR);

            if (searched != null &&
                    searched.SearchResults != null &&
                    searched.SearchResults.Proposals.Count() > 0 &&
                    searched.SearchResults.Proposals[0].ProposalParties.Count() > 0)
            {
                _proposal = new Proposal()
                {
                    ServiceId = service.ServiceId,
                    LanguageId = language.New_languageId,
                    StartSlotUniversalTime = searched.SearchResults.Proposals[0].Start.UniversalTime,
                    StartSlotUserTime = searched.SearchResults.Proposals[0].Start.UserTime,
                    EndSlotUniversalTime = searched.SearchResults.Proposals[0].End.UniversalTime,
                    EndSlotUserTime = searched.SearchResults.Proposals[0].End.UserTime,
                    ProposalUser = new ProposalUser()
                    {
                        Name = searched.SearchResults.Proposals[0].ProposalParties[0].DisplayName,
                        IdUser = searched.SearchResults.Proposals[0].ProposalParties[0].ResourceId,
                        TypeProposalUser = ProposalUser.TypeUser.User
                    }
                };
            }
            ///Ritorno l'oggetto
            return _proposal;
        }
        /// <summary>
        /// Ottiene un oggetto linguaggio dato il nome del linguaggio
        /// </summary>
        /// <param name="languageName"></param>
        /// <returns></returns>
        public New_language GetLanguageByLanguageName(string languageName)
        {
            if (string.IsNullOrEmpty(languageName))
                return null;
            ///Ricerca il linguaggio dato il suo nome
            return base.CurrentDataContext.New_languages.Where(c => c.New_name.ToUpper() == languageName.ToUpper() && c.DeletionStateCode == 0).SingleOrDefault();
        }
        /// <summary>
        /// Restituisce la language dato il nome oppure prendendo quella di defualt della country
        /// </summary>
        /// <param name="languageName"></param>
        /// <param name="idCountry"></param>
        /// <returns></returns>
        public New_language GetLanguageByLanguageNameOrDefaultNation(string languageName,New_country country)
        {
            if(string.IsNullOrEmpty(languageName))
                ///Default preso dalla nation
                return base.CurrentDataContext.New_languages.Where(c => c.New_languageId == country.New_DefaultLanguageId && c.DeletionStateCode == 0).SingleOrDefault();

            ///Cerco la lingua dato il nome
            var language =  base.CurrentDataContext.New_languages.Where(c => c.New_name.ToUpper() == languageName.ToUpper() && c.DeletionStateCode == 0).SingleOrDefault();
            ///Se la trovo restituisco altrimenti restituisco il default
            if (language != null)
                return language;
            ///Default preso dalla nation
            return base.CurrentDataContext.New_languages.Where(c => c.New_languageId == country.New_DefaultLanguageId && c.DeletionStateCode == 0).SingleOrDefault();
        }
        /// <summary>
        /// Ottiene l'oggetto servizio dato idlanguage e il tipo di servizio
        /// </summary>
        /// <param name="typeService"></param>
        /// <param name="idLanguage"></param>
        /// <returns></returns>
        public Service GetServiceByTypeAndLanguage(DataConstant.TypeService typeService, Guid idLanguage)
        {
            return (from service in base.CurrentDataContext.Services
                    join servGrou in base.CurrentDataContext.New_servicesconfigurations
                    on service.ServiceId equals servGrou.New_ServiceId
                    where servGrou.New_LanguageId == idLanguage &&
                    servGrou.New_ServiceType == (int)typeService &&
                    service.DeletionStateCode == 0
                    select service).SingleOrDefault();

        }


        /// <summary>
        /// Restituisce la lista dei vehicle model trovati per la stringa passata
        /// </summary>
        /// <param name="resultModel"></param>
        /// <returns></returns>
        public List<New_vehiclemodel> GetVehicleModelByDescrResultModel(string resultModel,Guid countryId)
        {
            var result = (from New_vehiclemodel rel in base.CurrentDataContext.New_vehiclemodels
                          join New_productmodel m in base.CurrentDataContext.New_productmodels
                            on rel.New_ModelId equals m.New_productmodelId
                          join New_vehiclemodellist l in base.CurrentDataContext.New_vehiclemodellists
                            on m.New_VehicleModelListId equals l.New_vehiclemodellistId
                          where l.New_vehiclemodellistId == (CurrentDataContext.New_vehiclemodellists.Where(c => c.New_CountryId.Value == countryId).OrderByDescending(c => c.CreatedOn.Value).Select(c => c.New_vehiclemodellistId).First())
                          && rel.New_name == resultModel.Trim()
                          select rel).Distinct().ToList();

            return result;
        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Assegno alla queue del team leader la service-appointment appena creata
        /// </summary>
        /// <param name="idEntity"></param>
        /// <param name="endPointQueueId"></param>
        /// <param name="sourceQueueId"></param>
        public void AssignDossierToTeamLeaderQueue(Guid idEntity, Guid endPointQueueId, Guid sourceQueueId)
        {
            try
            {
                RouteRequest route = new RouteRequest();
                route.EndpointId = endPointQueueId;
                route.RouteType = RouteType.Queue;
                route.SourceQueueId = sourceQueueId;
                route.Target = new TargetQueuedServiceAppointment() { EntityId = idEntity };
                base.CurrentCrmService.Execute(route);
            }
            catch (SoapException soap)
            {
                string err = soap.Detail.InnerText;
                throw;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        /// <summary>
        /// Recupero idqueue del tipo di attività appena creata
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public Guid? GetQueueIdByObjectId(Guid objectId)
        {
            return base.CurrentDataContext.QueueItems.Where(c => c.DeletionStateCode == 0 && c.ObjectId == objectId).Select(c => c.QueueId).SingleOrDefault();
        }
        /// <summary>
        /// Ottiene il country dato il suo nome
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public New_country GetCountryByCountryName(string country)
        {
            if (string.IsNullOrEmpty(country))
                return null;
            ///ottiene il country dato il nome
            return base.CurrentDataContext.New_countries.Where(c => c.New_name.ToUpper() == country.ToUpper() && c.DeletionStateCode == 0).SingleOrDefault();
        }
        /// <summary>
        /// Ritorna il modello trovato dato il suo nome e idcountry
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="idCountry"></param>
        /// <returns></returns>
        public New_productmodel GetModelByNameAndCountry(string modelName, Guid idCountry, Guid idLastActiveVehicleModelList)
        {
            if (string.IsNullOrEmpty(modelName) || idCountry == Guid.Empty)
                return null;
            ///Ottiene il modello
            return (from myobj in base.CurrentDataContext.New_productmodels
                    join listino in base.CurrentDataContext.New_vehiclemodellists
                    on myobj.New_VehicleModelListId equals listino.New_vehiclemodellistId
                    where listino.New_vehiclemodellistId == idLastActiveVehicleModelList
                    && listino.DeletionStateCode == 0 && myobj.New_name.ToUpper() == modelName.ToUpper()
                    select myobj).SingleOrDefault();
        }
        /// <summary>
        /// Ritorna il type trovato dato il suo nome e idcountry
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="idCountry"></param>
        /// <returns></returns>
        public New_producttype GetTypeByNameAndCountry(string typeName, Guid idCountry, Guid idLastActiveVehicleModelList)
        {
            if (string.IsNullOrEmpty(typeName) || idCountry == Guid.Empty)
                return null;

            return (from myobj in base.CurrentDataContext.New_producttypes
                    join listino in base.CurrentDataContext.New_vehiclemodellists
                    on myobj.New_VehicleModelListId equals listino.New_vehiclemodellistId
                    where listino.New_vehiclemodellistId == idLastActiveVehicleModelList
                    && listino.DeletionStateCode == 0 && myobj.New_name.ToUpper() == typeName.ToUpper()
                    select myobj).SingleOrDefault();
        }
        /// <summary>
        /// Ritorna il GVW trovato dato il suo nome e idcountry
        /// </summary>
        /// <param name="gvwName"></param>
        /// <param name="idCountry"></param>
        /// <returns></returns>
        public New_productgvw GetGvwByNameAndCountry(string gvwName, Guid idCountry, Guid idLastActiveVehicleModelList)
        {
            if (string.IsNullOrEmpty(gvwName) || idCountry == Guid.Empty)
                return null;

            return (from myobj in base.CurrentDataContext.New_productgvws
                    join listino in base.CurrentDataContext.New_vehiclemodellists
                    on myobj.New_VehicleModelListId equals listino.New_vehiclemodellistId
                    where listino.New_vehiclemodellistId == idLastActiveVehicleModelList
                    && listino.DeletionStateCode == 0 && myobj.New_name.ToUpper() == gvwName.ToUpper()
                    select myobj).SingleOrDefault();
        }
        /// <summary>
        /// Ritorna il WheelType trovato dato il suo nome e idcountry
        /// </summary>
        /// <param name="wheelTypeName"></param>
        /// <param name="idCountry"></param>
        /// <returns></returns>
        public New_productwheeltype GetWheelTypeByNameAndCountry(string wheelTypeName, Guid idCountry, Guid idLastActiveVehicleModelList)
        {
            if (string.IsNullOrEmpty(wheelTypeName) || idCountry == Guid.Empty)
                return null;

            return (from myobj in base.CurrentDataContext.New_productwheeltypes
                    join listino in base.CurrentDataContext.New_vehiclemodellists
                    on myobj.New_VehicleModelListId equals listino.New_vehiclemodellistId
                    where listino.New_vehiclemodellistId == idLastActiveVehicleModelList
                    && listino.DeletionStateCode == 0 && myobj.New_name.ToUpper() == wheelTypeName.ToUpper()
                    select myobj).SingleOrDefault();
        }
        /// <summary>
        /// Ritorna il Fuel trovato dato il suo nome e idcountry
        /// </summary>
        /// <param name="fuelName"></param>
        /// <param name="idCountry"></param>
        /// <returns></returns>
        public New_productfuel GetFuelByNameAndCountry(string fuelName, Guid idCountry, Guid idLastActiveVehicleModelList)
        {
            if (string.IsNullOrEmpty(fuelName) || idCountry == Guid.Empty)
                return null;

            return (from myobj in base.CurrentDataContext.New_productfuels
                    join listino in base.CurrentDataContext.New_vehiclemodellists
                    on myobj.New_VehicleModelListId equals listino.New_vehiclemodellistId
                    where listino.New_vehiclemodellistId == idLastActiveVehicleModelList
                    && listino.DeletionStateCode == 0 && myobj.New_name.ToUpper() == fuelName.ToUpper()
                    select myobj).SingleOrDefault();
        }
        /// <summary>
        /// Ritorna il Power trovato dato il suo nome e idcountry
        /// </summary>
        /// <param name="powerName"></param>
        /// <param name="idCountry"></param>
        /// <returns></returns>
        public New_productpower GetPowerByNameAndCountry(string powerName, Guid idCountry, Guid idLastActiveVehicleModelList)
        {
            if (string.IsNullOrEmpty(powerName) || idCountry == Guid.Empty)
                return null;

            return (from myobj in base.CurrentDataContext.New_productpowers
                    join listino in base.CurrentDataContext.New_vehiclemodellists
                    on myobj.New_VehicleModelListId equals listino.New_vehiclemodellistId
                    where listino.New_vehiclemodellistId == idLastActiveVehicleModelList
                    && listino.DeletionStateCode == 0 && myobj.New_name.ToUpper() == powerName.ToUpper()
                    select myobj).SingleOrDefault();
        }
        /// <summary>
        /// Ritorna il Cab Type trovato dato il suo nome e idcountry
        /// </summary>
        /// <param name="cabTypeName"></param>
        /// <param name="idCountry"></param>
        /// <returns></returns>
        public New_productcabtype GetCabTypeByNameAndCountry(string cabTypeName, Guid idCountry, Guid idLastActiveVehicleModelList)
        {
            if (string.IsNullOrEmpty(cabTypeName) || idCountry == Guid.Empty)
                return null;

            return (from myobj in base.CurrentDataContext.New_productcabtypes
                    join listino in base.CurrentDataContext.New_vehiclemodellists
                    on myobj.New_VehicleModelListId equals listino.New_vehiclemodellistId
                    where listino.New_vehiclemodellistId == idLastActiveVehicleModelList
                    && listino.DeletionStateCode == 0 && myobj.New_name.ToUpper() == cabTypeName.ToUpper()
                    select myobj).SingleOrDefault();
        }
        /// <summary>
        /// Ritorna il Suspension Type trovato dato il suo nome e idcountry
        /// </summary>
        /// <param name="suspensionTypeName"></param>
        /// <param name="idCountry"></param>
        /// <returns></returns>
        public New_productsuspensiontype GetSuspensionTypeByNameAndCountry(string suspensionTypeName, Guid idCountry, Guid idLastActiveVehicleModelList)
        {
            if (string.IsNullOrEmpty(suspensionTypeName) || idCountry == Guid.Empty)
                return null;

            return (from myobj in base.CurrentDataContext.New_productsuspensiontypes
                    join listino in base.CurrentDataContext.New_vehiclemodellists
                    on myobj.New_VehicleModelListId equals listino.New_vehiclemodellistId
                    where listino.New_vehiclemodellistId == idLastActiveVehicleModelList
                    && listino.DeletionStateCode == 0 && myobj.New_name.ToUpper().Equals(suspensionTypeName.ToUpper())
                    select myobj).ToList().Where(c => c.New_name == suspensionTypeName).SingleOrDefault();
        }
        /// <summary>
        /// Crea uno used vehicle model
        /// </summary>
        /// <param name="callBackData"></param>
        /// <param name="idLead"></param>
        /// <returns></returns>
        public RelationLeadUsedVehicleModel CreateUsedVehicleModelByFieldName(CallBackData callBackData, Guid idLead, Guid countryId)
        {
            ///Ottengo L'id dell'ultimo listino attivo
            Guid idLastActiveVehicleModelList = this.GetLastActiveVehicleModelListId(countryId);

            DynamicEntity _usedVehicleModel = new DynamicEntity("new_usedvehiclemodel");
            ///Popolo gli attributi dell'entità
            var _model = this.GetModelByNameAndCountry(callBackData.Model, countryId, idLastActiveVehicleModelList);
            if (_model != null)
                _usedVehicleModel.Properties.Add(new LookupProperty("new_modelid", new Lookup("new_productmodel", _model.New_productmodelId)));// callBackData.Nation));
            var _type = this.GetTypeByNameAndCountry(callBackData.Type, countryId, idLastActiveVehicleModelList);
            if (_type != null)
                _usedVehicleModel.Properties.Add(new LookupProperty("new_typeid", new Lookup("new_producttype", _type.New_producttypeId)));// callBackData.Nation));
            var _gvw = this.GetGvwByNameAndCountry(callBackData.GVW, countryId, idLastActiveVehicleModelList);
            if (_gvw != null)
                _usedVehicleModel.Properties.Add(new LookupProperty("new_gvwid", new Lookup("new_productgvw", _gvw.New_productgvwId)));// callBackData.Nation));
            var _wheel = this.GetWheelTypeByNameAndCountry(callBackData.WheelType, countryId, idLastActiveVehicleModelList);
            if (_wheel != null)
                _usedVehicleModel.Properties.Add(new LookupProperty("new_wheeltypeid", new Lookup("new_productwheeltype", _wheel.New_productwheeltypeId)));// callBackData.Nation));
            var _fuel = this.GetFuelByNameAndCountry(callBackData.Fuel, countryId, idLastActiveVehicleModelList);
            if (_fuel != null)
                _usedVehicleModel.Properties.Add(new LookupProperty("new_fuelid", new Lookup("new_productfuel", _fuel.New_productfuelId)));// callBackData.Nation));
            var _power = this.GetPowerByNameAndCountry(callBackData.Power, countryId, idLastActiveVehicleModelList);
            if (_power != null)
                _usedVehicleModel.Properties.Add(new LookupProperty("new_powerid", new Lookup("new_productpower", _power.New_productpowerId)));// callBackData.Nation));
            var _cabType = this.GetCabTypeByNameAndCountry(callBackData.CabType, countryId, idLastActiveVehicleModelList);
            if (_cabType != null)
                _usedVehicleModel.Properties.Add(new LookupProperty("new_cabtypeid", new Lookup("new_productcabtype", _cabType.New_productcabtypeId)));// callBackData.Nation));
            var _suspension = this.GetSuspensionTypeByNameAndCountry(callBackData.Suspension, countryId, idLastActiveVehicleModelList);
            if (_suspension != null)
                _usedVehicleModel.Properties.Add(new LookupProperty("new_suspensiontypeid", new Lookup("new_productsuspensiontype", _suspension.New_productsuspensiontypeId)));// callBackData.Nation));
            ///Lego il record al lead
            _usedVehicleModel.Properties.Add(new LookupProperty("new_leadid", new Lookup("contact", idLead)));// callBackData.Nation));
            ///vehicle model list
            _usedVehicleModel.Properties.Add(new LookupProperty("new_vehiclemodellistid", new Lookup("new_vehiclemodellist", idLastActiveVehicleModelList)));// callBackData.Nation));

            try
            {
                ///Creo 4 record per ogni lead
                RelationLeadUsedVehicleModel _relation = new RelationLeadUsedVehicleModel()
                {
                    IdUsedVehicleModelOthers = base.CurrentCrmService.Create(_usedVehicleModel),
                    IdUsedVehicleModelPromotions = base.CurrentCrmService.Create(_usedVehicleModel),
                    IdUsedVehicleModelPromptDelivery = base.CurrentCrmService.Create(_usedVehicleModel),
                    IdUsedVehicleModelVhlConfigurator = base.CurrentCrmService.Create(this.GetVhlVehicleModel(_usedVehicleModel, callBackData, idLead, countryId, idLastActiveVehicleModelList))
                };
                return _relation;
            }
            catch (SoapException ex)
            {
                
                throw;
            }
            
        }
        /// <summary>
        /// Aggiornamento del Lead aggiungendo i dettagli dei record creati sugli usedvehiclemodel
        /// </summary>
        /// <param name="relationLeadUsedVehicleModel"></param>
        /// <param name="idLead"></param>
        public void UpdateLeadWithUsedVehicleModel(RelationLeadUsedVehicleModel relationLeadUsedVehicleModel, Guid idLead)
        {
            DynamicEntity _lead = new DynamicEntity("contact");
            _lead.Properties.Add(new KeyProperty("contactid", new Key(idLead)));
            _lead.Properties.Add(new LookupProperty("new_vehiclepromptdeliveryid", new Lookup("new_usedvehiclemodel", relationLeadUsedVehicleModel.IdUsedVehicleModelPromptDelivery)));
            _lead.Properties.Add(new LookupProperty("new_vehiclevhlconfiguratorid", new Lookup("new_usedvehiclemodel", relationLeadUsedVehicleModel.IdUsedVehicleModelVhlConfigurator)));
            _lead.Properties.Add(new LookupProperty("new_vehiclepromotionid", new Lookup("new_usedvehiclemodel", relationLeadUsedVehicleModel.IdUsedVehicleModelPromotions)));
            _lead.Properties.Add(new LookupProperty("new_vehicleotherid", new Lookup("new_usedvehiclemodel", relationLeadUsedVehicleModel.IdUsedVehicleModelOthers)));
            try
            {
                ///Aggiorno entità
                base.CurrentCrmService.Update(_lead);
            }
            catch (SoapException x)
            {
                string y = x.Detail.InnerText;
                //throw new Exception("UpdateLeadWithUsedVehicleModel: " + x.Detail.InnerText);
                throw;
            }
            catch (Exception a)
            {
                throw;
            }
        }
        /// <summary>
        /// Creazione del lead
        /// </summary>
        /// <param name="callBackData"></param>
        /// <returns></returns>
        public Guid CreateLead(CallBackData callBackData, Guid idUserToAssign, Guid countryId, Guid languageId, DataConstant.TypeService typeService, DateTime startDateSlot,int offsetDate)
        {
            DynamicEntity _lead = new DynamicEntity("contact");
            ///Popolo gli attributi dell'entità
            _lead.Properties.Add(new CrmBooleanProperty("new_leadslave", new CrmBoolean(false)));
            if(callBackData.DataLeadCreation != DateTime.MinValue)
                _lead.Properties.Add(new CrmDateTimeProperty("new_datahidleadcreation", new CrmDateTime(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:s}", callBackData.DataLeadCreation))));
            _lead.Properties.Add(new StringProperty("firstname", callBackData.CustomerName));
            _lead.Properties.Add(new StringProperty("lastname", callBackData.CustomerSurname));
            _lead.Properties.Add(new StringProperty("address1_line1", callBackData.Address));
            _lead.Properties.Add(new StringProperty("address1_postalcode", callBackData.ZipCode));
            _lead.Properties.Add(new StringProperty("new_concatenatedzipcode", callBackData.ZipCode));
            _lead.Properties.Add(new StringProperty("address1_country", callBackData.Nation));
            _lead.Properties.Add(new StringProperty("address1_city", callBackData.City));
            _lead.Properties.Add(new StringProperty("address1_stateorprovince", callBackData.Province));
            _lead.Properties.Add(new LookupProperty("new_countryid", new Lookup("new_country", countryId)));// callBackData.Nation));
            _lead.Properties.Add(new StringProperty("emailaddress1", callBackData.EMail));
            _lead.Properties.Add(new StringProperty("fullname", string.Format(CultureInfo.InvariantCulture, "{0} {1}", callBackData.CustomerName, callBackData.CustomerSurname)));
            _lead.Properties.Add(new StringProperty("emailaddress1", callBackData.EMail));
            _lead.Properties.Add(new CrmDateTimeProperty("new_1schedulingdate", new CrmDateTime(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:s}", startDateSlot.AddMinutes(offsetDate)))));
            _lead.Properties.Add(new StringProperty("telephone1", callBackData.PhoneNumber));
            _lead.Properties.Add(new StringProperty("mobilephone", callBackData.MobilePhoneNumber));
            _lead.Properties.Add(new CrmBooleanProperty("new_flagprivacy", new CrmBoolean((callBackData.FlagPrivacy.HasValue) ? callBackData.FlagPrivacy.Value : false)));
            ///Controllo che il typecontact sia valorizzato
            if (string.IsNullOrEmpty(callBackData.TypeContact))
                _lead.Properties.Add(new PicklistProperty("new_typecontact", new Picklist((int)DataConstant.TYPE_CONTACT.NEW)));
            else
                _lead.Properties.Add(new PicklistProperty("new_typecontact", new Picklist((callBackData.TypeContact.ToUpperInvariant() == DataConstant.TYPE_CONTACT.USED.ToString()) ? (int)DataConstant.TYPE_CONTACT.USED : (int)DataConstant.TYPE_CONTACT.NEW)));
                
            ///Recupero il valore del title
            int titleValue = this.GetTitleCrm(callBackData.Title);
            if (titleValue != int.MinValue)
                _lead.Properties.Add(new PicklistProperty("new_title", new Picklist(titleValue)));
            _lead.Properties.Add(new StringProperty("new_modelpc", callBackData.Model));
            _lead.Properties.Add(new StringProperty("new_typepc", callBackData.Type));
            _lead.Properties.Add(new StringProperty("new_gvwpc", callBackData.GVW));
            _lead.Properties.Add(new StringProperty("new_wheeltypepc", callBackData.WheelType));
            _lead.Properties.Add(new StringProperty("new_fuelpc", callBackData.Fuel));
            _lead.Properties.Add(new PicklistProperty("new_leadsubstatus", new Picklist((int)DataConstant.LeadSubStatus.ToBeProcessed)));
            _lead.Properties.Add(new StringProperty("new_initiativesource", callBackData.InitiativeSource));
            _lead.Properties.Add(new StringProperty("new_initiativesourcereport", callBackData.InitiativeSourceReport));
            _lead.Properties.Add(new StringProperty("new_initiativesourcereportdetail", callBackData.InitiativeSourceReportDetail));
            _lead.Properties.Add(new StringProperty("new_promotion", callBackData.InitiativeSourceReport));
            _lead.Properties.Add(new StringProperty("new_companyname", callBackData.CompanyName));
            _lead.Properties.Add(new StringProperty("new_powerpc", callBackData.Power));
            _lead.Properties.Add(new StringProperty("new_cabtypepc", callBackData.CabType));
            _lead.Properties.Add(new StringProperty("new_suspensionpc", callBackData.Suspension));
            _lead.Properties.Add(new StringProperty("new_idleadsito", callBackData.IdLeadSite));            
            _lead.Properties.Add(new LookupProperty("new_languageid", new Lookup("new_language", languageId)));
            _lead.Properties.Add(new StringProperty("new_stocksearchedmodel", callBackData.StockSearchedModel));
            ///Ricavo il valore del brand
            int brandValue = this.GetBrandValue(callBackData.Brand);
            _lead.Properties.Add(new PicklistProperty("new_brand", new Picklist(brandValue)));
            if (callBackData.DueDate != DateTime.MinValue)
                _lead.Properties.Add(new CrmDateTimeProperty("new_duedate", new CrmDateTime(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:s}", callBackData.DueDate))));
            _lead.Properties.Add(new CrmBooleanProperty("new_flagprivacy2", new CrmBoolean((callBackData.FlagPrivacyDue.HasValue) ? callBackData.FlagPrivacyDue.Value : false)));
            _lead.Properties.Add(new StringProperty("new_idcare", callBackData.IdCare));
            _lead.Properties.Add(new StringProperty("new_comment", callBackData.CommentWebForm));
            _lead.Properties.Add(new StringProperty("new_promotionalcode", callBackData.CodePromotion));
            _lead.Properties.Add(new StringProperty("new_modelofinterest", callBackData.ModelOfInterest));
            if(callBackData.DesideredData != DateTime.MinValue)
                _lead.Properties.Add(new CrmDateTimeProperty("new_desireddata", new CrmDateTime(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:s}", callBackData.DesideredData))));
            ///Ottiene l'eventuale trascodifica del channel CRM
            int value = this.GetChannelCrm(callBackData.Canale);
            if (value != int.MinValue)
                _lead.Properties.Add(new PicklistProperty("new_channel", new Picklist(value)));
            ///inserisco gli stati
            _lead.Properties.Add(new PicklistProperty("new_leadstatus", new Picklist((int)DataConstant.LeadStatus.Open)));
            _lead.Properties.Add(new PicklistProperty("new_leadcategory", new Picklist(DataConstant.LEAD_LEAD_CATEGORY_TO_BE_PROCESSED)));

            ///AGGIUNGERE IL VALORE DEL GOTOTAB
            int result = this.GetGoToTabValueOrDefault(countryId, callBackData.InitiativeSource, callBackData.InitiativeSourceReport, callBackData.InitiativeSourceReportDetail);
            _lead.Properties.Add(new PicklistProperty("new_gototab", new Picklist(result)));
            ///Se il gototab == VHL Configurator allora inserisco il VHL selection
            ///altrimenti lo ometto
            if(result == (int)DataConstant.GoToTabLead.VHLConfigurator)
                _lead.Properties.Add(new StringProperty("new_vhlselection", callBackData.InitiativeSourceReportDetail));

            //Aggiorno il campo new_typeservice ASAP / BOOKING
            _lead.Properties.Add(new PicklistProperty("new_servicetype", new Picklist((int)typeService)));
            

            ///Creo il LEAD
            try
            {
                Guid idLead = base.CurrentCrmService.Create(_lead);
                DataUtility.Assign("contact", idLead, idUserToAssign, SecurityPrincipalType.User, base.CurrentCrmService);
                return idLead;
            }
            catch (SoapException x)
            {
                //throw new Exception("CreateLead: " + x.Detail.InnerText);
                throw;
            }
        }
        /// <summary>
        /// Ottiene il valore go to tab
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="initiativeSource"></param>
        /// <param name="initiativeSourceReport"></param>
        /// <param name="initiativeSourceReportDetail"></param>
        /// <returns></returns>
        public int GetGoToTabValueOrDefault(Guid countryId, string initiativeSource, string initiativeSourceReport, string initiativeSourceReportDetail)
        {
            ///Cerco eventuale go to tab
            var result = base.CurrentDataContext.New_gototabs.Where(c => c.New_CountryId.Value == countryId &&
                c.New_INITIATIVESOURCE == initiativeSource.Trim() &&
                c.New_INITIATIVESOURCEREPORT == initiativeSourceReport.Trim() &&
                c.New_INITIATIVESOURCEREPORTDETAIL == initiativeSourceReportDetail.Trim()).Select(c => c.New_GOTOTAB1).SingleOrDefault();
            if (result.HasValue) return result.Value;
            ///Ritorno il default
            result = base.CurrentDataContext.New_gototabs.Where(c => c.New_CountryId.Value == countryId && c.New_Default.Value == true).Select(c => c.New_GOTOTAB1.Value).SingleOrDefault();
            ///Ritorno l'eventuale default
            if (result.HasValue) return result.Value;
            ///Default del default ovvero imposto il valore della picklist a OTHER (LEAD - new_gototab)
            return (int)DataConstant.GoToTabLead.Other;
        }

        /// <summary>
        /// Creazione del dossier
        /// </summary>
        /// <param name="callBackData"></param>
        /// <returns></returns>
        public Guid CreateDossier(CallBackData callBackData, Guid idLead, Guid idOperator, Guid countryId, Guid languageId, Service service, DateTime startDateSlot, DateTime endDateSlot, DataConstant.TypeService typeService,int offsetDate)
        {

            DynamicEntity _dossier = new DynamicEntity("serviceappointment");
            ///Popolo gli attributi dell'entità
            ///Controllo che il typecontact sia valorizzato
            if (string.IsNullOrEmpty(callBackData.TypeContact))
                _dossier.Properties.Add(new PicklistProperty("new_typecontact", new Picklist((int)DataConstant.TYPE_CONTACT.NEW)));
            else
                _dossier.Properties.Add(new PicklistProperty("new_typecontact", new Picklist((callBackData.TypeContact.ToUpperInvariant() == DataConstant.TYPE_CONTACT.USED.ToString()) ? (int)DataConstant.TYPE_CONTACT.USED : (int)DataConstant.TYPE_CONTACT.NEW)));
            
            _dossier.Properties.Add(new StringProperty("new_initiativesource", callBackData.InitiativeSource));
            _dossier.Properties.Add(new StringProperty("new_initiativesourcereport", callBackData.InitiativeSourceReport));
            _dossier.Properties.Add(new StringProperty("new_initiativesourcereportdetail", callBackData.InitiativeSourceReportDetail));
            _dossier.Properties.Add(new LookupProperty("new_countryid", new Lookup("new_country", countryId)));// callBackData.Nation));
            _dossier.Properties.Add(new LookupProperty("new_languageid", new Lookup("new_language", languageId)));
            _dossier.Properties.Add(new StringProperty("new_phonenumber", callBackData.PhoneNumber));
            _dossier.Properties.Add(new LookupProperty("serviceid", new Lookup("service", service.ServiceId)));

            _dossier.Properties.Add(this.GetDynamicEntityParty("customers", "contact", idLead));
            _dossier.Properties.Add(new CrmDateTimeProperty("scheduledstart", new CrmDateTime(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:s}", startDateSlot.AddMinutes(offsetDate))))); // startDateSlot))));
            _dossier.Properties.Add(new CrmDateTimeProperty("scheduledend", new CrmDateTime(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:s}", endDateSlot.AddMinutes(offsetDate))))); //endDateSlot))));
            _dossier.Properties.Add(new StringProperty("subject", string.Format(CultureInfo.InvariantCulture, "{0} - {1} {2} - Scheduled", service.Name, callBackData.CustomerName, callBackData.CustomerSurname)));
            _dossier.Properties.Add(this.GetDynamicEntityParty("resources", EntityName.systemuser.ToString(), idOperator));
            _dossier.Properties.Add(new CrmNumberProperty("scheduleddurationminutes", new CrmNumber(service.Duration)));
            _dossier.Properties.Add(new PicklistProperty("new_servicetype", new Picklist((int)typeService)));
            _dossier.Properties.Add(new OwnerProperty("ownerid", new Owner("systemuser", idOperator)));

            //if(service.InitialStatusCode == 1 || service.InitialStatusCode == 2)
            //    _dossier.Properties.Add(new StatusProperty("statecode", new Status(0)));
            //else
            //    _dossier.Properties.Add(new StatusProperty("statecode", new Status(3)));
            //_dossier.Properties.Add(new StatusProperty("statuscode", new Status(service.InitialStatusCode)));
            //_dossier.Properties.Add(new CrmNumberProperty("scheduleddurationminutes", new CrmNumber(service.Duration)));


            ///CREO IL DOSSIER
            try
            {
                Guid idDossier = base.CurrentCrmService.Create(_dossier);
                ///Assegnazione di un service appointment
                DataUtility.Assign("serviceappointment", idDossier, idOperator, SecurityPrincipalType.User, base.CurrentCrmService);
                ///Riflessione sullo status del servizio per ora ignoro e creo sempre scheduled
                //if (service.InitialStatusCode == 1 || service.InitialStatusCode == 2)
                //    DataUtility.SetState("serviceappointment", idDossier, ServiceAppointmentState.Open.ToString(), service.InitialStatusCode, base.CurrentCrmService);
                //else
                //    DataUtility.SetState("serviceappointment", idDossier, ServiceAppointmentState.Scheduled.ToString(), service.InitialStatusCode, base.CurrentCrmService);
                ///IMPOSTO SEMPRE E COMUNQUE SCHEDULER / RESERVED
                DataUtility.SetState("serviceappointment", idDossier, ServiceAppointmentState.Scheduled.ToString(), DataConstant.DOSSIER_STATUS_CODE_RESERVED, base.CurrentCrmService);

                return idDossier;
            }
            catch (SoapException x)
            {
                string y = x.Detail.InnerText;
                string crm = "idLead: " + idLead + " / " + "idOperator: " + idOperator + " / " + "countryId:" + countryId + " / " + "languageId:" + languageId + "/ serviceId:" + service.ServiceId;
                //throw new Exception("CreateDossier: " + x.Detail.InnerText + " " + x.Message + " " + x.StackTrace + Environment.NewLine + Environment.NewLine + crm);
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// Restituisce owner per le mail
        /// </summary>
        /// <returns></returns>
        public SystemUser GetEmailUser()
        {
            return this.CurrentDataContext.SystemUsers.Where(c => c.DeletionStateCode == 0 && c.FullName.Contains(ConfigurationManager.AppSettings["SendMailCrmAdminUsername"])).SingleOrDefault();
        }
        /// <summary>
        /// Creazione dei una entity party per la mia dynamic entity
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="partyName"></param>
        /// <param name="partyId"></param>
        /// <returns></returns>
        public DynamicEntityArrayProperty GetDynamicEntityParty(string propertyName, string partyName, Guid partyId)
        {

            Lookup to = new Lookup() { type = partyName, Value = partyId };
            Property partyid = new LookupProperty() { Name = "partyid", Value = to };
            DynamicEntity party = new DynamicEntity() { Name = "activityparty" };
            party.Properties.Add(partyid);
            DynamicEntity[] party_vett = new DynamicEntity[] { party };
            DynamicEntityArrayProperty proprietà = new DynamicEntityArrayProperty() { Name = propertyName, Value = party_vett };
            return proprietà;
        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Restituisce la dynamic entity per il VHL in base al workflow e ai controlli fatti
        /// su initiative source report detail
        /// </summary>
        /// <param name="dinamicEntity"></param>
        /// <param name="callBackData"></param>
        /// <param name="idLead"></param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        private DynamicEntity GetVhlVehicleModel(DynamicEntity dinamicEntity, CallBackData callBackData, Guid idLead, Guid countryId, Guid idLastActiveVehicleModelList)
        {
            ///CASO VHL SELECTION
            if (!string.IsNullOrEmpty(callBackData.InitiativeSourceReportDetail))
            {
                var vehicleModelResult = this.GetVehicleModelByDescrResultModel(callBackData.InitiativeSourceReportDetail.Trim(), countryId);
                ///Son nel caso in cui il modell presente nell InitiativeSourceReportDetail è stato trovato in maniera univoca 
                ///nella tabella di relazione per il nome modello e la country (si considera l'ultimo listino per quella country)
                if (vehicleModelResult != null && vehicleModelResult.Count == 1)
                {
                    DynamicEntity _usedVehicleModel = new DynamicEntity("new_usedvehiclemodel");
                    ///Popolo gli attributi dell'entità
                    _usedVehicleModel.Properties.Add(new LookupProperty("new_modelid", new Lookup("new_productmodel", vehicleModelResult[0].New_ModelId.Value)));// callBackData.Nation));
                    if(vehicleModelResult[0].New_TypeId.HasValue)
                        _usedVehicleModel.Properties.Add(new LookupProperty("new_typeid", new Lookup("new_producttype", vehicleModelResult[0].New_TypeId.Value)));// callBackData.Nation))
                    if (vehicleModelResult[0].New_GVWId.HasValue)
                        _usedVehicleModel.Properties.Add(new LookupProperty("new_gvwid", new Lookup("new_productgvw", vehicleModelResult[0].New_GVWId.Value)));// callBackData.Nation));
                    if (vehicleModelResult[0].New_CabTypeId.HasValue)
                        _usedVehicleModel.Properties.Add(new LookupProperty("new_cabtypeid", new Lookup("new_productcabtype", vehicleModelResult[0].New_CabTypeId.Value)));// callBackData.Nation));
                    ///Lego il record al lead
                    _usedVehicleModel.Properties.Add(new LookupProperty("new_leadid", new Lookup("contact", idLead)));// callBackData.Nation));
                    ///Lego used vehicle model all'ultimo listino
                    _usedVehicleModel.Properties.Add(new LookupProperty("new_vehiclemodellistid", new Lookup("new_vehiclemodellist", idLastActiveVehicleModelList)));// callBackData.Nation));

                    return _usedVehicleModel;
                }
            }
            ///Ritorno la dynamic entity creata in precedenza
            ///poichè i controlli fatti su InitiativeSourceReportDetail non permettono il cambio di workflow
            return dinamicEntity;
        }
        /// <summary>
        /// Restituisco il valoree Picklist del brand
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        private int GetBrandValue(string brand)
        {
            if (string.IsNullOrEmpty(brand))
                return (int)DataConstant.LeadBrand.Iveco;
            ///Cerco il valore passato
            if (brand.ToUpperInvariant() == ((Enum)DataConstant.LeadBrand.Iveco).GetStringValueOf().ToUpperInvariant())
                return (int)DataConstant.LeadBrand.Iveco;
            else if (brand.ToUpperInvariant() == ((Enum)DataConstant.LeadBrand.IvecoUsed).GetStringValueOf().ToUpperInvariant())
                return (int)DataConstant.LeadBrand.IvecoUsed;
            return (int)DataConstant.LeadBrand.Iveco;
        }
        /// <summary>
        /// Ottiene il valore del crm per il channel passato
        /// </summary>
        /// <param name="channelSito"></param>
        /// <returns></returns>
        private int GetChannelCrm(string channelSito)
        {
            if (string.IsNullOrEmpty(channelSito))
                return (int)DataConstant.ChannelLead.Other;

            int value = 0;
            if (int.TryParse(channelSito, out value))
            {
                if (value == (int)DataConstant.ChannelLead.IvecoUsedWebSite)
                    return (int)DataConstant.ChannelLead.IvecoUsedWebSite;
                else if (value == (int)DataConstant.ChannelLead.IvecoWebSite)
                    return (int)DataConstant.ChannelLead.IvecoWebSite;
                else if (value == (int)DataConstant.ChannelLead.Other)
                    return (int)DataConstant.ChannelLead.Other;
                else if (value == (int)DataConstant.ChannelLead.WebSearchEngine)
                    return (int)DataConstant.ChannelLead.WebSearchEngine;
                else
                    return (int)DataConstant.ChannelLead.Other;
            }
            return (int)DataConstant.ChannelLead.Other;


        }
        /// <summary>
        /// Dato il valore del sito di TITLE lo confronto con la picklist del crm
        /// </summary>
        /// <param name="titleSito"></param>
        /// <returns></returns>
        private int GetTitleCrm(string titleSito)
        {
            if (string.IsNullOrEmpty(titleSito))
                return int.MinValue;

            if (titleSito.ToUpperInvariant() == DataConstant.LeadTitle.Mr.ToString().ToUpperInvariant())
                return (int)DataConstant.LeadTitle.Mr;
            else if (titleSito.ToUpperInvariant() == DataConstant.LeadTitle.Mrs.ToString().ToUpperInvariant())
                return (int)DataConstant.LeadTitle.Mrs;
            else if (titleSito.ToUpperInvariant() == DataConstant.LeadTitle.Ms.ToString().ToUpperInvariant())
                return (int)DataConstant.LeadTitle.Ms;

            return int.MinValue;
        }
        private SearchResponse FindOperatorByParameters(Service service, New_language language, string countryName, DateTime startDate, DateTime endDate, int offsetDate, int numberOfResult)
        {
            return this.FindOperatorByParameters(service, language, countryName, startDate, endDate, offsetDate, numberOfResult, new RequiredResource[] { });
        }
        private SearchResponse FindOperatorByParameters(Service service, New_language language, string countryName, DateTime startDate, DateTime endDate, int offsetDate, int numberOfResult, RequiredResource[] requiredResource)
        {
            ///Creo la richiesta per ottenere l'operatore libero            
            AppointmentRequest appointmentReq = new AppointmentRequest();
            appointmentReq.RequiredResources = requiredResource;
            appointmentReq.Direction = SearchDirection.Forward;
            appointmentReq.Duration = service.Duration;
            appointmentReq.NumberOfResults = numberOfResult;
            appointmentReq.ServiceId = service.ServiceId;

            // The search window describes the time when the resource can be scheduled.
            // It must be set.
            appointmentReq.SearchWindowStart = new CrmDateTime();
            appointmentReq.SearchWindowStart.Value = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:s}", startDate.AddMinutes(offsetDate));
            appointmentReq.SearchWindowEnd = new CrmDateTime();
            appointmentReq.SearchWindowEnd.Value = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:s}", endDate.AddMinutes(offsetDate));
            appointmentReq.UserTimeZoneCode = 110;//this.GetTimeZoneCodeByCountryName(countryName);
            //appointmentReq.RecurrenceTimeZoneCode = 85;
            // Create the request object.
            SearchRequest search = new SearchRequest();
            // Set the properties of the request object.
            search.AppointmentRequest = appointmentReq;

            try
            {
                // Execute the request.
                return base.CurrentCrmService.Execute(search) as SearchResponse;
            }
            catch (SoapException x)
            {
                string y = x.Detail.InnerText;
                //throw new Exception("FindFreeSlotOperator: " + x.Detail.InnerText);
                throw;
            }
            catch (Exception a)
            {
                throw;
            }
        }
        #endregion
    }
}
