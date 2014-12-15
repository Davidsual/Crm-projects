using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Iveco.LeadManagement.Presenter
{
    [Serializable()]
    public partial class CallBackData
    {
        public DateTime DataLeadCreation { get; set; }
        public string FileName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerSurname { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Nation { get; set; }
        public string EMail { get; set; }
        public string PhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public bool? FlagPrivacy { get; set; }
        public string Brand { get; set; }
        public string StockSearchedModel { get; set; }
        public DateTime DueDate { get; set; }
        public string TypeContact { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public string GVW { get; set; }
        public string WheelType { get; set; }
        public string Fuel { get; set; }
        public bool? FlagPrivacyDue { get; set; }
        public string IdCare { get; set; }
        public string InitiativeSource { get; set; }
        public string InitiativeSourceReport { get; set; }
        public string InitiativeSourceReportDetail { get; set; }
        public string CompanyName { get; set; }
        public string Power { get; set; }
        public string CabType { get; set; }
        public string Suspension { get; set; }
        public string CommentWebForm { get; set; }
        public string IdLeadSite { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public string CodePromotion { get; set; }
        public string ModelOfInterest { get; set; }
        public DateTime DesideredData { get; set; }
        public string Canale { get; set; }

    }
}
