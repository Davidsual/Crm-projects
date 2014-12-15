using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Iveco.LeadManagement.Presenter
{
    [Serializable()]
    public sealed class GetDealerResult
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
        /// Dealer Company Name
        /// </summary>
        public string DealerCompanyName { get; set; }
        /// <summary>
        /// Dealer Code 
        /// </summary>
        public string DealerCode { get; set; }
        /// <summary>
        /// Dealer Responsible
        /// </summary>
        public string DealerResponsible { get; set; }
        /// <summary>
        /// Dealer Referent E-Mail
        /// </summary>
        public string DealerEmail { get; set; }
        /// <summary>
        /// Marketing Accoung
        /// </summary>
        public string MarketingAccount { get; set; }
        /// <summary>
        /// E mail marketing account
        /// </summary>
        public string EmailMarketingAccount { get; set; }
        /// <summary>
        /// Is Critical Customer 
        /// </summary>
        public bool IsCriticalCustomer { get; set; }
        /// <summary>
        /// Identifica un entità all'interno di lead management
        /// </summary>
        public int CriticalReasonCode { get; set; }
        /// <summary>
        /// Dealer manager name
        /// </summary>
        public string DealerManagerName { get; set; }
        /// <summary>
        /// Dealer manager email
        /// </summary>
        public string DealerManagerMail { get; set; }
    }
}
