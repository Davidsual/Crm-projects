﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reply.Iveco.LeadManagement.Presenter.Model;

namespace Reply.Iveco.LeadManagement.Presenter
{
    [Serializable()]
    public sealed class CheckLoginResult
    {
        /// <summary>
        /// Indica se l'operazione è andata a buon fine
        /// </summary>
        public bool IsSuccessful { get; set; }
        /// <summary>
        /// Eventuale errore se issuccessful è a false
        /// </summary>
        public string ErrorDescription { get; set; }
        /// <summary>
        /// Codice Errore se issuccessful è a false
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// Cyurrent User
        /// </summary>
        public SystemUser CurrentUser { get; set; }
    }
}
