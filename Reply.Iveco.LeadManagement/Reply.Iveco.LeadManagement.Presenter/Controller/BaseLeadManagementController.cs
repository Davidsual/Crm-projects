using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Reply.Iveco.LeadManagement.Presenter.Model;

namespace Reply.Iveco.LeadManagement.Presenter
{
    public partial class BaseLeadManagementController : IDisposable
    {
        #region PRIVATE MEMBERS
        private DataAccessLayer _currentDataAccessLayer = null;
        private CrmDealerAccessLayer _currentCrmDealerAccessLayer = null;
        private object _currentLockObject = new object();
        private string _currentOrganization = string.Empty;
        private string _currentUser = string.Empty;
        private HttpContext _currentContext = null;
        #endregion

        #region PROTECTED PROPERTY
        /// <summary>
        /// Oggetto per l'accesso ai dati
        /// </summary>
        protected CrmDealerAccessLayer CurrentCrmDealerAccessLayer
        {
            get
            {
                lock (_currentLockObject)
                {
                    if (_currentCrmDealerAccessLayer == null)
                        _currentCrmDealerAccessLayer = new CrmDealerAccessLayer(this.CurrentOrganization, this.CurrentContext);
                    return _currentCrmDealerAccessLayer;
                }
            }
        }
        /// <summary>
        /// Oggetto per l'accesso ai dati
        /// </summary>
        protected DataAccessLayer CurrentDataAccessLayer
        {
            get 
            {
                lock (_currentLockObject)
                {
                    if (_currentDataAccessLayer == null)
                        _currentDataAccessLayer = new DataAccessLayer(this.CurrentOrganization, this.CurrentContext);
                    return _currentDataAccessLayer;
                }
            }
        }
        #endregion

        #region PRIVATE PROPERTY
        /// <summary>
        /// Organizaion name
        /// </summary>
        public string CurrentOrganization
        {
            get { return _currentOrganization; }
            set { _currentOrganization = value; }
        }

        public string CurrentUser
        {
            get { return _currentUser; }
            set { _currentUser = value; }
        }

        /// <summary>
        /// Contesto web corrente
        /// </summary>
        public HttpContext CurrentContext
        {
            get { return _currentContext; }
            set { _currentContext = value; }
        }
        #endregion

        #region CTOR
        /// <summary>
        /// Costruttore
        /// </summary>
        public BaseLeadManagementController(string organizationName,HttpContext context)
        {
            this.CurrentOrganization = organizationName;
            this.CurrentContext = context;
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Rilascio delle risorse
        /// </summary>
        public virtual void Dispose()
        {
            if (this._currentDataAccessLayer != null)
            {
                this._currentDataAccessLayer.Dispose();
                this._currentDataAccessLayer = null;
            }
            if (this._currentCrmDealerAccessLayer != null)
            {
                this._currentCrmDealerAccessLayer.Dispose();
                this._currentCrmDealerAccessLayer = null;
            }
        }
        #endregion
    }
}
