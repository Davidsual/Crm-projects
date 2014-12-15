using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Reflection;
using System.Configuration;


namespace Reply.Iveco.LeadManagement.LoadContactService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        #region CTOR
        /// <summary>
        /// Costruttore
        /// </summary>
        public ProjectInstaller()
        {
            InitializeComponent();
        } 
        #endregion

        #region INSTALL / UNINSTALL
        /// <summary>
        /// Disinstalla il servizio
        /// </summary>
        /// <param name="savedState"></param>
        public override void Uninstall(IDictionary savedState)
        {
            this.LoadContactService.ServiceName = this.GetConfigurationValue("ServiceName");
            this.LoadContactService.DisplayName = this.GetConfigurationValue("ServiceName");
            base.Uninstall(savedState);
        }
        /// <summary>
        /// Installa il servizio
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            this.LoadContactService.ServiceName = this.GetConfigurationValue("ServiceName");
            this.LoadContactService.DisplayName = this.GetConfigurationValue("ServiceName");
            base.Install(stateSaver);
        } 
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Valore Service Name dal file app.config
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetConfigurationValue(string key)
        {
            Assembly service = Assembly.GetAssembly(typeof(LoadLead));
            Configuration config = ConfigurationManager.OpenExeConfiguration(service.Location);
            if (config.AppSettings.Settings[key] != null)
            {
                return config.AppSettings.Settings[key].Value;
            }
            else
            {
                throw new IndexOutOfRangeException("Settings collection does not contain the requested key:" + key);
            }
        }
        #endregion
    }
}
