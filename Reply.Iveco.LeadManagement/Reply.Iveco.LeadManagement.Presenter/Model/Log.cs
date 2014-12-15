using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Web.Services.Protocols;
using Microsoft.Crm.Sdk;

namespace Reply.Iveco.LeadManagement.Presenter.Model
{
    public static class Log
    {
        #region Log

        public static void Error(SoapException ex)
        {
            try
            {
                using (new CrmImpersonator())
                {
                    if (!EventLog.SourceExists(System.Reflection.Assembly.GetExecutingAssembly().FullName))
                        EventLog.CreateEventSource(System.Reflection.Assembly.GetExecutingAssembly().FullName, "Application");
                    EventLog.WriteEntry(System.Reflection.Assembly.GetExecutingAssembly().FullName, ex.Detail.InnerText + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
                }
            
            }
            catch (SoapException e)
            {
               
            }
            catch (Exception e)
            {

            }

        }
        public static void Error(Exception ex)
        {
            try
            {
                using (new CrmImpersonator())
                {
                    if (!EventLog.SourceExists(System.Reflection.Assembly.GetExecutingAssembly().FullName))
                        EventLog.CreateEventSource(System.Reflection.Assembly.GetExecutingAssembly().FullName, "Application");
                    EventLog.WriteEntry(System.Reflection.Assembly.GetExecutingAssembly().FullName, ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
                }

             }
            catch (SoapException e)
            {
               
            }
            catch (Exception e)
            {

            }
        }
        public static void Debug(string msg)
        {
            try
            {
                using (new CrmImpersonator())
                {
                    if (!EventLog.SourceExists(System.Reflection.Assembly.GetExecutingAssembly().FullName))
                        EventLog.CreateEventSource(System.Reflection.Assembly.GetExecutingAssembly().FullName, "Application");
                    EventLog.WriteEntry(System.Reflection.Assembly.GetExecutingAssembly().FullName, msg, EventLogEntryType.Information);
                }
            }
            catch (SoapException e)
            {

            }
            catch (Exception e)
            {

            }
        }

        #endregion
    }
}
