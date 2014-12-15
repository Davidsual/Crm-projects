using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Reply.Seat.InvioSmsCrm.DataAccess;

namespace Reply.Seat.InvioSmsCrm.WindowsService
{
    /// <summary>
    /// Servizio che gestisce l'invio degli sms
    /// </summary>
    public partial class InvioSmsCrm : ServiceBase
    {
        #region PRIVATE MEMBERS
        private DataAccessLayer _currentAccessLayer = null;
        private CrmAdapter.CrmAdapter _currentCrmAdapter = null;
        #endregion

        #region PRIVATE PROPERTY
        /// <summary>
        /// Accesso ai dati
        /// </summary>
        private DataAccessLayer CurrentAccessLayer 
        { 
            get 
            { 
                if(_currentAccessLayer == null)
                    _currentAccessLayer = DataAccessLayer.Instance;
                return _currentAccessLayer;
            } 
        }
        /// <summary>
        /// Accesso al Crm in Set
        /// </summary>
        protected CrmAdapter.CrmAdapter CurrentCrmAdapter
        {
            get
            {
                if (_currentCrmAdapter == null)
                    _currentCrmAdapter = new CrmAdapter.CrmAdapter();
                return _currentCrmAdapter;
            }
        }
        #endregion

        #region CTOR
        /// <summary>
        /// Costruttore
        /// </summary>
        public InvioSmsCrm()
        {
            InitializeComponent();
        } 
        #endregion

        #region RUN METHODS
        /// <summary>
        /// Inizializzazzione del servizio
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            try
            {

            }
            catch
            {
                ///MAI IN ECCEZZIONE
                ///politica di retry
            }
        }
        /// <summary>
        /// Stop del servizio rilascio eventuali risorse
        /// </summary>
        protected override void OnStop()
        {
            if (_currentAccessLayer != null)
                _currentAccessLayer.Dispose();
            if (_currentCrmAdapter != null)
                _currentAccessLayer.Dispose();
        } 
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Invio Sms
        /// </summary>
        private void SendSms()
        {

        }
        /// <summary>
        /// Recupero dei parametri di startup
        /// </summary>
        private void ReadStartUpParameters()
        {

        }
        #endregion
    }
}
