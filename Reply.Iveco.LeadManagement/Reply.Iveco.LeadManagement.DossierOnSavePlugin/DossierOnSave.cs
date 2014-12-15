using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using System.Diagnostics;
using Microsoft.Win32;
using Xrm;
[assembly: CLSCompliant(true)]

namespace Reply.Iveco.LeadManagement.DossierOnSavePlugin
{
    public class DossierOnSave : IPlugin
    {
        #region PRIVATE CONSTANTS
        private const string CLASS_NAME = "DossierOnSave";
        private const string CSI = "CSI";
        private const string ACTIVITYID = "activityid";
        #endregion

        #region PRIVATE MEMBERS
        private XrmDataContext _xrmContext;
        private ICrmService _crmService;
        #endregion

        #region PROTECTED PROPERTIES
        protected IPluginExecutionContext Context { get; set; }
        protected ICrmService CrmService
        {
            get
            {
                if (_crmService == null)
                    _crmService = Context.CreateCrmService(true);
                return _crmService;
            }
        }
        protected XrmDataContext XrmContext
        {
            get
            {
                if (_xrmContext == null)
                    _xrmContext = new XrmDataContext(this.Context.OrganizationName + ".CRMOnline");
                return _xrmContext;
            }
        }
        protected Guid CurrentUserId { get; set; }
        #endregion

        #region IPlugin Members

        public void Execute(IPluginExecutionContext context)
        {
            this.Context = context;

            Debug("Begin plugin");
            Debug("InitiatingUser: " + GetUserFullName(context.InitiatingUserId));
            Debug("Current User: " + GetUserFullName(context.UserId));
            this.CurrentUserId = context.UserId;

            RunPluginBody();

            Debug("End plugin");
        }

        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Esegue il corpo del plugin
        /// </summary>
        private void RunPluginBody()
        {
            #region STAMPA ELENCO PROPERTY
            Debug("MessageName=" + Context.MessageName);
            Debug("PrimaryEntityName=" + Context.PrimaryEntityName);
            foreach (PropertyBagEntry property in Context.InputParameters.Properties)
            {
                Debug(property.Name + " - " + property.Value.GetType().ToString());
            }
            #endregion

            ///Per le modifiche, il target è un oggetto di tipo DynamicEntity
            if (Context.InputParameters.Properties.Contains(ParameterName.Target) && Context.InputParameters.Properties[ParameterName.Target] is DynamicEntity)
            {
                Debug("L'input parameter contiene una property di tipo Dynamic Entity chiamata Target");
                DynamicEntity targetDossier = (DynamicEntity)Context.InputParameters[ParameterName.Target];
                foreach (Property p in targetDossier.Properties)
                    Debug(p.Name + " " + p.GetType().ToString());
                if (targetDossier.Properties.Contains(ACTIVITYID) && targetDossier[ACTIVITYID] is Key)
                {
                    Debug("Il dossier contiene una property di tipo Key chiamata activityid");
                    serviceappointment dossier = this.GetDossierById(((Key)targetDossier[ACTIVITYID]).Value);
                    if (dossier == null)
                    {
                        Error("Impossibile recuperare il dossier con id " + ((Key)targetDossier[ACTIVITYID]).Value.ToString());
                        return;
                    }
                    if (!dossier.new_datetimecontact1.HasValue)
                    {
                        Debug("Il dossier non ha la data di primo contatto valorizzata");
                        return;
                    }
                    activityparty party = dossier.customers.FirstOrDefault();
                    if (party == null)
                    {
                        Debug("Il lead non è valorizzato all'interno del dossier");
                        return;
                    }
                    contact lead = this.GetContactById(party.partyid.Value);
                    if (lead == null)
                    {
                        Error("Non è stato possibile recuperare il lead con id " + dossier.Contact_ServiceAppointments_id.Value);
                        return;
                    }
                    Debug("Aggiorno le informazioni di primo contatto sul lead");
                    if (!dossier.subject.ToUpperInvariant().Contains(CSI))
                    {
                        lead.new_1profilingcontactdate = dossier.new_datetimecontact1.Value;
                        lead.new_1profilingcontactuserid = dossier.new_useridcontact1id.Value;
                        Debug("Nuove informazioni: " + lead.new_1profilingcontactuserid + " " + lead.new_1profilingcontactdate.Value.ToShortDateString());
                    }
                    else
                    {
                        lead.new_1csicontactdate = dossier.new_datetimecontact1.Value;
                        lead.new_1csicontactuserid = dossier.new_useridcontact1id.Value;
                        Debug("Nuove informazioni: " + lead.new_1csicontactuserid + " " + lead.new_1csicontactdate.Value.ToShortDateString());
                    }
                    this.XrmContext.UpdateObject(lead);
                    this.XrmContext.SaveChanges();
                }
                else
                {
                    Error("Il dossier non contiene una property di tipo Key chiamata activityid");
                    foreach (Property p in targetDossier.Properties)
                        Debug(p.Name + " " + p.GetType().ToString());
                }
            }
            else
                Error("L'input parameter non contiene una property di tipo Dynamic Entity chiamata Target");

        }
        /// <summary>
        /// Ottiene un lead a partire dal suo contactId
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private contact GetContactById(Guid contactId)
        {
            return XrmContext.contacts.SingleOrDefault(c => c.contactid == contactId);
        }
        /// <summary>
        /// Ricava il fullname del systemuser a partire dal suo id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        string GetUserFullName(Guid userId)
        {
            return XrmContext.systemusers.Single(u => u.systemuserid == userId).fullname;
        }
        ///// <summary>
        ///// Ottiene la URL del server dal registro
        ///// </summary>
        ///// <returns></returns>
        //private static string GetServerUrl()
        //{
        //    RegistryKey regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\MSCRM");
        //    Uri uri = new Uri(regKey.GetValue("ServerUrl").ToString());
        //    return uri.Host;
        //}
        /// <summary>
        /// Ottiene un dossier a partire dal suo activityId
        /// </summary>
        /// <param name="dossierId"></param>
        /// <returns></returns>
        private serviceappointment GetDossierById(Guid dossierId)
        {
            return XrmContext.serviceappointments.SingleOrDefault(d => d.activityid == dossierId);
        } 
        #endregion

        #region STATIC METHODS
        private static void LogEvent(string msg, EventLogEntryType msgType)
        {
            try
            {
                using (new Microsoft.Crm.Sdk.CrmImpersonator())
                {
                    if (!EventLog.SourceExists(CLASS_NAME))
                        EventLog.CreateEventSource(CLASS_NAME, "Application");
                    EventLog.WriteEntry(CLASS_NAME, msg, msgType);
                }
            }
            catch { }
        }
        public static void WriteEvent(string msg)
        {
            LogEvent(msg, EventLogEntryType.Information);
        }
        public static void Error(string msg)
        {
            LogEvent(msg, EventLogEntryType.Error);

        }
        public static void Debug(string msg)
        {
#if DEBUG
            LogEvent(msg, EventLogEntryType.Information);
#endif
        }

        #endregion
    }
}
