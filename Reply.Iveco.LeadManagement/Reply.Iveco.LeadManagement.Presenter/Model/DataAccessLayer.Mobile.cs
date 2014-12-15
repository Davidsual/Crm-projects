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
using Reply.Iveco.LeadManagement.Presenter.CrmSdk.Discovery;
using Microsoft.Crm.SdkTypeProxy;
using System.Net;
using System.Globalization;

namespace Reply.Iveco.LeadManagement.Presenter.Model
{
    public partial class DataAccessLayer : BaseDataAccessLayer, IDisposable
    {
        #region PUBLIC METHODS
        /// <summary>
        /// Ritorna la guid del crm dell'utente
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public SystemUser CheckUserNamePasswordIFD(string userName, string password)
        {
            ///Controllo se sul CRM esiste
            string domain = userName.Split('\\')[0];
            string user = userName.Split('\\')[1];
#if DEBUG
            string server = "http://to0crm03/";
#else
            string server = DataUtility.GetBaseServiceUrl().Replace("MSCRMServices", string.Empty);
#endif
            bool result = CheckUserOnCrm(this.CurrentOrgName, server, domain, user, password);
            if (!result)
                throw new Exception("User not found");

            return this.GetSystemUserByUserNameAndPassword(domain, user);
        }
        /// <summary>
        /// Crea un nuovo lead (subset) da mobile
        /// </summary>
        /// <param name="param"></param>
        /// <param name="idOperator"></param>
        public void SetLeadMobile(SetLeadParameter param, Guid idOperator)
        {
            DynamicEntity _lead = new DynamicEntity("contact");
            ///Popolo gli attributi dell'entità
            _lead.Properties.Add(new CrmBooleanProperty("new_leadslave", new CrmBoolean(false)));
            _lead.Properties.Add(new CrmDateTimeProperty("new_datahidleadcreation", new CrmDateTime(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:s}", DateTime.Now))));
            _lead.Properties.Add(new StringProperty("firstname", param.FirstName));
            _lead.Properties.Add(new StringProperty("lastname", param.LastName));
            _lead.Properties.Add(new LookupProperty("new_countryid", new Lookup("new_country", param.CountryId)));// callBackData.Nation));
            _lead.Properties.Add(new StringProperty("fullname", string.Format(CultureInfo.InvariantCulture, "{0} {1}", param.FirstName, param.LastName)));
            _lead.Properties.Add(new StringProperty("telephone1", param.TelephoneNumber));
            _lead.Properties.Add(new CrmBooleanProperty("new_flagprivacy", new CrmBoolean(true)));
            _lead.Properties.Add(new PicklistProperty("new_typecontact", new Picklist(param.TypeContactCode)));
            _lead.Properties.Add(new PicklistProperty("new_leadsubstatus", new Picklist((int)DataConstant.LeadSubStatus.ToBeProcessed)));
            _lead.Properties.Add(new LookupProperty("new_languageid", new Lookup("new_language", param.LanguageId)));
            ///inserisco gli stati
            _lead.Properties.Add(new PicklistProperty("new_leadstatus", new Picklist((int)DataConstant.LeadStatus.Open)));
            _lead.Properties.Add(new PicklistProperty("new_leadcategory", new Picklist(DataConstant.LEAD_LEAD_CATEGORY_TO_BE_PROCESSED)));
            ///Creo il LEAD
            try
            {
                Guid idLead = base.CurrentCrmService.Create(_lead);
                DataUtility.Assign("contact", idLead, idOperator, SecurityPrincipalType.User, base.CurrentCrmService);
            }
            catch (SoapException x)
            {
                //throw new Exception("CreateLead: " + x.Detail.InnerText);
                throw new Exception(x.Detail.InnerText);
            }
        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Ottiene systemuser dato username e password
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private SystemUser GetSystemUserByUserNameAndPassword(string domain,string userName)
        {
            return base.CurrentDataContext.SystemUsers.Where(c => c.DeletionStateCode == 0 && c.IsDisabled == false && c.DomainName.ToUpper() == string.Format(@"{0}\{1}", domain.ToUpper(), userName.ToUpper())).SingleOrDefault();
        }


        /// <summary>
        /// Authenticate the user using IFD (Internet Facing Deployment). The class
        /// constructor sets the values of the public variables (CrmService, etc).
        /// </summary>
        /// <param name="organization">Name of the user's organization.</param>
        /// <param name="server">Microsoft Dynamics CRM server URL.
        /// For example: https://myserver.</param>
        /// <param name="domain">Name of the domain hosting the user's system
        /// account.</param>
        /// <param name="username">User's system account name.</param>
        /// <param name="password">User's account password.</param>
        private static bool CheckUserOnCrm(string organization, string server, string domain,
                             string username, string password)
        {
            try
            {
                //Remove any trailing forward slash from the end of the server URL.
                server = server.TrimEnd(new char[] { '/' });

                // Initialize an instance of the CrmDiscoveryService Web service proxy.
                CrmDiscoveryService disco = new CrmDiscoveryService();
                disco.UseDefaultCredentials = true;
                disco.Credentials = new NetworkCredential(username, password, domain);
                disco.Url = server + "/MSCRMServices/2007/AD/CrmDiscoveryService.asmx";


                //Retrieve the ticket.
                RetrieveCrmTicketRequest ticketRequest =
                    new RetrieveCrmTicketRequest();
                ticketRequest.OrganizationName = organization;
                ticketRequest.UserId = domain + "\\" + username;
                ticketRequest.Password = password;
                RetrieveCrmTicketResponse ticketResponse =
                    (RetrieveCrmTicketResponse)disco.Execute(ticketRequest);

                return true;

            }
            catch (SoapException exs)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        } 
        #endregion
    }
}
