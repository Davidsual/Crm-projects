using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Seat.DinamichePromozionali.Test.DinamichePromozionaliServicesWsdl
{
    public partial class GetStatusCampaignResult
    {
        public override string ToString()
        {
            StringBuilder _str = new StringBuilder();
            _str.Append("IsSuccessfull: " + this.IsSuccessfull.ToString() + Environment.NewLine);
            _str.Append("ChiamanteCampagnaID: " + this.ChiamanteCampagnaId.ToString() + Environment.NewLine);
            _str.Append("Privacy: " + this.PrivacyChiamanteCampagna + Environment.NewLine);
            _str.Append("StatoChiamanteCampagna: " + this.StatoChiamanteCampagna + Environment.NewLine);
            _str.Append("StatusCode: " + this.StatusCode + Environment.NewLine);
            _str.Append("StatusDescription: " + this.StatusDescription + Environment.NewLine);
            _str.Append("TestoMessaggioPushAP: " + this.TestoMessaggioPushAP + Environment.NewLine);
            _str.Append("TestoMessaggioRichiestaPrivacy: " + this.TestoMessaggioRichiestaPrivacy + Environment.NewLine);
            _str.Append("TestoMessaggioVincitaAPFisso: " + this.TestoMessaggioVincitaAPFisso + Environment.NewLine);
            _str.Append("TestoMessaggioVincitaAPMobile: " + this.TestoMessaggioVincitaAPMobile + Environment.NewLine);
            _str.Append("UrlLogoBanner: " + this.UrlLogoBanner + Environment.NewLine);
            _str.Append("UrlLogoCampagnaPush: " + this.UrlLogoCampagnaPush + Environment.NewLine);
            _str.Append("UrlLogoCampagnaVincita: " + this.UrlLogoCampagnaVincita + Environment.NewLine);
            _str.Append("UrlLogoPrivacy: " + this.UrlLogoPrivacy + Environment.NewLine);
            _str.Append(Environment.NewLine);
            return _str.ToString();
        }
    }
    public partial class GetManagementPrizeResult
    {
        public override string ToString()
        {
            StringBuilder _str = new StringBuilder();
            _str.Append("IsSuccessfull: " + this.IsSuccessfull.ToString() + Environment.NewLine);
            _str.Append("StatusCode: " + this.StatusCode + Environment.NewLine);
            _str.Append("StatusDescription: " + this.StatusDescription + Environment.NewLine);
            _str.Append(Environment.NewLine);
            return _str.ToString();
        }
    }

    public partial class SetPrivacyResult
    {
        public override string ToString()
        {
            StringBuilder _str = new StringBuilder();
            _str.Append("IsSuccessfull: " + this.IsSuccessfull.ToString() + Environment.NewLine);
            _str.Append("StatusCode: " + this.StatusCode + Environment.NewLine);
            _str.Append("StatusDescription: " + this.StatusDescription + Environment.NewLine);
            _str.Append(Environment.NewLine);
            return _str.ToString();
        }
    }
    public partial class SetOnlyPrivacyResult
    {
        public override string ToString()
        {
            StringBuilder _str = new StringBuilder();
            _str.Append("IsSuccessfull: " + this.IsSuccessfull.ToString() + Environment.NewLine);
            _str.Append("StatusCode: " + this.StatusCode + Environment.NewLine);
            _str.Append("StatusDescription: " + this.StatusDescription + Environment.NewLine);
            _str.Append(Environment.NewLine);
            return _str.ToString();
        }
    }
    public partial class GetChiamanteCampagnaResult
    {
        public override string ToString()
        {
            StringBuilder _str = new StringBuilder();
            _str.Append("IsSuccessfull: " + this.IsSuccessfull.ToString() + Environment.NewLine);
            _str.Append("StatusCode: " + this.StatusCode + Environment.NewLine);
            _str.Append("StatusDescription: " + this.StatusDescription + Environment.NewLine);
            _str.Append(Environment.NewLine);
            _str.Append("CodiceCifrato: " + this.CodiceCifrato + Environment.NewLine);
            _str.Append("StatoPartecipante: " + this.StatoPartecipante + Environment.NewLine);
            _str.Append("NomeCampagna: " + this.NomeCampagna + Environment.NewLine);
            _str.Append("Data fine campagna: " + this.DataFineCampagna + Environment.NewLine);
            _str.Append("NumChiamateResidue: " + this.NumChiamateResidue + Environment.NewLine);
            _str.Append("NumVinciteRimanenti: " + this.NumVinciteRimanenti + Environment.NewLine);
            _str.Append("NumChiamateEffetuate: " + this.NumChiamateEffetuate + Environment.NewLine);
            _str.Append("Privacy: " + this.Privacy + Environment.NewLine);
            _str.Append("NumSmsInviati: " + this.NumSmsInviati + Environment.NewLine);
            _str.Append("NumCodiciPromozionaliUsati: " + this.NumCodiciPromozionaliUsati + Environment.NewLine);
            _str.Append("NumChiamateGratuite: " + this.NumChiamateGratuite + Environment.NewLine);
            _str.Append("NumOggetti: " + this.NumOggetti + Environment.NewLine);
            return _str.ToString();

        }
    }
}
