using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reply.Seat.DinamichePromozionali.DataAccess.Model;
//using Reply.Seat.DinamichePromozionali.CrmAdapter.CrmServiceWsdl;
using Reply.Seat.DinamichePromozionali.CrmAdapter.CrmServicePerformance;
using System.Web;

namespace Reply.Seat.DinamichePromozionali.BL
{
    /// <summary>
    /// Property comuni della classe
    /// </summary>
    public partial class Manager : BaseManager, IDisposable
    {
        #region PRIVATE MEMBERS
        /// <summary>
        /// Current Owner
        /// </summary>
        private Func<Campaign, Owner> _currentOwner = (campaign) =>
        {
            Guid ownerId;
            if (campaign.OwnerId.HasValue)
                ownerId = campaign.OwnerId.Value;
            else
                throw new DinamichePromozionaliException(StatusTranscoding.STATUS_CODE_NULL_OWNER_CAMPAGNA, StatusTranscoding.STATUS_DESC_NULL_OWNER_CAMPAGNA, Guid.Empty, campaign);
            return new Owner() { Value = ownerId, type=EntityName.systemuser.ToString() };
        };
        #endregion

        #region CTOR
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="context"></param>
        public Manager(HttpContext context)
            : base(context)
        {

        }
        public Manager()
        {

        }

        #endregion

        #region IDisposable Members
        /// <summary>
        /// Rilascio delle risorse
        /// </summary>
        public void Dispose()
        {
            if (this.CurrentAccessLayer != null)
                this.CurrentAccessLayer.Dispose();
            if (this.CurrentCrmAdapter != null)
                this.CurrentCrmAdapter.Dispose();
            this.CurrentCallType = null;
            this.CurrentCampaign = null;
            this.CurrentChiamanteCampagna = null;
            this.CurrentLead = null;
            ///Force Garbage collector
            GC.Collect();
        }

        #endregion
    }
}
