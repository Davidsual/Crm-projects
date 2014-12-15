using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using System.Runtime.Serialization;

namespace Reply.Iveco.LeadManagement.Presenter
{
    public partial class HeaderAuthentication : SoapHeader
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
