using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Iveco.LeadManagement.Presenter
{
    public sealed class DealerInfo
    {

        public string MarketingAccount { get; set; }
        public string EmailMarketingAccount { get; set; }
        public string DealerResponsible { get; set; }
        public string DealerEmail { get; set; }
        public string DealerCode { get; set; }
        public string DealerCompanyName { get; set; }
        public string IdLeadCrm { get; set; }
        public bool Critical { get; set; }
        public int CriticalReason { get; set; }

    }
}
