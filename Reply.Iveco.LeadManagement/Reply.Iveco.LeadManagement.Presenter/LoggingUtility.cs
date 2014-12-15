using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
[assembly: CLSCompliant(true)]

namespace Reply.Iveco.LeadManagement.Presenter
{
    public static class LoggingUtility
    {
        #region PRIVATE MEMBERS
        private const string APPLICATION = "Application"; 
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Scrive su l'event Viewer i messaggi (errore,warning,debug)
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="msgType"></param>
        /// <param name="className"></param>
        private static void LogEvent(string msg, EventLogEntryType msgType,string className)
        {
            try
            {
                if (!EventLog.SourceExists(className))
                    EventLog.CreateEventSource(className, APPLICATION);
                EventLog.WriteEntry(className, msg, msgType);
            }
            catch { }
        } 
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Sceive un evento di tipo information
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="className"></param>
        public static void WriteEvent(string msg, string className)
        {
            LogEvent(msg, EventLogEntryType.Information, className);
        }
        /// <summary>
        /// Scrive un evento di tipo errore
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="className"></param>
        public static void Error(string msg, string className)
        {
            LogEvent(msg, EventLogEntryType.Error, className);

        }
        /// <summary>
        /// Scrive solo in debug
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="className"></param>
        public static void Debug(string msg, string className)
        {
#if DEBUG
            LogEvent(msg, EventLogEntryType.Information, className);
#endif
        } 
        #endregion
    }
}
