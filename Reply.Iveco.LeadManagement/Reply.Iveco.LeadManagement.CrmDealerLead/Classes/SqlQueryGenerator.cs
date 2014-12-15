using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reply.Iveco.LeadManagement.CrmDealerLead.Classes
{
    public static class SqlQueryGenerator
    {
        public static string GetConnectionString(string orgName)
        {
            string _res = @"
                SELECT ConnectionString
                FROM Organization O
                WHERE
	                O.State = 1
	                AND O.IsDeleted = 0
	                AND O.UniqueName= '" + orgName + @"'
                ";
            return _res;
        }

        public static string GetLeadsInClosedActivities(string filterLeads)
        {
            string _res = @"
                DECLARE  @TMPT TABLE( LeadId uniqueidentifier )

                INSERT INTO @TMPT
                SELECT RegardingObjectId
                FROM ActivityPointer
                WHERE
                StateCode = 1
                AND DeletionStateCode = 0
                AND RegardingObjectTypeCode = 4
                AND RegardingObjectId IN " + filterLeads + @"

                INSERT INTO @TMPT
                SELECT P.PartyId
                FROM ActivityParty P
	                INNER JOIN ActivityPointer ACT ON P.ActivityId = ACT.ActivityId
                WHERE
                ACT.StateCode = 1
                AND ACT.DeletionStateCode = 0
                AND P.PartyObjectTypeCode = 4
                AND P.PartyId IN " + filterLeads + @"

                SELECT DISTINCT LeadId from @TMPT
                ";
            
            return _res;
        }
    }
}
