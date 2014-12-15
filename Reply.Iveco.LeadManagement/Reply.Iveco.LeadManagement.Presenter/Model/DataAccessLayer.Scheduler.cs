using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Data;
using System.Data.Linq;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Crm.Sdk;
using System.Web.Services.Protocols;
using Microsoft.Crm.Sdk.Query;

namespace Reply.Iveco.LeadManagement.Presenter.Model
{
    public partial class DataAccessLayer : BaseDataAccessLayer, IDisposable
    {
        #region PUBLIC METHODS
        /// <summary>
        /// Restituisce tutte le country censite
        /// </summary>
        /// <returns></returns>
        public List<New_country> GetCountries()
        {
            return base.CurrentDataContext.New_countries.Where(c => c.DeletionStateCode == 0).OrderBy(c => c.New_name).ToList();
        }
        /// <summary>
        /// Restituisce tutti i language censiti
        /// </summary>
        /// <returns></returns>
        public List<New_language> GetLanguages()
        {
            return base.CurrentDataContext.New_languages.Where(c => c.DeletionStateCode == 0).OrderBy(c => c.New_name).ToList();
        }
        /// <summary>
        /// Ottiene il service configurator dato il id language e il type service
        /// </summary>
        /// <param name="idLanguage"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public New_servicesconfiguration GetServiceConfiguratorByIdLanguageAndServiceType(Guid idLanguage,DataConstant.TypeService serviceType)            
        {
            return (from serConf in base.CurrentDataContext.New_servicesconfigurations
                    where serConf.New_LanguageId == idLanguage &&    
                    serConf.New_ServiceType == (int)DataConstant.TypeService.BOOKING &&
                    serConf.DeletionStateCode == 0
                    select serConf).SingleOrDefault();

        }

