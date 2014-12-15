using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;

namespace Reply.Iveco.LeadManagement.Presenter
{
    public partial class Auth
    {
        public bool IsAuthenticated { get; set; }
        public DateTime DateAuthentication { get; set; }

    }
}
