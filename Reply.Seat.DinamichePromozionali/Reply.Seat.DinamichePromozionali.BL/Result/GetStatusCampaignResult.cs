using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Seat.DinamichePromozionali.BL
{
    [Serializable()]
    public sealed class GetStatusCampaignResult
    {
        #region PUBLIC PROPERTY
        public bool IsSuccessfull { get; set; }
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }

        public Guid ChiamanteCampagnaId { get; set; }

        public string UrlLogoBanner { get; set; }
        public string UrlLogoPrivacy { get; set; }
        public string UrlLogoCampagnaPush { get; set; }
        public string UrlLogoCampagnaVincita { get; set; }

        public string TestoMessaggioVincitaAPFisso { get; set; }
        public string TestoMessaggioVincitaAPMobile { get; set; }
        public string TestoMessaggioPushAP { get; set; }
        public string TestoMessaggioRichiestaPrivacy { get; set; }

        public string StatoChiamanteCampagna { get; set; }
        public string PrivacyChiamanteCampagna { get; set; }
        public string ModalitaComunicazionePremio { get; set; }
        #endregion
    }
}
