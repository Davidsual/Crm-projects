using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Reply.Iveco.LeadManagement.Presenter;
using System.Web.UI.HtmlControls;
using System.Threading;
using System.Globalization;

namespace Reply.Iveco.LeadManagement.Web.UI
{
    /// <summary>
    /// Schedulatore
    /// </summary>
    public partial class Scheduler : BasePageUserControl
    {
        #region PRIVATE MEMBERS

        #endregion

        #region PUBLIC MEMBERS
        //public delegate void DelegateOperationCompleted();
        //public event DelegateOperationCompleted OnOperationComplete;
        #endregion

        #region PUBLIC PROPERTY
        public int CurrentColumnCount
        {
            get 
            { 
                if(this.CurrentDataScheduler != null)
                {
                    TimeSpan spanDay = this.CurrentDataScheduler.EndDateScheduler - this.CurrentDataScheduler.StartDateScheduler;
                    return spanDay.Days+1;
                }
                return  0; 
            }
        }
        #endregion

        #region PRIVATE PROPERTY
        private DataScheduler CurrentDataScheduler
        {
            get
            {
                return ViewState["CurrentDataScheduler"] as DataScheduler;
            }
            set
            {
                ViewState["CurrentDataScheduler"] = value;
            }
        }
        #endregion

        #region EVENTS
        /// <summary>
        /// Caricamento user control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ///Primo caricamento della pagina
            }
            else
            {
                ///Rilevo il click sul caledandario
                //if (HttpContext.Current.Request.Params["__EVENTTARGET"].Contains("tbCell"))
                //{
                //    string _id = HttpContext.Current.Request.Params["__EVENTARGUMENT"].ToString();
                //    ///TODO:
                //    ///Da cancellare e rivedere
                //    Thread.Sleep(1000);
                //    var _test = this.CurrentDataScheduler.LstRow.Where(c => c.RowId == Convert.ToInt32(_id.Split('$')[1].ToString())).SingleOrDefault().LstDataSchedulerRowCell.Where(cell => cell.ColumnId == Convert.ToInt32(_id.Split('$')[0].ToString()) &&
                //                 cell.RowId == Convert.ToInt32(_id.Split('$')[1].ToString())).SingleOrDefault();
                //    _test.IsSelected = true;
                //    _test.CellName = "Prenotato";
                //    this.rpdCalendar.DataSource = this.CurrentDataScheduler.LstRow;
                //    this.rpdCalendar.DataBind();
                //    ///Sollevo evento di operazione completata
                //    if (this.OnOperationComplete != null)
                //        this.OnOperationComplete();
                //}
            }

        }
        /// <summary>
        /// Evento di creazione della singola riga
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rpdCalendar_ItemDataBound(object sender, RepeaterItemEventArgs rpd)
        {
            if (rpd.Item.ItemType == ListItemType.Header)
            {
                ///Genero Header 
                Repeater _rptHeader = rpd.Item.FindControl("rpdCalendarHeader") as Repeater;
                
                if (_rptHeader != null)
                {
                    TimeSpan span = this.CurrentDataScheduler.EndDateScheduler.Date - this.CurrentDataScheduler.StartDateScheduler.Date;
                    List<DateTime> _lst = new List<DateTime>(span.Days);
                    for (int i = 0; i <= span.Days; i++)
                    {
                        _lst.Add(this.CurrentDataScheduler.StartDateScheduler.Date.AddDays(i));
                    }
                    _rptHeader.DataSource = _lst;
                    _rptHeader.DataBind();
                }

            }
            if (rpd.Item.ItemType == ListItemType.Item || rpd.Item.ItemType == ListItemType.AlternatingItem)
            {
                //<%# ((Reply.Iveco.LeadManagement.Presenter.DataSchedulerRow)Container.DataItem).RowName%>
                ///Aggiungo il nome dello slot es: 07:00 / 08:00
                var lblRowSlotName = rpd.Item.FindControl("lblRowSlotDescr") as Label;

                if(lblRowSlotName != null)
                {
                    //string start = 
                    string start = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ((DataSchedulerRow)rpd.Item.DataItem).StartTimeSlot.AddHours(2).Hour.ToString(CultureInfo.InvariantCulture), ((DataSchedulerRow)rpd.Item.DataItem).StartTimeSlot.ToString("mm"));
                    string end = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", ((DataSchedulerRow)rpd.Item.DataItem).EndTimeSlot.AddHours(2).Hour.ToString(CultureInfo.InvariantCulture), ((DataSchedulerRow)rpd.Item.DataItem).EndTimeSlot.ToString("mm")); ;
                    lblRowSlotName.Text = string.Format(CultureInfo.InvariantCulture, "{0} - {1}", start, end);
                }

                ///Attacco il datasource alle celle della riga
                Repeater rep = rpd.Item.FindControl("rpdCalendarRow") as Repeater;
                rep.DataSource = ((DataSchedulerRow)rpd.Item.DataItem).DataSchedulerRowCell.OrderBy(c => c.CellDate).ToList();
                rep.DataBind();
            }
        }
        /// <summary>
        /// Creazione riga del calendario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rpdCalendarRow_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var _cell = e.Item.FindControl("tbCell") as HtmlTableCell;
                var lblOccupationAsap = e.Item.FindControl("lblOccupationAsap") as Label;
                var lblOccupationCSI = e.Item.FindControl("lblOccupationCSI") as Label;
                var lblOccupationBook = e.Item.FindControl("lblOccupationBook") as Label;

                ///Popolo i dettagli dell'occupazione/slot
                if (lblOccupationAsap != null && lblOccupationCSI != null && lblOccupationBook != null)
                {
                    var data = e.Item.DataItem as DataSchedulerRowCell;
                    if(data.ConflictASAP > 0)
                        lblOccupationAsap.Text = string.Format(CultureInfo.InvariantCulture,"{0} / {1}",data.OccupationASAP,data.ConflictASAP);
                    else
                        lblOccupationAsap.Text = string.Format(CultureInfo.InvariantCulture, "{0}", data.OccupationASAP);

                    if (data.ConflictCSI > 0)
                        lblOccupationCSI.Text = string.Format(CultureInfo.InvariantCulture, "{0} / {1}", data.OccupationCSI, data.ConflictCSI);
                    else
                        lblOccupationCSI.Text = string.Format(CultureInfo.InvariantCulture, "{0}", data.OccupationCSI);

                    if (data.ConflictBooking > 0)
                        lblOccupationBook.Text = string.Format(CultureInfo.InvariantCulture, "{0} / {1}", data.OccupationBooking, data.ConflictBooking);
                    else
                        lblOccupationBook.Text = string.Format(CultureInfo.InvariantCulture, "{0}", data.OccupationBooking);

                }

                if (_cell != null)
                {
                    if (((DataSchedulerRowCell)e.Item.DataItem).AvailableSlot == 0 &&
                        ((DataSchedulerRowCell)e.Item.DataItem).OccupationASAP == 0 &&
                        ((DataSchedulerRowCell)e.Item.DataItem).OccupationBooking == 0 &&
                        ((DataSchedulerRowCell)e.Item.DataItem).OccupationCSI == 0)
                    {
                        _cell.Style.Add("background-color", "#B0B0B0"); //grigio
                    }
                    else if (((DataSchedulerRowCell)e.Item.DataItem).AvailableSlot == 0
                        &&
                        (((DataSchedulerRowCell)e.Item.DataItem).OccupationASAP > 0 ||
                        ((DataSchedulerRowCell)e.Item.DataItem).OccupationBooking > 0 ||
                        ((DataSchedulerRowCell)e.Item.DataItem).OccupationCSI > 0)
                        &&
                        ((((DataSchedulerRowCell)e.Item.DataItem).ConflictASAP > 0 ||
                        ((DataSchedulerRowCell)e.Item.DataItem).ConflictBooking > 0 ||
                        ((DataSchedulerRowCell)e.Item.DataItem).ConflictCSI > 0)))
                    {
                        _cell.Style.Add("background-color", "#FF3333"); ///rosso
                    }
                    else if (((DataSchedulerRowCell)e.Item.DataItem).AvailableSlot == 0
                        &&
                        (((DataSchedulerRowCell)e.Item.DataItem).OccupationASAP > 0 ||
                        ((DataSchedulerRowCell)e.Item.DataItem).OccupationBooking > 0 ||
                        ((DataSchedulerRowCell)e.Item.DataItem).OccupationCSI > 0)
                        &&
                        ((((DataSchedulerRowCell)e.Item.DataItem).ConflictASAP == 0 &&
                        ((DataSchedulerRowCell)e.Item.DataItem).ConflictBooking == 0 &&
                        ((DataSchedulerRowCell)e.Item.DataItem).ConflictCSI == 0)))
                    {
                        _cell.Style.Add("background-color", "#FFFF33"); ///giallo
                    }
                    else if (((DataSchedulerRowCell)e.Item.DataItem).AvailableSlot > 0
                        &&
                        ((((DataSchedulerRowCell)e.Item.DataItem).ConflictASAP == 0 &&
                        ((DataSchedulerRowCell)e.Item.DataItem).ConflictBooking == 0 &&
                        ((DataSchedulerRowCell)e.Item.DataItem).ConflictCSI == 0)))
                    {
                        _cell.Style.Add("background-color", "#06ce65"); /// verde
                    }
                    else if (((DataSchedulerRowCell)e.Item.DataItem).AvailableSlot > 0
                        &&
                        ((((DataSchedulerRowCell)e.Item.DataItem).ConflictASAP > 0 ||
                        ((DataSchedulerRowCell)e.Item.DataItem).ConflictBooking > 0 ||
                        ((DataSchedulerRowCell)e.Item.DataItem).ConflictCSI > 0)))
                    {
                        _cell.Style.Add("background-color", "#00CCFF"); /// BLU
                    }
                    else
                    {

                    }
                }
            }
        }
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Carica lo scheduler
        /// </summary>
        public void LoadScheduler(string organizationName,string country,string language)
        {
            try
            {
                //Setto i parametri nel viewstate del controllo
                this.CurrentOrganizationName = organizationName;
                this.CountryParameter = country;
                this.LanguageParameter = language;
                ///Carico il calendario
                this.CurrentDataScheduler = base.CurrentController.GetAdministratorSchedulerByCountryAndLanguage(this.CountryParameter, this.LanguageParameter); //this.GetFakeData();
                this.rpdCalendar.DataSource = this.CurrentDataScheduler.SchedulerRows.OrderBy(c => c.StartTimeSlot.Hour).OrderBy(c => c.StartTimeSlot.Minute).ToList();
                this.rpdCalendar.DataBind();
            }
            catch(Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "alert('Language or Country not supported!');", true);
            }
        }
        #endregion

        #region PRIVATE METHODS

        #endregion
    }

}