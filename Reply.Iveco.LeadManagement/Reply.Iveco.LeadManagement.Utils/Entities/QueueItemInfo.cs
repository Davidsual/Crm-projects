using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Data.SqlClient;

namespace Reply.Iveco.LeadManagement.Utils.Entities
{
    public class QueueItemInfo
    {
        public QueueItemInfo() { }

        public QueueItemInfo(string data)
        {
            string[] _info = data.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            ObjectId = _info[0];
            ObjectTypeCode = _info[1];
        }

        private string GetLookupField(string lookupId, string lookupName, string lookupType)
        {
            if (string.IsNullOrEmpty(lookupName))
            {
                return "<NOBR class=datetime></NOBR>";
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "<NOBR title=\"{0}\">" +
                    "<SPAN class=gridLui ondblclick=clearTimer() contentEditable=false onclick=handleGridClick() otype=\"{1}\" oid=\"{2}\" isLink=\"1\">" +
                    "<IMG class=ms-crm-Lookup-Item alt=\"\" src=\"/_imgs/ico_16_{1}.gif\">{0}</SPAN></NOBR>",
                    lookupName, lookupType, lookupId);
            }
        }

        private string GetTextField(string fieldText)
        {
            if (string.IsNullOrEmpty(fieldText))
            {
                return "<NOBR class=datetime></NOBR>";
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture, "<NOBR title=\"{0}\">{0}</NOBR>", fieldText);
            }
        }

        public override string ToString()
        {
            return
                this.ObjectId + ";" +
                this.ObjectTypeCode + ";" +

                GetTextField(this.StartTime) + ";" +
                GetTextField(this.DateTimeContact1) + ";" +
                GetTextField(this.ResultContact1) + ";" +
                GetTextField(this.DateTimeContact2) + ";" +
                GetTextField(this.ResultContact2) + ";" +
                GetTextField(this.OverbookingTime);
            
                //GetLookupField(this.CreatedById, this.CreatedByName, "8") + ";" +
                //GetLookupField(this.CreatedByBuId, this.CreatedByBuName, "10") + ";" +
                //GetLookupField(this.SubjectId, this.SubjectName, "129") + ";" +
                //GetLookupField(this.OwnerId, this.OwnerName, "8") + ";" +
                //GetTextField(this.CaseNumber);
        }

        public string QueueItemKey
        {
            get
            {
                return "{" + ObjectId.ToUpper() + "}," + ObjectTypeCode;
            }
        }

        public string ObjectId { get; set; }
        public string ObjectTypeCode { get; set; }

        public string StartTime { get; set; }
        public string DateTimeContact1 { get; set; }
        public string ResultContact1 { get; set; }
        public string DateTimeContact2 { get; set; }
        public string ResultContact2 { get; set; }
        public string OverbookingTime { get; set; }

        public static List<QueueItemInfo> FromSqlReader(SqlDataReader reader, string[] valoriPicklist)
        {
            List<QueueItemInfo> _res = new List<QueueItemInfo>();
            while (reader.Read())
            {
                QueueItemInfo _nuovo = new QueueItemInfo();

                int _minOffset = -1 * (reader.IsDBNull(8) ? 0 : reader.GetInt32(8));

                _nuovo.ObjectId = reader.GetGuid(0).ToString();
                _nuovo.ObjectTypeCode = reader.GetString(1);

                _nuovo.StartTime = reader.IsDBNull(2) ? string.Empty : reader.GetDateTime(2).AddMinutes(_minOffset).ToString("dd/MM/yyyy HH:mm");

                _nuovo.DateTimeContact1 = reader.IsDBNull(3) ? string.Empty : reader.GetDateTime(3).AddMinutes(_minOffset).ToString("dd/MM/yyyy HH:mm");
                _nuovo.ResultContact1 = reader.IsDBNull(4) ? string.Empty : valoriPicklist[reader.GetInt32(4)];

                _nuovo.DateTimeContact2 = reader.IsDBNull(5) ? string.Empty : reader.GetDateTime(5).AddMinutes(_minOffset).ToString("dd/MM/yyyy HH:mm");
                _nuovo.ResultContact2 = reader.IsDBNull(6) ? string.Empty : valoriPicklist[reader.GetInt32(6)];

                _nuovo.OverbookingTime = reader.IsDBNull(7) ? string.Empty : reader.GetInt32(7).ToString();
                
                _res.Add(_nuovo);
            }
            return _res;
        }

    }
}
