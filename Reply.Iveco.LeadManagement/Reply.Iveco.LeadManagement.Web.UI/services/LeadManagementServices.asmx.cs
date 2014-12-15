using System;
using System.Web;
using System.Web.Services;
using Reply.Iveco.LeadManagement.Presenter;
using Reply.Iveco.LeadManagement.Presenter.Model;
using System.Web.Services.Protocols;
using System.Globalization;
using System.Configuration;

namespace Reply.Iveco.LeadManagement.Web.UI
{
    /// <summary>
    /// Classe che espone a servizi i metodi per la gestione del booking
    /// </summary>
    [WebService(Namespace = "_http://schemas.microsoft.com/crm/2006/WebServices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class LeadManagementServices : System.Web.Services.WebService
    {

        #region PRIVATE MEMBERS
        private const string USERNAME_IVECO1 = "HOPLO1";
        private const string USERNAME_IVECO2 = "IVECOCOM1";
        private const string USERNAME_IVECO3 = "DYNAMICFUN1";
        private const string PASSWORD_IVECO = "V2SU6KBP";
        private const string PASSWORD_IVECO3 = "V4DUtKBD";
        #endregion

        #region PUBLIC MEMBERS
        public HeaderAuthentication headerAuthentication; 
        #endregion

        #region PRIVATE MEMBERS
        private const string ORGANIZATION_NAME_LEADMANAGEMENT = DataConstant.ORGANIZATION_NAME_TEST;
        #endregion

        #region WEB METHODS
        /// <summary>
        /// Inserisce un appuntamento di tipo ASAP o BOOKING con tutti i dati che arrivano da sito
        /// </summary>
        /// <param name="typeService"></param>
        /// <param name="callbackData"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>       
        [WebMethod]
        [SoapHeader("headerAuthentication", Direction = SoapHeaderDirection.In)] 
        public SetAppointmentResult SetAppointment(string serviceType, Reply.Iveco.LeadManagement.Presenter.CallBackData callbackData, DateTime startDate, DateTime endDate)
        {
            
            DataConstant.TypeService typeService;
            SetAppointmentResult result = new SetAppointmentResult()
                {
                    IsSuccessful = true,
                    ErrorCode = ExceptionConstant.SUCCESSFUL_OPERATION_CODE,
                    ErrorDescription = ExceptionConstant.SUCCESSFUL_OPERATION_DESC
                };

            try
            {
                ///Controllo autorizzazioni
                if (!CheckHeaderAuthorization(headerAuthentication))
                    throw new WsNotAutorizedUserException();

                string _orgname = string.Empty;

#if DEBUG
                _orgname = ORGANIZATION_NAME_LEADMANAGEMENT;
#else
                //var org = HttpContext.Current.Request.PhysicalPath.Substring(0, HttpContext.Current.Request.PhysicalPath.IndexOf(@"\services"));
                //org = org.ToLower().Substring(org.LastIndexOf(@"\") + 1).Replace("custom", string.Empty);
                //_orgname = org;
                _orgname = ConfigurationManager.AppSettings["CurrentOrganizationName"];
#endif

                ///Contollo parametri in ingresso
                if (string.IsNullOrEmpty(serviceType) || callbackData == null)
                    throw new InvalidInputParameterException();

                ///IDENTIFICO IN MANIERA ROBUSTA IL TIPO DI SERVIZIO
                if (serviceType.ToUpperInvariant().Trim() == DataConstant.TypeService.ASAP.ToString().ToUpperInvariant())
                    typeService = DataConstant.TypeService.ASAP;
                else if (serviceType.ToUpperInvariant().Trim() == DataConstant.TypeService.BOOKING.ToString().ToUpperInvariant())
                    typeService = DataConstant.TypeService.BOOKING;
                else
                    typeService = DataConstant.TypeService.CSI;

                ///Bonifica dei maledetti null
                callbackData = SetDefaultValues(callbackData);
                ///IN BASE AL TIPO DI SERVIZIO TROVATO GESTISCO LE CHIAMATE AL CONTROLLER
                switch (typeService)
                {
                    case DataConstant.TypeService.ASAP:
                        ///Set appointment asap
                        using (LeadManagementController controller = new LeadManagementController(_orgname, HttpContext.Current))
                            controller.SetAppointmentAsap(callbackData);

                        break;
                    case DataConstant.TypeService.BOOKING:
                        ///Set appointment booking
                        using (LeadManagementController controller = new LeadManagementController(_orgname, HttpContext.Current))
                            controller.SetAppointmentBooking(callbackData, startDate, endDate);

                        break;
                    case DataConstant.TypeService.CSI:
                        result.IsSuccessful = false;
                        result.ErrorDescription = ExceptionConstant.SERVICETYPE_NOT_SUPPORTED_DESC;
                        result.ErrorCode = ExceptionConstant.SERVICETYPE_NOT_SUPPORTED_CODE;
                        break;
                    default:
                        break;
                }
                ///Ritorno il risultato
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;

                if (ex is ICustomException)
                {
                    var exception = ((ICustomException)ex);
                    result.ErrorDescription = exception.Descr;
                    result.ErrorCode = exception.Code;
                }
                else
                {
                    //result.ErrorDescription = ex.Message + " " + ex.StackTrace;
                    //if(ex.InnerException != null)
                    //    result.ErrorDescription += Environment.NewLine + ex.InnerException.Message + " " + ex.InnerException.StackTrace;
                    result.ErrorDescription = ExceptionConstant.INTERNAL_ERROR_DESC;
                    result.ErrorCode = ExceptionConstant.INTERNAL_ERROR_CODE;
                }
                return result;
            }
        }
        /// <summary>
        /// Ottiene i dettagli del calendario dato la country e il language
        /// </summary>
        /// <param name="country"></param>
        /// <param name="language"></param>
        /// <returns></returns>        
        [WebMethod]
        [SoapHeader("headerAuthentication", Direction = SoapHeaderDirection.In)] 
        public GetCalendarResult GetCalendar(string country, string language)
        {
            ///Oggetto di ritorno
            GetCalendarResult result = new GetCalendarResult()
                {
                    IsSuccessful = true,
                    ErrorCode = ExceptionConstant.SUCCESSFUL_OPERATION_CODE,
                    ErrorDescription = ExceptionConstant.SUCCESSFUL_OPERATION_DESC

                };
            try
            {
                ///Controllo autorizzazioni
                if (!CheckHeaderAuthorization(headerAuthentication))
                    throw new WsNotAutorizedUserException();

                string _orgname = string.Empty;

#if DEBUG
                _orgname = ORGANIZATION_NAME_LEADMANAGEMENT;
#else
                var org = HttpContext.Current.Request.PhysicalPath.Substring(0, HttpContext.Current.Request.PhysicalPath.IndexOf(@"\services"));
                org = org.ToLower().Substring(org.LastIndexOf(@"\")+1).Replace("custom",string.Empty);
                _orgname = org;
#endif

                using (LeadManagementController controller = new LeadManagementController(_orgname, HttpContext.Current))
                    result.DataSchedulerCalendar = controller.GetSiteSchedulerByCountryAndLanguage(country, language);
                
                ///Ritorno il risultato
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;

                if (ex is ICustomException)
                {
                    var exception = ((ICustomException)ex);
                    result.ErrorDescription = exception.Descr;
                    result.ErrorCode = exception.Code;
                }
                else
                {
                    result.ErrorDescription = ExceptionConstant.INTERNAL_ERROR_DESC;
                    result.ErrorCode = ExceptionConstant.INTERNAL_ERROR_CODE;
                }
                return result;
            }
        }
        /// <summary>
        /// Inserimento del dealer
        /// </summary>
        /// <param name="dealerParameter"></param>
        /// <returns></returns>
        [WebMethod]
        [SoapHeader("headerAuthentication", Direction = SoapHeaderDirection.In)] 
        public SetDealerResult SetDealer(SetDealerParameter dealerParameter)
        {
            ///Oggetto di ritorno
            SetDealerResult result = new SetDealerResult()
            {
                IsSuccessful = true,
                ErrorCode = ExceptionConstant.SUCCESSFUL_OPERATION_CODE,
                ErrorDescription = ExceptionConstant.SUCCESSFUL_OPERATION_DESC
            };
            try
            {
                ///Controllo autorizzazioni
                if (!CheckHeaderAuthorization(headerAuthentication))
                    throw new WsNotAutorizedUserException();

                string _orgname = string.Empty;

#if DEBUG
                _orgname = ORGANIZATION_NAME_LEADMANAGEMENT;
#else
                var org = HttpContext.Current.Request.PhysicalPath.Substring(0, HttpContext.Current.Request.PhysicalPath.IndexOf(@"\services"));
                org = org.ToLower().Substring(org.LastIndexOf(@"\") + 1).Replace("custom", string.Empty);
                _orgname = org;
#endif

                using (LeadManagementController controller = new LeadManagementController(_orgname, HttpContext.Current))
                    controller.SetDealer(dealerParameter);

                ///Ritorno il risultato
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                ///Controllo se è una mia eccezzione personalizzata
                if (ex is ICustomException)
                {
                    var exception = ((ICustomException)ex);
                    result.ErrorDescription = exception.Descr;
                    result.ErrorCode = exception.Code;
                }
                else
                {
                    result.ErrorDescription = ex.Message + " - " + ex.StackTrace;
                    if (ex.InnerException != null)
                        result.ErrorDescription += Environment.NewLine + ex.InnerException.Message + " - " + ex.InnerException.StackTrace;
                    result.ErrorCode = ExceptionConstant.INTERNAL_ERROR_CODE;
                }
                return result;
            }
        }
        /// <summary>
        /// Ottiene i dettagli del dealer
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [WebMethod]
        //[SoapHeader("headerAuthentication", Direction = SoapHeaderDirection.In)]
        public GetDealerResult GetDealer(GetDealerParameter dealerParameter)
        {
            /////Oggetto di ritorno
            //GetDealerResult result = new GetDealerResult()
            //{
            //    IsSuccessful = true,
            //    ErrorCode = ExceptionConstant.SUCCESSFUL_OPERATION_CODE,
            //    ErrorDescription = ExceptionConstant.SUCCESSFUL_OPERATION_DESC
            //};
            try
            {
                GetDealerResult res = null;
                string _orgname = string.Empty;
                ///Controllo autorizzazioni
                //if (!CheckHeaderAuthorization(headerAuthentication))
                //    throw new WsNotAutorizedUserException();
                
#if DEBUG
                _orgname = ORGANIZATION_NAME_LEADMANAGEMENT;
#else
                var org = HttpContext.Current.Request.PhysicalPath.Substring(0, HttpContext.Current.Request.PhysicalPath.IndexOf(@"\services"));
                org = org.ToLower().Substring(org.LastIndexOf(@"\") + 1).Replace("custom", string.Empty);
                _orgname = org;
#endif
                ///Ottengo il dealer
                using (LeadManagementController controller = new LeadManagementController(_orgname, HttpContext.Current))
                    res = controller.GetDealer(dealerParameter);

                res.IsSuccessful = true;
                res.ErrorCode = ExceptionConstant.SUCCESSFUL_OPERATION_CODE;
                res.ErrorDescription = ExceptionConstant.SUCCESSFUL_OPERATION_DESC;

                ///Ritorno il risultato
                return res;
            }
            catch (Exception ex)
            {
                GetDealerResult result = new GetDealerResult();
                result.IsSuccessful = false;
                ///Controllo se è una mia eccezzione personalizzata
                if (ex is ICustomException)
                {
                    var exception = ((ICustomException)ex);
                    result.ErrorDescription = exception.Descr;
                    result.ErrorCode = ex.Message + " - " + ex.StackTrace;
                }
                else
                {
                    result.ErrorDescription = ExceptionConstant.INTERNAL_ERROR_DESC;
                    result.ErrorCode = ex.Message + " - " + ex.StackTrace;
                }
                return result;
            }
            throw new NotImplementedException();
        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Controllo dell'header authentication
        /// </summary>
        /// <param name="headerAuthentication"></param>
        private static bool CheckHeaderAuthorization(HeaderAuthentication headerAuthentication)
        {
            ///General object test
            if (headerAuthentication == null || string.IsNullOrEmpty(headerAuthentication.Username)  || string.IsNullOrEmpty(headerAuthentication.Password))
                return false;
            ///Check username
            if (headerAuthentication.Username.ToUpperInvariant() != USERNAME_IVECO1 &&
                headerAuthentication.Username.ToUpperInvariant() != USERNAME_IVECO2 &&
                headerAuthentication.Username.ToUpperInvariant() != USERNAME_IVECO3)
                return false;
            ///check password
            if (headerAuthentication.Password.ToUpperInvariant() != PASSWORD_IVECO.ToUpperInvariant() && headerAuthentication.Password.ToUpperInvariant() != PASSWORD_IVECO3.ToUpperInvariant())
                return false;

            return true;
        }
        /// <summary>
        /// Imposta i default values
        /// </summary>
        /// <param name="callbackData"></param>
        private static Reply.Iveco.LeadManagement.Presenter.CallBackData SetDefaultValues (Reply.Iveco.LeadManagement.Presenter.CallBackData callbackData)
        {
            if (callbackData == null) return callbackData;

            if (string.IsNullOrEmpty(callbackData.Address)) callbackData.Address = string.Empty;
            if (string.IsNullOrEmpty(callbackData.Brand)) callbackData.Brand = string.Empty;
            if (string.IsNullOrEmpty(callbackData.CabType)) callbackData.CabType = string.Empty;
            if (string.IsNullOrEmpty(callbackData.Canale)) callbackData.Canale = string.Empty;
            if (string.IsNullOrEmpty(callbackData.City)) callbackData.City = string.Empty;
            if (string.IsNullOrEmpty(callbackData.CodePromotion)) callbackData.CodePromotion = string.Empty;
            if (string.IsNullOrEmpty(callbackData.CommentWebForm)) callbackData.CommentWebForm = string.Empty;
            if (string.IsNullOrEmpty(callbackData.CompanyName)) callbackData.CompanyName = string.Empty;
            if (string.IsNullOrEmpty(callbackData.CustomerName)) callbackData.CustomerName = string.Empty;
            if (string.IsNullOrEmpty(callbackData.CustomerSurname)) callbackData.CustomerSurname = string.Empty;
            if (string.IsNullOrEmpty(callbackData.EMail)) callbackData.EMail = string.Empty;
            if (string.IsNullOrEmpty(callbackData.FileName)) callbackData.FileName = string.Empty;

            if (!callbackData.FlagPrivacy.HasValue) callbackData.FlagPrivacy = false;
            if (!callbackData.FlagPrivacyDue.HasValue) callbackData.FlagPrivacyDue = false;

            if (string.IsNullOrEmpty(callbackData.Fuel)) callbackData.Fuel = string.Empty;
            if (string.IsNullOrEmpty(callbackData.GVW)) callbackData.GVW = string.Empty;
            if (string.IsNullOrEmpty(callbackData.IdCare)) callbackData.IdCare = string.Empty;
            if (string.IsNullOrEmpty(callbackData.IdLeadSite)) callbackData.IdLeadSite = string.Empty;
            if (string.IsNullOrEmpty(callbackData.InitiativeSource)) callbackData.InitiativeSource = string.Empty;
            if (string.IsNullOrEmpty(callbackData.InitiativeSourceReport)) callbackData.InitiativeSourceReport = string.Empty;
            if (string.IsNullOrEmpty(callbackData.InitiativeSourceReportDetail)) callbackData.InitiativeSourceReportDetail = string.Empty;
            if (string.IsNullOrEmpty(callbackData.Language)) callbackData.Language = string.Empty;
            if (string.IsNullOrEmpty(callbackData.MobilePhoneNumber)) callbackData.MobilePhoneNumber = string.Empty;
            if (string.IsNullOrEmpty(callbackData.Model)) callbackData.Model = string.Empty;
            if (string.IsNullOrEmpty(callbackData.ModelOfInterest)) callbackData.ModelOfInterest = string.Empty;
            if (string.IsNullOrEmpty(callbackData.Nation)) callbackData.Nation = string.Empty;
            if (string.IsNullOrEmpty(callbackData.PhoneNumber)) callbackData.PhoneNumber = string.Empty;
            if (string.IsNullOrEmpty(callbackData.Power)) callbackData.Power = string.Empty;
            if (string.IsNullOrEmpty(callbackData.Province)) callbackData.Province = string.Empty;
            if (string.IsNullOrEmpty(callbackData.StockSearchedModel)) callbackData.StockSearchedModel = string.Empty;
            if (string.IsNullOrEmpty(callbackData.Suspension)) callbackData.Suspension = string.Empty;
            if (string.IsNullOrEmpty(callbackData.Title)) callbackData.Title = string.Empty;
            if (string.IsNullOrEmpty(callbackData.Type)) callbackData.Type = string.Empty;
            if (string.IsNullOrEmpty(callbackData.TypeContact)) callbackData.TypeContact = string.Empty;
            if (string.IsNullOrEmpty(callbackData.WheelType)) callbackData.WheelType = string.Empty;
            if (string.IsNullOrEmpty(callbackData.ZipCode)) callbackData.ZipCode = string.Empty;


            return callbackData;
        }
        #endregion
    }
}
