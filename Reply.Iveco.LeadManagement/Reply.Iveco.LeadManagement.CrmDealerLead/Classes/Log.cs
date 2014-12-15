using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Web.Services.Protocols;

namespace Reply.Iveco.LeadManagement.CrmDealerLead.Classes
{
    public static class Log
    {
        #region Log

        public static void Error(string msg)
        {
            try
            {
                using (new Microsoft.Crm.Sdk.CrmImpersonator())
                {
                    if (!EventLog.SourceExists("CrmDealerLead"))
                        EventLog.CreateEventSource("CrmDealerLead", "Application");
                    EventLog.WriteEntry("CrmDealerLead", msg, EventLogEntryType.Error);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void Debug(string msg)
        {
            try
            {
                using (new Microsoft.Crm.Sdk.CrmImpersonator())
                {
                    if (!EventLog.SourceExists("CrmDealerLead"))
                        EventLog.CreateEventSource("CrmDealerLead", "Application");
                    EventLog.WriteEntry("CrmDealerLead", msg, EventLogEntryType.Information);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void Error(SoapException ex)
        {
            Error(ex.Detail.InnerText + Environment.NewLine + ex.StackTrace);
        }

        public static void Error(Exception ex)
        {
            Error(ex.Message + Environment.NewLine + ex.StackTrace);
        }

        #endregion
    }
}
