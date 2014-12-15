using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Iveco.LeadManagement.Presenter.Model
{
    [Serializable()]
    public sealed class ContactLead
    {
        public Guid IdOperatorUpload { get; set; }
        public string FileName { get; set; }
        public string IDLeadExternal { get; set; }
        public string IDLeadCRMLead { get; set; }
        public string Canale { get; set; }
        public string Campagna { get; set; }
        public string Customer_Name { get; set; }
        public string Country { get; set; }
        public string Customer_Surname { get; set; }
        public string Company_Name { get; set; }
        public string BusinessType { get; set; }
        public string JobDescription { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string EnderecoPostal { get; set; }
        public string Hamlet { get; set; }
        public string Province { get; set; }
        public string CustomerCountry { get; set; }
        public string PhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string OfficeNumber { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string ProfilingDataH { get; set; }
        public bool? CriticalCustomer { get; set; }
        public int CriticalReason { get; set; }
        public string CodicePromozione { get; set; }
        public bool FlagPrivacy { get; set; }
        public string MotivazioneCriticalCustomer { get; set; }
        public string TypeContact { get; set; }
        public string NotaProdottoDiInteresse { get; set; }
        public string NotaUsato { get; set; }
        public string NotaCliente { get; set; }
        public string VATCode { get; set; }
        public string TAXCode { get; set; }
        public string LegalForm { get; set; }
        public string NumberOfEmployees { get; set; }
        public string AnnualRevenue { get; set; }
        public string PreferredContactMethod { get; set; }
        public string EmailContact { get; set; }
        public string BulkEmailcontact { get; set; }
        public string PhoneContact { get; set; }
        public string FaxContact { get; set; }
        public string MailContact { get; set; }
    }

    [Serializable()]
    public sealed class ContactLeadUpload
    {
        public Guid IdOperatorUpload { get; set; }
        public string FileName { get; set; }
        public string IDLeadExternal { get; set; }
        public string IDLeadCRMLead { get; set; }
        public string Canale { get; set; }
        public string Campagna { get; set; }
        public string Customer_Name { get; set; }
        public string Country { get; set; }
        public string Customer_Surname { get; set; }
        public string Company_Name { get; set; }
        public string BusinessType { get; set; }
        public string JobDescription { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string EnderecoPostal { get; set; }
        public string Hamlet { get; set; }
        public string Province { get; set; }
        public string CustomerCountry { get; set; }
        public string PhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string OfficeNumber { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string ProfilingDataH { get; set; }
        public string CriticalCustomer { get; set; }
        public string CodicePromozione { get; set; }
        public string FlagPrivacy { get; set; }
        public string MotivazioneCriticalCustomer { get; set; }
        public string TypeContact { get; set; }
        public string NotaProdottoDiInteresse { get; set; }
        public string NotaUsato { get; set; }
        public string NotaCliente { get; set; }
        public string VATCode { get; set; }
        public string TAXCode { get; set; }
        public string LegalForm { get; set; }
        public string NumberOfEmployees { get; set; }
        public string AnnualRevenue { get; set; }
        public string PreferredContactMethod { get; set; }
        public string EmailContact { get; set; }
        public string BulkEmailcontact { get; set; }
        public string PhoneContact { get; set; }
        public string FaxContact { get; set; }
        public string MailContact { get; set; }
    }
}
