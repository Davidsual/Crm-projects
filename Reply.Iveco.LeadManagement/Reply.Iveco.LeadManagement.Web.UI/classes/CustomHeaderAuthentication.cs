using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel.Selectors;

namespace Reply.Iveco.LeadManagement.Web.UI
{
    public class CustomHeaderAuthentication : UserNamePasswordValidator
    {
        public CustomHeaderAuthentication()
        {

        }
        public override void Validate(string userName, string password)
        {
            throw new NotImplementedException();
        }
       
    }
}
