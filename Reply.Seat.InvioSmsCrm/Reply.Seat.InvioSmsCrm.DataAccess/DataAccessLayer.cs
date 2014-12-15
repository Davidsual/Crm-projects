using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Reply.Seat.InvioSmsCrm.DataAccess.Model;

namespace Reply.Seat.InvioSmsCrm.DataAccess
{
    /// <summary>
    /// Classe che espone metodi get per la retrieve dei dati
    /// </summary>
    public sealed class DataAccessLayer : IDisposable
    {
        #region PRIVATE MEMBERS
        private static volatile DataAccessLayer instance = null;
        private static readonly object syncRoot = new object();
        #endregion

        #region CTOR
        /// <summary>
        /// Costruttore
        /// </summary>
        public DataAccessLayer()
        {
        }
        #endregion

        #region PUBLIC MEMBERS
        /// <summary>
        /// Stato invio SMS
        /// </summary>
        public enum SmsStatoInvio
        {
            DaInviare = 1,
            Inviato ,
            Errore 
        }
        #endregion

        #region PUBLIC PROPERTY
        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static DataAccessLayer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new DataAccessLayer();
                    }
                }

                return instance;
            }
        }
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Restituisce tutti gli sms in base a dei filtri
        /// </summary>
        /// <param name="idTemplatePremio"></param>
        /// <returns></returns>
        public List<New_sms> GetSmsToSend()
        {
            using (MarketingSeatDataContext _model = new MarketingSeatDataContext())
            {
                _model.DeferredLoadingEnabled = false;
                _model.ObjectTrackingEnabled = false;
                return _model.New_sms.ToList();
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Rilascio delle risorse
        /// </summary>
        public void Dispose()
        {

        }

        #endregion
    }
}
