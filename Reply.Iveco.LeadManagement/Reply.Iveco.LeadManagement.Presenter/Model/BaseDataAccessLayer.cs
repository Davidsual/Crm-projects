using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Microsoft.Win32;
using System.Data;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy;


namespace Reply.Iveco.LeadManagement.Presenter.Model
{
    public abstract class BaseDataAccessLayer : IDisposable
    {
        #region PRIVATE MEMBERS
        private SqlConnection _sqlConnection = null;
        private HttpContext _context = null;
        private LeadManagementModelDataContext _model = null;
        private CrmDealerModelDataContext _crmDealerModel = null;
        private CrmService _crmService = null;
        private CrmService _crmServiceMail = null;
        private object _currentLockObject = new object();
         
        private string _orgName = string.Empty;
        private string _currentSqlConnectionString = string.Empty;
        #endregion

        #region PROTECTED PROPERTY
        /// <summary>
        /// Servizio per collegarsi al crm
        /// </summary>
        protected CrmService CurrentCrmServiceMail
        {
            get
            {
                ///Apertura CrmService
                lock (_currentLockObject)
                {
                    if (_crmServiceMail == null)
                        _crmServiceMail = DataUtility.LoadCrmServiceMail(this.CurrentOrgName, this.CurrentContext);
                    return _crmServiceMail;
                }
            }
        }
        /// <summary>
        /// Servizio per collegarsi al crm
        /// </summary>
        protected CrmService CurrentCrmService
        {
            get 
            {
                ///Apertura CrmService
                lock (_currentLockObject)
                {
                    if (_crmService == null)
                        _crmService = DataUtility.LoadCrmService(this.CurrentOrgName,this.CurrentContext);
                    return _crmService;
                }
            }
        }
        /// <summary>
        /// Accesso ai dati
        /// </summary>
        protected LeadManagementModelDataContext CurrentDataContext
        {
            get 
            {
                ///Apertura del datacontext
                lock (_currentLockObject)
                {
                    if (_model == null)
                    {
                        //throw new Exception("TEST Davide : " + this.CurrentSqlConnectionString);
                        _model = new LeadManagementModelDataContext(this.CurrentSqlConnectionString);
                        //_model = new LeadManagementModelDataContext("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=lmcert_MSCRM;Data Source=IVCLOUD01SSIC06");
                    }
                    return _model;
                }
            }
        }
        /// <summary>
        /// Accesso ai dati CrmDealerModelDataContext
        /// </summary>
        protected CrmDealerModelDataContext CurrentCrmDealerDataContext
        {
            get
            {
                ///Apertura del datacontext
                lock (_currentLockObject)
                {
                    if (_crmDealerModel == null)
                    {
                        //throw new Exception("TEST Davide : " + this.CurrentSqlConnectionString);
                        _crmDealerModel = new CrmDealerModelDataContext(this.CurrentSqlConnectionStringDealer);
                    }
                    return _crmDealerModel;
                }
            }
        }
        /// <summary>
        /// Restituisce la connection string
        /// </summary>
        protected string CurrentSqlConnectionStringDealer
        {
            get
            {
                lock (_currentLockObject)
                {
#if DEBUG
                    _currentSqlConnectionString = DataConstant.SQL_CONN_STRING_TEST_DEALER;
#else
                        if (string.IsNullOrEmpty(_currentSqlConnectionString))
                            _currentSqlConnectionString = DataUtility.GetSqlConnection(this.CurrentOrgName);
#endif
                    return _currentSqlConnectionString;
                }
            }
        }
        /// <summary>
        /// Restituisce la connection string
        /// </summary>
        protected string CurrentSqlConnectionString
        {
            get 
            {
                lock (_currentLockObject)
                {
                    #if DEBUG
                        //_currentSqlConnectionString = "Password=80.David;Persist Security Info=True;User ID=d.trotta;Initial Catalog=IvecoLeadManagement_MSCRM;Data Source=TO0CRM03";
                        _currentSqlConnectionString = DataConstant.SQL_CONN_STRING_TEST_LM ;
                    #else
                        if (string.IsNullOrEmpty(_currentSqlConnectionString))
                            _currentSqlConnectionString = DataUtility.GetSqlConnection(this.CurrentOrgName);
                    #endif
                    return _currentSqlConnectionString;
                }
            }
        }
        /// <summary>
        /// Current Organization Name
        /// </summary>
        protected string CurrentOrgName
        {
            get { return _orgName; }
            set { _orgName = value; }
        }
        /// <summary>
        /// Contesto corrente
        /// </summary>
        private HttpContext CurrentContext
        {
            get { return _context; }
            set { _context = value; }
        }
        /// <summary>
        /// Current SQL Connection
        /// </summary>
        protected SqlConnection CurrentSqlConnection
        {
            get
            {
                lock (_currentLockObject)
                {
                    if (_sqlConnection == null)
                    {
#if DEBUG
                        _sqlConnection = new SqlConnection(DataConstant.SQL_CONN_STRING_TEST_LM);
                        //_sqlConnection = new SqlConnection("Data Source=192.168.88.16;Initial Catalog=IvecoDSDealerGMBH_MSCRM;Integrated Security=SSPI");
                        //_sqlConnectionApplication = new SqlConnection("Data Source=192.168.88.16;Initial Catalog=Training_Deutschland_MSCRM;Integrated Security=SSPI");
                        _sqlConnection.Open();
                    
#else
                        if (this.CurrentContext == null)
                        {
                            //using (new CrmImpersonator())
                            //{
                            _sqlConnection = new SqlConnection();// (Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\MSCRM").GetValue("configdb").ToString().Replace("MSCRM_CONFIG", this.CurrentOrgName + "_MSCRM"));
                            _sqlConnection.SetCrmConnectionString(this.CurrentOrgName);
                            _sqlConnection.Open();
                        }
                        else
                        {
                            using (new CrmImpersonator())
                            {
                                _sqlConnection = new SqlConnection();// (Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\MSCRM").GetValue("configdb").ToString().Replace("MSCRM_CONFIG", this.CurrentOrgName + "_MSCRM"));
                                _sqlConnection.SetCrmConnectionString(this.CurrentOrgName);
                                _sqlConnection.Open();
                            }
                        }
#endif
                    //this.SqlTransaction = _sqlConnection.BeginTransaction();
                    //_sqlConnection = new SqlConnection("Password=sa;Persist Security Info=True;User ID=dtrotta;Initial Catalog=IvecoSvilGmbh_MSCRM;Data Source=192.168.90.8");
                    //_sqlConnection.Open();
                    }
                }
                return _sqlConnection;
            }
        }
        #endregion

