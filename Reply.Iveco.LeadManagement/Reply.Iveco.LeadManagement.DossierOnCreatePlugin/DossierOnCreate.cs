using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Xrm;
using System.Diagnostics;

namespace Reply.Iveco.LeadManagement.DossierOnCreatePlugin
{
    public class DossierOnCreate : IPlugin
    {
        #region PRIVATE CONSTANTS
        private const string CLASS_NAME = "DossierOnCreate";
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
                    _xrmContext = new XrmDataContext(this.Context.OrganizationName+".CRMOnline");
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
            if (Context.OutputParameters.Contains(ParameterName.Id) && Context.OutputParameters[ParameterName.Id] is Guid)
            {
                Debug("L'OutputParameters contiene una property di tipo Guid chiamata Id");
                serviceappointment dossier = this.GetDossierById((Guid)Context.OutputParameters[ParameterName.Id]);
                if (dossier == null)
                {
                    Error("Impossibile recuperare il dossier con id " + ((Guid)Context.OutputParameters[ParameterName.Id]).ToString());
                    return;
                }
                if (dossier.scheduledstart.HasValue && dossier.createdon.HasValue)
                {
                    dossier.new_overbookingdays = (dossier.scheduledstart.Value - dossier.createdon.Value).Days;
                    this.XrmContext.UpdateObject(dossier);
                    this.XrmContext.SaveChanges();
                }
                else
                {
                    Debug("I giorni di overbooking possono essere calcolati solo se sia la data di schedulazione che la data di creazione sono valorizzate");
                    Debug("Data creazione: " + (dossier.createdon.HasValue ? dossier.createdon.Value.ToShortDateString() : "N/A") + " Data schedulazione: " + (dossier.scheduledstart.HasValue ? dossier.scheduledstart.Value.ToShortDateString() : "N/A"));
                    return;
                }
            }
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
