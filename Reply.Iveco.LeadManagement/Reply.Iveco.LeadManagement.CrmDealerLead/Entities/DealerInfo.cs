using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Crm.Sdk;

using Reply.Iveco.LeadManagement.CrmDealerLead.Classes;

namespace Reply.Iveco.LeadManagement.CrmDealerLead.Entities
{
    [Serializable]
    public class DealerInfo
    {
        public DealerInfo() { }

        //Campo new_iddealer
        public string DealerCode { get; set; }

        //Campo new_businessid (Name)
        public string DealerName { get; set; }

        //Campo new_buivecomarket
        public bool IsMarketBu { get; set; }

        //Campo new_userid
        public Guid DefaultSeller { get; set; }

        //Campo new_dealerdefaultuserusedid
        public Guid DealerLeadUsed { get; set; }

        //Campo new_dealerdefaultusernewid
        public Guid DealerLeadNew { get; set; }

        public static DealerInfo FromDynamicEntity(DynamicEntity source)
        {
            DealerInfo _res = new DealerInfo(){
                DealerName = ((Lookup)source["new_businessid"]).name,
                DealerCode = source.GetString("new_iddealer"),
                DefaultSeller = source.GetLookup("new_userid"),
                DealerLeadUsed = source.GetLookup("new_dealerdefaultuserusedid"),
                DealerLeadNew = source.GetLookup("new_dealerdefaultusernewid"),
                IsMarketBu = source.GetBoolean("new_buivecomarket")
            };
            return _res;
        }
    }
}
