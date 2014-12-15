using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Crm.Sdk;
using System.Diagnostics;
using System.Web.Services.Protocols;

namespace Reply.Iveco.LeadManagement.Utils.Classes
{
    public static class Log
    {
        public static void Error(SoapException ex)
        {
            try
            {
                using (new CrmImpersonator())
                {
                    if (!EventLog.SourceExists("Utilities"))
                        EventLog.CreateEventSource("Utilities", "Application");
                    EventLog.WriteEntry("Utilities", ex.Detail.InnerText + Environment.NewLine + ex.StackTrace, EventLogEntryType.Information);
                }
            }
            catch (Exception)
            {

            }
        }
        public static void Error(Exception ex)
        {
            try
            {
                using (new CrmImpersonator())
                {
                    if (!EventLog.SourceExists("Utilities"))
                        EventLog.CreateEventSource("Utilities", "Application");
                    EventLog.WriteEntry("Utilities", ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
                }
            }
            catch (Exception) { }
        }
        public static void Debug(string msg)
        {
            try
            {
                using (new CrmImpersonator())
                {
                    if (!EventLog.SourceExists("Utilities"))
                        EventLog.CreateEventSource("Utilities", "Application");
                    EventLog.WriteEntry("Utilities", msg, EventLogEntryType.Information);
                }
            }
            catch (Exception) { }
        }
    }
}
