using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using System.Xml;
using Reply.Iveco.LeadManagement.Presenter;
using System.Web;
using System.Globalization;
using System.Configuration;

namespace Reply.Iveco.LeadManagement.LoadLeadService
{
    public partial class LoadLead : ServiceBase
    {
        #region PRIVATE MEMEBERS
        System.Timers.Timer _timer = null;
        #endregion

        #region PRIVATE PROPERTY
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
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("ServiceName")))
                throw new ArgumentException("Config parameters not founds");
            ///Imposto il service name
            this.ServiceName = ConfigurationManager.AppSettings.Get("ServiceName");
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
        /// Deserializza XML in una classe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T DeserializeXml<T>(string fileName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (XmlTextReader reader = new XmlTextReader(fileName))
            {
                return (T)ser.Deserialize(reader);
            }
        }
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
                ///Recupero le varie organization NAME
                var ret = DeserializeXml<Configurator>(string.Format(CultureInfo.InvariantCulture,"{0}\\{1}", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Configurator.xml"));                
                ///Per ogni organization name lancio il metodo
                ///Lancio i lcaricamento
                foreach (var orgname in ret.OrganizationName)
                {
                    using (DealerController currentControl = new DealerController(orgname, HttpContext.Current))
                        currentControl.ProcessUploadedLead();
                }
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
