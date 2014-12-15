using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reply.Iveco.LeadManagement.CrmDealerLead.Entities
{
    public class LeadMgmtConnectionInfo
    {
        public LeadMgmtConnectionInfo()
        {

        }

        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
