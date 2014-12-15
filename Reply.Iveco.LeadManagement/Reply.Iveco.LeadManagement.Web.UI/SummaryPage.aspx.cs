using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Threading;
using Reply.Iveco.LeadManagement.Presenter;

namespace Reply.Iveco.LeadManagement.Web.UI
{
    public partial class SummaryPage : BasePage
    {
        #region PRIVATE MEMBERS
        private const string ASAP = "ASAP";
        #endregion

        #region PRIVATE PROPERTY
        /// <summary> 
        /// Current CallBack Data
        /// </summary>
        private CallBackData CallBackData
        {
            get { return ViewState["CallBackData"] as CallBackData; }
            set { ViewState["CallBackData"] = value; }
        }
        #endregion

        #region EVENTS
        /// <summary>
        /// Caricamento pagina
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //base.CurrentController.SetAppointmentAsap(this.GetDataFake());
                //var _return = base.CurrentController.Test("francese", "France");
                return;
                ///Lingua corrente
                this.ddlLingua.SelectedValue = base.CurrentLanguage;
                ///Carico l'oggetto con gli eventuali parametri che arrivano in POST
                this.LoadPostParameter();
            }

        }
        /// <summary>
        /// Cambio della lingua
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlLingua_SelectedIndexChanged(object sender, EventArgs e)
        {
            ///Lingua attuale
            base.CurrentLanguage = this.ddlLingua.SelectedValue;
        }
        /// <summary>
        /// Click invia,controllo il capcha
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnInvia_Click(object sender, ImageClickEventArgs e)
        {
            ///controllo captcha
            this.ctrlCaptcha.ValidateCaptcha(this.txtCaptcha.Text.Trim());
            ///controllo capcha
            if (this.ctrlCaptcha.UserValidated)
            {
                base.Authentication = new Auth() { IsAuthenticated = true, DateAuthentication = DateTime.Now };
                ///Redirigo in base alla scelta dell'utente
                if (((ImageButton)sender).CommandArgument.ToUpper() == ASAP)
                    ScriptManager.RegisterClientScriptBlock(this.updPnlCapcha, this.GetType(), "redirect", "self.location.href = 'ConfirmPage.aspx';", true);
                else
                    ScriptManager.RegisterClientScriptBlock(this.updPnlCapcha, this.GetType(), "redirect", "self.location.href = 'BookingPage.aspx';", true);
            }
            else
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "al1", "alert('utente non riconosciuto');", true);

        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Carico l'oggetto con gli eventuali parametri che arrivano in POST
        /// </summary>
        private void LoadPostParameter()
        {
            ///Carico i dati che provengono dal sito
            this.CallBackData = new CallBackData()
                {
                    DataLeadCreation = DateTime.Now,
                    CustomerName = string.Empty,
                    CustomerSurname = string.Empty,
                    Address = string.Empty,
                    ZipCode = string.Empty,
                    City = string.Empty,
                    Province = string.Empty,
                    Nation = string.Empty,
                    EMail = string.Empty,
                    PhoneNumber = string.Empty,
                    MobilePhoneNumber = string.Empty,
                    FlagPrivacy = null,
                    Brand = string.Empty,
                    StockSearchedModel = string.Empty,
                    DueDate = DateTime.Now,
                    TypeContact = string.Empty,
                    Model = string.Empty,
                    Type = string.Empty,
                    GVW = string.Empty,
                    WheelType = string.Empty,
                    Fuel = string.Empty,
                    FlagPrivacyDue = null,
                    IdCare = string.Empty,
                    InitiativeSource = string.Empty,
                    InitiativeSourceReport = string.Empty,
                    InitiativeSourceReportDetail = string.Empty,
                    CompanyName = string.Empty,
                    Power = string.Empty,
                    CabType = string.Empty,
                    Suspension = string.Empty,
                    CommentWebForm = string.Empty,
                    IdLeadSite = string.Empty,
                    Title = string.Empty,
                    Language = string.Empty,
                    CodePromotion = string.Empty,
                    ModelOfInterest = string.Empty,
                    DesideredData = DateTime.Now,
                    Canale = string.Empty
                };
        }

        private CallBackData GetDataFake()
        {
            return new CallBackData()
            {
                DataLeadCreation = DateTime.Now,
                CustomerName = "Attilio",
                CustomerSurname = "Lombardo",
                Address = "Via grugliakin 12",
                ZipCode = "10044",
                City = "Grugliasco",
                Province = "Torino",
                Nation = "France",
                EMail = "e.sammuri@reply.it",
                PhoneNumber = "3402311262",
                MobilePhoneNumber = "3402311262",
                FlagPrivacy = true,
                Brand = "My Brand is Eraldo",
                StockSearchedModel = "Cuccia cane",
                DueDate = DateTime.Now,
                TypeContact = "NUOVO",
                Model = "STRALIS",
                Type = "Furgone",
                GVW = "5,0 tonnellate",
                WheelType = "Ruota gemellata",
                Fuel = "Diesel",
                FlagPrivacyDue = false,
                IdCare = "idCare",
                InitiativeSource = "Initiative Source",
                InitiativeSourceReport = "Initiative source report",
                InitiativeSourceReportDetail = "Initiative source report detail",
                CompanyName = "Reply spa",
                Power = "280 Ch",
                CabType = "AT - Active Time (Cabine Profonde)",
                Suspension = "Mécanique",
                CommentWebForm = "Test inserimento del consultant",
                IdLeadSite = "id Lead Site",
                Title = "Titolo",
                Language = "Francese",
                CodePromotion = "CodePromotion",
                ModelOfInterest = "ModelOfInterest",
                DesideredData = DateTime.Now,
                Canale = "Canale"
            };
        }
        #endregion
    }
}
