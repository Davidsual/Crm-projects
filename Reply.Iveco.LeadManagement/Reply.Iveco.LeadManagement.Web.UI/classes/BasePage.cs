using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Reply.Iveco.LeadManagement.Presenter;
using System.Threading;
using System.Globalization;
using System.Xml.Linq;

namespace Reply.Iveco.LeadManagement.Web.UI
{
    public class BasePage : System.Web.UI.Page
    {
        #region PRIVATE MEMBERS
        private const string LANGUAGE_ITALIAN = "it-IT";
        private const string DEFAULT_ORGANIZATION_NAME = "lmcert";
        private static object _currentDealerController = null;
        private static object _currentController = null;
        //private string _currentOrganizationName = string.Empty;
        #endregion

        #region PUBLIC MEMBER
        protected enum TypeEnviroment
        {
            LeadManagement = 0,
            CrmDealer = 1
        }
        #endregion

        #region PRIVATE PROPERTY
        /// <summary>
        /// Ottiene un'istanza del controller
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected Func<TypeEnviroment, string,T> GetCurrentController<T>()
        {
            return (TypeEnviroment enviroment, string orgname) => 
            {
                ///Assegno organization name
                this.CurrentOrganizationName = orgname;
                if(enviroment == TypeEnviroment.LeadManagement)
                {
                        if (_currentController == null)
                            _currentController = Activator.CreateInstance(typeof(T), orgname, HttpContext.Current) as LeadManagementController;
                        return (T)_currentController;
                }
                else
                {
                        if (_currentDealerController == null)
                            _currentDealerController = Activator.CreateInstance(typeof(T), orgname, HttpContext.Current) as DealerController;
                        return (T)_currentDealerController;
                }
            };
        }
        /// <summary>
        /// Current Organization name
        /// </summary>
        protected string CurrentOrganizationName
        {
            get 
            {
                //if(
                //if (string.IsNullOrEmpty(_currentOrganizationName))
                //    _currentOrganizationName = DEFAULT_ORGANIZATION_NAME;
                return ViewState["CurrentOrganizationName"] as string; 
            }
            set { ViewState["CurrentOrganizationName"] = value; }
        }
        /// <summary>
        /// Current Language
        /// </summary>
        protected string CurrentLanguage
        {
            get {
                if (string.IsNullOrEmpty(ViewState["CurrentLanguage"] as string))
                    ViewState["CurrentLanguage"] = LANGUAGE_ITALIAN;
                return ViewState["CurrentLanguage"] as string;
            }
            set { ViewState["CurrentLanguage"] = value; }
        }
        /// <summary>
        /// Autenticazione utente
        /// </summary>
        protected Auth Authentication
        {
            get { return ViewState["Authentication"] as Auth; }
            set { ViewState["Authentication"] = value; }
        }
        /// <summary>
        /// Controllo sessione
        /// </summary>
        protected bool CheckSession { get; set; }
        #endregion

        #region EVENTS
        /// <summary>
        /// Controllo della sessione
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            if (this.CheckSession && this.Authentication == null)
                Response.Redirect("SummaryPage.aspx");
            base.OnInit(e);
        }
        /// <summary>
        /// Impostazione della lingua
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(this.CurrentLanguage);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(this.CurrentLanguage); 
            base.OnPreRender(e);
        }
        #endregion

        #region PROTECTED METHODS
        /// <summary>
        /// Legge le label e le imposta
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="codeString"></param>
        /// <returns></returns>
        protected string GetValueByLanguageCodeAndCodeString(string languageCode, string codeValue)
        {
            XDocument doc = null;
            try
            {
                doc = XDocument.Load(Server.MapPath("Resources/LabelsResource.xml"));
                var node = doc.Elements("Labels").Nodes().Where(c => c is XElement && ((XElement)c).Name.LocalName.ToUpper(CultureInfo.InvariantCulture) == codeValue.ToUpper(CultureInfo.InvariantCulture)).SingleOrDefault();
                if (node == null)
                    return string.Empty;
                return ((XElement)node).Elements("Text").Where(c => c.Attribute("lang").Value == languageCode).Select(c => c.Value).SingleOrDefault();
            }
            catch
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// Serializa in JSON ed encoda per url
        /// </summary>
        /// <param name="objToSerialize"></param>
        /// <returns></returns>
        protected string SerializeEncode<T>(T objToSerialize) where T : class
        {

            System.Web.Script.Serialization.JavaScriptSerializer oSerializer =
                new System.Web.Script.Serialization.JavaScriptSerializer();
            return Server.UrlEncode(oSerializer.Serialize(objToSerialize));
        }
        /// <summary>
        /// Decoda da url a json e deserializza
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objToDeserialize"></param>
        /// <returns></returns>
        protected T DecodeDeserialize<T>(string objToDeserialize) where T : class
        {
            System.Web.Script.Serialization.JavaScriptSerializer oSerializer =
                    new System.Web.Script.Serialization.JavaScriptSerializer();
            return oSerializer.Deserialize<T>(Server.UrlDecode(objToDeserialize));
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Rilascio delle risorse
        /// </summary>
        public sealed override void Dispose()
        {
            if (_currentDealerController != null && _currentDealerController is IDisposable)
                ((IDisposable)_currentDealerController).Dispose();
            if (_currentController != null && _currentController is IDisposable)
                ((IDisposable)_currentController).Dispose();
            base.Dispose();           
        }

        #endregion
    }
}