        /// <summary>
        /// Ottiene i dettagli per disegnare lo scheduler
        /// </summary>
        /// <param name="country"></param>
        /// <param name="language"></param>
        public DataSchedulerModel GetDataSchedulerByCountryAndLanguage(string country, string language)
        {
            try
            {
                var dataSchedulerModel = (from serConf in base.CurrentDataContext.New_servicesconfigurations
                        join lang in base.CurrentDataContext.New_languages on serConf.New_LanguageId equals lang.New_languageId
                        join ser in base.CurrentDataContext.Services on serConf.New_ServiceId equals ser.ServiceId
                        where serConf.DeletionStateCode.Value == 0 &&
                        serConf.New_ServiceType.Value == (int)DataConstant.TypeService.BOOKING &&                        
                        lang.New_name.ToUpper() == language.ToUpper()
                        select new DataSchedulerModel
                        {
                            ServiceConfigurationId = serConf.New_servicesconfigurationId,
                            LanguageId = lang.New_languageId,
                            ServiceId = ser.ServiceId,
                            AfternoonEndSlot = DateTime.Now.AddMinutes(15),//lang.New_AfternoonEndSlot,
                            AfternoonStartSlot = DateTime.Now,//lang.New_AfternoonStartSlot,
                            AnchorOffset = ser.AnchorOffset,
                            ASAPEndDays = 0,//lang.New_ASAPEndDays,
                            BookingEndDays = 0,//lang.New_BookingEndDays,
                            BookingStartDays = 0,//lang.New_BookingStartDays,
                            Duration = ser.Duration,//non utile
                            Granularity = ser.Granularity,//non utile
                            IsSchedulable = ser.IsSchedulable,//non utile
                            MorningEndSlot = DateTime.Now.AddMinutes(15),//lang.New_MorningEndSlot,
                            MorningStartSlot = DateTime.Now,//lang.New_MorningStartSlot,
                            ServiceConfigurationName = serConf.New_name,
                            SlotDurationHours = lang.New_SlotDuration //non utile
                        }).SingleOrDefault();
                ///Aggiungo i valori ora presi dalla country
                New_country newCountry = this.GetCountryByCountryName(country);
                dataSchedulerModel.ASAPEndDays = newCountry.New_ASAPEndDays;//non utile
                dataSchedulerModel.BookingEndDays = newCountry.New_BookingEndDays;
                dataSchedulerModel.BookingStartDays = newCountry.New_BookingStartDays;
                return dataSchedulerModel;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Restituisce la lista di tutti gli operatori assegnati ad un servizio
        /// </summary>
        /// <param name="idService"></param>
        /// <returns></returns>
        public List<Guid> GetOperatorsByIdService(Guid idService)
        {
            ///Recupero il Resource Group
            ///Se il Resource Group e di tipo 2 allora recupero XML che mi dà l'id dell'altro ResourceGroup
            var resourceGroup = (from ser in base.CurrentDataContext.Services
                                 join rs in base.CurrentDataContext.ResourceSpecs
                                 on ser.ResourceSpecId equals rs.ResourceSpecId
                                 join rg in base.CurrentDataContext.ResourceGroups
                                 on rs.GroupObjectId equals rg.ResourceGroupId
                                 where ser.ServiceId == idService && ser.DeletionStateCode == 0
                                 && rs.DeletionStateCode == 0 && rg.DeletionStateCode == 0
                                 select rg).SingleOrDefault();
            ///Nessun resourcegroup non trovato
            if (resourceGroup == null)
                throw new Exception("ResourceGroup not found");
            ///Controllo lo group type code
            if (resourceGroup.GroupTypeCode == (int)DataConstant.ResourceGroupTypeCode.Hidden)
            {
                ///Recupero il valore xml che contiene id del resource group
                string constraints = this.GetConstrainctByResourceGroupId(resourceGroup.ResourceGroupId);
                ///Recupero id del resource group
                Guid idResourceGroup = this.GetIdByConstraintsClasses(constraints.DeserializeXmlToClass<Constraints>())[0];
                ///Recupero il resource Group
                resourceGroup = this.GetResourceGroupById(idResourceGroup);
            }
            ///Recupero Xml che mi dà id degli utenti
            string idOperatorsString = this.GetConstrainctByResourceGroupId(resourceGroup.ResourceGroupId);
            ///Lista di tutti gli operatori in un servizio
            return this.GetIdByConstraintsClasses(idOperatorsString.DeserializeXmlToClass<Constraints>());
        }
        /// <summary>
        /// Ottiene tutti i service appointment occupati per tutti i servizi in base al range di data e il languageid
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        public List<BusySlot> GetServiceAppointmentBusyByRangeAndLanguageId(DateTime startDate, DateTime endDate, Guid languageId)
        {
            //return (from ServiceAppointment sapp in base.CurrentDataContext.ServiceAppointments
            //        where sapp.DeletionStateCode == 0 &&
            //        sapp.New_LanguageId == languageId &&
            //        sapp.StateCode != (int)ServiceAppointmentState.Open &&
            //        sapp.StateCode != (int)ServiceAppointmentState.Canceled &&
            //        sapp.ScheduledStart >= startDate &&
            //        sapp.ScheduledEnd <= endDate
            //        group sapp by new { Data = sapp.ScheduledEnd.Value, ServiceType = sapp.New_ServiceType }
            //            into g
            //            select new BusySlot
            //           {
            //               DateBusySlot = g.Key.Data,
            //               ServiceType = g.Key.ServiceType.Value,
            //               CountBusy = g.Select(c => c.ActivityId).Count()
            //           }).ToList();


            //var test = (from ServiceAppointment sapp in base.CurrentDataContext.ServiceAppointments
            //            where sapp.DeletionStateCode == 0 &&
            //            sapp.New_LanguageId == languageId &&
            //            (sapp.StateCode == (int)ServiceAppointmentState.Closed ||
            //            sapp.StateCode == (int)ServiceAppointmentState.Scheduled) &&
            //            sapp.ScheduledStart >= startDate &&
            //            sapp.ScheduledEnd <= endDate
            //            group sapp by new { Data = sapp.ScheduledEnd.Value, ServiceType = sapp.New_ServiceType,activity = sapp.ActivityId }
            //                into g
            //                select new
            //                {
            //                    data = g.Key.Data,
            //                    ServiceType = g.Key.ServiceType.Value,
            //                    CountBusy = g.Select(c => c.ActivityId).Distinct().Count(),
            //                    Att = g.Key.activity
            //                }).OrderBy(c => c.data).ToList();


            ValidateRequest request = new ValidateRequest();
            QueryExpression _queryAppt = new QueryExpression(EntityName.serviceappointment.ToString());
            _queryAppt.ColumnSet = new ColumnSet(new string[] { "activityid", "serviceid", "scheduledstart", "scheduledend", "resources", "new_servicetype" });
            _queryAppt.Criteria = new FilterExpression() { FilterOperator = LogicalOperator.And };
            _queryAppt.Criteria.AddCondition("new_languageid", ConditionOperator.Equal, new object[] { languageId });
            _queryAppt.Criteria.AddCondition("statecode", ConditionOperator.NotEqual, new object[] { (int)ServiceAppointmentState.Open });
            _queryAppt.Criteria.AddCondition("statecode", ConditionOperator.NotEqual, new object[] { (int)ServiceAppointmentState.Canceled });
            _queryAppt.Criteria.AddCondition("scheduledstart", ConditionOperator.GreaterEqual, new object[] { startDate.ToLocalTime().ToString("s") });
            _queryAppt.Criteria.AddCondition("scheduledend", ConditionOperator.LessEqual, new object[] { endDate.ToLocalTime().ToString("s") });
            RetrieveMultipleRequest _req = new RetrieveMultipleRequest()
            {
                Query = _queryAppt,
                ReturnDynamicEntities = true
            };
            ///Recupero tutti i service appointment coinvolti nel range passato come dynamic entity
            var servAppointDynamcic = ((RetrieveMultipleResponse)base.CurrentCrmService.Execute(_req)).BusinessEntityCollection.BusinessEntities;
            ///Creo la richiesta di validazione per i service appointment coinvolti
            request.Activities = new Microsoft.Crm.Sdk.BusinessEntityCollection()
            {
                EntityName = EntityName.serviceappointment.ToString(),
                BusinessEntities = servAppointDynamcic 
            };
            ///Eseguo la varidate
            ValidateResponse validated = (ValidateResponse)base.CurrentCrmService.Execute(request);
            ///Faccio il merge delle due liste
            var mergedServiceAppointment = (from DynamicEntity serv in servAppointDynamcic
                            join ValidationResult val in validated.Result
                              on ((Key)serv["activityid"]).Value equals val.ActivityId
                            select new ValidatedServiceAppointment()
                            {
                                ActivityId = ((Key)serv["activityid"]).Value,
                                DateScheduledEnd = Convert.ToDateTime(((CrmDateTime)serv["scheduledend"]).Value).ToUniversalTime(),
                                IsValid = val.ValidationSuccess,
                                ServiceId = ((Lookup)serv["serviceid"]).Value,
                                ServiceTypeCode = ((Picklist)serv["new_servicetype"]).Value
                            }).ToList();

            ///Ritorno la lista degli slot pieni
            return (from sapp in mergedServiceAppointment
                    group sapp by new { Data = sapp.DateScheduledEnd, ServiceType = sapp.ServiceTypeCode }
                        into g
                        select new BusySlot
                        {
                            DateBusySlot = g.Key.Data,
                            ServiceType = g.Key.ServiceType,
                            CountBusy = g.Select(c => c.ActivityId).Distinct().Count(),
                            CountConflict = g.Where(c => !c.IsValid).Count()
                        }).ToList();
        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Ottiene il resource group dato il suo id
        /// </summary>
        /// <param name="idResourceGroup"></param>
        /// <returns></returns>
        private ResourceGroup GetResourceGroupById(Guid idResourceGroup)
        {
            return base.CurrentDataContext.ResourceGroups.Where(c => c.DeletionStateCode == 0 && c.ResourceGroupId == idResourceGroup).SingleOrDefault();
        }
        /// <summary>
        /// Ritorna il valore della constraict dato il resourcegroup id
        /// </summary>
        /// <param name="resourceGroupId"></param>
        /// <returns></returns>
        private string GetConstrainctByResourceGroupId(Guid resourceGroupId)
        {
            return base.CurrentDataContext.ConstraintBasedGroups.Where(c => c.DeletionStateCode == 0 && c.ConstraintBasedGroupId == resourceGroupId).Select(c => c.Constraints).SingleOrDefault();
        }
        /// <summary>
        /// Legge l'oggetto constraints e ritorna le guid contenute
        /// </summary>
        /// <param name="constraints"></param>
        /// <returns></returns>
        private List<Guid> GetIdByConstraintsClasses(Constraints constraints)
        {
            if (constraints == null || constraints.Constraint == null || constraints.Constraint.Expression == null || string.IsNullOrEmpty(constraints.Constraint.Expression.Body))
                return null;
            List<Guid> ret = new List<Guid>(5);
            constraints.Constraint.Expression.Body = constraints.Constraint.Expression.Body.Replace("resource[\"Id\"]", string.Empty);
            constraints.Constraint.Expression.Body = constraints.Constraint.Expression.Body.Replace("==", string.Empty);
            constraints.Constraint.Expression.Body = constraints.Constraint.Expression.Body.Replace("||", string.Empty);
            constraints.Constraint.Expression.Body = constraints.Constraint.Expression.Body.Trim();
            string[] arr = constraints.Constraint.Expression.Body.Split('{');
            foreach (var item in arr)
            {
                if (string.IsNullOrEmpty(item))
                    continue;
                ret.Add(new Guid(item.Replace("{", string.Empty).Replace("}", string.Empty).Trim()));
            }
            ///Ritorno
            return ret;
        }
        #endregion
    }
    //public class DistinctTitle : IEqualityComparer<SourceType>
    //{
    //    public bool Equals(SourceType x, SourceType y)
    //    {
    //        return x.title.Equals(y.title);
    //    }

    //    public int GetHashCode(SourceType obj)
    //    {
    //        return obj.title.GetHashCode();
    //    }
    //}


}
