using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Data;
using System.Data.Linq;
using Microsoft.Crm.Sdk;
using System.Web.Services.Protocols;
using Reply.Iveco.LeadManagement.Presenter.CrmDealerService;
using System.Globalization;
using System.Configuration;
using System.Net;
using Microsoft.Crm.SdkTypeProxy;
using System.Xml;

namespace Reply.Iveco.LeadManagement.Presenter.Model
{
    public partial class DataAccessLayer : BaseDataAccessLayer, IDisposable
    {
        #region PUBLIC METHODS
        /// <summary>
        /// Aggiorna il lead con i dati in arrivo dal CrmLead
        /// </summary>
        /// <param name="parameter"></param>
        public void UpdateLeadWithDealerData(SetDealerParameter parameter, Contact lead, Guid idCountry, DataConstant.LeadStatus status, DataConstant.LeadCategory category, bool setDealerAssignementDate)
        {
            DynamicEntity _lead = new DynamicEntity("contact");
            ///Aggiorno i campi del lead
            _lead.Properties.Add(new KeyProperty("contactid", new Key(lead.ContactId)));
            _lead.Properties.Add(new StringProperty("new_dealercode", string.IsNullOrEmpty(parameter.DealerCode) ? string.Empty : parameter.DealerCode));
            _lead.Properties.Add(new StringProperty("new_dealercompanyname", string.IsNullOrEmpty(parameter.DealerCompanyName) ? string.Empty : parameter.DealerCompanyName));
            _lead.Properties.Add(new StringProperty("new_dealerresponsible", string.IsNullOrEmpty(parameter.DealerResponsible) ? string.Empty : parameter.DealerResponsible));
            _lead.Properties.Add(new StringProperty("new_emaildealer", string.IsNullOrEmpty(parameter.DealerEmail) ? string.Empty : parameter.DealerEmail));
            _lead.Properties.Add(new StringProperty("new_marketingaccount", string.IsNullOrEmpty(parameter.MarketingAccount) ? string.Empty : parameter.MarketingAccount));
            _lead.Properties.Add(new StringProperty("new_emailma", string.IsNullOrEmpty(parameter.EmailMarketingAccount) ? string.Empty : parameter.EmailMarketingAccount));

            ///if lo stato del lead è closed non aggiorno
            if (lead.New_leadstatus != (int)DataConstant.LeadStatus.Closed)
            {
                _lead.Properties.Add(new PicklistProperty("new_leadstatus", new Picklist((int)status)));
                _lead.Properties.Add(new PicklistProperty("new_leadcategory", new Picklist((int)category)));
            }
            ///La data solo nel caso di assegnazione dealer
            if (setDealerAssignementDate)
                _lead.Properties.Add(new CrmDateTimeProperty("new_dealerassignmentdate", new CrmDateTime(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:s}", DateTime.Now.ToUniversalTime()))));
            _lead.Properties.Add(new CrmBooleanProperty("new_dealeragree", new CrmBoolean(parameter.IsDealerAgree)));
            _lead.Properties.Add(new CrmBooleanProperty("new_criticalcustomer", new CrmBoolean(parameter.IsCriticalCustomer)));
            ///Ricerco critical motivation
            if (parameter.CriticalReasonCode != int.MinValue && parameter.CriticalReasonCode > 0)
            {
                ///Recupero l'eventuale id critical customer reason e valuto se metterla come lookup
                Guid idCriticalCustomerReason = this.GetCriticalCustomerReasonByCountryIdAndCod(idCountry, parameter.CriticalReasonCode);
                if (idCriticalCustomerReason != null && idCriticalCustomerReason != Guid.Empty)
                    _lead.Properties.Add(new LookupProperty("new_criticalcustomerreasonid", new Lookup("new_criticalcustomerreason", idCriticalCustomerReason)));
            }

            try
            {
                ///Aggiorno entità
                base.CurrentCrmService.Update(_lead);
            }
            catch (SoapException x)
            {
                string y = x.Detail.InnerText;
                //throw new Exception("UpdateLeadWithDealerData: " + x.Detail.InnerText);
                throw;
            }
            catch (Exception a)
            {
                throw;
            }
        }
        /// <summary>
        /// Restituisce un lead dato il suo id
        /// </summary>
        /// <param name="idLead"></param>
        /// <returns></returns>
        public Contact GetLeadByIdLead(string idLeadCrm)
        {
            return base.CurrentDataContext.Contacts.Where(c => c.New_idleadcrmlm == idLeadCrm).FirstOrDefault();
        }
        /// <summary>
        /// Ottiene il country dato il suo id
        /// </summary>
        /// <param name="idCountry"></param>
        /// <returns></returns>
        public New_country GetCountryById(Guid idCountry)
        {
            return base.CurrentDataContext.New_countries.Where(c => c.New_countryId == idCountry).FirstOrDefault();
        }
        /// <summary>
        /// Ottiene la language dato il suo id
        /// </summary>
        /// <param name="idLanguage"></param>
        /// <returns></returns>
        public New_language GetLanguageById(Guid idLanguage)
        {
            return base.CurrentDataContext.New_languages.Where(c => c.New_languageId == idLanguage).FirstOrDefault();
        }
        /// <summary>
        /// Ottiene i dati restituiti dal WS presente su crm dealer
        /// </summary>
        /// <param name="dealerParameter"></param>
        public Reply.Iveco.LeadManagement.Presenter.CrmDealerService.FindDealerResult GetDealerDataFromCrmDealer(GetDealerParameter dealerParameter, New_country country)
        {
            using (CrmDealerServices serv = new CrmDealerServices())
            {
                serv.Url = country.New_CRMDealerAddress;//ConfigurationManager.AppSettings["UrlCrmDealerService"];
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                if (ConfigurationManager.AppSettings["IsProxyEnable"] == "true")
                {
                    System.Net.WebProxy wp = new System.Net.WebProxy("proxy.reply.it", 8080);
                    wp.Credentials = new System.Net.NetworkCredential("d.trotta", "80.David", "REPLYNET");
                    System.Net.WebRequest.DefaultWebProxy = wp;
                }
                return serv.FindDealer(new Reply.Iveco.LeadManagement.Presenter.CrmDealerService.ContactLead() { TypeContact = dealerParameter.LeadType.ToString(CultureInfo.InvariantCulture), ZipCode = dealerParameter.ZipCode, CriticalCustomer = dealerParameter.IsFlagCritico }, country.New_organizationname);
            }
        }
        /// <summary>
        /// Ritorna i dati dello zip dato zipcode e countryid
        /// </summary>
        /// <param name="zipCode"></param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public List<BusinessEntity> GetFindZipDetailByZipCodeAndCountryId(string zipCode, Guid countryId)
        {
            return DataUtility.RetrieveMultipleDynamic(this.CurrentCrmService, new string[] { "new_name", "new_countryid" }, new object[] { (string)zipCode, countryId }, "new_territoriality", string.Empty);
        }
        /// <summary>
        /// Ottiene il dealer dato  il suo id
        /// </summary>
        /// <param name="idDealer"></param>
        /// <returns></returns>
        public New_dealer GetDealerById(Guid idDealer)
        {
            return this.CurrentDataContext.New_dealers.Where(c => c.DeletionStateCode == 0 && c.New_dealerId == idDealer).SingleOrDefault();
        }
        /// <summary>
        /// Invio una mail partendo dai dati del lead
        /// </summary>
        /// <param name="lead"></param>
        public void SendMailCrm(Contact lead, New_country country, string toEmail, string ccEmail)
        {
            var typecontact = lead.New_typecontact.Value;
            var typecontactconfirm = (lead.New_typecontactconfirm != null) ? lead.New_typecontactconfirm.Value : 1;
            var goToTab = lead.New_gototab.Value;
            int templateNew_type = 0;
            //se entra new e confermato new, inviato il tab del GoToTAB
            if (typecontact == 1 && typecontactconfirm == 1)
            {
                templateNew_type = goToTab;
            }
            //se entra new e modifico in used, invio il tab used
            else if (typecontact == 1 && typecontactconfirm == 2)
            {
                templateNew_type = 5;
            }
            //se entra used e confermato used, invio tab used
            else if (typecontact == 2 && typecontactconfirm == 2)
            {
                templateNew_type = 5;
            }
            //se entra used e modifico in new, invio il tab other
            else if (typecontact == 2 && typecontactconfirm == 1)
            {
                templateNew_type = 4;
            }

            var idQueue = ConfigurationManager.AppSettings["SetDealerQueueId"];
            List<string> toContact = new List<string>(1);
            toContact.Add(toEmail);
            ///invio la mail se trovo id queue e il template type è > 0
            if (IsGuid(idQueue) && templateNew_type > 0)
            {
                var user = this.GetEmailUser();
                if (user != null)
                    this.SendEmail(base.CurrentCrmServiceMail, new Guid(idQueue), toContact, new List<string>(), lead.ContactId, templateNew_type, country.New_countryId, user.SystemUserId);
                else
                    throw new Exception("E-Mail user not found");
            }
        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Restituisce id del Critical Customer Reason dato countryid e code
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        private Guid GetCriticalCustomerReasonByCountryIdAndCod(Guid countryId, int code)
        {
            return base.CurrentDataContext.New_criticalcustomerreasons.Where(c => c.New_CountryId == countryId && c.New_Code == code && c.DeletionStateCode == 0).Select(c => c.New_criticalcustomerreasonId).SingleOrDefault();
        }
        /// <summary>
        /// Check if a string is a guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private bool IsGuid(string guid)
        {
            try
            {
                var ret = new Guid(guid);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Invia una mail da crm
        /// </summary>
        /// <param name="service"></param>
        /// <param name="from"></param>
        /// <param name="toAddresses"></param>
        /// <param name="ccAddresses"></param>
        /// <param name="leadId"></param>
        /// <param name="templateNewType"></param>
        /// <param name="countryId"></param>
        private void SendEmail(CrmService crmService, Guid from, List<string> toAddresses, List<string> ccAddresses, Guid leadId, int templateNewType, Guid countryId,Guid leadOwnerId)
        {
            string msg = string.Empty;
            try
            {

                //Recupero l'entità template per new_type e country
                var ret = DataUtility.RetrieveMultipleDynamic(crmService, new string[] { "new_type", "new_countryid" }, new object[] { templateNewType, countryId }, "new_emailtemplate", string.Empty);
                DynamicEntity templateEntity = (DynamicEntity)ret[0];
                if (templateEntity == null)
                {
                    DynamicEntity logEntity = new DynamicEntity("new_log");
                    logEntity.Properties.Add(new StringProperty("new_logdetail", "Non trovato template"));
                    logEntity.Properties.Add(new StringProperty("new_name", "SendEmailLog"));
                    crmService.Create(logEntity);

                    return;
                }

                string description = string.Empty;
                if (templateEntity.Properties.Contains("new_text"))
                    description = templateEntity.Properties["new_text"].ToString();

                string emailSubject = string.Empty;
                if (templateEntity.Properties.Contains("new_name"))
                    emailSubject = templateEntity.Properties["new_name"].ToString();

                //Analizzo la description
                // Cerco i <var entityname='' key='' field='' fieldtype='' /> e li sostituisco con i rispettivi valori
                XmlDocument descXml = new XmlDocument();
                descXml.LoadXml(description);
                XmlNodeList vars = descXml.GetElementsByTagName("var");

                XmlNode varNode;
                string entity = string.Empty;
                Guid entityId = Guid.Empty;
                string field = string.Empty;
                string fieldType = "text";
                DynamicEntity fieldEntity;
                string entityKeyFieldName = string.Empty;
                string value = string.Empty;
                DynamicEntity contactEntity = null;

                Dictionary<XmlNode, string> nodeMapping = new Dictionary<XmlNode, string>();
                var contactEntities = DataUtility.RetrieveMultipleDynamic(crmService, new string[] { "contactid" }, new object[] { leadId }, "contact", string.Empty);
                if (contactEntities != null && contactEntities.Count > 0)
                    contactEntity = (DynamicEntity)contactEntities[0];
                for (int i = 0; i < vars.Count; i++)
                {
                    try
                    {
                        varNode = vars.Item(i);

                        value = String.Empty;
                        entity = String.Empty;
                        entityKeyFieldName = String.Empty;
                        field = String.Empty;
                        fieldType = String.Empty;

                        entity = varNode.Attributes["entityname"].Value;
                        entityKeyFieldName = varNode.Attributes["key"].Value;  //Il campo  all'interno del lead, campo in cui trovo la GUID dell'entità entityname
                        field = varNode.Attributes["field"].Value;
                        fieldType = varNode.Attributes["fieldtype"].Value;

                        Guid entityToSearchGUID = Guid.Empty;
                        fieldEntity = contactEntity; //Di default cerco nel lead, altrimenti cerco nell'entità indicata
                        if (entity == "contact")
                        {
                            entityToSearchGUID = leadId;
                        }
                        else if (contactEntity.Properties.Contains(entityKeyFieldName))
                        {
                            //Recupero la GUID dell'entità indicata in entityname
                            entityToSearchGUID = ((Lookup)contactEntity.Properties[entityKeyFieldName]).Value;
                            var fieldEntities = DataUtility.RetrieveMultipleDynamic(crmService, new string[] { entity + "id" }, new object[] { entityToSearchGUID }, entity, string.Empty);
                            fieldEntity = (DynamicEntity)fieldEntities[0];
                        }


                        if (fieldEntity.Properties.Contains(field))
                        {
                            if (fieldType.ToLower().Equals("lookup"))
                            {
                                value = ((Lookup)fieldEntity.Properties[field]).name;
                            }
                            else if (fieldType.ToLower().Equals("bit"))
                            {
                                value = ((CrmBoolean)fieldEntity.Properties[field]).name;
                            }
                            else if (fieldType.ToLower().Equals("text"))
                            {
                                if (!String.IsNullOrEmpty((string)fieldEntity.Properties[field]))
                                    value = fieldEntity.Properties[field].ToString();
                            }
                            else if (fieldType.ToLower().Equals("picklist"))
                            {
                                value = ((Picklist)fieldEntity.Properties[field]).name;
                            }
                            else if (fieldType.ToLower().Equals("datetime"))
                            {
                                value = ((CrmDateTime)fieldEntity.Properties[field]).Value;
                            }
                        }

                        XmlNode parent = varNode.ParentNode;

                        nodeMapping.Add(parent, value);

                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                }

                foreach (XmlNode parent in nodeMapping.Keys)
                {
                    parent.RemoveAll();
                    parent.InnerText = nodeMapping[parent];
                }

                description = descXml.OuterXml;

                // Create the 'From:' activity party for the e-mail.
                activityparty fromParty = new activityparty();
                fromParty.partyid = new Lookup();
                fromParty.partyid.type = EntityName.queue.ToString();
                fromParty.partyid.Value = from;

                // Create an e-mail message.
                email email = new email();

                // Set e-mail properties.
                string toAddress;
                List<activityparty> toAddressPartyList = new List<activityparty>();
                for (int i = 0; i < toAddresses.Count; i++)
                {
                    activityparty toParty = new activityparty();
                    toAddress = toAddresses[i];
                    toParty.addressused = toAddress;
                    toAddressPartyList.Add(toParty);
                }
                activityparty[] toAddressParties = toAddressPartyList.ToArray();

                string ccAddress;
                List<activityparty> ccAddressPartyList = new List<activityparty>();
                for (int i = 0; i < ccAddresses.Count; i++)
                {
                    activityparty ccParty = new activityparty();
                    ccAddress = ccAddresses[i];
                    ccParty.addressused = ccAddress;
                    ccAddressPartyList.Add(ccParty);
                }
                activityparty[] ccAddressParties = ccAddressPartyList.ToArray();

                email.to = toAddressParties;
                email.cc = ccAddressParties;
                email.from = new activityparty[] { fromParty };
                email.subject = emailSubject;
                email.description = description;
                email.regardingobjectid = new Lookup("contact", leadId);

                CrmBoolean direction = new CrmBoolean();
                direction.Value = true;
                email.directioncode = direction;
                email.ownerid = new Owner("systemuser", leadOwnerId);

                
                //Creo l'email nel crm
                Guid emailId = crmService.Create(email);

                //ASSEGNO L'EMAIL AL DESTINATARIO PRINCIPALE
                
                AssignRequest _reqAssign = new AssignRequest()
                {
                    Assignee = new SecurityPrincipal() { Type = SecurityPrincipalType.User, PrincipalId = leadOwnerId },
                    Target = new TargetOwnedEmail() { EntityId = emailId }
                };
                crmService.Execute(_reqAssign);
                
                // Create a SendEmail request.
                SendEmailRequest req = new SendEmailRequest();                
                req.EmailId = emailId;
                req.TrackingToken = "";
                req.IssueSend = true;
                
                // Send the e-mail message.
                crmService.Execute(req);
                
            }
            catch (SoapException e)
            {
                throw new Exception(e.Detail.InnerText);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion
    }
}
