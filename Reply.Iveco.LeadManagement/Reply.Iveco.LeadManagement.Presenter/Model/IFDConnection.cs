using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web.Services.Protocols;
using Reply.Iveco.LeadManagement.Presenter.CrmSdk;
using Reply.Iveco.LeadManagement.Presenter.CrmSdk.Discovery;
using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Crm.Sdk;


namespace Reply.Iveco.LeadManagement.Presenter.Model
{
    public class IFDConnection
    {
        #region PUBLIC MEMBERS
        /// <summary>
        /// A CrmService reference.
        /// </summary>
        public readonly CrmService CrmService = null;

        // URL of the Web application.
        public readonly string WebApplicationUrl = String.Empty;

        // GUID of the user's organization.
        public readonly Guid OrganizationId = Guid.Empty; 
        #endregion

        #region CTOR
        /// <summary>
        /// Costruttore
        /// </summary>
        public IFDConnection()
        {

        }
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="organization"></param>
        /// <param name="server"></param>
        /// <param name="domain"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public IFDConnection(string organization, string server, string domain,
                         string username, string password)
        {
            try
            {
                //Remove any trailing forward slash from the end of the server URL.
                server = server.TrimEnd(new char[] { '/' });

                // Initialize an instance of the CrmDiscoveryService Web service proxy.
                CrmDiscoveryService disco = new CrmDiscoveryService();
                disco.Url = server + "/MSCRMServices/2007/SPLA/CrmDiscoveryService.asmx";

                //Retrieve a list of available organizations.
                RetrieveOrganizationsRequest orgRequest =
                    new RetrieveOrganizationsRequest();
                orgRequest.UserId = domain + "\\" + username;
                orgRequest.Password = password;
                RetrieveOrganizationsResponse orgResponse =
                    (RetrieveOrganizationsResponse)disco.Execute(orgRequest);

                //Find the desired organization.
                foreach (OrganizationDetail orgdetail in orgResponse.OrganizationDetails)
                {
                    if (orgdetail.OrganizationName.ToUpperInvariant() == organization.ToUpperInvariant())
                    {
                        //Retrieve the ticket.
                        RetrieveCrmTicketRequest ticketRequest =
                            new RetrieveCrmTicketRequest();
                        ticketRequest.OrganizationName = organization;
                        ticketRequest.UserId = domain + "\\" + username;
                        ticketRequest.Password = password;
                        RetrieveCrmTicketResponse ticketResponse =
                            (RetrieveCrmTicketResponse)disco.Execute(ticketRequest);

                        //Create the CrmService Web service proxy.
                        CrmAuthenticationToken sdktoken = new CrmAuthenticationToken();
                        sdktoken.AuthenticationType = 2;
                        sdktoken.OrganizationName = organization;
                        sdktoken.CrmTicket = ticketResponse.CrmTicket;

                        CrmService = new CrmService();
                        CrmService.CrmAuthenticationTokenValue = sdktoken;
                        CrmService.Url = orgdetail.CrmServiceUrl;

                        WebApplicationUrl = orgdetail.WebApplicationUrl;
                        OrganizationId = orgdetail.OrganizationId;

                        break;
                    }
                }
            }
            catch (Exception)
            { }
        } 
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Ottiene un istanza del crm service
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="organization"></param>
        /// <param name="server"></param>
        /// <param name="domain"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public CrmService GetServiceIFDconnection(Guid userid, string organization, string server, string domain,
                         string username, string password)
        {
            try
            {
                CrmService oCrmService = new CrmService(); ;

                //Remove any trailing forward slash from the end of the server URL.
                server = server.TrimEnd(new char[] { '/' });

                // Initialize an instance of the CrmDiscoveryService Web service proxy.
                CrmDiscoveryService disco = new CrmDiscoveryService();
                disco.Url = server + "/MSCRMServices/2007/SPLA/CrmDiscoveryService.asmx";

                //Retrieve a list of available organizations.
                RetrieveOrganizationsRequest orgRequest =
                    new RetrieveOrganizationsRequest();
                orgRequest.UserId = domain + "\\" + username;
                orgRequest.Password = password;
                RetrieveOrganizationsResponse orgResponse =
                    (RetrieveOrganizationsResponse)disco.Execute(orgRequest);

                //Find the desired organization.
                foreach (OrganizationDetail orgdetail in orgResponse.OrganizationDetails)
                {
                    if (orgdetail.OrganizationName.ToUpperInvariant() == organization.ToUpperInvariant())
                    {
                        //Retrieve the ticket.
                        RetrieveCrmTicketRequest ticketRequest =
                            new RetrieveCrmTicketRequest();
                        ticketRequest.OrganizationName = organization;
                        ticketRequest.UserId = domain + "\\" + username;
                        ticketRequest.Password = password;
                        RetrieveCrmTicketResponse ticketResponse =
                            (RetrieveCrmTicketResponse)disco.Execute(ticketRequest);

                        //Create the CrmService Web service proxy.
                        CrmAuthenticationToken sdktoken = new CrmAuthenticationToken();
                        sdktoken.AuthenticationType = 2;
                        sdktoken.OrganizationName = organization;
                        sdktoken.CrmTicket = ticketResponse.CrmTicket;
                        sdktoken.CallerId = userid;

                        //oCrmService = new CrmService();
                        oCrmService.CrmAuthenticationTokenValue = sdktoken;
                        oCrmService.Url = orgdetail.CrmServiceUrl;



                        break;
                    }
                }
                return oCrmService;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// Ottiene un istanza del crm service
        /// </summary>
        /// <returns></returns>
        public CrmService GetCrmService()
        {
            return this.CrmService;
        } 
        #endregion

    }
}
