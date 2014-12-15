using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Iveco.LeadManagement.Presenter
{
    [Serializable()]
    public sealed class SetDealerParameter
    {
        /// <summary>
        /// Identificativo Lead
        /// </summary>
        public string IdLeadCrm { get; set; }
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
        /// Il dealer accetta o no il passaggio dati
        /// </summary>
        public bool IsDealerAgree { get; set; }
        /// <summary>
        /// Is Critical Customer 
        /// </summary>
        public bool IsCriticalCustomer { get; set; }
        /// <summary>
        /// Identifica un entità all'interno di lead management
        /// </summary>
        public int CriticalReasonCode { get; set; }
    }
}
