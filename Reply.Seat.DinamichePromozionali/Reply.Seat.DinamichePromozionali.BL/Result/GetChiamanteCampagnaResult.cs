using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Seat.DinamichePromozionali.BL
{
    [Serializable()]
    public sealed class GetChiamanteCampagnaResult
    {
        #region PUBLIC PROPERTY
        public bool IsSuccessfull { get; set; }
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; } 

        public string CodiceCifrato {get;set;}
        public string StatoPartecipante{get;set;}
        public string NomeCampagna {get;set;}
        public string DataFineCampagna { get; set; }
        public int NumChiamateResidue {get;set;}
        public int NumVinciteRimanenti {get;set;}
        public int NumChiamateEffetuate {get;set;}
        public string Privacy {get;set;}
        public int NumSmsInviati {get;set;}
        public int NumCodiciPromozionaliUsati {get;set;}
        public int NumChiamateGratuite {get;set;}
        public int NumOggetti{get;set;}
        #endregion
    }
}
