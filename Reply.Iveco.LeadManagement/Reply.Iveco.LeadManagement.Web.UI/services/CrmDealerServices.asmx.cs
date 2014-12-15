using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Reply.Iveco.LeadManagement.Presenter;
using Reply.Iveco.LeadManagement.Presenter.Model;
using System.Web.Services.Protocols;
using System.Globalization;
using Microsoft.Crm.Sdk;

namespace Reply.Iveco.LeadManagement.Web.UI
{
    /// <summary>
    /// Summary description for CrmDealerServices
    /// </summary>
    [WebService(Namespace = "_http://schemas.microsoft.com/crm/2006/WebServices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class CrmDealerServices : System.Web.Services.WebService
    {

        #region PRIVATE MEMBERS
        private const string USERNAME_IVECO1 = "hoplo1";
        private const string USERNAME_IVECO2 = "ivecocom1";
        private const string PASSWORD_IVECO = "v2su6kbp";
        #endregion

        #region PUBLIC MEMBERS
        public HeaderAuthentication headerAuthentication;
        #endregion


        [WebMethod]
        //[SoapHeader("headerAuthentication", Direction = SoapHeaderDirection.In)] 
        public FindDealerResult FindDealer(ContactLead lead, String orgName)
        {
            //Log.Debug("Chiamata SetLead effettuata.");
            FindDealerResult result = new FindDealerResult()
            {
                IsSuccessful = true
            };
            try
            {
                ///Controllo autorizzazioni
                //if (!this.CheckHeaderAuthorization(headerAuthentication))
                //  throw new WsNotAutorizedUserException();
                //using (new CrmImpersonator())
                //{
                    using (DealerController controller = new DealerController(orgName, HttpContext.Current))
                        result.dealer = controller.FindDealer(lead);
                    return result;
                //}
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.ErrorDescription = ex.Message + " - " + ex.StackTrace;
                result.ErrorCode = "000";
                return result;
            }
        }


        [WebMethod]
        //[SoapHeader("headerAuthentication", Direction = SoapHeaderDirection.In)] 
        public SetLeadResult SetLead(ContactLead lead, String orgName)
        {
            //Log.Debug("Chiamata SetLead effettuata.");
            SetLeadResult result = new SetLeadResult()
            {
                IsSuccessful = true
            };
            try
            {
                ///Controllo autorizzazioni
                //if (!this.CheckHeaderAuthorization(headerAuthentication))
                  //  throw new WsNotAutorizedUserException();

                //using (new CrmImpersonator())
                //{
                    using (DealerController controller = new DealerController(orgName, HttpContext.Current))
                        controller.SetLead(lead, Guid.Empty);
                    return result;
                //}
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.ErrorDescription = ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine;
                if (ex.InnerException != null)
                {
                    result.ErrorDescription += ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace; 
                }
                result.ErrorCode = "000";
                return result;
            }
        }

        ///// <summary>
        ///// Controllo dell'header authentication
        ///// </summary>
        ///// <param name="headerAuthentication"></param>
        //private static bool CheckHeaderAuthorization(HeaderAuthentication headerAuthentication)
        //{
        //    ///General object test
        //    if (headerAuthentication == null || string.IsNullOrEmpty(headerAuthentication.Username) || string.IsNullOrEmpty(headerAuthentication.Password))
        //        return false;
        //    ///Check username
        //    if (headerAuthentication.Username.ToLowerInvariant() != USERNAME_IVECO1 &&
        //        headerAuthentication.Username.ToLowerInvariant() != USERNAME_IVECO2)
        //        return false;
        //    ///check password
        //    if (headerAuthentication.Password.ToLowerInvariant() != PASSWORD_IVECO)
        //        return false;

        //    return true;
        //}
    }
}
