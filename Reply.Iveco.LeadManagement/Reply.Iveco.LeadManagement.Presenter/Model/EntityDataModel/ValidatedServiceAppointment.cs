using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Iveco.LeadManagement.Presenter.Model
{
    public partial class ValidatedServiceAppointment
    {
        public Guid ActivityId { get; set; }
        public Guid ServiceId { get; set; }
        public bool IsValid { get; set; }
        public DateTime DateScheduledEnd { get; set; }
        public int ServiceTypeCode { get; set; }

    }
}
