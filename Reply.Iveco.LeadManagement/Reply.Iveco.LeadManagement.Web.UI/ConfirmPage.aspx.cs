using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Reply.Iveco.LeadManagement.Web.UI
{
    public partial class ConfirmPage : BasePage
    {
        #region EVENTS
        /// <summary>
        /// Caricamento pagina
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Controllo la sessione
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.CheckSession = true;
            base.OnInit(e);
        }
        #endregion

    }
}
