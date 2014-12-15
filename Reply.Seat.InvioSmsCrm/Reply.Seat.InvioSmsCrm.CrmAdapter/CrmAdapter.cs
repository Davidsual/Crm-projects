using System;
using System.Net;
//using Reply.Seat.DinamichePromozionali.CrmAdapter.CrmServiceWsdl;
using Reply.Seat.DinamichePromozionali.CrmAdapter.CrmServicePerformance;
using System.Configuration;
[assembly: CLSCompliant(true)]
namespace Reply.Seat.InvioSmsCrm.CrmAdapter
{
    public sealed class CrmAdapter : IDisposable
    {
        #region PRIVATE MEMBERS
        #endregion

        #region PRIVATE MEMBERS
        //private CrmService _crmService;
        #endregion

        #region PRIVATE PROPERTY
        #endregion

        #region PUBLIC MEMBERS
        //public static Settings AppConfig { get { return Settings.Default; } }
        /// <summary>
        /// Restituisce un istanza del crm Service
        /// </summary>
        //public CrmService CurrentCrmService
        //{
        //    get
        //    {
        //        //if (this.CurrentContextWeb.Application["CRMSERVICE"] != null)
        //        //    //return this.CurrentContextWeb.Application["CRMSERVICE"] as CrmService;
        //        //    return this.CurrentContextWeb.Application["CRMSERVICE"] as CrmService;

        //        //if (_crmService == null)
        //        //{
        //        //    _crmService = new CrmService(ConfigurationManager.AppSettings["CrmUrl"].Trim(new char[] { '/' }) + "/MSCRMServices/2007/CrmService.asmx");
        //        //    //_crmService.Url = ConfigurationManager.AppSettings["CrmUrl"].Trim(new char[] { '/' }) + "/MSCRMServices/2007/CrmService.asmx";
        //        //    _crmService.Credentials = GetCrmCredentials();
        //        //    _crmService.CrmAuthenticationTokenValue = new CrmAuthenticationToken() { OrganizationName = ConfigurationManager.AppSettings["CrmOrganization"] };
        //        //    // Turn on unsafe connection sharing
        //        //    _crmService.UnsafeAuthenticatedConnectionSharing = true;

        //        //    if (Convert.ToBoolean(ConfigurationManager.AppSettings["UseCrmProxy"]))
        //        //    {
        //        //        WebProxy proxy = new WebProxy(ConfigurationManager.AppSettings["ProxyUrl"]);
        //        //        proxy.Credentials = GetProxyCredentials();
        //        //        _crmService.Proxy = proxy;
        //        //    }
        //        //    //this.CurrentContextWeb.Application["CRMSERVICE"] = _crmService;
        //        //}
                
        //        return _crmService;
        //    }
        //}
        #endregion

        #region CTOR
        /// <summary>
        /// Current Context
        /// </summary>
        /// <param name="context"></param>
        public CrmAdapter()
        {
        }
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Aggiorna un sms con lo stato
        /// </summary>
        /// <param name="lead"></param>
        public void UpdateSMS(new_sms sms)
        {
            UpdateEntity(sms);
        }
        #endregion
        
        #region PRIVATE METHODS
        /// <summary>
        /// Restituisce le credenziali del Crm
        /// </summary>
        /// <returns></returns>
        private System.Net.ICredentials GetCrmCredentials()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CrmUserName"]))
                return new NetworkCredential(ConfigurationManager.AppSettings["CrmUserName"], ConfigurationManager.AppSettings["CrmUserPassword"], ConfigurationManager.AppSettings["CrmUserDomain"]);
            else
                return CredentialCache.DefaultNetworkCredentials;
        }
        /// <summary>
        /// Restituisce le credenziali del proxy
        /// </summary>
        /// <returns></returns>
        private System.Net.ICredentials GetProxyCredentials()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ProxyUserName"]))
                return new NetworkCredential(ConfigurationManager.AppSettings["ProxyUserName"], ConfigurationManager.AppSettings["ProxyUserPassword"], ConfigurationManager.AppSettings["ProxyUserDomain"]);
            else
                return CredentialCache.DefaultNetworkCredentials;
        }
        /// <summary>
        /// Aggiorna un'entity
        /// </summary>
        /// <param name="entity"></param>
        private void UpdateEntity(BusinessEntity entity)
        {
            //try
            //{
            //    CurrentCrmService.Update(entity);
            //}
            //catch (SoapException e)
            //{
            //    throw new Exception(e.Detail.InnerText, e);
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Rilascio Risorse
        /// </summary>
        public void Dispose()
        {

        }

        #endregion
    }
}
