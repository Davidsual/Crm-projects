using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Iveco.LeadManagement.Presenter
{
    [Serializable()]
    public sealed class SetMassiveAppointmentAsapReturn
    {
        public int StartNumberRecord { get; set; }
        public int ErrorLoadRecord { get; set; }
    }
}
