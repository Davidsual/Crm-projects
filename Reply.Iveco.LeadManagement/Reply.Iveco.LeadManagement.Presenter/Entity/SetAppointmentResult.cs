using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Iveco.LeadManagement.Presenter
{
    [Serializable()]
    public sealed class SetAppointmentResult
    {
        /// <summary>
        /// Indica se l'operazione è andata a buon fine
        /// </summary>
        public bool IsSuccessful { get; set; }
        /// <summary>
        /// Eventuale errore se issuccessful è a false
        /// </summary>
        public string ErrorDescription { get; set; }
        /// <summary>
        /// Codice Errore se issuccessful è a false
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// Data inizio appuntamento / Facoltativo in caso di ASAP
        /// </summary>
        public DateTime startDate { get; set; }
        /// <summary>
        /// Data inizio appuntamento / Facoltativo in caso di ASAP
        /// </summary>
        public DateTime endDate { get; set; }
    }
}
