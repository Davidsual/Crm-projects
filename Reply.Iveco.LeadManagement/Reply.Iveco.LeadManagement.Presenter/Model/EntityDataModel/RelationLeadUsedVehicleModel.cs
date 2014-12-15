using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Iveco.LeadManagement.Presenter.Model
{
    [Serializable()]
    public sealed class RelationLeadUsedVehicleModel
    {
        public Guid IdUsedVehicleModelPromptDelivery { get; set; }
        public Guid IdUsedVehicleModelVhlConfigurator { get; set; }
        public Guid IdUsedVehicleModelPromotions { get; set; }
        public Guid IdUsedVehicleModelOthers { get; set; }

    }
}
