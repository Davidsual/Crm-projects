using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reply.Seat.DinamichePromozionali.DataAccess.Model;
using Reply.Seat.DinamichePromozionali.DataAccess;
using System.Web;

namespace Reply.Seat.DinamichePromozionali.BL
{
    /// <summary>
    /// Gestisce il flusso delle richiesta campagna
    /// </summary>
    public partial class Manager : BaseManager, IDisposable
    {
        #region PUBLIC METHODS
        /// <summary>
        /// Ritorna il chiamante campagna cone le sue informazioni
        /// </summary>
        /// <param name="idChiamanteCampagna"></param>
        /// <returns></returns>
        public GetChiamanteCampagnaResult GetChiamanteCampagna(Guid idChiamanteCampagna, HttpContext wsContext)
        {
            NewMessage(LogLevel.Message, "------------------------------------------------------------------------------------");
            NewMessage(LogLevel.Message, "Inizio GetChiamanteCampagna (richiesta ricevuta da " + wsContext.User.Identity.Name + "@" + wsContext.Request.UserHostAddress + ") con parametri "
                                         + idChiamanteCampagna);

            GetChiamanteCampagnaResult _result = new GetChiamanteCampagnaResult();
            ///Controllo parametro in ingresso
            if (idChiamanteCampagna == Guid.Empty)
            {
                _result.IsSuccessfull = false;
                _result.StatusCode = StatusTranscoding.STATUS_CODE_MISS_PARAMETERS;
                _result.StatusDescription = StatusTranscoding.STATUS_DESC_MISS_PARAMETERS;
                return _result;
            }
            
            try
            {
                ///Ottiene un chiamantecampagna dato il suo id
                New_chiamantecampagna _chiamanteCampagna = this.CurrentAccessLayer.GetChiamanteCampagnaById(idChiamanteCampagna);
                ///Controllo di aver trovato un chiamante campagna
                if (_chiamanteCampagna == null)
                {
                    _result.IsSuccessfull = false;
                    _result.StatusCode = StatusTranscoding.STATUS_CODE_CHIMANTE_CAMPAGNA_NOT_FOUND;
                    _result.StatusDescription = StatusTranscoding.STATUS_DESC_CHIMANTE_CAMPAGNA_NOT_FOUND;
                    return _result;
                }
                if (!_chiamanteCampagna.New_CampagnaId.HasValue)
                {
                    _result.IsSuccessfull = false;
                    _result.StatusCode = StatusTranscoding.STATUS_CODE_ID_CAMPAGNA_NOT_FOUND;
                    _result.StatusDescription = StatusTranscoding.STATUS_DESC_ID_CAMPAGNA_NOT_FOUND;
                    return _result;
                }
                ///Estraggo la campagna per recuperare la data fine campagna
                Campaign _campaign = this.CurrentAccessLayer.GetCampaignById(_chiamanteCampagna.New_CampagnaId.Value);

                if (_campaign == null)
                {
                    _result.IsSuccessfull = false;
                    _result.StatusCode = StatusTranscoding.STATUS_CODE_CAMPAGNA_NOT_FOUND;
                    _result.StatusDescription = StatusTranscoding.STATUS_DESC_CAMPAGNA_NOT_FOUND;
                    return _result;
                }
                ///Recupero la campagna associata
                _result.IsSuccessfull = true;
                _result.StatusCode = StatusTranscoding.STATUS_CODE_OK;
                _result.StatusDescription = StatusTranscoding.STATUS_DESC_OK;
                ///Imposto a int.MinValue i campi interi che sul database hanno valore NULL
                _result.NomeCampagna = _chiamanteCampagna.New_CampagnaIdName;
                _result.DataFineCampagna = (_campaign.ActualEnd.HasValue)?_campaign.ActualEnd.Value.Date.ToString("dd/MM/yyyy"):string.Empty;
                _result.CodiceCifrato = _chiamanteCampagna.New_name;
                _result.NumChiamateEffetuate = (_chiamanteCampagna.New_ChiamateEffettuate.HasValue) ? _chiamanteCampagna.New_ChiamateEffettuate.Value : int.MinValue;
                _result.NumChiamateResidue = (_chiamanteCampagna.New_ChiamateResidue.HasValue) ? _chiamanteCampagna.New_ChiamateResidue.Value : int.MinValue;
                _result.NumVinciteRimanenti = (_chiamanteCampagna.New_VinciteRimanenti.HasValue)?_chiamanteCampagna.New_VinciteRimanenti.Value:int.MinValue;
                ///Calcolo i valori eseguendo delle count dalle tabelle associate al chiamante campagna
                _result.NumChiamateGratuite = this.CurrentAccessLayer.GetNumChiamateGratuiteByIdChiamanteCampagna(_chiamanteCampagna.New_chiamantecampagnaId);
                _result.NumCodiciPromozionaliUsati = this.CurrentAccessLayer.GetNumCodiciPromozionaliUsatiByIdChiamanteCampagna(_chiamanteCampagna.New_chiamantecampagnaId);
                _result.NumOggetti = this.CurrentAccessLayer.GetNumOggettiByIdChiamanteCampagna(_chiamanteCampagna.New_chiamantecampagnaId);
                _result.NumSmsInviati = this.CurrentAccessLayer.GetNumSmsInviatiByIdChiamanteCampagna(_chiamanteCampagna.New_chiamantecampagnaId);
                ///Valore della privacy e dello stato in cui si trova il partecipante
                _result.Privacy = (_chiamanteCampagna.New_Privacy.HasValue)?((DataAccessLayer.Privacy)Enum.ToObject(typeof(DataAccessLayer.Privacy), _chiamanteCampagna.New_Privacy.Value)).ToString().ToUpper():string.Empty;
                _result.StatoPartecipante = (_chiamanteCampagna.New_StatoPartecipante.HasValue)?((DataAccessLayer.StatoPartecipanteInChiamanteCampagna)Enum.ToObject(typeof(DataAccessLayer.StatoPartecipanteInChiamanteCampagna), _chiamanteCampagna.New_StatoPartecipante.Value)).ToString().ToUpper():string.Empty;

                NewMessage(LogLevel.Message, "Fine GetChiamanteCampagna (richiesta ricevuta da " + wsContext.User.Identity.Name + "@" + wsContext.Request.UserHostAddress + ") con parametri "
                             + idChiamanteCampagna);
                NewMessage(LogLevel.Message, "");
                
                ///Ritorno l'oggetto popolato
                return _result;
            }
            catch(Exception ex)
            {
                ///Loggo eventuali exception
                NewMessage(LogLevel.Message, "Errore durante l'esecuzione del metodo GetChiamanteCampagna " + ex.Message);
                throw;
            }
        }
        #endregion

    }
}
