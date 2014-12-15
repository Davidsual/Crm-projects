using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Reply.Seat.DinamichePromozionali.CrmAdapter.CrmServiceWsdl;
using Reply.Seat.DinamichePromozionali.CrmAdapter.CrmServicePerformance;
using Reply.Seat.DinamichePromozionali.DataAccess;
using Reply.Seat.DinamichePromozionali.DataAccess.Model;
using System.Web;

namespace Reply.Seat.DinamichePromozionali.BL
{
    /// <summary>
    /// Base Class PageController gestisce i metodi standard di accesso 
    /// </summary>
    public abstract class BaseManager
    {
        #region PRIVATE MEMBER
        private static volatile Manager instance = null;
        private static readonly object syncRoot = new object();
    	private CrmAdapter.CrmAdapter _currentCrmAdapter = null;
        private HttpContext _currentContextWeb = null;
	    #endregion

        ///// <summary>
        ///// Singleton Instance
        ///// </summary>
        //public static Manager Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            lock (syncRoot)
        //            {
        //                if (instance == null)
        //                    instance = new Manager();
        //            }
        //        }

        //        return instance;
        //    }
        //}

        #region CTOR
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="context"></param>
        public BaseManager(HttpContext context)
        {
            this.CurrentContextWeb = context;
        }
        public BaseManager()
        {

        }
        #endregion

        #region PROTECTED PROPERTY

        /// <summary>
        /// Accesso ai dati
        /// </summary>
        protected DataAccessLayer CurrentAccessLayer { get { return DataAccessLayer.Instance; } }
        /// <summary>
        /// Accesso al Crm in Set
        /// </summary>
        protected CrmAdapter.CrmAdapter CurrentCrmAdapter 
        { 
            get
            {
                if (_currentCrmAdapter == null)
                    _currentCrmAdapter = new Reply.Seat.DinamichePromozionali.CrmAdapter.CrmAdapter(this.CurrentContextWeb);
                return _currentCrmAdapter;
            }
        }
        #endregion

        #region PRIVATE PROPERTY
        public HttpContext CurrentContextWeb
        {
            get { return _currentContextWeb; }
            set { _currentContextWeb = value; }
        }
        /// <summary>
        /// Oggetto di log
        /// </summary>
        private Logger _logger;
        #endregion

        #region PROTECTED METHODS
        /// <summary>
        /// Crea una CrmLookup
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected Lookup CreaLookup(string entityName, Guid id)
        {
            return new Lookup() { type = entityName, Value = id };
        }
        /// <summary>
        /// Crea un CrmDateTime a partire da un DateTime
        /// </summary>
        /// <param name="valore"></param>
        /// <returns></returns>
        protected CrmDateTime CreaDateTime(DateTime valore)
        {
            if (valore == null)
                return null;
            if (valore.Kind != DateTimeKind.Local)
                valore = valore.ToLocalTime();
            return new CrmDateTime() { Value = valore.ToString("yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) };
        }

        /// <summary>
        /// Esegue la chimata alla classe di log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnNewMessage(object sender, EventArgs e)
        {
            Message message = (Message)sender;
            DateTime now = DateTime.Now;
            Console.WriteLine(now.ToLongTimeString()+"."+now.Millisecond + " " + message.ToShortString());
            if (_logger == null) _logger = new Logger();
            _logger.WriteCsvLine(message);
        }
        /// <summary>
        /// Risponde alla richiesta di log generando un oggetto Message
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="descrizione"></param>
        public void NewMessage(LogLevel tipo, string descrizione)
        {
            Message message = new Message()
            {
                Type = tipo,
                Detail = descrizione,
            };

            OnNewMessage(message, null);
        }
        #endregion
    }
}
