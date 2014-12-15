using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Reply.Iveco.LeadManagement.Web.UI.Test.LeadManagementServiceBase;




namespace Reply.Iveco.LeadManagement.Web.UI.Test
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btnGetCalendar_Click(object sender, EventArgs e)
        {
            try
            {
                LeadManagementServices client = new LeadManagementServices();
                client.Timeout = 100000;
                client.Url = "http://to0crm03ws.cloud.reply.eu:8080/LeadManagement/services/LeadManagementServices.asmx";
                client.HeaderAuthenticationValue = new HeaderAuthentication() { Username = "hoplo1", Password = "v2su6kbp" };
                client.GetCalendar("Francia", "francese");
                
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected void btnSetAppointment_Click(object sender, EventArgs e)
        {

        }

        protected void btnSetDealert_Click(object sender, EventArgs e)
        {
            LeadManagementServices client = new LeadManagementServices();
            client.Url = "";
            client.Timeout = 100000;
            client.HeaderAuthenticationValue = new HeaderAuthentication() { Username = "hoplo1", Password = "v2su6kbp" };
            client.SetDealer(new Reply.Iveco.LeadManagement.Web.UI.Test.LeadManagementServiceBase.SetDealerParameter()
                {
                    IdLeadCrm = "LEAD-336",
                    DealerCode = "0000063874",
                    DealerCompanyName = "AMBROSIANA CARRI",
                    DealerEmail = "s.scozzi@reply.it",
                    DealerResponsible = "Salvatore Scozzi",
                    MarketingAccount = "",
                    EmailMarketingAccount = ""
                });
        }
    }
}
