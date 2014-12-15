using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Reply.Iveco.LeadManagement.Presenter;

namespace Reply.Iveco.LeadManagement.Web.UI
{
    public partial class BasePageUserControl : UserControl
    {
        #region PRIVATE MEMBERS
        private const string LANGUAGE_ITALIAN = "it-IT";
        private string _languageParameter = string.Empty;
        private string _countryParameter = string.Empty;
        //private string _currentOrganizationName = string.Empty;
        private LeadManagementController _currentController = null;
        #endregion

        #region PROTECTED PROPERTY
        /// <summary>
        /// Current Organization name
        /// </summary>
        protected string CurrentOrganizationName
        {
            get
            {
                return ViewState["CurrentOrganizationName"] as string;
            }
            set { ViewState["CurrentOrganizationName"] = value; }
        }
        /// <summary>
        /// Current Controller
        /// </summary>
        protected LeadManagementController CurrentController
        {
            get 
            {
                if (_currentController == null)
                    _currentController = new LeadManagementController(this.CurrentOrganizationName, HttpContext.Current);
                return _currentController; 
            }
        }
        /// <summary>
        /// Country as Parameter
        /// </summary>
        protected string CountryParameter
        {
            get { return _countryParameter; }
            set { _countryParameter = value; }
        }
        /// <summary>
        /// Language as parameter
        /// </summary>
        protected string LanguageParameter
        {
            get { return _languageParameter; }
            set { _languageParameter = value; }
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
        }
        /// <summary>
        /// Autenticazione utente
        /// </summary>
        protected Auth Authentication
        {
            get { return ViewState["Authentication"] as Auth; }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Rilascio delle risorse
        /// </summary>
        public sealed override void Dispose()
        {
            if (_currentController != null)
                _currentController.Dispose();
            base.Dispose();
        }
        #endregion
    }
}
