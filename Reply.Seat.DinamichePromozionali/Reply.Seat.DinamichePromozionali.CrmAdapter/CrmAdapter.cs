using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Reply.Seat.DinamichePromozionali.CrmAdapter.CrmServiceWsdl;
using Reply.Seat.DinamichePromozionali.CrmAdapter.CrmServicePerformance;
using Reply.Seat.DinamichePromozionali.CrmAdapter.Properties;
using System.Net;
using System.Web.Services.Protocols;
using System.Configuration;
using System.Web;
[assembly: CLSCompliant(true)]
namespace Reply.Seat.DinamichePromozionali.CrmAdapter
{
    public sealed class CrmAdapter : IDisposable
    {
        #region PRIVATE MEMBERS
        private HttpContext _currentContext = null;
        #endregion

        #region PRIVATE PROPERTY
        /// <summary>
        /// Contesto corrente
        /// </summary>
        private HttpContext CurrentContextWeb
        {
            get { return _currentContext; }
            set { _currentContext = value; }
        } 
        #endregion

        #region PUBLIC MEMBERS
        //public static Settings AppConfig { get { return Settings.Default; } }
        /// <summary>
        /// Restituisce un istanza del crm Service
        /// </summary>
        public CrmService CRMService
        {
            get
            {
                if (this.CurrentContextWeb.Application["CRMSERVICE"] != null)
                    //return this.CurrentContextWeb.Application["CRMSERVICE"] as CrmService;
                    return this.CurrentContextWeb.Application["CRMSERVICE"] as CrmService;

                if (_crmService == null)
                {
                    _crmService = new CrmService(ConfigurationManager.AppSettings["CrmUrl"].Trim(new char[] { '/' }) + "/MSCRMServices/2007/CrmService.asmx");
                    //_crmService.Url = ConfigurationManager.AppSettings["CrmUrl"].Trim(new char[] { '/' }) + "/MSCRMServices/2007/CrmService.asmx";
                    _crmService.Credentials = GetCrmCredentials();
                    _crmService.CrmAuthenticationTokenValue = new CrmAuthenticationToken() { OrganizationName = ConfigurationManager.AppSettings["CrmOrganization"] };
                    // Turn on unsafe connection sharing
                    _crmService.UnsafeAuthenticatedConnectionSharing = true;

                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["UseCrmProxy"]))
                    {
                        WebProxy proxy = new WebProxy(ConfigurationManager.AppSettings["ProxyUrl"]);
                        proxy.Credentials = GetProxyCredentials();
                        _crmService.Proxy = proxy;
                    }
                    this.CurrentContextWeb.Application["CRMSERVICE"] = _crmService;
                }
                
                return _crmService;
            }
        }
        #endregion

        #region PRIVATE MEMBERS
        private CrmService _crmService;
        #endregion

        #region CTOR
        /// <summary>
        /// Current Context
        /// </summary>
        /// <param name="context"></param>
        public CrmAdapter(HttpContext context)
        {
            this.CurrentContextWeb = context;
        }
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Crea un Lead
        /// </summary>
        /// <param name="lead"></param>
        /// <returns></returns>
        public Guid CreateLead(lead lead, Owner owner)
        {
            return CreateNewEntity(lead,owner);
        }
        /// <summary>
        /// Crea un chiamante campagna
        /// </summary>
        /// <param name="chiamanteCampagna"></param>
        /// <returns></returns>
        public Guid CreateChiamanteCampagna(new_chiamantecampagna chiamanteCampagna, Owner owner)
        {
            return CreateNewEntity(chiamanteCampagna, owner);
        }
        /// <summary>
        /// Crea un SMS
        /// </summary>
        /// <param name="sms"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public Guid CreaSMS(new_sms sms,Owner owner)
        {
            return CreateNewEntity(sms, owner);
        }
        /// <summary>
        /// Crea una chiamata gratuita
        /// </summary>
        /// <param name="chiamataGratuita"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public Guid CreaChiamataGratuita(new_callgratuita chiamataGratuita, Owner owner)
        {
            return CreateNewEntity(chiamataGratuita, owner);
        }
        /// <summary>
        /// Crea una call
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        public Guid CreateCall(new_call call, Owner owner)
        {
            return CreateNewEntity(call, owner);
        }
        /// <summary>
        /// Aggiorna chiamante Campagna
        /// </summary>
        /// <param name="chiamanteCampagna"></param>
        public void UpdateChiamanteCampagna(new_chiamantecampagna chiamanteCampagna)
        {
            UpdateEntity(chiamanteCampagna);
        }
        /// <summary>
        /// Aggiorna Partecipante
        /// </summary>
        /// <param name="lead"></param>
        public void UpdateLead(lead lead)
        {
            UpdateEntity(lead);
        }
        /// <summary>
        /// Aggiorna Codice Promozionale
        /// </summary>
        /// <param name="codicePromozionale"></param>
        public void UpdateCodicePromozionale(new_codicepromozionale codicePromozionale)
        {
            UpdateEntity(codicePromozionale);
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
        /// Crea una nuova entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private Guid CreateNewEntity(BusinessEntity entity,Owner owner)
        {
            try
            {
                Guid retGuid = Guid.Empty;
                retGuid = CRMService.Create(entity);
                return retGuid;
            }
            catch (SoapException e)
            {
                throw new Exception(e.Detail.InnerText, e);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Aggiorna un'entity
        /// </summary>
        /// <param name="entity"></param>
        private void UpdateEntity(BusinessEntity entity)
        {
            try
            {
                CRMService.Update(entity);
            }
            catch (SoapException e)
            {
                throw new Exception(e.Detail.InnerText, e);
            }
            catch (Exception e)
            {
                throw e;
            }
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
