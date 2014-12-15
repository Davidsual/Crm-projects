using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Reply.Iveco.LeadManagement.Presenter.Model;
using System.Data.SqlClient;
using System.IO;
using System.Data.Linq;
using Microsoft.Crm.Sdk;
using System.Web.Services.Protocols;
using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Crm.Sdk.Query;
using System.Collections;

namespace Reply.Iveco.LeadManagement.Presenter
{
    public class DealerController : BaseLeadManagementController, IDisposable
    //public class DealerController : BaseDataAccessLayer, IDisposable
    {

        #region CTOR
        /// <summary>
        /// Costruttore
        /// </summary>
        public DealerController(string organizationName, HttpContext context) :
            base(organizationName, context)
        {

        }
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Salva i dati che arrivano
        /// </summary>
        /// <param name="leads"></param>
        /// <param name="currentUser"></param>
        public List<ContactLeadUpload> SetMassiveLeadToTableLog(List<ContactLeadUpload> leads)
        {
            List<ContactLeadUpload> leadError = new List<ContactLeadUpload>();
            Func<ContactLeadUpload, bool> InsertSingleLead = delegate(ContactLeadUpload item)
            {
                try
                {
                    ///Inserisce il lead
                    base.CurrentCrmDealerAccessLayer.SetLeadLog(item);
                    return true;
                }
                catch
                {
                    throw;
                }
            };
            try
            {
                bool result = false;
                leads.ForEach(item =>
                {
                    ///Inserimento Lead
                    result = InsertSingleLead(item);
                    if (!result) leadError.Add(item);
                });
                return leadError;
            }
            catch (OperationCanceledException opex)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        /// <summary>
        /// Processo i lead uplodati
        /// </summary>
        public void ProcessUploadedLead()
        {
            //LoggingUtility.WriteEvent("Start Process Upload Lead " + DateTime.Now.ToShortTimeString(), ControllerConstant.LEAD_MANAGEMENT_LOAD_LEAD_EVENT_NAME);
            List<New_leadlog> leadLogToProcess = null;
            ContactLead contactLead = null;
            Guid idLeadLog = Guid.Empty;
            try
            {
                ///Leggo tutti i valori dalla tabella callback data
                leadLogToProcess = base.CurrentCrmDealerAccessLayer.GetLeadLogBySourceAndState(DataConstant.ContactLeadSource.Excel, DataConstant.ContactLeadState.ToCreate);
            }
            catch (Exception ex)
            {
                LoggingUtility.WriteEvent("Error during retrive call backdata " + ex.Message + " " + ex.StackTrace + " " + DateTime.Now.ToShortTimeString(), ControllerConstant.LEAD_MANAGEMENT_LOAD_LEAD_EVENT_NAME);
                return;
            }
            ///Esco se non trovo niente
            if (leadLogToProcess == null || leadLogToProcess.Count <= 0) return;

            //LoggingUtility.WriteEvent("Founded: " + leadLogToProcess.Count.ToString() + " " + DateTime.Now.ToShortTimeString(), ControllerConstant.LEAD_MANAGEMENT_LOAD_LEAD_EVENT_NAME);

            ///Converto il record new_callback in callbackData
            leadLogToProcess.ForEach(newLeadLog =>
            {
                idLeadLog = Guid.Empty;
                idLeadLog = newLeadLog.New_leadlogId;
                ///Ottengo l'oggetto callbackdata
                contactLead = this.NewContactLeadFromNewLeadLog(newLeadLog);

                try
                {
                    ///Per ognuno lancio la SetDeal
                    this.SetLead(contactLead, contactLead.IdOperatorUpload);
                    ///Aggiorna come OK
                    base.CurrentCrmDealerAccessLayer.UpdateLeadLogDataStatus(idLeadLog, DataConstant.ContactLeadState.Done, string.Empty);
                }
                catch (Exception ex)
                {
                    base.CurrentCrmDealerAccessLayer.UpdateLeadLogDataStatus(idLeadLog, DataConstant.ContactLeadState.Error, ex.Message);
                    //if (ex is ICustomException && ((ICustomException)ex).IdCallBackData != Guid.Empty)
                    //{
                    //    ///Aggiorno lo stato di errore
                    //    base.CurrentDataAccessLayer.UpdateCallBackDataStatus(((ICustomException)ex).IdCallBackData, DataConstant.CallBackDataState.Error, string.Format("{0} {1}", ((ICustomException)ex).Code, ((ICustomException)ex).Descr));
                    //}
                    //else if (ex is SetAppointmentException)
                    //{
                    //    ///Aggiorno lo stato di errore
                    //    base.CurrentDataAccessLayer.UpdateCallBackDataStatus(((SetAppointmentException)ex).IdCallBackData, DataConstant.CallBackDataState.Error, string.Format("{0} {1}", ((SetAppointmentException)ex).Message, ((SetAppointmentException)ex).StackTrace));
                    //}
                    LoggingUtility.Error("Error during Set Lead: " + ex.Message + " " + ex.StackTrace + " " + ((ex.InnerException != null) ? ex.InnerException.StackTrace : string.Empty) + " " + DateTime.Now.ToShortTimeString(), ControllerConstant.DEALER_LOAD_LEAD_EVENT_NAME);

                }
            });

            //LoggingUtility.WriteEvent("Finisched " + DateTime.Now.ToShortTimeString(), ControllerConstant.LEAD_MANAGEMENT_LOAD_LEAD_EVENT_NAME);
        }

        /// <summary>
        /// Carica massivamente i lead
        /// </summary>
        /// <param name="leads"></param>
        public void SetMassiveLead(List<ContactLead> leads)
        {
            List<ContactLead> leadError = new List<ContactLead>();
            Func<ContactLead, Guid, bool> InsertSingleLead = delegate(ContactLead item, Guid idOperatorUpload)
            {
                try
                {
                    ///Inserisce il lead
                    this.SetLead(item, idOperatorUpload);
                    return true;
                }
                catch
                {
                    return false;
                }
            };
            try
            {
                bool result = false;
                leads.ForEach(item =>
                {
                    ///Inserimento Lead
                    result = InsertSingleLead(item, item.IdOperatorUpload);
                    if (!result) leadError.Add(item);
                });
            }
            catch (OperationCanceledException opex)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        

        /// <summary>
        /// Salvataggio del lead
        /// </summary>
        /// <param name="lead"></param>
        /// <param name="currentUser"></param>
        public void SetLead(ContactLead lead, Guid currentUser)
        {



            //Effettuo i controlli pre-creazione applicando le logiche richieste
            if (string.IsNullOrEmpty(lead.Canale))
            {
                //Se canale non inviato, utilizzo quello di default (Other-20)
                lead.Canale = "20";
            }

            
            if (String.IsNullOrEmpty(lead.Campagna))
                lead.Campagna = "Marketing Campaign";

            if (String.IsNullOrEmpty(lead.Company_Name))
                lead.Company_Name = string.Empty;


            if (String.IsNullOrEmpty(lead.Address))
                lead.Address = string.Empty;

            if (String.IsNullOrEmpty(lead.Campagna))
                lead.Campagna = string.Empty;

            if (String.IsNullOrEmpty(lead.BusinessType))
                lead.BusinessType = string.Empty;

            if (String.IsNullOrEmpty(lead.ZipCode))
                lead.ZipCode = string.Empty;

            if (String.IsNullOrEmpty(lead.City))
                lead.City = string.Empty;

            if (String.IsNullOrEmpty(lead.Country))
                lead.Country = string.Empty;


            if (String.IsNullOrEmpty(lead.Email))
                lead.Email = string.Empty;

            if (String.IsNullOrEmpty(lead.PhoneNumber))
                lead.PhoneNumber = string.Empty;

            if (String.IsNullOrEmpty(lead.MobilePhoneNumber))
                lead.MobilePhoneNumber = string.Empty;

            //Se sono in ITALIA, l'indirizzo deve essere maiuscolo
            if (!String.IsNullOrEmpty(lead.Country))
            {
                if (lead.Country.ToUpperInvariant() == "ITALIA")
                {
                    if (!String.IsNullOrEmpty(lead.Address))
                        lead.Address = lead.Address.ToUpperInvariant();

                    if (!String.IsNullOrEmpty(lead.City))
                        lead.City = lead.City.ToUpperInvariant();

                    if (!String.IsNullOrEmpty(lead.Country))
                        lead.Country = lead.Country.ToUpperInvariant();

                    if (!string.IsNullOrEmpty(lead.Hamlet))
                        lead.Hamlet = lead.Hamlet.ToUpperInvariant();

                    if (!String.IsNullOrEmpty(lead.Province))
                    {
                        //In Italia la provincia deve essere 2
                        if (lead.Province.Length > 2)
                            lead.Province = string.Empty;
                        else
                            lead.Province = lead.Province.ToUpperInvariant();

                    }

                }
            }

            //E' obbligatorio che ci sia almeno o il Company_Name o il Customer_Surname
            if (String.IsNullOrEmpty(lead.Company_Name) && String.IsNullOrEmpty(lead.Customer_Surname))
            {
                //Se serve, lo traccio da qualche parte
                throw new Exception("Company Name and Surname are empty!");
            }
            else
            {
                base.CurrentCrmDealerAccessLayer.SetLead(lead, currentUser);
            }
        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Ritorna un contact Lead popolato da un record proventinete dal new_leadlog
        /// </summary>
        /// <param name="leadLog"></param>
        /// <returns></returns>
        private ContactLead NewContactLeadFromNewLeadLog(New_leadlog leadLog)
        {
            ContactLead contactLead = new ContactLead()
            {
                Address = leadLog.New_ADDRESS + string.Empty,
                AnnualRevenue = leadLog.New_ANNUALREVENUE + string.Empty,
                BulkEmailcontact = leadLog.New_BULKEMAILCONTACT + string.Empty,
                BusinessType = leadLog.New_BUSINESSTYPE + string.Empty,
                Campagna = leadLog.New_CAMPAIGN + string.Empty,
                Canale = leadLog.New_CHANNEL + string.Empty,
                City = leadLog.New_CITY + string.Empty,
                CodicePromozione = leadLog.New_PROMOTIONALCODE + string.Empty,
                Company_Name = leadLog.New_COMPANY_NAME + string.Empty,
                Country = leadLog.New_CUSTOMERCOUNTRY + string.Empty,
                Customer_Name = leadLog.New_CUSTOMER_NAME + string.Empty,
                Customer_Surname = leadLog.New_CUSTOMER_SURNAME + string.Empty,
                CustomerCountry = leadLog.New_CUSTOMERCOUNTRY + string.Empty,
                Email = leadLog.New_EMAIL + string.Empty,
                EmailContact = leadLog.New_EMAILCONTACT + string.Empty,
                EnderecoPostal = leadLog.New_ENDERECOPOSTAL + string.Empty,
                Fax = leadLog.New_FAX + string.Empty,
                FaxContact = leadLog.New_FAXCONTACT + string.Empty,
                FileName = leadLog.New_name + string.Empty,
                Hamlet = leadLog.New_HAMLET + string.Empty,
                //IDLeadCRMLead = leadLog.New_IDLEADCRMLEAD + string.Empty,
                IDLeadExternal = leadLog.New_IDLEADEXTERNAL + string.Empty,
                IdOperatorUpload = leadLog.OwnerId.Value,
                JobDescription = leadLog.New_INTERLOCUTORROLE + string.Empty,
                LegalForm = leadLog.New_LEGALFORM + string.Empty,
                MailContact = leadLog.New_MAILCONTACT + string.Empty,
                MobilePhoneNumber = leadLog.New_MOBILEPHONENUMBER + string.Empty,
                MotivazioneCriticalCustomer = leadLog.New_MOTIVAZIONECRITICALCUSTOMER + string.Empty,
                NotaCliente = leadLog.New_CUSTOMERNOTE + string.Empty,
                NotaProdottoDiInteresse = leadLog.New_PRODUCTOFINTERESTNOTE + string.Empty,
                NotaUsato = leadLog.New_USEDNOTE + string.Empty,
                NumberOfEmployees = leadLog.New_NUMBEROFEMPLOYEES + string.Empty,
                OfficeNumber = leadLog.New_OFFICENUMBER + string.Empty,
                PhoneContact = leadLog.New_PHONECONTACT + string.Empty,
                PhoneNumber = leadLog.New_PHONENUMBER + string.Empty,
                PreferredContactMethod = leadLog.New_PREFERREDCONTACTMETHOD + string.Empty,
                ProfilingDataH = leadLog.New_PROFILINGDATEH + string.Empty,
                Province = leadLog.New_PROVINCE + string.Empty,
                TAXCode = leadLog.New_TAXCODE + string.Empty,
                TypeContact = leadLog.New_TYPECONTACT + string.Empty,
                VATCode = leadLog.New_VATCODE + string.Empty,
                ZipCode = leadLog.New_ZIPCODE + string.Empty
            };
            if (!string.IsNullOrEmpty(leadLog.New_FLAGPRIVACY))
            {
                bool ret;
                if (Boolean.TryParse(leadLog.New_FLAGPRIVACY, out ret))
                    contactLead.FlagPrivacy = ret;
            }
            if (!string.IsNullOrEmpty(leadLog.New_CRITICALCUSTOMER))
            {
                if (leadLog.New_CRITICALCUSTOMER.Contains("0") || leadLog.New_CRITICALCUSTOMER.Contains("1"))
                {
                    contactLead.CriticalCustomer = (leadLog.New_CRITICALCUSTOMER.Contains("0")) ? false : true;
                }
            }
            return contactLead;
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Rilascio delle risorse
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }
        #endregion

        public DealerInfo FindDealer(ContactLead lead)
        {
            DealerInfo dealer = new DealerInfo();

            //Effettuo tutti i ragionamenti di assegnazione sullo zipcode e ritorno le informazioni sul dealar trovato
            if (String.IsNullOrEmpty(lead.ZipCode))
                lead.ZipCode = string.Empty;

            if (lead.CriticalCustomer == null)
                lead.CriticalCustomer = false;

            if (String.IsNullOrEmpty(lead.TypeContact))
                lead.TypeContact = "1";

            return base.CurrentCrmDealerAccessLayer.FindDealer(lead);

        }
    }
}
