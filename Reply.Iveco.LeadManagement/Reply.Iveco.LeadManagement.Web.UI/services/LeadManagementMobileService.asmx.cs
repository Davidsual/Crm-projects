using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Reply.Iveco.LeadManagement.Presenter;
using Reply.Iveco.LeadManagement.Presenter.Model;

namespace Reply.Iveco.LeadManagement.Web.UI.services
{
    /// <summary>
    /// Summary description for LeadManagementMobileService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class LeadManagementMobileService : System.Web.Services.WebService
    {
        [WebMethod]
        public CheckLoginResult CheckLogin(string userName, string password)
        {
            CheckLoginResult result = new CheckLoginResult()
            {
                IsSuccessful = true,
                ErrorCode = ExceptionConstant.SUCCESSFUL_OPERATION_CODE,
                ErrorDescription = ExceptionConstant.SUCCESSFUL_OPERATION_DESC
            };

            try
            {
                string _orgname = string.Empty;

#if DEBUG
                _orgname = DataConstant.ORGANIZATION_NAME_TEST;
#else
                var org = HttpContext.Current.Request.PhysicalPath.Substring(0, HttpContext.Current.Request.PhysicalPath.IndexOf(@"\services"));
                org = org.ToLower().Substring(org.LastIndexOf(@"\")+1).Replace("custom",string.Empty);
                _orgname = org;
#endif

                using (LeadManagementController controller = new LeadManagementController(_orgname, HttpContext.Current))
                    result.CurrentUser = controller.CheckLogin(userName, password);
                if (result.CurrentUser == null) result.CurrentUser = new Reply.Iveco.LeadManagement.Presenter.Model.SystemUser();
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
                    result.ErrorDescription = ex.Message + " " + ex.StackTrace;
                    result.ErrorCode = ExceptionConstant.INTERNAL_ERROR_CODE;
                }
                return result;
            }
            
        }
        /// <summary>
        /// Restiuisce tutte le language
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public GetLanguagesResult GetLanguages()
        {
            GetLanguagesResult result = new GetLanguagesResult()
            {
                IsSuccessful = true,
                ErrorCode = ExceptionConstant.SUCCESSFUL_OPERATION_CODE,
                ErrorDescription = ExceptionConstant.SUCCESSFUL_OPERATION_DESC
            };
            try
            {

                string _orgname = string.Empty;

#if DEBUG
                _orgname = DataConstant.ORGANIZATION_NAME_TEST;
#else
                var org = HttpContext.Current.Request.PhysicalPath.Substring(0, HttpContext.Current.Request.PhysicalPath.IndexOf(@"\services"));
                org = org.ToLower().Substring(org.LastIndexOf(@"\")+1).Replace("custom",string.Empty);
                _orgname = org;
#endif

                using (LeadManagementController controller = new LeadManagementController(_orgname, HttpContext.Current))
                    result.Languages =  controller.GetAllLanguage();

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
                    result.ErrorDescription = ex.Message + " " + ex.StackTrace;
                    result.ErrorCode = ExceptionConstant.INTERNAL_ERROR_CODE;
                }
                return result;
            }

        }
        /// <summary>
        /// Restiuisce tutte le country
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public GetCountriesResult GetCountries()
        {
            GetCountriesResult result = new GetCountriesResult()
            {
                IsSuccessful = true,
                ErrorCode = ExceptionConstant.SUCCESSFUL_OPERATION_CODE,
                ErrorDescription = ExceptionConstant.SUCCESSFUL_OPERATION_DESC
            };
            try
            {

                string _orgname = string.Empty;

#if DEBUG
                _orgname = DataConstant.ORGANIZATION_NAME_TEST;
#else
                var org = HttpContext.Current.Request.PhysicalPath.Substring(0, HttpContext.Current.Request.PhysicalPath.IndexOf(@"\services"));
                org = org.ToLower().Substring(org.LastIndexOf(@"\") + 1).Replace("custom", string.Empty);
                _orgname = org;
#endif

                using (LeadManagementController controller = new LeadManagementController(_orgname, HttpContext.Current))
                    result.Countries = controller.GetAllCountry();

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
                    result.ErrorDescription = ex.Message + " " + ex.StackTrace;
                    result.ErrorCode = ExceptionConstant.INTERNAL_ERROR_CODE;
                }
                return result;
            }
        }
        
        [WebMethod]
        public SetLeadResult SetLead(SetLeadParameter param, Guid idOperator)
        {
            SetLeadResult result = new SetLeadResult()
            {
                IsSuccessful = true,
                ErrorCode = ExceptionConstant.SUCCESSFUL_OPERATION_CODE,
                ErrorDescription = ExceptionConstant.SUCCESSFUL_OPERATION_DESC
            };
            try
            {
                string _orgname = string.Empty;

#if DEBUG
                _orgname = DataConstant.ORGANIZATION_NAME_TEST;
#else
                var org = HttpContext.Current.Request.PhysicalPath.Substring(0, HttpContext.Current.Request.PhysicalPath.IndexOf(@"\services"));
                org = org.ToLower().Substring(org.LastIndexOf(@"\") + 1).Replace("custom", string.Empty);
                _orgname = org;
#endif

                using (LeadManagementController controller = new LeadManagementController(_orgname, HttpContext.Current))
                    controller.SetLeadMobile(param, idOperator);

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
                    result.ErrorDescription = ex.Message + " " + ex.StackTrace;
                    result.ErrorCode = ExceptionConstant.INTERNAL_ERROR_CODE;
                }
                return result;
            }
        }
    }
}
