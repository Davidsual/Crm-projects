﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Seat.DinamichePromozionali.BL
{
    [Serializable()]
    public sealed class GetManagementPrizeResult
    {
        #region PUBLIC PROPERTY
        public bool IsSuccessfull { get; set; }
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; } 
        #endregion
    }
}
