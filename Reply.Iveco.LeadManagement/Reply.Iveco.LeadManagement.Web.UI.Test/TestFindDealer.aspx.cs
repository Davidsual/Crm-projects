using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Reply.Iveco.LeadManagement.Web.UI.Test.CrmDealerServices;
using System.Net;

namespace Reply.Iveco.LeadManagement.Web.UI.Test
{
    public partial class TestFindDealer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            CrmDealerServices.CrmDealerServices service = new CrmDealerServices.CrmDealerServices();

            //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            //System.Net.WebProxy wp = new System.Net.WebProxy("proxy.reply.it", 8080);
            //wp.Credentials = new System.Net.NetworkCredential("internet.guest", "Reply.96", "REPLYNET");
            //System.Net.WebRequest.DefaultWebProxy = wp;
            ////serv.Endpoint.Address = new System.ServiceModel.EndpointAddress(serviceUrl);
            ////var ret = serv.SetLead(null, "ivecoSvilitalia");


            //service.Url = "http://ec2-174-129-214-113.compute-1.amazonaws.com:5555/Leadmanagement/services/CrmDealerServices.asmx";


           


            ContactLead leadToCreate = new ContactLead();
            //leadToCreate.Customer_Name = this.txtName.Text;
            //leadToCreate.Customer_Surname = this.txtSurname.Text;
            //leadToCreate.IDLeadCRMLead = this.txtIdLead.Text;
            leadToCreate.ZipCode = this.txtZipCode.Text;
            leadToCreate.TypeContact = this.txtTypeContact.Text;
            string criticalString = this.txtCritical.Text;
            if (criticalString == "false")
                leadToCreate.CriticalCustomer = false;
            else if (criticalString == "true")
                leadToCreate.CriticalCustomer = true;
            

            FindDealerResult result = service.FindDealer(leadToCreate, "IvecoSvilItalia");
            


        }
    }
}
