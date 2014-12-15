using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using Reply.Iveco.LeadManagement.Presenter;
using Reply.Iveco.LeadManagement.Presenter.Model;

namespace Reply.Iveco.LeadManagement.Web.UI
{
    public partial class AdministratorScheduler : BasePage
    {
        #region PRIVATE PROPERTY
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
        /// <summary>
        /// Istanza del controller
        /// </summary>
        private LeadManagementController CurrentCtrl
        {
            get
            {
                return base.GetCurrentController<LeadManagementController>().Invoke(TypeEnviroment.LeadManagement, base.CurrentOrganizationName);
            }
        }
        #endregion

        #region EVENTS
        /// <summary>
        /// Caricamento della pagina
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ///Recupero l'organization name dal querystring
#if DEBUG
                base.CurrentOrganizationName = DataConstant.ORGANIZATION_NAME_TEST;
#else           
                if (!string.IsNullOrEmpty(Request.QueryString["orgname"]))
                    base.CurrentOrganizationName = Request.QueryString["orgname"];
                else
                    throw new ArgumentException("Miss organization name");
#endif
                this.LoadFields();
            }
                
        }
        /// <summary>
        /// Carica il calendario con i parametri di lingua e country
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnShowCalendar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.ddlCountry.SelectedValue) || string.IsNullOrEmpty(this.ddlLanguage.SelectedValue))
                return;
            ///Ordino allo user control di caricarsi
            this.MyScheduler.LoadScheduler(this.CurrentOrganizationName, this.ddlCountry.SelectedValue.Trim(), this.ddlLanguage.SelectedValue.Trim());
        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Carica tutti i campi
        /// </summary>
        private void LoadFields()
        {
            ///Recupero i valori
            var language = this.CurrentCtrl.GetAllLanguage();
            var country = this.Countries = this.CurrentCtrl.GetAllCountry();
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
            ///Valori iniziali
            this.ddlCountry.Items.Insert(0, new ListItem(string.Empty, string.Empty));
            this.ddlLanguage.Items.Insert(0, new ListItem(string.Empty, string.Empty));
        }
        #endregion  

        /// <summary>
        /// Selezione l'eventuale default language per la country
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
    }
}
