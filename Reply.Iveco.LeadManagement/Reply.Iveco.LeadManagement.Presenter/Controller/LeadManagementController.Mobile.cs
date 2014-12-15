using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Reply.Iveco.LeadManagement.Presenter.Model;

namespace Reply.Iveco.LeadManagement.Presenter
{
    /// <summary>
    /// Gestione della BL
    /// </summary>
    public partial class LeadManagementController : BaseLeadManagementController, IDisposable
    {
        #region PUBLIC METHODS
        /// <summary>
        /// Controlla la login del crm
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public SystemUser CheckLogin(string userName, string password)
        {
            return base.CurrentDataAccessLayer.CheckUserNamePasswordIFD(userName, password);
        }
        /// <summary>
        /// Salva un lead che arriva da mobile
        /// </summary>
        /// <param name="param"></param>
        /// <param name="idOperator"></param>
        public void SetLeadMobile(SetLeadParameter param, Guid idOperator)
        {
            ///Ottengo il country
            New_country country = base.CurrentDataAccessLayer.GetCountryById(param.CountryId);
            ///Sollevo errore se country è null
            if (country == null) throw new CountryNotFoundException() { };
            ///Ottengo la lingua
            New_language _language = base.CurrentDataAccessLayer.GetLanguageById(param.LanguageId);
            ///Controllo che language sia valorizzato
            if (_language == null) throw new LanguageNotFoundException() {};
            this.SetAppointmentAsap(new CallBackData()
            {
                CustomerName = param.FirstName,
                CustomerSurname = param.LastName,
                Nation = country.New_name,
                Language = _language.New_name,
                PhoneNumber = param.TelephoneNumber,
                TypeContact = param.TypeContactCode.ToString(),
                Address = string.Empty,
                ZipCode = string.Empty,
                Brand = string.Empty,
                WheelType = string.Empty,
                StockSearchedModel = string.Empty,
                CabType = string.Empty,
                Canale = string.Empty,
                City = string.Empty,
                CodePromotion = string.Empty,
                CommentWebForm = string.Empty,
                CompanyName = string.Empty,
                DataLeadCreation = DateTime.Now,
                Suspension = string.Empty,
                DesideredData = DateTime.Now,
                DueDate = DateTime.Now,
                EMail = string.Empty,
                FileName = string.Empty,
                FlagPrivacy = true,
                FlagPrivacyDue = true,
                Fuel = string.Empty,
                GVW = string.Empty,
                IdCare = string.Empty,
                IdLeadSite = string.Empty,
                InitiativeSource = param.InitiativeSource,
                InitiativeSourceReport = param.InitiativeSourceReport,
                InitiativeSourceReportDetail = param.InitiativeSourceReportDetail,
                MobilePhoneNumber = string.Empty,
                Model = string.Empty,
                ModelOfInterest = string.Empty,
                Power = string.Empty,
                Province = string.Empty,
                Title = string.Empty,
                Type = string.Empty
            });
            
            //this.CurrentDataAccessLayer.SetLeadMobile(param, idOperator);
        }
        #endregion
    }
}
