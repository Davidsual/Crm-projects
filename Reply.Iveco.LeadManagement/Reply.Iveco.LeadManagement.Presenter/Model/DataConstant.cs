using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Reflection;

namespace Reply.Iveco.LeadManagement.Presenter.Model
{
    public static class DataConstant
    {
        public const string ORGANIZATION_NAME_TEST = "lmcert";
        public const string URL_WS_LM = "http://192.168.88.22/MSCRMServices/2007/CrmService.asmx";
        public const string SQL_CONN_STRING_TEST_DEALER = "Password=sa;Persist Security Info=True;User ID=dtrotta;Initial Catalog=IvecoSvilItalia_MSCRM;Data Source=192.168.90.8";
        public const string SQL_CONN_STRING_TEST_LM = "Password=81.David;Persist Security Info=True;User ID=d.trotta;Initial Catalog=lmcert_MSCRM;Data Source=192.168.88.23";

        //public const string ORGANIZATION_NAME_TEST = "ivecoleadmanagement";
        //public const string URL_WS_LM = "http://to0crm03/MSCRMServices/2007/CrmService.asmx";
        //public const string SQL_CONN_STRING_TEST_DEALER = "Password=sa;Persist Security Info=True;User ID=dtrotta;Initial Catalog=IvecoSvilItalia_MSCRM;Data Source=192.168.90.8";
        //public const string SQL_CONN_STRING_TEST_LM = "Password=80.David;Persist Security Info=True;User ID=d.trotta;Initial Catalog=IvecoLeadManagement_MSCRM;Data Source=to0crm03";
        
        
        public const int OBJECT_TYPE_CODE_LEAD = 4;
        ///public const string DEFAULT_ORGANIZATION_NAME = "IvecoLeadManagement";
        public const string DEFAULT_GO_TO_TAB = "default";
        public const int LEAD_LEAD_CATEGORY_TO_BE_PROCESSED = 1;
        public const int LEAD_LEAD_CATEGORY_DUPLICATE = 6;
        public const int DOSSIER_STATUS_CODE_RESERVED = 4;
        public const int DOSSIER_STATUS_CODE_CANCELED = 9;
        /// <summary>
        /// tipo di servizio
        /// </summary>
        //[DataContract(Namespace = "http://Reply.Iveco.LeadManagement.Presenter.Model/2007/04", Name = "TypeService")]
        [Flags]
        public enum TypeService
        {
            //[EnumMember()]
            ASAP = 1,
            //[EnumMember()]
            BOOKING = 2,
            CSI = 3
        }
        /// <summary>
        /// Resource group type code
        /// </summary>
        public enum ResourceGroupTypeCode
        {
            Static = 0,
            Dynamic = 1,
            Hidden = 2
        }
        /// <summary>
        /// Status Lead
        /// </summary>
        public enum LeadStatus
        {
            Open = 1,
            DealerToBeAssigned = 2,
            WaitingForCSI = 3,
            Closed = 4
        }
        /// <summary>
        /// Category status del Lead
        /// </summary>
        public enum LeadCategory
        {
            ToBeProcessed = 1,
            ToBeRecalled = 2,
            Profiled = 3,
            DispatchedToTheMarket = 4,
            DispatchedToTheDealer = 5,
            Duplicated = 6,
            NotRelevant = 7,
            NotFound = 8,
            NoAnswer = 9,
            WrongNumber = 10,
            NotExistentNumber = 11,
            VehicleToBeImported = 12,
            Refusal = 13,
            InfoRequestNoPurchaseIntention = 14,
            InfoRequestPurchasePosticipated = 15,
            GeneralInfoToBeManaged = 16,
            Complaint = 17,
            CSIDone = 18,
            NoCSINotFound = 19,
            NoCSIRefusal = 20,
            NoCSIWrongNumber = 21,
            WrongMarket = 22
        }
        /// <summary>
        /// Tipo di contatto
        /// </summary>
        public enum TYPE_CONTACT
        {
            NEW = 1,
            USED = 2
        }
        /// <summary>
        /// Tipi di go to tab
        /// </summary>
        public enum TypeGoToTab
        {
            PromptDelivery = 1,
            VHLConfigurator = 2,
            Promotions = 3,
            Other = 4,
            Used = 5,
            CSI = 6,
            Information = 7
        }
        public enum CallBackSource
        {
            WsAsap = 1,
            WsBooking = 2,
            UploadFile = 3
        }
        public enum CallBackDataState
        {
            ToCreate = 1,
            Done = 2,
            Error = 3,
        }
        public enum LeadSubStatus
        {
            Useful = 1,
            NotUseful = 2,
            Duplicated = 3,
            ToBeProcessed = 4
        }
        public enum LeadBrand
        {
            [DescriptionAttribute("Iveco")]
            Iveco = 1,
            [DescriptionAttribute("Iveco Used")]
            IvecoUsed = 2
        }
        public enum LeadTitle
        {
            Mr = 1,
            Mrs = 2,
            Ms = 3
        }
        public enum ChannelLead
        {
            [DescriptionAttribute("Web search engine")]
            WebSearchEngine = 6,
            [DescriptionAttribute("Iveco Website")]
            IvecoWebSite = 9,
            [DescriptionAttribute("Iveco used web site")]
            IvecoUsedWebSite = 21,
            [DescriptionAttribute("Other")]
            Other =20
        }
        /// <summary>
        /// State del caricmaento massimo dei lead sul crm dealer
        /// </summary>
        public enum ContactLeadSource : int
        {
            Excel = 1
        }
        /// <summary>
        /// Status del caricmaento massimo dei lead sul crm dealer
        /// </summary>
        public enum ContactLeadState : int
        {
            ToCreate = 1,
            Done = 2,
            Error = 3
        }
        /// <summary>
        /// Picklist go to tab all'interno del lead
        /// </summary>
        public enum GoToTabLead : int
        {
            PromptDelivery = 1,
            VHLConfigurator = 2,
            Promotions = 3,
            Other = 4,
            Used = 5,
            CSI = 6,
            Information = 7
        }
        public static string GetStringValueOf(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

        ///Crm Dealer
        public enum PreferedMethodOfContactLead
        {
            Any =1,
            EMail = 2,
            Phone = 3,
            Fax = 4,
            Mail = 5,
            Mobile = 200000
        }
    }

    public class StringValueAttribute : Attribute
    {
        #region Properties
        /// <summary>
        /// Holds the stringvalue for a value in an enum.
        /// </summary>
        public string StringValue { get; protected set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor used to init a StringValue Attribute
        /// </summary>
        /// <param name="value"></param>
        public StringValueAttribute(string value)
        {
            this.StringValue = value;
        }
        #endregion

    }


}
