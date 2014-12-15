using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Reply.Iveco.LeadManagement.Utils.Classes
{
    public static class SqlQueryStatement
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

//        public static string GetIncidentInfo(List<string> values)
//        {
//            if (values.Count == 0)
//                values.Add(Guid.Empty.ToString() + ",112");

//            StringBuilder _sb = new StringBuilder("(");
//            foreach (string _valId in values)
//            {
//                _sb.Append("'" + _valId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0].ToSqlGuid() + "',");
//            }

//            string _res = @"
//                    select I.incidentid, N'112', I.createdby, I.createdbyname, I.subjectid, I.subjectidname, S.businessunitid, S.businessunitidname, I.OwnerId, I.OwnerIdName, I.Ticketnumber
//                    from Incident I inner join systemuser S on I.createdby = S.systemuserid
//                    where I.incidentId in 
//            " + _sb.ToString().TrimEnd(new char[] { ',' }) + ")";
//            return _res;
//        }

        public static string GetActivityInfo(List<string> values, string userId)
        {
            if (values.Count == 0)
                values.Add(Guid.Empty.ToString() + ",4200");

            StringBuilder _sb = new StringBuilder("(");
            foreach (string _valId in values)
            {
                _sb.Append("'" + _valId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0].ToSqlGuid() + "',");
            }

            string _res = @"

                    DECLARE @USEROFFSET int
                    SELECT @USEROFFSET = timezonebias FROM usersettings where systemuserid = '" + userId.ToSqlGuid() + @"'

                    select I.activityid, cast( I.activitytypecode as nvarchar),
                        S.scheduledstart,
                        new_datetimecontact1,
                        ISNULL(new_resultcontact1, 0),
                        new_datetimecontact2,
                        ISNULL(new_resultcontact2, 0),
                        new_overbookingdays,
                        ISNULL(@USEROFFSET, 0)
                    from activitypointer I inner join serviceappointment S on I.activityid = S.activityid
                    where I.activityid in 
            " + _sb.ToString().TrimEnd(new char[] { ',' }) + ")";
            return _res;
        }

    }
}
