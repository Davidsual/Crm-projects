using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Iveco.LeadManagement.Presenter.Model
{
    public sealed partial class Proposal
    {
        public Guid ServiceId { get; set; }
        public Guid LanguageId { get; set; }
        public DateTime StartSlotUniversalTime { get; set; }
        public DateTime StartSlotUserTime { get; set; }
        public DateTime EndSlotUniversalTime { get; set; }
        public DateTime EndSlotUserTime { get; set; }
        public ProposalUser ProposalUser { get; set; }

    }

    public sealed partial class ProposalUser
    {
        public enum TypeUser
        {
            User = 0,
            TeamLeader = 1
        } 

        public string Name { get; set; }
        public Guid IdUser { get; set; }
        public TypeUser TypeProposalUser { get; set; }
    }
}
