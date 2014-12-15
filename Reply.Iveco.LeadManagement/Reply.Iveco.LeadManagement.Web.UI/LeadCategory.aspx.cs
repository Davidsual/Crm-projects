using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using Reply.Iveco.LeadManagement.Presenter;
using System.Globalization;

namespace Reply.Iveco.LeadManagement.Web.UI
{
    public partial class LeadCategory : BasePage
    {
        #region PRIVATE MEMBERS
        private const string CLOSED = "closed";
        #endregion

        #region PRIVATE PROPERTY
        /// <summary>
        /// Current controller
        /// </summary>
        private LeadManagementController CurrentCtrl
        {
            get
            {
                return base.GetCurrentController<LeadManagementController>().Invoke(TypeEnviroment.LeadManagement, this.CurrentOrganizationName);
            }
        }
        /// <summary>
        /// Tipo di bottone
        /// </summary>
        private string CurrentKindButton
        {
            get
            {
                return ViewState["CurrentKindButton"] as string;
            }
            set
            {
                ViewState["CurrentKindButton"] = value;
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
                if (!string.IsNullOrEmpty(Request.QueryString["orgname"]))
                    base.CurrentOrganizationName = Request.QueryString["orgname"];
                else
                    throw new ArgumentException("Miss l'organization name");
                ///Tipologia di bottone
                if (!string.IsNullOrEmpty(Request.QueryString["button"]))
                    this.CurrentKindButton = Request.QueryString["button"];
                else
                    throw new ArgumentException("Miss kind of button");

                ///Rendo visibile l'eventuale pannello con i checkbox
                this.divMail.Visible = (this.CurrentKindButton.ToLower() == CLOSED);
                ///Recupero i dati a seconda del tipo di bottone premuto
                this.ddlLeadCategory.DataSource = (this.CurrentKindButton.ToLower() == CLOSED) ? this.CurrentCtrl.GetPickListLeadCategoryClosed() : this.CurrentCtrl.GetPickListLeadCategoryClosedCsi();
                this.ddlLeadCategory.DataValueField = "AttributeValue";
                this.ddlLeadCategory.DataTextField = "Value";
                this.ddlLeadCategory.DataBind();
                this.ddlLeadCategory.Items.Insert(0, new ListItem(string.Empty, string.Empty));
            }
        }
        #endregion

        #region PRIVATE METHODS
        
        #endregion
    }
}
