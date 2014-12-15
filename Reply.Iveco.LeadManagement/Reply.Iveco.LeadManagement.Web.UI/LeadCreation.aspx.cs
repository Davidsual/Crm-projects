using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Reply.Iveco.LeadManagement.Presenter;
using Reply.Iveco.LeadManagement.Presenter.Model;
using System.Globalization;
using System.Text;
using Reply.Iveco.LeadManagement.Web.UI.services;


namespace Reply.Iveco.LeadManagement.Web.UI
{
    public partial class LeadCreation : BasePage
    {
        #region PRIVATE PROPERTY
        /// <summary>
        /// Current controller
        /// </summary>
        private LeadManagementController CurrentCtrl
        {
            get
            {
                return base.GetCurrentController<LeadManagementController>().Invoke(TypeEnviroment.LeadManagement, base.CurrentOrganizationName);
            }
        }
        /// <summary>
        /// Contiene il datasource delle country
        /// </summary>
        private List<New_country> Countries
        {
            get
            {
                return ViewState["Countries"] as List<New_country>;
            }
            set
            {
                ViewState["Countries"] = value;
            }
        }
        #endregion

        #region EVENT
        /// <summary>
        /// Caricamento pagina
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["orgname"]))
                base.CurrentOrganizationName = Request.QueryString["orgname"];
            else
                throw new ArgumentException("Manca l'organization name");

            if (!Page.IsPostBack)
            {
                this.txtStartDate.Text = DateTime.Now.Date.ToShortDateString();
                this.txtEndDate.Text = DateTime.Now.Date.ToShortDateString();
                this.txtDataLeadCreation.Text = DateTime.Now.Date.ToShortDateString();
                this.txtDueDate.Text = DateTime.Now.Date.ToShortDateString();
                this.txtDesideredData.Text = DateTime.Now.Date.ToShortDateString();
                ///country e language
                var country = this.Countries =  this.CurrentCtrl.GetAllCountry();
                var language = this.CurrentCtrl.GetAllLanguage();
                var brand = this.CurrentCtrl.GetAllBrand();
                var channel = this.CurrentCtrl.GetAllChannel();
                var typeContact = this.CurrentCtrl.GetAllTypeContact();
                ///Bind country
                this.ddlCountry.DataSource = country;
                this.ddlCountry.DataValueField = "new_name";
                this.ddlCountry.DataTextField = "new_name";
                this.ddlCountry.DataBind();
                ///Bind language
                this.ddlLanguage.DataSource = language;
                this.ddlLanguage.DataValueField = "new_name";
                this.ddlLanguage.DataTextField = "new_name";
                this.ddlLanguage.DataBind();
                ///brand
                this.ddlBrand.DataSource = brand;
                this.ddlBrand.DataValueField = "value";
                this.ddlBrand.DataTextField = "value";
                this.ddlBrand.DataBind();
                ///channel
                this.ddlCanale.DataSource = channel;
                this.ddlCanale.DataValueField = "AttributeValue";
                this.ddlCanale.DataTextField = "value";
                this.ddlCanale.DataBind();
                ///typecontact
                this.ddlTypeContact.DataSource = typeContact;
                this.ddlTypeContact.DataValueField = "value";
                this.ddlTypeContact.DataTextField = "value";
                this.ddlTypeContact.DataBind();
                ///Valori iniziali
                this.ddlCountry.Items.Insert(0, new ListItem(string.Empty, string.Empty));
                this.ddlLanguage.Items.Insert(0, new ListItem(string.Empty, string.Empty));
                this.ddlBrand.Items.Insert(0, new ListItem(string.Empty, string.Empty));
                this.ddlCanale.Items.Insert(0, new ListItem(string.Empty, string.Empty));
                this.ddlTypeContact.Items.Insert(0, new ListItem(string.Empty, string.Empty));

            }
        }
        /// <summary>
        /// Chiamata ASAP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                ///Controllo obbligatorietà dei campi
                if (string.IsNullOrEmpty(this.txtTelNum.Text.Trim()) || this.ddlCountry.SelectedValue == string.Empty)
                    throw new Exception("Popolare i campi obbligatori contrassegnati dall'asterisco");

                this.lblErrore.Text = string.Empty;
                var data = this.GetDataFromForm();
                this.CurrentCtrl.SetAppointmentAsap(data);
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "alert('Operazione conclusa con successo');", true);
            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "err", "alert('" + ex.Message.Replace("'", "\\'") + "');", true);
                this.lblErrore.Text = ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace;
            }
        }
        /// <summary>
        /// Chiamata booking
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button2_Click(object sender, EventArgs e)
        {
            this.lblErrore.Text = string.Empty;
            DateTime start,end;
            try
            {
                
                ///Controllo obbligatorietà dei campi
                if (string.IsNullOrEmpty(this.txtTelNum.Text.Trim()) || 
                    this.ddlCountry.SelectedValue == string.Empty)
                    throw new Exception("Popolare i campi obbligatori contrassegnati dall'asterisco");
                if(string.IsNullOrEmpty(this.txtStartTime.Text.Trim()) || string.IsNullOrEmpty(this.txtEndTime.Text.Trim()) ||
                   !DateTime.TryParse(this.txtStartDate.Text.Trim() + " " + this.txtStartTime.Text.Trim(), out start) ||
                   !DateTime.TryParse(this.txtEndDate.Text.Trim() + " " + this.txtEndTime.Text.Trim(),out end))
                    throw new Exception("Date inizio - fine schedulazione non valide");


                //DateTime start = Convert.ToDateTime(this.txtStartDate.Text.Trim() + " " + this.txtStartTime.Text.Trim());
                //DateTime end = Convert.ToDateTime(this.txtEndDate.Text.Trim() + " " + this.txtEndTime.Text.Trim());

                this.CurrentCtrl.SetAppointmentBooking(this.GetDataFromForm(), start, end);
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "alert('Operazione conclusa con successo');", true);

            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "err", "alert('" + ex.Message.Replace("'", "\\'") + "');", true);
                this.lblErrore.Text = ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace;
            }
        }
        /// <summary>
        /// selezione della country
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            var country = ((DropDownList)sender).SelectedValue;
            var ret = this.Countries.Where(c => c.New_name.ToLower() == country.ToLower()).Select(c => c.New_DefaultLanguageIdName).SingleOrDefault();
            if (!string.IsNullOrEmpty(ret))
                this.ddlLanguage.SelectedValue = ret;
        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Recupera i dati dalla forma e li inserisce in un oggetto
        /// </summary>
        /// <returns></returns>
        private CallBackData GetDataFromForm()
        {
            try
            {
                return new CallBackData()
                {
                    DataLeadCreation = (!string.IsNullOrEmpty(this.txtDataLeadCreation.Text.Trim())) ? Convert.ToDateTime(this.txtDataLeadCreation.Text.Trim()) : DateTime.MinValue,
                    CustomerName = this.txtNome.Text.Trim(),
                    CustomerSurname = this.txtCognome.Text.Trim(),
                    Address = this.txtIndirizzo.Text.Trim(),
                    ZipCode = this.txtCap.Text.Trim(),
                    City = this.txtCitta.Text.Trim(),
                    Province = this.txtProvincia.Text.Trim(),
                    Nation = this.ddlCountry.SelectedValue,
                    EMail = this.txtEMail.Text.Trim(),
                    PhoneNumber = this.txtTelNum.Text.Trim(),
                    MobilePhoneNumber = this.txtNumCellPhone.Text.Trim(),
                    FlagPrivacy = true,
                    Brand = this.ddlBrand.SelectedValue,
                    DueDate = (!string.IsNullOrEmpty(this.txtDueDate.Text.Trim())) ? Convert.ToDateTime(this.txtDueDate.Text.Trim()) : DateTime.MinValue,
                    TypeContact = this.ddlTypeContact.SelectedValue,
                    Model = this.txtModel.Text.Trim(),
                    Type = this.txtType.Text.Trim(),
                    GVW = this.txtGVW.Text.Trim(),
                    WheelType = this.txtWheelType.Text.Trim(),
                    Fuel = this.txtFuel.Text.Trim(),
                    FlagPrivacyDue = false,
                    IdCare = this.txtIdCare.Text.Trim(),
                    InitiativeSource = this.txtInitiativeSource.Text.Trim(),
                    InitiativeSourceReport = this.txtInitiativeSourceReport.Text.Trim(),
                    InitiativeSourceReportDetail = this.txtInitiativeSourceReportDetail.Text.Trim(),
                    CompanyName = this.txtCompanyName.Text.Trim(),
                    Power = this.txtPower.Text.Trim(),
                    CabType = this.txtCabType.Text.Trim(),
                    Suspension = this.txtSuspension.Text.Trim(),
                    CommentWebForm = this.txtCommentWebForm.Text.Trim(),
                    IdLeadSite = this.txtidLeadSite.Text.Trim(),
                    Title = this.txtTitle.Text.Trim(),
                    Language = this.ddlLanguage.SelectedValue,
                    CodePromotion = this.txtCodePromotion.Text.Trim(),
                    ModelOfInterest = this.txtModelOfInterest.Text.Trim(),
                    StockSearchedModel = this.txtStockSearched.Text.Trim(),
                    DesideredData = (!string.IsNullOrEmpty(this.txtDesideredData.Text.Trim())) ? Convert.ToDateTime(this.txtDesideredData.Text.Trim()) : DateTime.MinValue,
                    Canale = this.ddlCanale.SelectedValue
                };
            }
            catch
            {
                throw new Exception("Formato dei dati inseriti non conforme al tipo di campo");
            }
        } 
        #endregion
    }
}
