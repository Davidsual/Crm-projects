using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Iveco.LeadManagement.Presenter.Model
{
    [Serializable()]
    public partial class DataSchedulerModel
    {
        public Guid ServiceConfigurationId { get; set; }
        public string ServiceConfigurationName { get; set; }
        public Guid ServiceId { get; set; }
        public Guid LanguageId { get; set; }
        public int? SlotDurationHours { get; set; }
        public DateTime? MorningStartSlot { get; set; }
        public DateTime? MorningEndSlot { get; set; }
        public DateTime? AfternoonStartSlot { get; set; }
        public DateTime? AfternoonEndSlot { get; set; }
        public int? BookingStartDays { get; set; }
        public int? BookingEndDays { get; set; }
        public int? AnchorOffset { get; set; }
        public int Duration { get; set; }
        public bool IsSchedulable { get; set; }
        public int? ASAPEndDays { get; set; }
        public string Granularity { get; set; }
    }
}