        #region CTOR
        /// <summary>
        /// Costruttore obbligatorio Organization name
        /// </summary>
        /// <param name="orgnanizationName"></param>
        public BaseDataAccessLayer(string orgnanizationName, HttpContext context)
        {
            ///Assegno organizationName e Context
            this.CurrentOrgName = orgnanizationName;
            this.CurrentContext = context;
        }
        #endregion

        #region PROTECTED METHODS

        #endregion

        #region IDisposable Members
        /// <summary>
        /// Rilascia eventuali risorse
        /// </summary>
        public virtual void Dispose()
        {
            ///Ripulisco eventuale connessione al db
            if (this._sqlConnection != null)
            {
                this.CurrentSqlConnection.Dispose();
                this._sqlConnection = null;
            }
            ///Ripulisco eventuali datacontex aperti
            if (this._model != null)
            {
                this._model.Dispose();
                this._model = null;
            }
            ///Ripulisco eventuali crm service aperti
            if (this._crmService != null)
            {
                this._crmService.Dispose();
                this._crmService = null;
            }
            if (this._crmServiceMail != null)
            {
                this._crmServiceMail.Dispose();
                this._crmServiceMail = null;
            }
            ///Ripulisco data context del crm dealer
            if (this._crmDealerModel != null)
            {
                _crmDealerModel.Dispose();
                _crmDealerModel = null;
            }
        }

        #endregion
    }
}
