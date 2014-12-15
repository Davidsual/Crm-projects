using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reply.Seat.DinamichePromozionali.DataAccess;
using Reply.Seat.DinamichePromozionali.DataAccess.Model;

namespace Reply.Seat.DinamichePromozionali.BL
{
    public static class TagTranscoding
    {
        #region PUBLIC STATIC METHODS
        /// <summary>
        /// Restituisce la string trascodificata
        /// </summary>
        /// <param name="inputValue"></param>
        /// <param name="campagna"></param>
        /// <param name="chiamanteCampagna"></param>
        /// <param name="codPromozionale"></param>
        /// <returns></returns>
        public static string GetTranscoding(string inputValue, Campaign campagna, New_chiamantecampagna chiamanteCampagna,New_premio templatePremio, string codPromozionale)
        {
            if (string.IsNullOrEmpty(inputValue))
                return inputValue;
           ///Aggiungo codice promozionale
            if (!string.IsNullOrEmpty(codPromozionale))
                inputValue = inputValue.Replace(Tag.CODICE_PROMOZIONALE, codPromozionale);

            if (campagna != null)
            {
                if(campagna.New_NumeroChiamateSoglia.HasValue)
                    inputValue = inputValue.Replace(Tag.CALL_SOGLIA, campagna.New_NumeroChiamateSoglia.Value.ToString());
                if(campagna.ActualEnd.HasValue)
                    inputValue =  inputValue.Replace(Tag.DATA_SCADENZA_CAMPAGNA, campagna.ActualEnd.Value.ToShortDateString());
                if(!string.IsNullOrEmpty(campagna.Name))
                    inputValue = inputValue.Replace(Tag.NOME_CAMPAGNA, campagna.Name);

                if (chiamanteCampagna != null)
                {
                    if(chiamanteCampagna.New_ChiamateResidue.HasValue && campagna.New_NumeroChiamateSoglia.HasValue)
                    {
                        inputValue = inputValue.Replace(Tag.CALL_EFFETTUATE, (campagna.New_NumeroChiamateSoglia.Value - chiamanteCampagna.New_ChiamateResidue.Value).ToString());
                    }
                    if(chiamanteCampagna.New_ChiamateResidue.HasValue)
                        inputValue = inputValue.Replace(Tag.CALL_RIMANENTI, chiamanteCampagna.New_ChiamateResidue.Value.ToString());
                    ///Valori delle vincite
                    if (chiamanteCampagna.New_VinciteRimanenti.HasValue)
                    {
                        inputValue = inputValue.Replace(Tag.VINCITE_RIMANENTI, chiamanteCampagna.New_VinciteRimanenti.Value.ToString());
                        if (templatePremio != null && templatePremio.New_PremioRicorrente.HasValue && chiamanteCampagna.New_VinciteRimanenti.HasValue)
                        {
                            inputValue = inputValue.Replace(Tag.VINCITE_SOGLIA, Convert.ToInt32(templatePremio.New_PremioRicorrente.Value).ToString());
                            inputValue = inputValue.Replace(Tag.VINCITE_EFFETTUATE, (Convert.ToInt32(templatePremio.New_PremioRicorrente.Value) - chiamanteCampagna.New_VinciteRimanenti.Value).ToString());
                        }
                    }
                }
            }

            return inputValue;
        }
        /// <summary>
        /// Replace della stringa trascodificata con i valori del current
        /// </summary>
        /// <param name="inputValue"></param>
        /// <param name="currentTag"></param>
        /// <returns></returns>
        public static string GetTranscoding(string inputValue, Tag currentTag)
        {
            foreach (KeyValuePair<string,string> item in currentTag.CurrentTagList)
	        {
                inputValue = inputValue.Replace(item.Key, item.Value);
	        }
            return inputValue;
        }
        #endregion
    }
    /// <summary>
    /// Tag Mappati
    /// </summary>
    public sealed class Tag
    {
        #region PRIVATE MEMBERS
        private Dictionary<string, string> _currentTagList = null;

        #endregion

        #region PUBLIC MEMBERS
        public const string CALL_EFFETTUATE = "[Call Effettuate]";
        public const string CALL_RIMANENTI = "[Call Rimanenti]";
        public const string CALL_SOGLIA = "[Call Soglia]";
        public const string DATA_SCADENZA_CAMPAGNA = "[Data Scadenza Campagna]";
        public const string NOME_CAMPAGNA = "[Nome Campagna]";
        public const string CODICE_PROMOZIONALE = "[Codice Promozionale]";
        public const string VINCITE_RIMANENTI = "[Vincite Rimanenti]";
        public const string VINCITE_EFFETTUATE = "[Vincite Effettuate]";
        public const string VINCITE_SOGLIA = "[Vincite Soglia]";
        /// <summary>
        /// CodTag
        /// </summary>
        public enum CodiceTag
        {
            AggiungiCallEffettuate,
            AggiungiCallRimanenti,
            AggiungiCallSoglia,
            AggiungiDataScadenzaCampagna,
            AggiungiNomeCampagna,
            AggiungiCodicePromozionale,
            AggiungiVinciteRimanenti,
            AggiungiVinciteEffettuate,
            AggiungiVinciteSoglia
        }
        #endregion

        #region PUBLIC PROPERTY
        /// <summary>
        /// Lista di tag e valore
        /// </summary>
        public Dictionary<string, string> CurrentTagList
        {
            get { return _currentTagList; }
        }
        #endregion

        #region CTOR
        /// <summary>
        /// Costruttore
        /// </summary>
        public Tag()
        {
            _currentTagList = new Dictionary<string, string>();
        }
        #endregion

        #region PUBLIC MEMBERS
        /// <summary>
        /// Aggiunge un valore
        /// </summary>
        /// <param name="codiceTag"></param>
        /// <param name="value"></param>
        public void AddTag(CodiceTag codiceTag, string value)
        {
            switch (codiceTag)
            {
                case CodiceTag.AggiungiCallEffettuate:
                    _currentTagList[CALL_EFFETTUATE] = value;
                    break;
                case CodiceTag.AggiungiCallRimanenti:
                    _currentTagList[CALL_RIMANENTI] = value;
                    break;
                case CodiceTag.AggiungiCallSoglia:
                    _currentTagList[CALL_SOGLIA] = value;
                    break;
                case CodiceTag.AggiungiDataScadenzaCampagna:
                    _currentTagList[DATA_SCADENZA_CAMPAGNA] = value;
                    break;
                case CodiceTag.AggiungiNomeCampagna:
                    _currentTagList[NOME_CAMPAGNA] = value;
                    break;
                case CodiceTag.AggiungiCodicePromozionale:
                    _currentTagList[CODICE_PROMOZIONALE] = value;
                    break;
                case CodiceTag.AggiungiVinciteEffettuate:
                    _currentTagList[VINCITE_EFFETTUATE] = value;
                    break;
                case CodiceTag.AggiungiVinciteRimanenti:
                    _currentTagList[VINCITE_RIMANENTI] = value;
                    break;
                case CodiceTag.AggiungiVinciteSoglia:
                    _currentTagList[VINCITE_SOGLIA] = value;
                    break;               
                default:
                    break;
            }
        }
        #endregion
    }
}
