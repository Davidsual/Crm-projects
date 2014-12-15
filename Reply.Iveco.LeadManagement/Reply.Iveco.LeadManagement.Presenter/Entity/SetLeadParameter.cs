using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Iveco.LeadManagement.Presenter
{
    [Serializable()]
    public sealed class SetLeadParameter
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid CountryId { get; set; }
        public Guid LanguageId { get; set; }
        public int TypeContactCode { get; set; }
        public string TelephoneNumber { get; set; }
        public string InitiativeSource { get; set; }
        public string InitiativeSourceReport { get; set; }
        public string InitiativeSourceReportDetail { get; set; }
    }
}
