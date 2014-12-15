using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Iveco.LeadManagement.Presenter
{
    public static class ExceptionConstant
    {
        public const string LANGUANGE_NOT_FOUND_CODE = "04";
        public const string LANGUANGE_NOT_FOUND_DESC = "Language not found";
        public const string COUNTRY_NOT_FOUND_CODE = "05";
        public const string COUNTRY_NOT_FOUND_DESC = "Country/Nation not found";
        public const string SERVICETYPE_NOT_FOUND_CODE = "06";
        public const string SERVICETYPE_NOT_FOUND_DESC = "Service Type not found";
        public const string LEAD_DATA_NOT_FOUND_CODE = "07";
        public const string LEAD_DATA_NOT_FOUND_DESC = "Lead data not found";
        public const string INVALID_INPUT_PARAMETER_CODE = "01";
        public const string INVALID_INPUT_PARAMETER_DESC = "Parameter [Nation,Phone Number] not found";
        public const string OPERATOR_NOT_FOUND_CODE = "08";
        public const string OPERATOR_NOT_FOUND_DESC = "Operator not found";
        public const string INTERNAL_ERROR_CODE = "09";
        public const string INTERNAL_ERROR_DESC = "Internal error";
        public const string SERVICETYPE_NOT_SUPPORTED_CODE = "10";
        public const string SERVICETYPE_NOT_SUPPORTED_DESC = "Service Type not supported";
        public const string QUEUE_NOT_FOUND_CODE = "11";
        public const string QUEUE_NOT_FOUND_DESC = "Queue not found";
        public const string WS_NOT_AUTORIZED_USER_CODE = "12";
        public const string WS_NOT_AUTORIZED_USER_DESC = "Denied Access Ws";
        public const string SUCCESSFUL_OPERATION_CODE = "00";
        public const string SUCCESSFUL_OPERATION_DESC = "Successful operation";
    }
}
