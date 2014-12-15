using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

using System.Web;
using Reply.Iveco.LeadManagement.Presenter;
using System.Configuration;

namespace Reply.Iveco.LeadManagement.LoadContactService
{
    /// <summary>
    /// Caricamento massivo dei contact
    /// </summary>
    public partial class LoadLead : ServiceBase
    {
        #region PRIVATE MEMEBERS
        private string _currentOrganizationName = string.Empty;
        System.Timers.Timer _timer = null;
        //private const string ORGANIZATION_NAME = "IvecoLeadManagement";
        #endregion

        #region PRIVATE PROPERTY
        /// <summary>
        /// Nome dell'organization name corrente
        /// </summary>
        private string CurrentOrganizationName
        {
            get 
            {
                if (string.IsNullOrEmpty(_currentOrganizationName))
                    throw new ArgumentException("Organization Name not found");
                return _currentOrganizationName; 
            }
            set { _currentOrganizationName = value; }
        }
        /// <summary>
        /// Timer windows Service
        /// </summary>
        private System.Timers.Timer CurrentTimer
        {
            get { return _timer; }
            set { _timer = value; }
        }
        #endregion

        #region CTOR
        /// <summary>
        /// Costruttore
        /// </summary>
        public LoadLead()
        {
            InitializeComponent();
            ///Controllo di avere il file di configurazione e di fare la retrieve corretta dei parametri
            if(string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("ServiceName")) ||
                string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("OrganizationName")))
                throw new ArgumentException("Config parameters not founds");
            ///Imposto il service name
            this.ServiceName = ConfigurationManager.AppSettings.Get("ServiceName");
            this.CurrentOrganizationName = ConfigurationManager.AppSettings.Get("OrganizationName"); 
        } 
        #endregion

        #region EVENT
        /// <summary>
        /// Inizio del servizio
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            ///Start del serivizio
            Thread.Sleep(3000);
            try
            {
                ///istanzia il timer
                this.CurrentTimer = new System.Timers.Timer(60000);
                this.CurrentTimer.Enabled = true;
                this.CurrentTimer.Elapsed += new System.Timers.ElapsedEventHandler(CurrentTimer_Elapsed);
                this.CurrentTimer.Start();
            }
            catch
            {

                ///Windows Service non và in errore
            }
        }
        /// <summary>
        /// Fine del servizio
        /// </summary>
        protected override void OnStop()
        {
            this.CurrentTimer.Stop();
            this.CurrentTimer.Dispose();
            this.CurrentTimer = null;
        } 
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Tick dell'orologgio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ///Fermo il timer
            this.CurrentTimer.Stop();
            try
            {
                ///Lancio i lcaricamento
                using (LeadManagementController currentControl = new LeadManagementController(this.CurrentOrganizationName, HttpContext.Current))
                    currentControl.ProcessUploadedLead();
            }
            catch 
            {
            }
            ///Faccio ripartire il timer
            this.CurrentTimer.Start();
        }
        #endregion
    }
}
