using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Reply.Iveco.LeadManagement.Web.UI.Test.CrmDealerServices;
using Reply.Iveco.LeadManagement.Presenter;
using System.Net;

namespace Reply.Iveco.LeadManagement.Web.UI.Test
{
    public partial class TestSetLead : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //CrmMobileService.LeadManagementMobileService serv = new Reply.Iveco.LeadManagement.Web.UI.Test.CrmMobileService.LeadManagementMobileService();
            //serv.CheckLogin(@"replynet\d.trotta", "80.Furer");
            try
            {
                LeadManagementServiceBase.LeadManagementServices serv = new Reply.Iveco.LeadManagement.Web.UI.Test.LeadManagementServiceBase.LeadManagementServices();
                //serv.Url = "http://to0crm03.cloud.reply.eu/customivecoleadmanagement/services/LeadManagementServices.asmx";
                //var rest = serv.GetDealer(new Reply.Iveco.LeadManagement.Web.UI.Test.LeadManagementServiceBase.GetDealerParameter()
                //{
                //    CountryId = new Guid("FF60F284-1814-E011-882D-005056326A2D"),
                //    IsFlagCritico = false,
                //    LeadType = 1,
                //    ZipCode = "000"
                //});
                serv.HeaderAuthenticationValue = new Reply.Iveco.LeadManagement.Web.UI.Test.LeadManagementServiceBase.HeaderAuthentication()
                {
                    Username = "DYNAMICFUN1",
                    Password = "V4DUtKBD"
                };
                serv.Url = "https://lmcert.crmdealer.iveco.com/customlmcert/services/LeadManagementServices.asmx";
                serv.Proxy = new WebProxy("proxy.reply.it", 8080);
                serv.Proxy.Credentials = new NetworkCredential("d.trotta","80.David", "replynet");
                var t = serv.GetCalendar("italia", "italiano");
                //var t = serv.SetAppointment("ASAP", new Reply.Iveco.LeadManagement.Web.UI.Test.LeadManagementServiceBase.CallBackData()
                //{
                //    CustomerName = "FRA",
                //    DataLeadCreation = new DateTime(2011, 02, 12, 14, 00, 00),
                //    FlagPrivacy = true,
                //    FlagPrivacyDue = true,
                //    DueDate = new DateTime(2011, 02, 12, 14, 00, 00),
                //    DesideredData = new DateTime(2011, 02, 12, 14, 00, 00),
                //    //CustomerSurname = "TRO",
                //    Language = "francese",
                //    Nation = "FRANCIA",
                //    //TypeContact = "NEW",
                //    //InitiativeSource = "landing page/new/italia",
                //    //InitiativeSourceReport = "ECODAILY Tom Tom",
                //    //InitiativeSourceReportDetail = "Google",
                //    PhoneNumber = "211231"
                //}, DateTime.MinValue, DateTime.MinValue);

                Response.Write("TEst");
                //CrmDealerServices.CrmDealerServices serv = new Reply.Iveco.LeadManagement.Web.UI.Test.CrmDealerServices.CrmDealerServices();
                //serv.Url = "http://localhost:29540/services/CrmDealerServices.asmx";
                //serv.FindDealer(new ContactLead() { TypeContact = "1", ZipCode = "000", CriticalCustomer = false }, "ivecosvilitalia");

                //CrmMobileService.LeadManagementMobileService serv = new Reply.Iveco.LeadManagement.Web.UI.Test.CrmMobileService.LeadManagementMobileService();
                //serv.Url = "https://lmcert.crmdealer.iveco.com/Customlmcert/services/LeadManagementMobileService.asmx";
                ////serv.SetLead(new Reply.Iveco.LeadManagement.Web.UI.Test.CrmMobileService.SetLeadParameter()
                ////{
                ////    InitiativeSource = "landing page/new/italia",
                ////    InitiativeSourceReport = "ECODAILY Tom Tom",
                ////    InitiativeSourceReportDetail = "Google",
                ////    CountryId = new Guid("df7ff5e1-3204-e011-882d-005056326a2d"),
                ////    LanguageId = new Guid("8de6d85d-ea01-e011-882d-005056326a2d"),
                ////   FirstName = "Davdasdi",
                ////   LastName="trodsatta",
                ////   TelephoneNumber = "213213",
                ////   TypeContactCode = 1
                ////}, new Guid("8a093d7e-2e01-e011-882d-005056326a2d"));
                //var r = serv.CheckLogin(@"IVECOCLOUD\crmdnsadmin","CrmReply100");
            }
            catch (Exception ex)
            {

                throw;
            }
            //var rest = serv.SetDealer(new Reply.Iveco.LeadManagement.Web.UI.Test.LeadManagementServiceBase.SetDealerParameter()
            //    {
            //        IdLeadCrm = "LEAD-869",
            //        DealerCode = "987654321",
            //        DealerCompanyName = "Test Laura & Davide",
            //        DealerResponsible = "Laura Tornatore",
            //        DealerEmail = "l.tornatore@reply.it",
            //        IsDealerAgree = false
            //    });
            //serv.SetDealer(new Reply.Iveco.LeadManagement.Web.UI.Test.LeadManagementServiceBase.SetDealerParameter()
            //{
            //     CriticalReasonCode = 1,
            //     DealerCode = "",
            //     DealerCompanyName = "",
            //     DealerEmail = string.Empty,
            //     DealerResponsible ="Davide Trotta",
            //     EmailMarketingAccount ="trottadavide@hotmail.com",
            //     IdLeadCrm = "LEAD-13",
            //     IsCriticalCustomer = true,    
            //     IsDealerAgree = true,
            //     MarketingAccount ="Luisa Corna"
            //});
            //using (LeadManagementController currentControl = new LeadManagementController("IvecoLeadManagement", HttpContext.Current))
            //        currentControl.ProcessUploadedLead();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            CrmDealerServices.CrmDealerServices service = new CrmDealerServices.CrmDealerServices(); 
            ContactLead leadToCreate = new ContactLead();
            leadToCreate.Customer_Name = this.txtName.Text;
            leadToCreate.Customer_Surname = this.txtSurname.Text;
            leadToCreate.IDLeadCRMLead = this.txtIdLead.Text;
            leadToCreate.ZipCode = this.txtZipCode.Text;
            leadToCreate.City = this.txtCity.Text;
            leadToCreate.Country = this.txtCountry.Text;
            leadToCreate.BusinessType = this.txtBT.Text;
            leadToCreate.Campagna = this.txtCampaign.Text;
            leadToCreate.FlagPrivacy = Boolean.Parse(this.txtPrivacy.Text);
            leadToCreate.JobDescription = this.txtRoleCode.Text;
            leadToCreate.NotaProdottoDiInteresse = this.txtNotaProdotto.Text;

            service.SetLead(leadToCreate, "IvecoSvilItalia");
            
        }
    }
}
