using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Reply.Iveco.LeadManagement.LoadLeadService
{
    [Serializable()]
    public sealed class Configurator
    {
        public Collection<string> OrganizationName { get; set; }
    }
}
