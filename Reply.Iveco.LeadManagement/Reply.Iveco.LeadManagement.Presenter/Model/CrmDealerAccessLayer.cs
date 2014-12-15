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
using Microsoft.Crm.SdkTypeProxy.Metadata;
using Microsoft.Crm.Sdk.Metadata;
using System.Globalization;

namespace Reply.Iveco.LeadManagement.Presenter.Model
{
    public partial class CrmDealerAccessLayer : BaseDataAccessLayer, IDisposable
    {

        Dictionary<string, string> channel = new Dictionary<string, string>();
        CrmUtils crm;
        bool marketUser;


        #region CTOR
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="organizationName"></param>
        public CrmDealerAccessLayer(string organizationName, HttpContext context)
            : base(organizationName, context)
        {

        }
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Aggiorna il record cambiando stato e descrizione errore
        /// </summary>
        /// <param name="idCallBackData"></param>
        /// <param name="state"></param>
        /// <param name="errorDescription"></param>
        public void UpdateLeadLogDataStatus(Guid idLeadLog, DataConstant.ContactLeadState state, string errorDescription)
        {
            DynamicEntity _callBackData = new DynamicEntity("new_leadlog");
            _callBackData.Properties.Add(new KeyProperty("new_leadlogid", new Key(idLeadLog)));
            _callBackData.Properties.Add(new PicklistProperty("new_status", new Picklist((int)state)));
            _callBackData.Properties.Add(new StringProperty("new_statusdescription", errorDescription));
            try
            {
                //using (new CrmImpersonator())
                base.CurrentCrmService.Update(_callBackData);
            }
            catch (SoapException x)
            {
                throw new Exception("UpdateLeadLogDataStatus: " + x.Detail.InnerText);
            }
        }
        /// <summary>
        /// Ottengo tutti i lead log da processare
        /// </summary>
        /// <param name="source"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public List<New_leadlog> GetLeadLogBySourceAndState(DataConstant.ContactLeadSource source, DataConstant.ContactLeadState state)
        {
            return base.CurrentCrmDealerDataContext.New_leadlogs.Where(c => c.New_Source == (int)source && c.New_Status == (int)state && c.DeletionStateCode == 0).ToList();
        }
        /// <summary>
        /// Salvo sulla lead log il record che mi arriva in ingresso
        /// </summary>
        /// <param name="lead"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public Guid SetLeadLog(ContactLeadUpload lead)
        {
            DynamicEntity _callBackData = new DynamicEntity("new_leadlog");
            ///Popolo gli attributi dell'entità
            _callBackData.Properties.Add(new StringProperty("new_name", lead.FileName));
            //_callBackData.Properties.Add(new StringProperty("new_statusdescription", ""));
            _callBackData.Properties.Add(new StringProperty("new_idleadexternal", lead.IDLeadExternal));
            //_callBackData.Properties.Add(new StringProperty("new_idleadcrmlead", lead.IDLeadCRMLead));
            _callBackData.Properties.Add(new StringProperty("new_channel", lead.Canale));
            _callBackData.Properties.Add(new StringProperty("new_campaign", lead.Campagna));
            _callBackData.Properties.Add(new StringProperty("new_customer_name", lead.Customer_Name));
            _callBackData.Properties.Add(new StringProperty("new_customer_surname", lead.Customer_Surname));
            _callBackData.Properties.Add(new StringProperty("new_company_name", lead.Company_Name));
            _callBackData.Properties.Add(new StringProperty("new_businesstype", lead.BusinessType));
            _callBackData.Properties.Add(new StringProperty("new_interlocutorrole", lead.JobDescription));
            _callBackData.Properties.Add(new StringProperty("new_address", lead.Address));
            _callBackData.Properties.Add(new StringProperty("new_city", lead.City));
            _callBackData.Properties.Add(new StringProperty("new_zipcode", lead.ZipCode));
            _callBackData.Properties.Add(new StringProperty("new_enderecopostal", lead.EnderecoPostal));
            _callBackData.Properties.Add(new StringProperty("new_hamlet", lead.Hamlet));
            _callBackData.Properties.Add(new StringProperty("new_province", lead.Province));
            _callBackData.Properties.Add(new StringProperty("new_customercountry", lead.CustomerCountry));
            _callBackData.Properties.Add(new StringProperty("new_phonenumber", lead.PhoneNumber));
            _callBackData.Properties.Add(new StringProperty("new_mobilephonenumber", lead.MobilePhoneNumber));
            _callBackData.Properties.Add(new StringProperty("new_officenumber", lead.OfficeNumber));
            _callBackData.Properties.Add(new StringProperty("new_fax", lead.Fax));
            _callBackData.Properties.Add(new StringProperty("new_email", lead.Email));
            _callBackData.Properties.Add(new StringProperty("new_profilingdateh", lead.ProfilingDataH));
            _callBackData.Properties.Add(new StringProperty("new_criticalcustomer", lead.CriticalCustomer));
            _callBackData.Properties.Add(new StringProperty("new_promotionalcode", lead.CodicePromozione));
            _callBackData.Properties.Add(new StringProperty("new_flagprivacy", lead.FlagPrivacy));
            _callBackData.Properties.Add(new StringProperty("new_motivazionecriticalcustomer", lead.MotivazioneCriticalCustomer));
            _callBackData.Properties.Add(new StringProperty("new_typecontact", lead.TypeContact));
            _callBackData.Properties.Add(new StringProperty("new_productofinterestnote", lead.NotaProdottoDiInteresse));
            _callBackData.Properties.Add(new StringProperty("new_usednote", lead.NotaUsato));
            _callBackData.Properties.Add(new StringProperty("new_customernote", lead.NotaCliente));
            _callBackData.Properties.Add(new StringProperty("new_vatcode", lead.VATCode));
            _callBackData.Properties.Add(new StringProperty("new_taxcode", lead.TAXCode));
            _callBackData.Properties.Add(new StringProperty("new_legalform", lead.LegalForm));
            _callBackData.Properties.Add(new StringProperty("new_numberofemployees", lead.NumberOfEmployees));
            _callBackData.Properties.Add(new StringProperty("new_annualrevenue", lead.AnnualRevenue));
            _callBackData.Properties.Add(new StringProperty("new_preferredcontactmethod", lead.PreferredContactMethod));
            _callBackData.Properties.Add(new StringProperty("new_emailcontact", lead.EmailContact));
            _callBackData.Properties.Add(new StringProperty("new_bulkemailcontact", lead.BulkEmailcontact));
            _callBackData.Properties.Add(new StringProperty("new_phonecontact", lead.PhoneContact));
            _callBackData.Properties.Add(new StringProperty("new_faxcontact", lead.FaxContact));
            _callBackData.Properties.Add(new StringProperty("new_mailcontact", lead.MailContact));
            ///inserisco gli stati
            _callBackData.Properties.Add(new PicklistProperty("new_status", new Picklist((int)DataConstant.ContactLeadState.ToCreate)));
            _callBackData.Properties.Add(new PicklistProperty("new_source", new Picklist((int)DataConstant.ContactLeadSource.Excel)));

            ///Creo il callbackdata
            try
            {
                //using (new CrmImpersonator())
                //{
                Guid idLeadLog = Guid.Empty;
                //using (new CrmImpersonator())
                //{
                ///Crea una riga di lead log
                idLeadLog = base.CurrentCrmService.Create(_callBackData);
                ///Assegna owner id
                AssignRequest richiesta = new AssignRequest()
                {
                    Target = new TargetOwnedDynamic()
                    {
                        EntityId = idLeadLog,
                        EntityName = "new_leadlog"
                    }
                     ,
                    Assignee = new SecurityPrincipal()
                    {
                        PrincipalId = lead.IdOperatorUpload,
                        Type = SecurityPrincipalType.User
                    }
                };
                base.CurrentCrmService.Execute(richiesta);

                ///ritorna idLeadLog
                return idLeadLog;
                //}
            }
            catch (SoapException x)
            {
                string y = x.Detail.InnerText;
                throw new Exception("SetLeadLog: " + x.Detail.InnerText);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// Crea un nuovo Lead
        /// </summary>
        /// <param name="lead"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public Guid SetLead(ContactLead lead, Guid currentUser)
        {
 
                crm = new CrmUtils(base.CurrentCrmService);

                //Innanzitutto vediamo se è un utente di mercato o no
                //Utente di mercato ha almeno un ruolo che contiene IVECO
                //Guid currentUserGuid = new Guid(currentUser);

                if (currentUser != Guid.Empty)
                {
                    QueryExpression query = new QueryExpression(EntityName.role.ToString());
                    query.ColumnSet = new AllColumns();
                    FilterExpression filter = new FilterExpression();
                    filter.FilterOperator = LogicalOperator.And;
                    ConditionExpression condition = new ConditionExpression();
                    condition.AttributeName = "name";
                    condition.Operator = ConditionOperator.Like;
                    condition.Values = new string[] { "Iveco%" };
                    filter.Conditions.Add(condition);
                    query.Criteria = filter;

                    LinkEntity sepp_actp = query.AddLink("systemuserroles", "roleid", "roleid");
                    sepp_actp.LinkCriteria = new FilterExpression() { FilterOperator = LogicalOperator.And };
                    sepp_actp.LinkCriteria.AddCondition("systemuserid", ConditionOperator.Equal, currentUser);

                    List<BusinessEntity> roles = crm.RetrieveMultipleDynamic(query);
                    if (roles != null && roles.Count > 0)
                    {
                        //E' utente di mercato --> lead pubblico
                        marketUser = true;
                    }
                    else
                    {
                        //E' utente dealer --> lead privato
                        marketUser = false;
                    }

                }
                else
                {
                    //Se non inviata Guid, la interpreto come mercato
                    marketUser = true;
                }


                DynamicEntity _contactlead = new DynamicEntity("lead");

                if (!marketUser)
                {
                    //Se non è un utente di mercato, valorizzo BUname e BUcode in base all'utente....sarà poi il pluginOnCreate che valorizzerà 
                    //lo user in base al new/used

                    //ATTENZIONE: potrebbe essere BU figlia, quindi risalgo fino al padre
                    // ---- in pratica, BU è figlia se non ha il parametro di configurazione --> risalgo fino alla prima BU con parametro di configurazione
                    // ----------altrimenti, BU padre       

                    //Recupero la BU dalla systemuser
                    DynamicEntity confParamEntity = GetConfParamFromUser(currentUser);

                    string dealerCode = "";
                    string dealerName = "";
                    if (confParamEntity != null)
                    {
                        dealerCode = confParamEntity.Properties["new_iddealer"].ToString();
                        dealerName = ((Lookup)confParamEntity.Properties["new_businessid"]).name;

                        _contactlead.Properties.Add(new StringProperty("new_dealercode", dealerCode));
                        _contactlead.Properties.Add(new StringProperty("new_dealer", dealerName));
                    }
                }



                if (!channel.ContainsKey("6")) //Vedo se è stato già popolato
                {
                    channel.Add("6", "Web search engine");
                    channel.Add("9", "Iveco Website");
                    channel.Add("21", "Iveco used web site");
                    channel.Add("20", "Other");
                }


                ////Classificazione NACE
                ////Leggo il codice inviato come BusinessType, che corrisponde al codice della trascodifica da cercare in 3 tabelleCARE
                ////relative ai 3 campi NACE
                Guid sezioneTrascodificaId = this.GetTrascodingIdShortDescrizioneByFields("lead", "new_sezioneid", lead.BusinessType);
                if (sezioneTrascodificaId != Guid.Empty && sezioneTrascodificaId != null)
                {
                    //E' il valore di new_sezioneid
                    _contactlead.Properties.Add(new LookupProperty("new_sezioneid", new Lookup("new_trascodifica", sezioneTrascodificaId)));
                }
                else
                {
                    var ret = GetTrascodingCodLivelloSuperioreIdByFields("lead", "new_gruppoid", lead.BusinessType);
                    if (ret.Value != Guid.Empty && ret.Key != Guid.Empty)
                    {
                        //E' il valore di new_gruppoid --> posso recuperare il valore di new_sezioneid con il new_codicelivellosuperiore
                        _contactlead.Properties.Add(new LookupProperty("new_sezioneid", new Lookup("new_trascodifica", ret.Value))); ///livello superiore
                        _contactlead.Properties.Add(new LookupProperty("new_gruppoid", new Lookup("new_trascodifica", ret.Key))); // trascodifica id
                    }
                    else
                    {
                        var value = GetTrascodingCodLivelloSuperioreIdByFields("lead", "new_classeid", lead.BusinessType);
                        if (value.Value != Guid.Empty && value.Key != Guid.Empty)
                        {
                            _contactlead.Properties.Add(new LookupProperty("new_classeid", new Lookup("new_trascodifica", value.Key)));// trascodifica id
                            _contactlead.Properties.Add(new LookupProperty("new_gruppoid", new Lookup("new_trascodifica", value.Value)));///livello superiore
                            //Prendo l'entità relativa al record di livello superiore
                            //trascodingGuidSuperiore = Guid.Empty;
                            DynamicEntity transcodingEntitySuperiore = crm.RetrieveFirstDynamic(new string[] { "new_trascodificaid" }, new object[] { value.Value }, "new_trascodifica");
                            //DynamicEntity transcodingEntitySuperiore = crm.RetrieveDynamic("new_trascodifica", trascodingGuidSuperiore);
                            Guid trascodingGuidSuperiore = ((Lookup)transcodingEntitySuperiore.Properties["new_codicelivellosuperioreid"]).Value;
                            _contactlead.Properties.Add(new LookupProperty("new_sezioneid", new Lookup("new_trascodifica", trascodingGuidSuperiore)));
                        }
                    }
                }


                //_contactlead.Properties.Add(new StringProperty("new_idleadexternal", lead.IDLeadExternal));
                if (!String.IsNullOrEmpty(lead.IDLeadCRMLead))
                    _contactlead.Properties.Add(new StringProperty("new_idleadcrmleadmgmt", lead.IDLeadCRMLead));

                _contactlead.Properties.Add(new PicklistProperty("new_channel", new Picklist(Int32.Parse(lead.Canale, CultureInfo.InvariantCulture))));

                if (!String.IsNullOrEmpty(lead.Customer_Name))
                    _contactlead.Properties.Add(new StringProperty("firstname", lead.Customer_Name));

                if (!String.IsNullOrEmpty(lead.Customer_Surname))
                    _contactlead.Properties.Add(new StringProperty("lastname", lead.Customer_Surname));

                if (!String.IsNullOrEmpty(lead.TypeContact))
                    _contactlead.Properties.Add(new PicklistProperty("new_type", new Picklist(Int32.Parse(lead.TypeContact, CultureInfo.InvariantCulture))));

                if (!String.IsNullOrEmpty(lead.JobDescription))
                {
                    lead.JobDescription = lead.JobDescription.Split('.')[0];
                    _contactlead.Properties.Add(new PicklistProperty("new_interlocutorrole", new Picklist((int)Decimal.Parse(lead.JobDescription, CultureInfo.InvariantCulture))));
                }

                if (!String.IsNullOrEmpty(lead.Company_Name))
                    _contactlead.Properties.Add(new StringProperty("companyname", lead.Company_Name));

                //Se è un utente di mercato --> lead pubblico
                if (marketUser)
                    _contactlead.Properties.Add(new CrmBooleanProperty("new_ispublic", new CrmBoolean(true)));
                else
                    _contactlead.Properties.Add(new CrmBooleanProperty("new_ispublic", new CrmBoolean(false)));

                if (lead.CriticalCustomer != null)
                    _contactlead.Properties.Add(new CrmBooleanProperty("new_criticalcustomer", new CrmBoolean((bool)lead.CriticalCustomer)));

                if (lead.CriticalReason != 0) //0 è il valore che viene mandato per non assegnare nulla
                    _contactlead.Properties.Add(new PicklistProperty("new_criticalreason", new Picklist(lead.CriticalReason)));
                
                
                if (!String.IsNullOrEmpty(lead.Address))
                    _contactlead.Properties.Add(new StringProperty("address1_line1", lead.Address));

                if (!String.IsNullOrEmpty(lead.Province))
                    _contactlead.Properties.Add(new StringProperty("address1_stateorprovince", lead.Province));

                if (!String.IsNullOrEmpty(lead.PhoneNumber))
                    _contactlead.Properties.Add(new StringProperty("address1_telephone1", lead.PhoneNumber));

                if (!String.IsNullOrEmpty(lead.MobilePhoneNumber))
                    _contactlead.Properties.Add(new StringProperty("address1_telephone2", lead.MobilePhoneNumber));

                if (!String.IsNullOrEmpty(lead.IDLeadExternal))
                    _contactlead.Properties.Add(new StringProperty("new_idleadexternal", lead.IDLeadExternal));

                if (!String.IsNullOrEmpty(lead.OfficeNumber))
                    _contactlead.Properties.Add(new StringProperty("address1_telephone3", lead.OfficeNumber));

                if (!String.IsNullOrEmpty(lead.Fax))
                    _contactlead.Properties.Add(new StringProperty("address1_fax", lead.Fax));

                if (!String.IsNullOrEmpty(lead.Email))
                    _contactlead.Properties.Add(new StringProperty("emailaddress1", lead.Email));

                if (!String.IsNullOrEmpty(lead.ProfilingDataH))
                {
                    ///il formato che ci aspettiamo è yyyy-mm-dd hh:mm:ss
                    //lead.ProfilingDataH = "2011-02-25 10:38";
                    var ret = DateTime.MinValue;
                    if (DateTime.TryParse(lead.ProfilingDataH, out ret))
                    {
                        _contactlead.Properties.Add(new CrmDateTimeProperty("new_profilingdate", new CrmDateTime(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:s}", ret))));
                    }
                }

                if (!String.IsNullOrEmpty(lead.CodicePromozione))
                    _contactlead.Properties.Add(new StringProperty("new_promotionalcode", lead.CodicePromozione));

                if (!String.IsNullOrEmpty(lead.ZipCode))
                    _contactlead.Properties.Add(new StringProperty("address1_postalcode", lead.ZipCode));

                if (!String.IsNullOrEmpty(lead.City))
                    _contactlead.Properties.Add(new StringProperty("address1_city", lead.City));

                if (!String.IsNullOrEmpty(lead.Country))
                    _contactlead.Properties.Add(new StringProperty("address1_country", lead.Country));

                if (!String.IsNullOrEmpty(lead.Email))
                    _contactlead.Properties.Add(new StringProperty("emailaddress1", lead.Email));

                if (!String.IsNullOrEmpty(lead.PhoneNumber))
                    _contactlead.Properties.Add(new StringProperty("address1_telephone1", lead.PhoneNumber));

                if (!String.IsNullOrEmpty(lead.Campagna))
                    _contactlead.Properties.Add(new StringProperty("new_campaignname", lead.Campagna));


                if (!String.IsNullOrEmpty(lead.MobilePhoneNumber))
                    _contactlead.Properties.Add(new StringProperty("address1_telephone2", lead.MobilePhoneNumber));

                if (!String.IsNullOrEmpty(lead.NotaProdottoDiInteresse))
                    _contactlead.Properties.Add(new StringProperty("new_productinformation", lead.NotaProdottoDiInteresse));

                if (!String.IsNullOrEmpty(lead.NotaCliente))
                    _contactlead.Properties.Add(new StringProperty("new_customernotes", lead.NotaCliente));


                if (!String.IsNullOrEmpty(lead.NotaUsato))
                    _contactlead.Properties.Add(new StringProperty("new_usedinformation", lead.NotaUsato));


                //if (lead.FlagPrivacy != null)
                //{

                    ////Il flag privacy è gestito al contrario sul CRMDealer
                    //if (lead.FlagPrivacy == true)
                    //    lead.FlagPrivacy = false;
                    //else if (lead.FlagPrivacy == false)
                    //    lead.FlagPrivacy = true;

                    _contactlead.Properties.Add(new CrmBooleanProperty("new_privacy", new CrmBoolean(lead.FlagPrivacy)));
                //}

                _contactlead.Properties.Add(new CrmDateTimeProperty("lastusedincampaign", CrmDateTime.Now));


                //if (String.IsNullOrEmpty(lead.IDLeadCRMLead))
                //{
                //    _contactlead.Properties.Add(new CrmBooleanProperty("new_isimported", new CrmBoolean(true)));
                //}

                ///Davide Trotta 21/02/2011
                ///Campi del lead da aggiungere
                ///Subject campagna
                int channelCode = -1;
                string subject = string.Empty;
                if (!string.IsNullOrEmpty(lead.Canale))
                {
                    if (int.TryParse(lead.Canale, out channelCode))
                    {
                        subject = this.GetPicklistText(DataConstant.OBJECT_TYPE_CODE_LEAD, "new_channel", 1040, channelCode);
                    }
                }
                if (!string.IsNullOrEmpty(subject))
                    _contactlead.Properties.Add(new StringProperty("subject", string.Format(CultureInfo.InvariantCulture, "{0} - {1}", lead.Campagna, subject)));
                else
                    _contactlead.Properties.Add(new StringProperty("subject", string.Format(CultureInfo.InvariantCulture, "{0}", lead.Campagna)));

                /// 
                /// 
                ///hamlet
                if (!string.IsNullOrEmpty(lead.Hamlet))
                {
                    ///In maiuscolo se siamo in italia
                    _contactlead.Properties.Add(new StringProperty("new_hamlet", lead.Hamlet));
                }
                ///motivazione critical customer
                if (!string.IsNullOrEmpty(lead.MotivazioneCriticalCustomer))
                {
                    int ret = 0;
                    if (int.TryParse(lead.MotivazioneCriticalCustomer, out ret))
                    {
                        ///compreso da 1 a 7
                        if (ret >= 1 && ret <= 7)
                            _contactlead.Properties.Add(new PicklistProperty("new_criticalreason", new Picklist(ret)));
                    }
                }
                ///vatcod
                if (!string.IsNullOrEmpty(lead.VATCode))
                    _contactlead.Properties.Add(new StringProperty("new_vatcode", lead.VATCode));
                ///taxcode
                if (!string.IsNullOrEmpty(lead.TAXCode))
                    _contactlead.Properties.Add(new StringProperty("new_taxcode", lead.TAXCode));

                ///Aggiungo le trascodifiche per 
                ///Legal Form
                if (!string.IsNullOrEmpty(lead.LegalForm))
                {
                    Guid id = this.GetTrascodingIdByField("lead", "new_legalformid", lead.LegalForm);
                    if (id != Guid.Empty && id != null)
                        _contactlead.Properties.Add(new LookupProperty("new_legalformid", new Lookup("new_trascodifica", id)));
                }
                ///Number Of Employee
                if (!string.IsNullOrEmpty(lead.NumberOfEmployees))
                {
                    Guid id = this.GetTrascodingIdByField("lead", "new_numberofemployeesid", lead.NumberOfEmployees);
                    if (id != Guid.Empty && id != null)
                        _contactlead.Properties.Add(new LookupProperty("new_numberofemployeesid", new Lookup("new_trascodifica", id)));
                }
                ///Annual Rebeuve
                if (!string.IsNullOrEmpty(lead.AnnualRevenue))
                {
                    Guid id = this.GetTrascodingIdByField("lead", "new_revenueid", lead.AnnualRevenue);
                    if (id != Guid.Empty && id != null)
                        _contactlead.Properties.Add(new LookupProperty("new_revenueid", new Lookup("new_trascodifica", id)));
                }
                ///Boolean Contact
                if (!string.IsNullOrEmpty(lead.BulkEmailcontact))
                {
                    bool ret = false;
                    if (lead.BulkEmailcontact.Contains("0") || lead.BulkEmailcontact.Contains("1"))
                    {
                        ret = (lead.BulkEmailcontact.Contains("0")) ? false : true;
                        _contactlead.Properties.Add(new CrmBooleanProperty("donotbulkemail", new CrmBoolean(ret)));
                    }
                }
                if (!string.IsNullOrEmpty(lead.MailContact))
                {
                    bool ret = false;
                    if (lead.MailContact.Contains("0") || lead.MailContact.Contains("1"))
                    {
                        ret = (lead.MailContact.Contains("0")) ? false : true;
                        _contactlead.Properties.Add(new CrmBooleanProperty("donotpostalmail", new CrmBoolean(ret)));
                    }
                }
                if (!string.IsNullOrEmpty(lead.FaxContact))
                {
                    bool ret = false;
                    if (lead.FaxContact.Contains("0") || lead.FaxContact.Contains("1"))
                    {
                        ret = (lead.FaxContact.Contains("0")) ? false : true;
                        _contactlead.Properties.Add(new CrmBooleanProperty("donotfax", new CrmBoolean(ret)));
                    }
                }
                if (!string.IsNullOrEmpty(lead.EmailContact))
                {
                    bool ret = false;
                    if (lead.EmailContact.Contains("0") || lead.EmailContact.Contains("1"))
                    {
                        ret = (lead.EmailContact.Contains("0")) ? false : true;
                        _contactlead.Properties.Add(new CrmBooleanProperty("donotemail", new CrmBoolean(ret)));
                    }
                }
                if (!string.IsNullOrEmpty(lead.PhoneContact))
                {
                    bool ret = false;
                    if (lead.PhoneContact.Contains("0") || lead.PhoneContact.Contains("1"))
                    {
                        ret = (lead.PhoneContact.Contains("0")) ? false : true;
                        _contactlead.Properties.Add(new CrmBooleanProperty("donotphone", new CrmBoolean(ret)));
                    }
                }
                if (!string.IsNullOrEmpty(lead.PreferredContactMethod))
                {
                    int ret = 0;
                    if (int.TryParse(lead.PreferredContactMethod, out ret))
                    {
                        if (ret == (int)DataConstant.PreferedMethodOfContactLead.Any)
                            _contactlead.Properties.Add(new PicklistProperty("preferredcontactmethodcode", new Picklist(ret)));
                        else if (ret == (int)DataConstant.PreferedMethodOfContactLead.EMail)
                            _contactlead.Properties.Add(new PicklistProperty("preferredcontactmethodcode", new Picklist(ret)));
                        else if (ret == (int)DataConstant.PreferedMethodOfContactLead.Fax)
                            _contactlead.Properties.Add(new PicklistProperty("preferredcontactmethodcode", new Picklist(ret)));
                        else if (ret == (int)DataConstant.PreferedMethodOfContactLead.Mail)
                            _contactlead.Properties.Add(new PicklistProperty("preferredcontactmethodcode", new Picklist(ret)));
                        else if (ret == (int)DataConstant.PreferedMethodOfContactLead.Mobile)
                            _contactlead.Properties.Add(new PicklistProperty("preferredcontactmethodcode", new Picklist(ret)));
                        else if (ret == (int)DataConstant.PreferedMethodOfContactLead.Phone)
                            _contactlead.Properties.Add(new PicklistProperty("preferredcontactmethodcode", new Picklist(ret)));
                    }
                }
                ///Is manual sempre impostato a false quando si passa dalla set lead
                _contactlead.Properties.Add(new CrmBooleanProperty("new_ismanual", new CrmBoolean(false)));
                try
                {
                    Guid leadId = Guid.Empty;
                    //using (new CrmImpersonator())
                    //{
                    //Creo il lead
                    leadId = base.CurrentCrmService.Create(_contactlead);

                    try
                    {
                        //Se il lead proviene da file, setto lo stato del lead su Imported (se leadIdCrmLead non è valorizzato)
                        //Altrimenti, se id valorizzato, proviene da LeadManagement e quindi stato Iveco
                        SetStateLeadRequest setleadstate = new SetStateLeadRequest();
                        setleadstate.EntityId = leadId;
                        setleadstate.LeadState = LeadState.Open;

                        if (String.IsNullOrEmpty(lead.IDLeadCRMLead))
                            setleadstate.LeadStatus = 2; //Imported     //Prima ho settato il bit nascosto isimported sul form (nascosto)
                        else
                            setleadstate.LeadStatus = 200001; //Iveco

                        base.CurrentCrmService.Execute(setleadstate);

                        return leadId;
                    }
                    catch (SoapException x)
                    {
                        //Potrebbe andare in errore perchè il plugin che scatta in creazione cancella il lead appena creato
                        return Guid.Empty;
                    }
                    catch (Exception e)
                    {
                        //Potrebbe andare in errore perchè il plugin che scatta in creazione cancella il lead appena creato
                        return Guid.Empty;
                    }
                }
                catch (SoapException x)
                {
                    string y = x.Detail.InnerText;
                    throw new Exception("SetContactLead: " + x.Detail.InnerText);
                }
                catch (Exception e)
                {

                    throw new Exception("SetContactLead: " + e.Message);
                }

            
        }

        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Restituisce il valore di una tracodifica
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        private Guid GetTrascodingIdShortDescrizioneByFields(string tableName, string attributeName, string attributeValue)
        {
            return (from New_tablecrm crm in base.CurrentCrmDealerDataContext.New_tablecrms
                    join New_tabellacare care in base.CurrentCrmDealerDataContext.New_tabellacares
                        on crm.new_tablecareid equals care.New_tabellacareId
                    join New_trascodifica tras in base.CurrentCrmDealerDataContext.New_trascodificas
                        on care.New_tabellacareId equals tras.new_tabellacaresaid
                    where crm.DeletionStateCode == 0 &&
                    care.DeletionStateCode == 0 &&
                    crm.New_name == tableName &&
                    crm.New_nomecampocrm == attributeName &&
                    tras.New_ShortDescrizioneCARE == attributeValue
                    select tras.New_trascodificaId).SingleOrDefault();
        }
        /// <summary>
        /// Restituisce il valore di una tracodifica
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        private Guid GetTrascodingIdByField(string tableName, string attributeName, string attributeValue)
        {
            return (from New_tablecrm crm in base.CurrentCrmDealerDataContext.New_tablecrms
                    join New_tabellacare care in base.CurrentCrmDealerDataContext.New_tabellacares
                        on crm.new_tablecareid equals care.New_tabellacareId
                    join New_trascodifica tras in base.CurrentCrmDealerDataContext.New_trascodificas
                        on care.New_tabellacareId equals tras.new_tabellacaresaid
                    where crm.DeletionStateCode == 0 &&
                    care.DeletionStateCode == 0 &&
                    crm.New_name == tableName &&
                    crm.New_nomecampocrm == attributeName &&
                    tras.New_DescrizioneCARE == attributeValue
                    select tras.New_trascodificaId).SingleOrDefault();
        }
        /// <summary>
        /// Restituisce il valore di una tracodifica
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        private KeyValuePair<Guid, Guid> GetTrascodingCodLivelloSuperioreIdByFields(string tableName, string attributeName, string attributeValue)
        {
            var ret = (from New_tablecrm crm in base.CurrentCrmDealerDataContext.New_tablecrms
                       join New_tabellacare care in base.CurrentCrmDealerDataContext.New_tabellacares
                           on crm.new_tablecareid equals care.New_tabellacareId
                       join New_trascodifica tras in base.CurrentCrmDealerDataContext.New_trascodificas
                           on care.New_tabellacareId equals tras.new_tabellacaresaid
                       where crm.DeletionStateCode == 0 &&
                       care.DeletionStateCode == 0 &&
                       crm.New_name == tableName &&
                       crm.New_nomecampocrm == attributeName &&
                       tras.New_ShortDescrizioneCARE == attributeValue
                       select new
                           {
                               trascodingId = tras.New_trascodificaId,
                               codLivelloSuperiore = tras.new_codicelivellosuperioreid
                           }).SingleOrDefault();

            if (ret != null)
                return new KeyValuePair<Guid, Guid>(ret.trascodingId, ret.codLivelloSuperiore.Value);
            return new KeyValuePair<Guid, Guid>(Guid.Empty, Guid.Empty);
        }
        #endregion

        private Guid GetCampaignActivity(Guid campaignGuid)
        {
            //using (new CrmImpersonator())
            //{
            DynamicEntity campaignActEntity = crm.RetrieveFirstDynamic(new string[] { "regardingobjectid" }, new object[] { campaignGuid }, "campaignactivity");
            if (campaignActEntity != null)
                return ((Key)campaignActEntity.Properties["activityid"]).Value;
            else
                return Guid.Empty;
            //}
        }

        private Guid GetDefaultSeller()
        {
            Guid defaultSellerId = Guid.Empty;
            //using (new CrmImpersonator())
            //{
            //L'utente a cui assegnare le campagne è il defaul seller della root BU
            //Root BU
            List<BusinessEntity> rootBUEntities = crm.RetrieveMultipleDynamicWithNull(new string[] { "parentbusinessunitid" }, "businessunit");
            if (rootBUEntities.Count > 0)
            {
                DynamicEntity rootBUEntity = (DynamicEntity)rootBUEntities[0];
                Guid buRootId = ((Key)rootBUEntity.Properties["businessunitid"]).Value;

                //Recupero i parametri di configurazione per la BU root
                List<BusinessEntity> rootConfParamEntities = crm.RetrieveMultipleDynamic(new string[] { "new_businessid" }, new object[] { buRootId }, "new_parametridiconfigurazione");
                if (rootConfParamEntities.Count > 0)
                {
                    //Log.Debug("Trovata BU root");
                    DynamicEntity rootConfParamEntity = (DynamicEntity)rootConfParamEntities[0];
                    //Vedo nell BUroot se per quel tenant gli used devono andare al mercato
                    defaultSellerId = ((Lookup)rootConfParamEntity.Properties["new_userid"]).Value;
                }
            }
            else
            {
                //Se sono qui vuol dire che non ho trovato nessun root: ERRORE
                Log.Debug("Non trovata BU root...");

            }
            //}
            return defaultSellerId;
        }

        private void CreateCampaign(Guid leadId, ContactLead lead, Guid defaultSellerId)
        {
            AddItemCampaignActivityResponse createdCampaignActivityItem = null;
            Guid createdCampaignActivityId = Guid.Empty;
            try
            {

                // Set up the CRM Service.  
                CrmService service = base.CurrentCrmService;
                //service.PreAuthenticate = true;

                //using (new CrmImpersonator())
                //{
                #region Setup Data Required

                // Create a campaign.
                campaign sampleCampaign = new campaign();
                sampleCampaign.name = lead.Campagna;
                sampleCampaign.ownerid = new Owner("systemuser", defaultSellerId);

                Guid createdCampaignId = service.Create(sampleCampaign);


                //Setto lo statsreason della campagna a "Launched"
                SetStateCampaignRequest campStatusReq = new SetStateCampaignRequest();
                campStatusReq.EntityId = createdCampaignId;
                campStatusReq.CampaignState = CampaignState.Active;
                campStatusReq.CampaignStatus = 2; //Launched
                service.Execute(campStatusReq);


                //Aggiorno il lead
                DynamicEntity leadToUpdate = new DynamicEntity(EntityName.lead.ToString());
                leadToUpdate.Properties.Add(new KeyProperty("leadid", new Key(leadId)));
                //campaignGuid = ((Key)sampleCampaign.Properties["campaignid"]).Value;
                leadToUpdate.Properties.Add(new LookupProperty("campaignid", new Lookup("campaign", createdCampaignId)));
                base.CurrentCrmService.Update(leadToUpdate);
                Log.Debug("Aggiornato lead");

                // Create a campaign activity.
                campaignactivity sampleActivity = new campaignactivity();
                sampleActivity.regardingobjectid = new Lookup();
                sampleActivity.regardingobjectid.type = EntityName.campaign.ToString();
                //sampleActivity.regardingobjectid.Value = createdCampaign.id;
                sampleActivity.regardingobjectid.Value = createdCampaignId;
                sampleActivity.subject = lead.Campagna + " " + channel[lead.Canale];
                sampleActivity.channeltypecode = new Picklist(1);  // 1 == phone


                sampleActivity.ownerid = new Owner("systemuser", defaultSellerId);

                createdCampaignActivityId = service.Create(sampleActivity);

                //Devo settare il channel come il lead
                DynamicEntity campaignActivityEntity = crm.RetrieveFirstDynamic(new string[] { "activityid" }, new object[] { createdCampaignActivityId }, "campaignactivity");
                if (campaignActivityEntity != null)
                {
                    DynamicEntity campaignActToUpdate = new DynamicEntity(EntityName.campaignactivity.ToString());
                    Guid campaignActGuid = ((Key)campaignActivityEntity.Properties["activityid"]).Value;
                    campaignActToUpdate.Properties.Add(new KeyProperty("activityid", new Key(campaignActGuid)));
                    campaignActToUpdate.Properties.Add(new PicklistProperty("new_channel", new Picklist(Int32.Parse(lead.Canale, CultureInfo.InvariantCulture))));
                    base.CurrentCrmService.Update(campaignActToUpdate);
                }


                // Create a list to add to the activity.
                list marketingList = new list();
                marketingList.listname = lead.Campagna + " " + channel[lead.Canale];
                marketingList.createdfromcode = new Picklist();
                marketingList.createdfromcode.Value = 4;    // 4 == lead


                marketingList.ownerid = new Owner("systemuser", defaultSellerId);

                Guid createdMarketingListId = service.Create(marketingList);

                //// Create an account to add to the marketing list.  
                //// This will be the name returned by the RetrieveMembersBulkOperation message.
                //account sampleAccount = new account();
                //sampleAccount.name = "Fourth Coffee";

                Guid createdLeadId = leadId;

                AddMemberListRequest addAccountRequest = new AddMemberListRequest();
                addAccountRequest.ListId = createdMarketingListId;
                addAccountRequest.EntityId = createdLeadId;

                service.Execute(addAccountRequest);

                // First, associate the list with the campaign.
                AddItemCampaignRequest addListToCampaignRequest = new AddItemCampaignRequest();
                addListToCampaignRequest.CampaignId = createdCampaignId;
                addListToCampaignRequest.EntityName = EntityName.list;
                addListToCampaignRequest.EntityId = createdMarketingListId;

                AddItemCampaignResponse createdCampaignItem = (AddItemCampaignResponse)service.Execute(addListToCampaignRequest);

                // Then, associate the list with the campaign activity.
                AddItemCampaignActivityRequest addListToCampaignActivityRequest = new AddItemCampaignActivityRequest();
                addListToCampaignActivityRequest.CampaignActivityId = createdCampaignActivityId;
                addListToCampaignActivityRequest.EntityName = EntityName.list;
                addListToCampaignActivityRequest.ItemId = createdMarketingListId;

                createdCampaignActivityItem = (AddItemCampaignActivityResponse)service.Execute(addListToCampaignActivityRequest);

                #endregion
                //}
                // Create a phone activity.
                Guid phoneGuid = CreatePhoneCall(leadId, lead.Campagna + " " + channel[lead.Canale], lead.PhoneNumber, createdCampaignActivityId, GetDefaultSeller());
                //phonecall samplePhoneCall = new phonecall();

                //activityparty toParty = new activityparty();
                //toParty.partyid = new Lookup();
                //toParty.partyid.type = EntityName.lead.ToString();
                //toParty.partyid.Value = leadId;

                //samplePhoneCall.to = new activityparty[] { toParty };
                //samplePhoneCall.subject = lead.Campagna + " " + channel[lead.Canale];
                //samplePhoneCall.regardingobjectid = new Lookup(EntityName.campaignactivity.ToString(), createdCampaignActivityId);
                //samplePhoneCall.ownerid = new Owner("systemuser", defaultSellerId);
                //samplePhoneCall.scheduledend = CrmDateTime.Now;

                //Guid phoneGuid = service.Create(samplePhoneCall);
                //using (new CrmImpersonator())
                //{
                //Devo settare il channel come il lead
                DynamicEntity phoneEntity = crm.RetrieveFirstDynamic(new string[] { "activityid" }, new object[] { phoneGuid }, "phonecall");
                if (phoneEntity != null)
                {
                    DynamicEntity phoneToUpdate = new DynamicEntity(EntityName.phonecall.ToString());
                    Guid phoneCallGuid = ((Key)phoneEntity.Properties["activityid"]).Value;
                    phoneToUpdate.Properties.Add(new KeyProperty("activityid", new Key(phoneGuid)));
                    phoneToUpdate.Properties.Add(new PicklistProperty("new_channel", new Picklist(Int32.Parse(lead.Canale, CultureInfo.InvariantCulture))));
                    base.CurrentCrmService.Update(phoneToUpdate);
                }
                //}


                //// The owner property is REQUIRED.
                //WhoAmIRequest systemUserRequest = new WhoAmIRequest();
                //WhoAmIResponse systemUser = (WhoAmIResponse)service.Execute(systemUserRequest);

                //// Execute a bulk operation.
                //DistributeCampaignActivityRequest distributeCampaignRequest = new DistributeCampaignActivityRequest();
                //distributeCampaignRequest.Activity = samplePhoneCall;
                //distributeCampaignRequest.CampaignActivityId = createdCampaignActivityId;
                //distributeCampaignRequest.Propagate = true;
                //distributeCampaignRequest.SendEmail = false;
                //distributeCampaignRequest.Owner = new Moniker();
                //distributeCampaignRequest.Owner.Id = systemUser.UserId;
                //distributeCampaignRequest.Owner.Name = EntityName.systemuser.ToString();

                //DistributeCampaignActivityResponse distributeCampaignResponse = (DistributeCampaignActivityResponse)service.Execute(distributeCampaignRequest);

                //// Execute the request.
                //RetrieveMembersBulkOperationRequest getMembers = new RetrieveMembersBulkOperationRequest();
                //getMembers.BulkOperationId = distributeCampaignResponse.BulkOperationId;
                //getMembers.BulkOperationSource = BulkOperationSource.CampaignActivity;
                //getMembers.EntitySource = EntitySource.Account;
                //getMembers.ReturnDynamicEntities = false;

                //RetrieveMembersBulkOperationResponse membersResponse = (RetrieveMembersBulkOperationResponse)service.Execute(getMembers);

            }
            catch (Exception e)
            {

            }
        }

        private Guid CreatePhoneCall(Guid leadId, string subject, string phone, Guid campaignActivityId, Guid userId)
        {
            // Create a phone activity.
            phonecall samplePhoneCall = new phonecall();

            activityparty toParty = new activityparty();
            toParty.partyid = new Lookup();
            toParty.partyid.type = EntityName.lead.ToString();
            toParty.partyid.Value = leadId;

            samplePhoneCall.to = new activityparty[] { toParty };
            samplePhoneCall.subject = subject;
            samplePhoneCall.regardingobjectid = new Lookup(EntityName.campaignactivity.ToString(), campaignActivityId);
            samplePhoneCall.ownerid = new Owner("systemuser", userId);
            samplePhoneCall.scheduledend = CrmDateTime.Now;
            samplePhoneCall.phonenumber = phone;
            //using (new CrmImpersonator())
            //{
            return crm.Create(samplePhoneCall);
            //}
        }


        private DynamicEntity GetConfParamFromBU(Guid BUid)
        {
            //new_businessid in confparam
            DynamicEntity confParamEntity = crm.RetrieveFirstDynamic(new string[] { "new_businessid" }, new object[] { BUid }, "new_parametridiconfigurazione");
            if (confParamEntity != null)
            {
                return confParamEntity;
            }
            else
            {

                return null;
            }
        }

        private DynamicEntity GetConfParamFromUser(Guid Userid)
        {
            DynamicEntity confParamEntity = null;
            DynamicEntity systemUserEntity = crm.RetrieveFirstDynamic(new string[] { "systemuserid" }, new object[] { Userid }, "systemuser");
            if (systemUserEntity != null)
            {
                Guid BUid = ((Lookup)systemUserEntity.Properties["businessunitid"]).Value;
                while (confParamEntity == null)
                {
                    confParamEntity = GetConfParamFromBU(BUid);
                    DynamicEntity buEntity = null;
                    if (confParamEntity == null)
                    {
                        buEntity = crm.RetrieveFirstDynamic(new string[] { "businessunitid" }, new object[] { BUid }, "businessunit");
                        if (buEntity.Properties.Contains("parentbusinessunitid"))
                        {
                            BUid = ((Lookup)buEntity.Properties["parentbusinessunitid"]).Value;
                        }
                    }
                    else
                    {
                        return confParamEntity;
                    }

                }

                return null;

            }
            return null;

        }

        /// <summary>
        /// Ottiene un valore della picklist trascodificato in lingua
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="langCode"></param>
        /// <param name="picklistValue"></param>
        /// <returns></returns>
        private string GetPicklistText(int objectTypeCodeEntity, string attributeName, int langCode, int picklistValue)
        {
            return base.CurrentCrmDealerDataContext.StringMaps.Where(c => c.ObjectTypeCode == objectTypeCodeEntity && c.LangId == langCode && c.AttributeName == attributeName && c.AttributeValue == picklistValue).Select(c => c.Value).SingleOrDefault();
        }

        /// <summary>
        /// Rilascio delle risorse       
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        public DealerInfo FindDealer(ContactLead lead)
        {
            crm = new CrmUtils(base.CurrentCrmService);

            DealerInfo dealer = new DealerInfo();

            Guid leadId = Guid.Empty;
            Guid userId = Guid.Empty;
            Guid BUid = Guid.Empty;
            string iddealer = String.Empty;
            string leadCode = (string)lead.IDLeadCRMLead;
            bool flagCritico = false;
            int criticalReasonCode = 0;


            //Vado a controllare per la BU root il flag new/used, rispetto al new_type del lead
            //Root BU
            List<BusinessEntity> rootBUEntities = crm.RetrieveMultipleDynamicWithNull(new string[] { "parentbusinessunitid" }, "businessunit");

            DynamicEntity rootBUEntity = new DynamicEntity();
            DynamicEntity rootConfParamEntity = new DynamicEntity();
            bool usedLeadToMarket = false;

            if (rootBUEntities.Count > 0)
            {
                rootBUEntity = (DynamicEntity)rootBUEntities[0];
                Guid buRootId = ((Key)rootBUEntity.Properties["businessunitid"]).Value;

                //Recupero i parametri di configurazione per la BU root
                List<BusinessEntity> rootConfParamEntities = crm.RetrieveMultipleDynamic(new string[] { "new_businessid" }, new object[] { buRootId }, "new_parametridiconfigurazione");
                if (rootConfParamEntities.Count > 0)
                {
                    //Log.Debug("Trovata BU root");
                    rootConfParamEntity = (DynamicEntity)rootConfParamEntities[0];
                    //Vedo nell BUroot se per quel tenant gli used devono andare al mercato
                    usedLeadToMarket = ((CrmBoolean)rootConfParamEntity.Properties["new_usedleadtomarket"]).Value;
                }
            }
            else
            {
                //Se sono qui vuol dire che non ho trovato nessun root: ERRORE
                Log.Debug("Non trovata BU root...");
            }


            //Vedo se lead critico o no
            bool criticalLead = (bool)lead.CriticalCustomer;
            bool leadused;
            if (lead.TypeContact == "1")
                leadused = false;
            else
                leadused = true;

            

            DynamicEntity confParamDealerEntity = null;
            if (leadused)
            {
                if (usedLeadToMarket)
                {
                    //Lead usato + usati al mercato
                    //Vado nella BU root e prendo il default market user used
                    userId = ((Lookup)rootConfParamEntity.Properties["new_marketdefaultuserusedid"]).Value;
                    //crm.Assign("lead", leadId, userId, SecurityPrincipalType.User);

                    marketUser = true;
                    flagCritico = true;
                      
                 }
                 else
                 {
                    //Lead usato + usato NON al mercato
                    //Cerco dealer in base allo zipCode
                    List<DynamicEntity> confParamEntities = GetConfParamDealer(lead.ZipCode);
                    if (confParamEntities.Count >= 1)
                    {
                        //trovati dealer con quello zipcode
                        if (confParamEntities.Count == 1)
                        {
                                //Log.Debug("Trovato un solo dealer con quello zipcode");
                                confParamDealerEntity = confParamEntities[0];
                                if (((CrmBoolean)confParamDealerEntity.Properties["new_crmdealeradoptedbydealer"]).Value) //Se ha aderito al CRMDealer
                                {
                                    //Log.Debug("Il dealer ha aderito");
                                    //HA aderito al CRMDealer
                                    if (criticalLead)
                                    {
                                        //lead critico
                                        userId = ((Lookup)confParamDealerEntity.Properties["new_marketdefaultuserusedid"]).Value;

                                        marketUser = true;
                                        flagCritico = true;
                                    }
                                    else
                                    {
                                        //lead non critico
                                        userId = ((Lookup)confParamDealerEntity.Properties["new_dealerdefaultuserusedid"]).Value;

                                        BUid = ((Lookup)confParamDealerEntity.Properties["new_businessid"]).Value;
                                        //Log.Debug("BUid " + BUid.ToString());

                                        iddealer = confParamDealerEntity.Properties["new_iddealer"].ToString();
                                        //Log.Debug("iddealer " + iddealer);

                                        marketUser = false;
                                    }
                                }
                                else
                                {
                                    //Il dealer non ha aderito
                                    
                                    //if (!String.IsNullOrEmpty(leadCode))
                                    //{
                                  
                                           
                                            userId = ((Lookup)confParamDealerEntity.Properties["new_dealerdefaultuserusedid"]).Value;
                                           
                                            BUid = ((Lookup)confParamDealerEntity.Properties["new_businessid"]).Value;
                                            //Log.Debug("BUid " + BUid.ToString());

                                            iddealer = confParamDealerEntity.Properties["new_iddealer"].ToString();
                                            //Log.Debug("iddealer " + iddealer);

                                            marketUser = false;
                                    //}
                                    //else
                                    //{
                                    //    //Non essendoci il CRMIdLead, non è arrivato da CRMLead e quindi non lo cancello, ma lo assegno al market (new/used) della BU dealer
                                    //    userId = ((Lookup)confParamDealerEntity.Properties["new_marketdefaultuserusedid"]).Value;
                                    //    marketUser = true;
                                    //    flagCritico = true;

                                    //}

                                }
                            }
                            else
                            {
                                //Trovato più di un dealer con quello zipcode
                                //Log.Debug("Trovato più di un dealer con quello zipcode");
                                //Controllo se almeno uno ha aderito.....prendo il primo che trovo e che abbia aderito
                                confParamDealerEntity = null;
                                bool dealerFound = false;
                                for(int i=0; i<confParamEntities.Count;i++)
                                {
                                    confParamDealerEntity = confParamEntities[i];
                                    if(((CrmBoolean)confParamDealerEntity.Properties["new_crmdealeradoptedbydealer"]).Value) //Se ha aderito al CRMDealer
                                    {
                                        //HA aderito al CRMDealer
                                        //Log.Debug("HA aderito al CRMDealer");
                                        userId = ((Lookup)confParamDealerEntity.Properties["new_marketdefaultuserusedid"]).Value;
                                        dealerFound = true;
                                        marketUser = true;

                                        flagCritico = true;
                                        criticalReasonCode = 7;

                                        break;
                                    }
                                }
                                if (confParamDealerEntity == null || dealerFound == false)
                                {
                                   
                                    //inviare flag dealer agree a 0
                                    ////Se è valorizzato il campo new_idleadcrmleadmgmt --> chiamo il webservice di ritorno verso CRMLead
                                    //if (!String.IsNullOrEmpty(leadCode))
                                    //{
                                        
                                       userId = ((Lookup)confParamDealerEntity.Properties["new_marketdefaultuserusedid"]).Value;
                                            
                                       marketUser = true;
                                       flagCritico = true;

                                       //11Marzo---------------
                                       criticalReasonCode = 7;
                                       //----------------------
                                    //}
                                    //else
                                    //{
                                    //    //Non essendoci il CRMIdLead, non è arrivato da CRMLead e quindi non lo cancello, ma lo assegno al market (new/used) della BU dealer
                                    //    userId = ((Lookup)confParamDealerEntity.Properties["new_marketdefaultuserusedid"]).Value;
                                    
                                    //    marketUser = true;
                                    //    flagCritico = true;
                                    //}

                                }

                            }
                        }
                        else
                        {
                            //non trovati dealer per quello zipcode
                            //Log.Debug("non trovati dealer per quello zipcode");
                            userId = ((Lookup)rootConfParamEntity.Properties["new_marketdefaultuserusedid"]).Value;
                            criticalReasonCode = 8;
                            marketUser = true;
                            flagCritico = true;
                        }
                    }
                }
                else
                {
                    //Lead nuovo
                    //Log.Debug("Lead nuovo");
                    //Cerco dealer in base allo zipCode
                    List<DynamicEntity> confParamEntities = GetConfParamDealer(lead.ZipCode);
                    if (confParamEntities.Count >= 1)
                    {
                        //trovati dealer con quello zipcode
                        //Log.Debug("trovati dealer con quello zipcode");
                        if (confParamEntities.Count == 1)
                        {
                            //Log.Debug("Trovato un dealer");
                            confParamDealerEntity = confParamEntities[0];
                            if (((CrmBoolean)confParamDealerEntity.Properties["new_crmdealeradoptedbydealer"]).Value) //Se ha aderito al CRMDealer
                            {
                                //Log.Debug("Dealer ha aderito");
                                //HA aderito al CRMDealer
                                if (criticalLead)
                                {
                                    //lead critico
                                    userId = ((Lookup)confParamDealerEntity.Properties["new_marketdefaultusernewid"]).Value;
                                 
                                    marketUser = true;
                                    flagCritico = true;
                                }
                                else
                                {
                                    //lead non critico
                                    userId = ((Lookup)confParamDealerEntity.Properties["new_dealerdefaultusernewid"]).Value;
                                   
                                    BUid = ((Lookup)confParamDealerEntity.Properties["new_businessid"]).Value;
                                    iddealer = confParamDealerEntity.Properties["new_iddealer"].ToString();
                                    marketUser = false;
                                    
                                }
                            }
                            else
                            {
                                //Il dealer non ha aderito
                                //if (!String.IsNullOrEmpty(leadCode))
                                //{
                                  
                                        userId = ((Lookup)confParamDealerEntity.Properties["new_dealerdefaultusernewid"]).Value;
                                      
                                        BUid = ((Lookup)confParamDealerEntity.Properties["new_businessid"]).Value;
                                       
                                        iddealer = confParamDealerEntity.Properties["new_iddealer"].ToString();
                                       
                                        marketUser = false;

                                        
                                //}
                                //else
                                //{
                                //    //Non essendoci il CRMIdLead, non è arrivato da CRMLead e quindi non lo cancello, ma lo assegno al market (new/used) della BU dealer
                                //    userId = ((Lookup)confParamDealerEntity.Properties["new_marketdefaultusernewid"]).Value;
                          
                                //    marketUser = true;
                                //    flagCritico = true;
                                //}

                              

                            }
                        }
                        else
                        {
                            //Trovato più di un dealer con quello zipcode
                            //Log.Debug("Trovato più di un dealer con quello zipcode");
                            //Controllo se almeno uno ha aderito.....prendo il primo che trovo e che abbia aderito
                            confParamDealerEntity = null;
                            bool dealerFound = false;
                            for (int i = 0; i < confParamEntities.Count; i++)
                            {
                                confParamDealerEntity = confParamEntities[i];
                                if (((CrmBoolean)confParamDealerEntity.Properties["new_crmdealeradoptedbydealer"]).Value) //Se ha aderito al CRMDealer
                                {
                                    //HA aderito al CRMDealer
                                    userId = ((Lookup)confParamDealerEntity.Properties["new_marketdefaultusernewid"]).Value;
                                    

                                    //BUid = ((Lookup)confParamDealerEntity.Properties["new_businessid"]).Value;
                                    //iddealer = confParamDealerEntity.Properties["new_iddealer"].ToString();
                                    marketUser = true;
                                    dealerFound = true;

                                    flagCritico = true;
                                    criticalReasonCode = 7;
                                  
                                    break;
                                }
                            }
                            if (confParamDealerEntity == null || dealerFound == false)
                            {
                                //Log.Debug("inviare flag dealer agree a 0");
                                //inviare flag dealer agree a 0
                                //if (!String.IsNullOrEmpty(leadCode))
                                //{
                                   
                                     
                                        userId = ((Lookup)confParamDealerEntity.Properties["new_marketdefaultusernewid"]).Value;

                                        marketUser = true;
                                        flagCritico = true;

                                        //11Marzo---------------
                                        criticalReasonCode = 7;
                                        //----------------------

                                
                             
                                //}
                                //else
                                //{
                                //    //Non essendoci il CRMIdLead, non è arrivato da CRMLead e quindi non lo cancello, ma lo assegno al market (new/used) della BU dealer
                                //    userId = ((Lookup)confParamDealerEntity.Properties["new_marketdefaultusernewid"]).Value;
                                   
                                //    marketUser = true;
                                //    flagCritico = true;
                            
                                //}

                            }

                        }
                    }
                    else
                    {
                        //non trovati dealer per quello zipcode
                        //Log.Debug("non trovati dealer per quello zipcode");
                        userId = ((Lookup)rootConfParamEntity.Properties["new_marketdefaultusernewid"]).Value;
                        marketUser = true;
                        criticalReasonCode = 8;
                        flagCritico = true;
   
                    }
                }

                string userName = "";
                string email = "";
                List<BusinessEntity> userEntities = crm.RetrieveMultipleDynamic(new string[] { "systemuserid" }, new object[] { userId }, "systemuser");
                if (userEntities.Count > 0)
                {
                    try
                    {
                        DynamicEntity userEntity = (DynamicEntity)userEntities[0];
                        string firstname = userEntity.Properties["firstname"].ToString();
                        string lastname = userEntity.Properties["lastname"].ToString();
                        userName = firstname + " " + lastname;
                        


                        if (userEntity.Properties.Contains("internalemailaddress"))
                            email = userEntity.Properties["internalemailaddress"].ToString();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                //Recupero la BUname
                //Log.Debug("Recupero la BUname");
                string buname = "";
                List<BusinessEntity> buEntities = crm.RetrieveMultipleDynamic(new string[] { "businessunitid" }, new object[] { BUid }, "businessunit");
                if (buEntities.Count > 0)
                {
                    DynamicEntity buEntity = (DynamicEntity)buEntities[0];
                    buname = buEntity.Properties["name"].ToString();
                }

              
          
                if (marketUser)
                {
                    dealer.MarketingAccount = userName;
                    dealer.EmailMarketingAccount = email;
                }
                else
                {
                    dealer.DealerResponsible = userName;
                    dealer.DealerEmail = email;
                    dealer.DealerCode = iddealer;
                    dealer.DealerCompanyName = buname;
                }

                dealer.Critical = flagCritico;
                dealer.CriticalReason = criticalReasonCode;

                //dealer.IdLeadCrm = leadCode;
                         
            return dealer;
        }


        private List<DynamicEntity> GetConfParamDealer(string zipcode)
        {
            List<DynamicEntity> confParamEntities = new List<DynamicEntity>();
            List<Guid> BUIds = new List<Guid>();


            //Vado a cercare tra le territorialità quella con lo zipcode indicato
            List<BusinessEntity> terrEntities = crm.RetrieveMultipleDynamic(new string[] { "new_zipcode" }, new object[] { zipcode }, "new_territorialitydealer");
            //Log.Debug("Trovate territorialità: " + terrEntities.Count.ToString());

            DynamicEntity terrEntity = null;
            Guid terrDealerId = Guid.Empty;
            Guid confParamId = Guid.Empty;

            Guid BUid = Guid.Empty;
            DynamicEntity confParamEntity = null;
            List<BusinessEntity> buEntities = new List<BusinessEntity>();
            DynamicEntity bu = null;
            bool isdisabled = false;
            //Log.Debug("Trovate " + terrEntities.Count + " territorialità per lo zipcode " + zipcode);
            for (int i = 0; i < terrEntities.Count; i++)
            {
                terrEntity = (DynamicEntity)terrEntities[i];

                terrDealerId = ((Key)terrEntity.Properties["new_territorialitydealerid"]).Value;
                confParamId = ((Lookup)terrEntity.Properties["new_configurationparameterid"]).Value;
                confParamEntity = crm.RetrieveFirstDynamic(new string[] { "new_parametridiconfigurazioneid" }, new object[] { confParamId }, "new_parametridiconfigurazione");

                //Vado a recuperare le informazioni che mi servono dal configuration parameter indicato
                //Log.Debug("Cerco il parametro di configurazione");

                //Controllo se la relativa BU è attiva o se è stata disabilitata
                //Log.Debug("Controllo se la relativa BU è attiva o se è stata disabilitata");
                BUid = ((Lookup)confParamEntity.Properties["new_businessid"]).Value;
                //Log.Debug("BUid: " + BUid.ToString());
                buEntities = crm.RetrieveMultipleDynamic(new string[] { "businessunitid" }, new object[] { BUid }, "businessunit");
                //Log.Debug("buEntities.Count " + buEntities.Count.ToString());
                if (buEntities != null && buEntities.Count > 0)
                {
                    bu = (DynamicEntity)buEntities[0];
                    if (bu.Properties.Contains("isdisabled"))
                    {
                        //Log.Debug("Aggiornato isdisabled");
                        isdisabled = ((CrmBoolean)bu.Properties["isdisabled"]).Value;
                    }

                    //Log.Debug("BU disabilitata? " + isdisabled.ToString());
                    if (!isdisabled)
                    //if (!((CrmBoolean)bu.isdisabled).Value)
                    {
                        //Se non è disabilitata la BU, posso considerare il parametro di configurazione trovato

                        //Se la BU non è già presente nella lista, aggiungo il parametro di configurazione
                        if (!BUIds.Contains(BUid))
                        {
                            //Log.Debug("BU aggiunta");
                            BUIds.Add(BUid);
                            confParamEntities.Add(confParamEntity);

                        }
                        else
                        {
                            Log.Debug("BU già aggiunta, non la aggiungo");
                        }
                    }

                }
                //confParamEntities.Add(crm.RetrieveFirstDynamic(new string[] { "new_parametridiconfigurazioneid" }, new object[] { confParamId }, "new_parametridiconfigurazione"));
            }
            return confParamEntities;
        }

    }
}

