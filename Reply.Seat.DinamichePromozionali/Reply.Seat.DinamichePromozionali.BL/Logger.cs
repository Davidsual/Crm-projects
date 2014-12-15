using System;
using System.IO;
using System.Linq;
using log4net;
using log4net.Appender;
using log4net.Repository;

// Configure log4net using the .config file
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Reply.Seat.DinamichePromozionali.BL
{
    public partial class Logger
    {
        #region PRIVATE MEMBERS
        string _path;
        private ILog _logger = log4net.LogManager.GetLogger(typeof(Logger)); 
        #endregion

        #region PRIVATE PROPERTY
        private string procid
        {
            get
            {
                return
                    //Environment.MachineName + "-" +
                    //System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime().ToString("s") + "-" + 
                System.Diagnostics.Process.GetCurrentProcess().Id.ToString();
            }
        } 
        #endregion

        #region CTOR
        public Logger()
        {
            log4net.GlobalContext.Properties["procid"] = procid;

        }
        public Logger(string path)
        {
            log4net.GlobalContext.Properties["procid"] = procid;
            Initialize(path);
            _path = path;
        }
        #endregion

        #region PUBLIC METHODS

        public void WriteCsvLine(Message message)
        {
            //string line = DateTime.Now.ToString() + "|" + message.Row + "|" + message.Type.ToString() + "|" + message.Detail + "|" + message.StackTrace + Environment.NewLine;
            string line = DateTime.Now.ToString() + "|" + message.Type.ToString() + "|" + message.Detail + "|" + message.StackTrace + Environment.NewLine;
            _logger.Info(line);
        }
        public void Initialize(string logFile)
        {
            //get the current logging repository for this application 
            ILoggerRepository repository = _logger.Logger.Repository;
            //get all of the appenders for the repository 
            IAppender[] appenders = repository.GetAppenders();

            //only change the file path on the 'FileAppenders' 
            foreach (IAppender appender in (from iAppender in appenders
                                            where iAppender is FileAppender
                                            select iAppender))
            {
                FileAppender fileAppender = appender as FileAppender;
                //set the path to your logDirectory using the original file name defined 
                //in configuration 
                fileAppender.File = logFile;
                //make sure to call fileAppender.ActivateOptions() to notify the logging 
                //sub system that the configuration for this appender has changed. 
                fileAppender.ActivateOptions();
            }
        } 
        #endregion
    }
    /// <summary>
    /// Level Log
    /// </summary>
    public enum LogLevel
    {
        Trace = 0,
        Message,
        Warning,
        Error,
        Exception
    }
    [Serializable]
    public class Message
    {
        #region CTOR
        /// <summary>
        /// Costruttore
        /// </summary>
        public Message()
        {

        } 
        #endregion

        #region PUBLIC PROPERTY
        public LogLevel Type { get; set; }
        public string Detail { get; set; }
        public string StackTrace { get; set; }
        public string Row { get; set; } 
        #endregion

        #region PUBLIC METHODS
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(StackTrace))
                return "[Row#" + Row + "] " + Type.ToString() + ": " + Detail + " @ " + StackTrace;
            else
                return "[Row#" + Row + "] " + Type.ToString() + ": " + Detail;
        }
        public string ToShortString()
        {
            if (string.IsNullOrEmpty(Detail))
                return Environment.NewLine;
            if (Type == LogLevel.Trace && string.IsNullOrEmpty(Row))
                return Detail + Environment.NewLine;
            else if (Type <= LogLevel.Message)
                return "[" + Row + "] " + Detail + Environment.NewLine;
            else if (Type == LogLevel.Exception)
                return "[" + Row + "] " + Type + ": " + Detail + " @ " + StackTrace + Environment.NewLine;
            else
                return "[" + Row + "] " + Type + ": " + Detail + Environment.NewLine;
        } 
        #endregion
    }
}
