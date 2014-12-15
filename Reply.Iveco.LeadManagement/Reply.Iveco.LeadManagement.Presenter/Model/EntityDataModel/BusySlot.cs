using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Iveco.LeadManagement.Presenter.Model
{
    public sealed class BusySlot
    {
        public DateTime DateBusySlot { get; set; }
        public int ServiceType { get; set; }
        public int CountBusy { get; set; }
        public int CountConflict { get; set; }
    }
}
