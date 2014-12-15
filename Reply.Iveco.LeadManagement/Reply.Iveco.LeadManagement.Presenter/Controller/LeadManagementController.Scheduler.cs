using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reply.Iveco.LeadManagement.Presenter.Model;
using System.Globalization;

namespace Reply.Iveco.LeadManagement.Presenter
{
    /// <summary>
    /// Gestione dei metodi per la creazione del calendario
    /// </summary>    
    public partial class LeadManagementController : BaseLeadManagementController, IDisposable
    {
        #region PUBLIC METHODS
        /// <summary>
        /// Restituisce lo scheduler del calendario amministratore
        /// </summary>
        /// <param name="countryName"></param>
        /// <param name="languageName"></param>
        /// <returns></returns>
        public DataScheduler GetAdministratorSchedulerByCountryAndLanguage(string countryName, string languageName)
        {
            return this.GetSchedulerByCountryAndLanguage(countryName, languageName, true);
        }
        /// <summary>
        /// Restituisce il calendario con le disponibilità dato il country e il language
        /// tutte le date e time sono da considerarsi orario di greenwitch
        /// </summary>
        /// <param name="countryName"></param>
        /// <param name="languageName"></param>
        /// <returns></returns>
        public DataScheduler GetSiteSchedulerByCountryAndLanguage(string countryName, string languageName)
        {
            return this.GetSchedulerByCountryAndLanguage(countryName, languageName, false);
        }
        /// <summary>
        /// Restituisce tutte le country 
        /// </summary>
        /// <returns></returns>
        public List<New_country> GetAllCountry()
        {
            return base.CurrentDataAccessLayer.GetCountries();
        }
        /// <summary>
        /// Restituisce tutte le language recensite nel sistema
        /// </summary>
        /// <returns></returns>
        public List<New_language> GetAllLanguage()
        {
            return base.CurrentDataAccessLayer.GetLanguages();
        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Crea il calendario della disponibilità differenziando se è un calendario per il sito o per amministratore
        /// </summary>
        /// <param name="countryName"></param>
        /// <param name="languageName"></param>
        /// <param name="isAdminScheduler"></param>
        /// <returns></returns>
        private DataScheduler GetSchedulerByCountryAndLanguage(string countryName, string languageName, bool isAdminScheduler)
        {
            List<List<Proposal>> allOperatorOccupationSlot = null;
            DateTime startDateScheduler = DateTime.MinValue;
            DateTime endDateScheduler = DateTime.MinValue;
            ///controllo i parametri (obligatorietà)
            //if (!this.CheckSetAppointmentParameter(callBackData, countryName, typeService, startDate, endDate))
            //    throw new InvalidInputParameterException();
            ///Recupero offset in base all'utente per fixare (debug / release) che lavorano con fusorari differenti
            int currentOffset = base.CurrentDataAccessLayer.GetOffsetDateByCurrentUser();
            ///Ottengo il country
            New_country country = base.CurrentDataAccessLayer.GetCountryByCountryName(countryName);
            ///Sollevo errore se country è null
            if (country == null) throw new CountryNotFoundException();
            ///Ottengo la lingua
            New_language language = base.CurrentDataAccessLayer.GetLanguageByLanguageName(languageName);
            ///Controllo che language sia valorizzato
            if (language == null) throw new LanguageNotFoundException();
            ///Recupero il tipo di servizio
            Service _service = base.CurrentDataAccessLayer.GetServiceByTypeAndLanguage(DataConstant.TypeService.BOOKING, language.New_languageId);
            ////Controllo se service è stato trovato
            if (_service == null) throw new ServiceTypeNotFoundException();
            ///Ottengo il service configurator
            New_servicesconfiguration serviceConfigurator = base.CurrentDataAccessLayer.GetServiceConfiguratorByIdLanguageAndServiceType(language.New_languageId, DataConstant.TypeService.BOOKING);
            ///Ottengo i dati per creare asse x / y de l caldendar
            ///Start/End Date Calendar
            ///Se lo scheduler è richiesto dall'amministratore allora:
            if (isAdminScheduler)
            {
                startDateScheduler = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                endDateScheduler = startDateScheduler.AddDays(country.New_BookingEndDays.Value+1);
            }
            else
            {
                startDateScheduler = DateTime.Now.Date.AddDays(country.New_BookingStartDays.Value);
                endDateScheduler = startDateScheduler.AddDays(country.New_BookingEndDays.Value);
            }
            //endDateScheduler = startDateScheduler.AddDays(country.New_BookingEndDays.Value);

            ///Recupero tutti gli operatori che lavorano su un determianto servizio
            List<Guid> operators = base.CurrentDataAccessLayer.GetOperatorsByIdService(_service.ServiceId);
            ///Istanzio la collection che conterrà le disponibilità di tutti gli operatori per il calendario
            allOperatorOccupationSlot = new List<List<Proposal>>(operators.Count);
            ///Per ogni idoperatore ciclo e recupero le sue disponibilità giorno per giorno.
            operators.ForEach(operatorId =>
            {
                ///Per ogni operatore prendo la sua occupazione per tutto il calendario
                var ret = base.CurrentDataAccessLayer.FindFreeSlotOperatorsByIdOperator(operatorId, _service, language, string.Empty, startDateScheduler, endDateScheduler, currentOffset, 1000000);
                if(ret != null)
                    allOperatorOccupationSlot.Add(ret);
            });
            ///Creo il calendario e restituisco l'oggetto che contiene tutte le info da passare 
            return this.CreateScheduler(language.New_languageId, allOperatorOccupationSlot, country, serviceConfigurator, language, startDateScheduler, endDateScheduler,isAdminScheduler);
        }

        /// <summary>
        /// Crea l'oggetto di ritorno del calendario andando a controllare tutte le disponibilità
        /// </summary>
        /// <param name="allOperatorOccupationSlot"></param>
        /// <param name="country"></param>
        /// <param name="serviceConfigurator"></param>
        /// <param name="language"></param>
        /// <param name="startScheduler"></param>
        /// <param name="endScheduler"></param>
        /// <returns></returns>
        private DataScheduler CreateScheduler(Guid languageId,List<List<Proposal>> allOperatorOccupationSlot, New_country country, New_servicesconfiguration serviceConfigurator, New_language language, DateTime startScheduler, DateTime endScheduler,bool isAdminScheduler)
        {
            List<BusySlot> busySlot = null;
            Guid rowId = Guid.Empty;
            ///Oggetto da restituire 
            DataScheduler scheduler = new DataScheduler();
            ///Data inizio calendar
            scheduler.StartDateScheduler = startScheduler;
            ///Data fine calendar
            scheduler.EndDateScheduler = endScheduler;
            
            ///Totale Righr
            TimeSpan spanHour = language.New_BookingEndHour.Value - language.New_BookingStartHour.Value;
            scheduler.SchedulerRows = new List<DataSchedulerRow>(spanHour.Hours);

            ///Estraggo gli slot occupati solo se è uno scheduler per l'amministratore
            if(isAdminScheduler)
               busySlot =  base.CurrentDataAccessLayer.GetServiceAppointmentBusyByRangeAndLanguageId(new DateTime(scheduler.StartDateScheduler.Year, scheduler.StartDateScheduler.Month, scheduler.StartDateScheduler.Day, 0, 0, 0),
                new DateTime(scheduler.EndDateScheduler.Year, scheduler.EndDateScheduler.Month, scheduler.EndDateScheduler.Day, 23, 59, 59),
                languageId);
            
            ///Ciclo per le righe
            for (int i = 0; i < (spanHour.Hours); i+= language.New_SlotDuration.Value)
            {
                rowId = Guid.Empty;
                rowId = Guid.NewGuid();
                scheduler.SchedulerRows.Add(new DataSchedulerRow()
                {
                    RowId = rowId,
                    StartTimeSlot = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,language.New_BookingStartHour.Value.AddHours(i).Hour,language.New_BookingStartHour.Value.Minute,language.New_BookingStartHour.Value.Second),
                    EndTimeSlot = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, language.New_BookingStartHour.Value.AddHours(i + language.New_SlotDuration.Value).Hour, language.New_BookingStartHour.Value.Minute, language.New_BookingStartHour.Value.Second),
                    RowName = string.Format(CultureInfo.InvariantCulture, "{0}:{1} - {2}:{3}", language.New_BookingStartHour.Value.AddHours(i).Hour.ToString(CultureInfo.InvariantCulture), language.New_BookingStartHour.Value.AddHours(i).ToString("mm"), language.New_BookingStartHour.Value.AddHours(i + language.New_SlotDuration.Value).Hour.ToString(CultureInfo.InvariantCulture), language.New_BookingStartHour.Value.ToString("mm")),
                    DataSchedulerRowCell = this.GetSchedulerRowCellByParameters(
                                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, language.New_BookingStartHour.Value.AddHours(i).Hour, language.New_BookingStartHour.Value.Minute, language.New_BookingStartHour.Value.Second),
                                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, language.New_BookingStartHour.Value.AddHours(i + language.New_SlotDuration.Value).Hour, language.New_BookingStartHour.Value.Minute, language.New_BookingStartHour.Value.Second),
                                    scheduler.StartDateScheduler,
                                    scheduler.EndDateScheduler,
                                    allOperatorOccupationSlot,
                                    rowId, 
                                    busySlot)
                });
            }
            return scheduler;
        }
        /// <summary>
        /// Restituisce la lista di celle del per una data riga con i dettagli di disponibilità
        /// </summary>
        /// <param name="startHour"></param>
        /// <param name="endHour"></param>
        /// <param name="startDay"></param>
        /// <param name="endDay"></param>
        /// <param name="allOperatorOccupationSlot"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        private List<DataSchedulerRowCell> GetSchedulerRowCellByParameters(DateTime startHour, DateTime endHour, DateTime startDay, DateTime endDay, List<List<Proposal>> allOperatorOccupationSlot, Guid rowId, List<BusySlot> busySlot)
        {
            TimeSpan spanDay = endDay - startDay;
            List<DataSchedulerRowCell> cells = new List<DataSchedulerRowCell>(spanDay.Days);
            int disp = 0;
            for (int i = 0; i <= spanDay.Days; i++)
            {
                disp = 0;
                ///Ottiene il numero di slot liberi
                disp = this.GetAvailability(startHour, endHour, startDay.AddDays(i), allOperatorOccupationSlot);
                ///popolo la singola cella
                DateTime slotStartDate = new DateTime(startDay.AddDays(i).Year, startDay.AddDays(i).Month, startDay.AddDays(i).Day, startHour.Hour, startHour.Minute, 0);
                DateTime slotEndDate = new DateTime(startDay.AddDays(i).Year, startDay.AddDays(i).Month, startDay.AddDays(i).Day, endHour.Hour, endHour.Minute, 0);
                cells.Add(new DataSchedulerRowCell()
                {
                    CellDate = startDay.AddDays(i),
                    CellName = string.Format(CultureInfo.InvariantCulture, "slot {0}", disp.ToString(CultureInfo.InvariantCulture)),
                    ColumnId = Guid.NewGuid(),                    
                    RowId = rowId,
                    AvailableSlot = disp,
                    OccupationASAP = (busySlot == null) ? 0 : busySlot.Where(c => c.ServiceType == (int)DataConstant.TypeService.ASAP && c.DateBusySlot > slotStartDate && c.DateBusySlot <= slotEndDate).Sum(c => c.CountBusy),
                    OccupationBooking = (busySlot == null) ? 0 : busySlot.Where(c => c.ServiceType == (int)DataConstant.TypeService.BOOKING && c.DateBusySlot > slotStartDate && c.DateBusySlot <= slotEndDate).Sum(c => c.CountBusy),
                    OccupationCSI = (busySlot == null) ? 0 : busySlot.Where(c => c.ServiceType == (int)DataConstant.TypeService.CSI && c.DateBusySlot > slotStartDate && c.DateBusySlot <= slotEndDate).Sum(c => c.CountBusy),

                    ConflictASAP = (busySlot == null) ? 0 : busySlot.Where(c => c.ServiceType == (int)DataConstant.TypeService.ASAP && c.DateBusySlot > slotStartDate && c.DateBusySlot <= slotEndDate).Sum(c => c.CountConflict),
                    ConflictBooking = (busySlot == null) ? 0 : busySlot.Where(c => c.ServiceType == (int)DataConstant.TypeService.BOOKING && c.DateBusySlot > slotStartDate && c.DateBusySlot <= slotEndDate).Sum(c => c.CountConflict),
                    ConflictCSI = (busySlot == null) ? 0 : busySlot.Where(c => c.ServiceType == (int)DataConstant.TypeService.CSI && c.DateBusySlot > slotStartDate && c.DateBusySlot <= slotEndDate).Sum(c => c.CountConflict)
                });
                var ret = (busySlot == null) ? 0 : busySlot.Where(c => c.ServiceType == (int)DataConstant.TypeService.CSI && c.DateBusySlot > slotStartDate && c.DateBusySlot <= slotEndDate).Sum(c => c.CountBusy);
            }
            return cells;
        }
        /// <summary>
        /// Indica per una data cella la disponibilità degli operatori
        /// </summary>
        /// <param name="startHour"></param>
        /// <param name="endHour"></param>
        /// <param name="date"></param>
        /// <param name="allOperatorOccupationSlot"></param>
        /// <returns></returns>
        private int GetAvailability(DateTime startHour, DateTime endHour, DateTime date, List<List<Proposal>> allOperatorOccupationSlot)
        {
            int availability = 0;
            DateTime startTime = new DateTime(date.Year, date.Month, date.Day, startHour.Hour, startHour.Minute, 0);
            DateTime endTime = new DateTime(date.Year, date.Month, date.Day, endHour.Hour, endHour.Minute, 0);
            allOperatorOccupationSlot.ForEach(op =>
                {
                    availability += op.Where(c => c.StartSlotUniversalTime >= startTime && c.StartSlotUniversalTime < endTime).Count();
                });

            return availability;
        }       
        #endregion
    }
}
