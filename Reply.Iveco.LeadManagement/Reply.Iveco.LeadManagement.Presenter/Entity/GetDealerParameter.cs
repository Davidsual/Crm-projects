using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Iveco.LeadManagement.Presenter
{
    [Serializable()]
    public sealed class GetDealerParameter
    {
        /// <summary>
        /// Identificativo Lead
        /// </summary>
        public bool IsFlagCritico { get; set; }
        /// <summary>
        /// Dealer Company Name
        /// </summary>
        public int LeadType { get; set; }
        /// <summary>
        /// Dealer Code 
        /// </summary>
        public string ZipCode { get; set; }
        /// <summary>
        /// Dealer Responsible
        /// </summary>
        public Guid CountryId { get; set; }

    }
}
