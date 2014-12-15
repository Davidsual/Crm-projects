using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reply.Iveco.LeadManagement.Presenter.Model;
using System.Globalization;
using Microsoft.Crm.Sdk;

namespace Reply.Iveco.LeadManagement.Presenter
{
    /// <summary>
    /// Gestione dei metodi per la creazione del calendario
    /// </summary>    
    public partial class LeadManagementController : BaseLeadManagementController, IDisposable
    {
        #region PUBLIC METHODS
        /// <summary>
        /// Aggiorna il lead crea un nuovo dossier con i dati del dealer
        /// </summary>
        /// <param name="dealerParameter"></param>
        public void SetDealer(SetDealerParameter dealerParameter)
        {
            bool IsDealerCodeNamePresent = false;
            Proposal _proposal = null;
            DateTime startDate, endDate;

            try
            {
                ///Recupero offset in base all'utente per fixare (debug / release) che lavorano con fusorari differenti
                int currentOffset = base.CurrentDataAccessLayer.GetOffsetDateByCurrentUser();
                ///Recupero il Lead
                Contact lead = base.CurrentDataAccessLayer.GetLeadByIdLead(dealerParameter.IdLeadCrm);
                if (lead == null || !lead.New_typecontactconfirm.HasValue) throw new LeadNotFoundException();
                ///Ottengo il country
                New_country country = base.CurrentDataAccessLayer.GetCountryById(lead.New_countryid.Value);
                ///Sollevo errore se country è null
                if (country == null) throw new CountryNotFoundException();
                ///Ottengo la lingua
                New_language language = base.CurrentDataAccessLayer.GetLanguageById(lead.New_languageid.Value);
                ///Controllo che language sia valorizzato
                if (language == null) throw new LanguageNotFoundException();
                ///Recupero il tipo di servizio
                Service _service = base.CurrentDataAccessLayer.GetServiceByTypeAndLanguage(DataConstant.TypeService.CSI, language.New_languageId);
                ////Controllo se service è stato trovato
                if (_service == null) throw new ServiceTypeNotFoundException();
                ///Cerco di capire se ho dealercode / dealername popolati oppure no
                IsDealerCodeNamePresent = (!string.IsNullOrEmpty(dealerParameter.DealerCode) || !string.IsNullOrEmpty(dealerParameter.DealerCompanyName));
                ///Aggiorno il Lead aggiornando i campi e lo status
                ///Controllo se dealername e dealercode sono nulli o vuoti allora non inserisco neanche un DOSSIER e salvo
                ///il lead come dealertobeassigned e dispatchedtomarket
                if (IsDealerCodeNamePresent) ///Caso in cui dealername o dealercode o entrambi siano presenti
                    base.CurrentDataAccessLayer.UpdateLeadWithDealerData(dealerParameter, lead, country.New_countryId, DataConstant.LeadStatus.WaitingForCSI, DataConstant.LeadCategory.DispatchedToTheDealer,true);
                else //caso in cui i due dati non sono presenti
                    base.CurrentDataAccessLayer.UpdateLeadWithDealerData(dealerParameter, lead, country.New_countryId, DataConstant.LeadStatus.DealerToBeAssigned, DataConstant.LeadCategory.DispatchedToTheMarket,false);
                ///se ho dealer email oppure email marketing account (se non le ho oppure le ho tutte e due non faccio niente)
                
                ///invio la mail se devo se IS-DEALER-AGREE è a FALSE
                if(!dealerParameter.IsDealerAgree)
                {
                    if ((string.IsNullOrEmpty(dealerParameter.DealerEmail) && !string.IsNullOrEmpty(dealerParameter.EmailMarketingAccount)) ||
                        (!string.IsNullOrEmpty(dealerParameter.DealerEmail) && string.IsNullOrEmpty(dealerParameter.EmailMarketingAccount)))
                    {
                        string toEmail = (!string.IsNullOrEmpty(dealerParameter.DealerEmail))?dealerParameter.DealerEmail : dealerParameter.EmailMarketingAccount;
                        base.CurrentDataAccessLayer.SendMailCrm(lead, country,toEmail,string.Empty);
                    }
                }
                ///Se lo stato del lead è chiuso allora non creo il dossier CSI
                if (lead.New_leadstatus.Value == (int)DataConstant.LeadStatus.Closed) return;
                ///se il typecontactconfirm è new e dealer present  allora creo altrimenti esco
                ///se usato lo creo sempre
                ///ESCO se dealer code name è presente
                if (lead.New_typecontactconfirm.Value == (int)DataConstant.TYPE_CONTACT.NEW && !IsDealerCodeNamePresent) return;
                ///se è usato e New_UsedLeadToMarket è false e non ho delear code o dealer name allora esco
                if (lead.New_typecontactconfirm.Value == (int)DataConstant.TYPE_CONTACT.USED && !country.New_UsedLeadToMarket.Value && !IsDealerCodeNamePresent) return;
                ///Controllo se esiste già un DOSSIER CSI assegnato al lead trovato se esiste non vado avanti
                ///perchè non devo creare altri dossier
                List<ServiceAppointment> dossiers = base.CurrentDataAccessLayer.GetActviceCsiDossierByIdLeadAndServiceType(lead.ContactId, DataConstant.TypeService.CSI);
                ///se c'è già almeno un dossier allora esco
                if (dossiers != null && dossiers.Count() > 0)
                    return;
                ///Creo start date end date in cui cercare l'operatore
                startDate = DateTime.Now.AddDays(country.New_CSIDays.Value);
                endDate = startDate.AddDays(country.New_CSIEndDay.Value); 
                ///Faccio la find dell'operatore
                ///Recupero dal servizio del CRM l'operatore libero per i parametri passati in ingresso.
                _proposal = base.CurrentDataAccessLayer.FindFreeSlotOperator(_service, language, string.Empty, startDate, endDate, currentOffset);
                ///Popolo i dati da passare per creare il nuovo dossier
                CallBackData data = new CallBackData()
                {
                    TypeContact = (lead.New_typecontact.Value == (int)DataConstant.TYPE_CONTACT.NEW) ? DataConstant.TYPE_CONTACT.NEW.ToString() : DataConstant.TYPE_CONTACT.USED.ToString(),
                    InitiativeSource = lead.New_initiativesource,
                    InitiativeSourceReport = lead.New_initiativesourcereport,
                    InitiativeSourceReportDetail = lead.New_initiativesourcereportdetail,
                    PhoneNumber = lead.Telephone1,
                    CustomerName = lead.FirstName,
                    CustomerSurname = lead.LastName
                };
                ///Se non trvo niente allora inizio il workflow per OVERBOOKING
                if (_proposal == null || _proposal.ProposalUser == null)
                {
                    ///overbooking - creo il dossier
                    Guid idDosserOverbooking = base.CurrentDataAccessLayer.CreateDossier(data, lead.ContactId, country.New_TeamLeaderOverbookingId.Value,
                        country.New_countryId, language.New_languageId, _service,startDate, 
                        startDate.AddMinutes(_service.Duration), DataConstant.TypeService.CSI, 
                        currentOffset);

                    ///Recupero gli id queue per rifare la riassegnazione
                    Guid? idQueue = base.CurrentDataAccessLayer.GetQueueIdByObjectId(idDosserOverbooking);
                    ///Recupero id della queue dove destinare il dossier da processare
                    Guid? idTeamLeaderQueue = language.New_QueueOverbookingId;
                    ///Assegno il dossier alla queue del team leader
                    base.CurrentDataAccessLayer.AssignDossierToTeamLeaderQueue(idDosserOverbooking, idTeamLeaderQueue.Value, idQueue.Value);
                }
                else ///caso in cui viene trovato lo slot libero per l'operatore
                {
                    ///Creo un nuovo dossier
                    ///Creo dossier
                    Guid idDosser = base.CurrentDataAccessLayer.CreateDossier(data, lead.ContactId, _proposal.ProposalUser.IdUser, country.New_countryId, language.New_languageId, _service, _proposal.StartSlotUniversalTime, _proposal.EndSlotUniversalTime, DataConstant.TypeService.CSI, currentOffset);
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Ottiene un dealer
        /// </summary>
        /// <param name="dealerParameter"></param>
        public GetDealerResult GetDealer(GetDealerParameter dealerParameter)
        {
            ///Controllo se il lead è usato
            New_country country = this.CurrentDataAccessLayer.GetCountryById(dealerParameter.CountryId);
            ///Se non la trovo allora esco
            if (country == null) throw new CountryNotFoundException() { };
            ///Se la country è di tipo crm dealer  == true allora chiamo il WS e ritorno i parametri
            if (country.New_CRMDealer.HasValue && country.New_CRMDealer.Value)
            {
                ///Chiamo il WS e ritorno
                Reply.Iveco.LeadManagement.Presenter.CrmDealerService.FindDealerResult dealerInfo = this.CurrentDataAccessLayer.GetDealerDataFromCrmDealer(dealerParameter, country);
                ///Se c'è errore nel return allora solleve errore
                if (dealerInfo == null || !dealerInfo.IsSuccessful || dealerInfo.dealer == null) 
                    throw new ArgumentException("Dealer info from Crm Dealer not found!");
                
                ///Popolo oggetto di ritorno                
                return new GetDealerResult()
                {
                     DealerCode = dealerInfo.dealer.DealerCode,
                     DealerCompanyName = dealerInfo.dealer.DealerCompanyName,
                     DealerResponsible = dealerInfo.dealer.DealerResponsible,
                     MarketingAccount = dealerInfo.dealer.MarketingAccount,
                     DealerEmail = dealerInfo.dealer.DealerEmail,
                     EmailMarketingAccount = dealerInfo.dealer.EmailMarketingAccount,
                     DealerManagerMail = string.Empty,
                     DealerManagerName = string.Empty,
                     IsCriticalCustomer = dealerInfo.dealer.Critical,
                     CriticalReasonCode = dealerInfo.dealer.CriticalReason
                };
            }
            ///Recupero i dettagli del dealer dato lo zipcode e countryid
            var zipCodeDetails = this.CurrentDataAccessLayer.GetFindZipDetailByZipCodeAndCountryId(dealerParameter.ZipCode, country.New_countryId);
            ///Discrimino 2 strade se il dealer è usato piuttosto che non usato
            if (dealerParameter.LeadType == (int)DataConstant.TYPE_CONTACT.NEW)
            {
                ///Ritorna il valore del dealer lead new
                return this.GetDealerLeadNew(dealerParameter, country, zipCodeDetails);
            }
            else if (dealerParameter.LeadType == (int)DataConstant.TYPE_CONTACT.USED)
            {
                ///Ritorna il valore del dealer lead used
                return this.GetDealerLeadUsed(dealerParameter, country, zipCodeDetails);
            }
            else
                throw new ArgumentException("Lead Type error");
            
        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Ottiene il dealer lead con le informazioni per il tipo NEW
        /// </summary>
        /// <param name="dealerParameter"></param>
        /// <param name="country"></param>
        /// <param name="zipCodeDetails"></param>
        /// <returns></returns>
        private GetDealerResult GetDealerLeadNew(GetDealerParameter dealerParameter, New_country country, List<BusinessEntity> zipCodeDetails)
        {
            ///Se mpm trovo nessun dealer
            if (zipCodeDetails != null && zipCodeDetails.Count == 0) ///nessun dealer trovato
            {
                ///vado sulla country MARKET NEW
                return new GetDealerResult()
                {
                    EmailMarketingAccount = country.New_NewEmail,
                    MarketingAccount = string.Format(CultureInfo.InvariantCulture, "{0} {1}", country.New_NewName, country.New_NewSurname),
                    IsCriticalCustomer = true,
                    CriticalReasonCode = 8
                };
            }
            else if (zipCodeDetails != null && zipCodeDetails.Count == 1) ///trovato 1 solo dealer
            {
                DynamicEntity terrEntity = (DynamicEntity)zipCodeDetails[0];
                ////Recupero le informazioni del dealer
                New_dealer dealer = this.CurrentDataAccessLayer.GetDealerById(((Lookup)terrEntity.Properties["new_dealerid"]).Value);
                if (dealerParameter.IsFlagCritico)
                {
                    ///user new MARKET
                    return new GetDealerResult()
                    {
                        IsCriticalCustomer = true,
                        EmailMarketingAccount = dealer.New_MarketdefaultuserNEWEmail,
                        MarketingAccount = string.Format(CultureInfo.InvariantCulture, "{0} {1}", dealer.New_MarketdefaultuserNEWName, dealer.New_MarketdefaultuserNEWSurname)
                    };
                }
                else
                {
                    ///USER NEW DEALER
                    return new GetDealerResult()
                    {
                        DealerCode = dealer.New_DealerCode,
                        DealerCompanyName = dealer.New_name,
                        DealerEmail = dealer.New_DefaultuserNEWEmail,
                        DealerResponsible = string.Format(CultureInfo.InvariantCulture, "{0} {1}", dealer.New_DefaultuserNEWName, dealer.New_DefaultuserNEWSurname),
                        DealerManagerMail = dealer.New_DealerManagerEmail,
                        DealerManagerName = string.Format(CultureInfo.InvariantCulture, "{0} {1}", dealer.New_DealerManagerName, dealer.New_DealerManagerSurname)
                    };
                }
            }
            else if (zipCodeDetails != null && zipCodeDetails.Count > 1) ///trovati + di 1
            {
                ///più di uno
                DynamicEntity terrEntity = (DynamicEntity)zipCodeDetails[0];
                ////Recupero le informazioni del dealer
                New_dealer dealer = this.CurrentDataAccessLayer.GetDealerById(((Lookup)terrEntity.Properties["new_dealerid"]).Value);
               ///USER NEW MARKET
                return new GetDealerResult()
                {
                    IsCriticalCustomer = true,
                    EmailMarketingAccount = dealer.New_MarketdefaultuserNEWEmail,
                    MarketingAccount = string.Format(CultureInfo.InvariantCulture, "{0} {1}", dealer.New_MarketdefaultuserNEWName, dealer.New_MarketdefaultuserNEWSurname),
                    CriticalReasonCode = 7
                };
            }
            else
                throw new ArgumentException("Error during retrive zipcode details");
        }
        /// <summary>
        /// Ottiene il dealer lead con le informazioni per il tipo USED
        /// </summary>
        /// <param name="dealerParameter"></param>
        /// <param name="country"></param>
        /// <param name="zipCodeDetails"></param>
        /// <returns></returns>
        private GetDealerResult GetDealerLeadUsed(GetDealerParameter dealerParameter, New_country country, List<BusinessEntity> zipCodeDetails)
        {
            if (country.New_UsedLeadToMarket.HasValue && country.New_UsedLeadToMarket.Value)
            {
                ///vado sulla country MARKET USED
                return new GetDealerResult()
                {
                    IsCriticalCustomer = false,
                    EmailMarketingAccount = country.New_UsedEmail,
                    MarketingAccount = string.Format(CultureInfo.InvariantCulture, "{0} {1}", country.New_UsedName, country.New_Usedsurname)
                };
            }
            ///Controllo il numero di dettagli trovati tramite lo zip code
            ///Se mpm trovo nessun dealer
            if (zipCodeDetails != null && zipCodeDetails.Count == 0) ///nessun dealer trovato
            {
                ///vado sulla country MARKET USED
                return new GetDealerResult()
                {
                    EmailMarketingAccount = country.New_UsedEmail,
                    MarketingAccount = string.Format(CultureInfo.InvariantCulture, "{0} {1}", country.New_UsedName, country.New_Usedsurname),
                    IsCriticalCustomer = true,
                    CriticalReasonCode = 8
                };
            }
            else if (zipCodeDetails != null && zipCodeDetails.Count == 1) ///trovato 1 solo dealer
            {
                DynamicEntity terrEntity = (DynamicEntity)zipCodeDetails[0];
                ////Recupero le informazioni del dealer
                New_dealer dealer = this.CurrentDataAccessLayer.GetDealerById(((Lookup)terrEntity.Properties["new_dealerid"]).Value);
                if (dealerParameter.IsFlagCritico)
                {
                    ///user new MARKET
                    return new GetDealerResult()
                    {
                        IsCriticalCustomer = true,
                        EmailMarketingAccount = dealer.New_MarketdefaultuserUSEDEmail,
                        MarketingAccount = string.Format(CultureInfo.InvariantCulture, "{0} {1}", dealer.New_MarketdefaultuserUSEDName, dealer.New_MarketdefaultuserUSEDSurname)
                    };
                }
                else
                {
                    ///USER NEW DEALER
                    return new GetDealerResult()
                    {
                        DealerCode = dealer.New_DealerCode,
                        DealerCompanyName = dealer.New_name,
                        DealerEmail = dealer.New_DefaultuserUSEDEmail,
                        DealerResponsible = string.Format(CultureInfo.InvariantCulture, "{0} {1}", dealer.New_DefaultuserUSEDName, dealer.New_DefaultuserUSEDSurname),
                        DealerManagerMail = dealer.New_DealerManagerEmail,
                        DealerManagerName = string.Format(CultureInfo.InvariantCulture, "{0} {1}", dealer.New_DealerManagerName, dealer.New_DealerManagerSurname)
                    };
                }
            }
            else if (zipCodeDetails != null && zipCodeDetails.Count > 1) ///trovati + di 1
            {
                ///più di uno
                DynamicEntity terrEntity = (DynamicEntity)zipCodeDetails[0];
                ////Recupero le informazioni del dealer
                New_dealer dealer = this.CurrentDataAccessLayer.GetDealerById(((Lookup)terrEntity.Properties["new_dealerid"]).Value);
                ///USER NEW MARKET
                return new GetDealerResult()
                {
                    IsCriticalCustomer = true,
                    EmailMarketingAccount = dealer.New_MarketdefaultuserUSEDEmail,
                    MarketingAccount = string.Format(CultureInfo.InvariantCulture, "{0} {1}", dealer.New_MarketdefaultuserUSEDName, dealer.New_MarketdefaultuserUSEDSurname),
                    CriticalReasonCode = 7

                };
            }
            else
                throw new ArgumentException("Error during retrive zipcode details");                         
        }
        #endregion
    }
}
