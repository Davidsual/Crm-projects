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
        #region CTOR
        /// <summary>
        /// Costruttore
        /// </summary>
        public LeadManagementController(string organizationName) :
            base(organizationName, null)
        {

        }
        /// <summary>
        /// Costruttore
        /// </summary>
        public LeadManagementController(string organizationName, HttpContext context) : 
            base(organizationName,context)
        {
            
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Rilascio delle risorse
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }
        #endregion
    }
}
