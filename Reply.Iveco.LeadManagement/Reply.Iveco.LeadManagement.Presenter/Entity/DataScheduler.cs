using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reply.Iveco.LeadManagement.Presenter.Model;

namespace Reply.Iveco.LeadManagement.Presenter
{
    [Serializable()]
    public partial class DataScheduler
    {
        //public List<DataSchedulerHeader> LstColumn { get; set; }
        //public List<DataSchedulerRow> LstRow { get; set; }

        public DateTime StartDateScheduler { get; set; }
        public DateTime EndDateScheduler { get; set; }
        public List<DataSchedulerRow> SchedulerRows { get; set; }
    }
    /*
    [Serializable()]
    public partial class DataSchedulerHeader
    {
        public int ColumnId { get; set; }
        public string ColumnName { get; set; }
    }*/
    [Serializable()]
    public partial class DataSchedulerRow
    {
        public Guid RowId { get; set; }
        public string RowName { get; set; }
        public DateTime StartTimeSlot { get; set; }
        public DateTime EndTimeSlot { get; set; }
        public List<DataSchedulerRowCell> DataSchedulerRowCell { get; set; }
    }
    [Serializable()]
    public partial class DataSchedulerRowCell
    {
        public Guid RowId { get; set; }
        public Guid ColumnId { get; set; }
        public string CellName { get; set; }
        public DateTime CellDate { get; set; }
        public int AvailableSlot { get; set; }
        public int OccupationASAP { get; set; }
        public int OccupationCSI { get; set; }
        public int OccupationBooking { get; set; }
        public int ConflictASAP { get; set; }
        public int ConflictCSI { get; set; }
        public int ConflictBooking { get; set; }
    }
   
}
