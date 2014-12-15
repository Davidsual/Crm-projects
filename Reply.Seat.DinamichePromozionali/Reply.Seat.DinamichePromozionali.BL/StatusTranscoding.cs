using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reply.Seat.DinamichePromozionali.CrmAdapter.CrmServicePerformance;

namespace Reply.Seat.DinamichePromozionali.BL
{
    /// <summary>
    /// Codici e descrizione bus con voice finder
    /// </summary>
    public static class StatusTranscoding
    {
        #region DESCRIPTION
        
        #endregion

        public const string OK = "OK";
        public const string KO = "KO";

        public const string STATUS_CODE_UNEXPECTED_ERROR = "999";

        public const string STATUS_CODE_MISS_PARAMETERS = "000";
        public const string STATUS_DESC_MISS_PARAMETERS = "Parametri passati insufficienti";
        public const string STATUS_CODE_MISS_CAMPAIGN = "001";
        public const string STATUS_DESC_MISS_CAMPAIGN = "Campagna non trovata per il callType passato come parametro";
        public const string STATUS_CODE_MISS_CALLTYPE = "002";
        public const string STATUS_DESC_MISS_CALLTYPE = "CallType non trovato per il callType passato come parametro";
        public const string STATUS_CODE_MISS_TEMPLATE_PREMIO = "003";
        public const string STATUS_DESC_MISS_TEMPLATE_PREMIO = "Nessun template premio trovato per la campagna corrente";
        public const string STATUS_CODE_NULL_PREMIO_RICORRENTE = "004";
        public const string STATUS_DESC_NULL_PREMIO_RICORRENTE = "Valore NULL per il valore del premio ricorrente nel template premio";
        public const string STATUS_CODE_NULL_PRIVACY_LEAD = "005";
        public const string STATUS_DESC_NULL_PRIVACY_LEAD = "Valore NULL per il flag privacy sul partecipante";
        public const string STATUS_CODE_NULL_CHIAMATE_SOGLIA = "006";
        public const string STATUS_DESC_NULL_CHIAMATE_SOGLIA = "Valore NULL per il numero di chiamate soglia all'interno della campagna";
        public const string STATUS_CODE_CREATE_CHIAMANTE_CAMPAGNA = "007";
        public const string STATUS_DESC_CREATE_CHIAMANTE_CAMPAGNA = "Errore durante la creazione del chiamante campagna";
        public const string STATUS_CODE_OK = "008";
        public const string STATUS_DESC_OK = "Completato con successo";
        public const string STATUS_CODE_CREATE_CALL = "009";
        public const string STATUS_DESC_CREATE_CALL = "Errore durante la creazione della CALL";
        public const string STATUS_CODE_NULL_DATA_INIZIO_FASCIA1 = "010";
        public const string STATUS_DESC_NULL_DATA_INIZIO_FASCIA1 = "Valore NULL per la data inizio fascia 1 all'interno della campagna";
        public const string STATUS_CODE_NULL_DATA_INIZIO_FASCIA2 = "011";
        public const string STATUS_DESC_NULL_DATA_INIZIO_FASCIA2 = "Valore NULL per la data inizio fascia 2 all'interno della campagna";
        public const string STATUS_CODE_NULL_DATA_INIZIO_FASCIA3 = "012";
        public const string STATUS_DESC_NULL_DATA_INIZIO_FASCIA3 = "Valore NULL per la data inizio fascia 3 all'interno della campagna";
        public const string STATUS_CODE_NULL_DATA_FINE_FASCIA1 = "013";
        public const string STATUS_DESC_NULL_DATA_FINE_FASCIA1 = "Valore NULL per la data fine fascia 1 all'interno della campagna";
        public const string STATUS_CODE_NULL_DATA_FINE_FASCIA2 = "014";
        public const string STATUS_DESC_NULL_DATA_FINE_FASCIA2 = "Valore NULL per la data fine fascia 2 all'interno della campagna";
        public const string STATUS_CODE_NULL_DATA_FINE_FASCIA3 = "015";
        public const string STATUS_DESC_NULL_DATA_FINE_FASCIA3 = "Valore NULL per la data fine fascia 3 all'interno della campagna";
        public const string STATUS_CODE_NULL_STATO_PARTECIPANTE = "016";
        public const string STATUS_DESC_NULL_STATO_PARTECIPANTE = "Valore NULL per lo stato partecipante all'interno del chiamante campagna";
        public const string STATUS_CODE_RIFIUTO_PRIVACY_CHIAMANTE_CAMPAGNA = "017";
        public const string STATUS_DESC_RIFIUTO_PRIVACY_CHIAMANTE_CAMPAGNA = "Rifiuto privacy impostato sul chiamante campagna";
        public const string STATUS_CODE_GIA_VINCITORE_CAMPAGNA = "018";
        public const string STATUS_DESC_GIA_VINCITORE_CAMPAGNA = "Il chiamante campagna risulta essere già vincitore di una campagna";
        public const string STATUS_CODE_NULL_PRIVACY_CHIAMANTE_CAMPAGNA = "019";
        public const string STATUS_DESC_NULL_PRIVACY_CHIAMANTE_CAMPAGNA = "Nessun valore presente sulla privacy per il chiamante campagna";
        public const string STATUS_CODE_NONSO_PRIVACY_CHIAMANTE_CAMPAGNA = "020";
        public const string STATUS_DESC_NONSO_PRIVACY_CHIAMANTE_CAMPAGNA = "Flag privacy su chiamante campagna impostato a NON SO";
        public const string STATUS_CODE_NULL_OWNER_CAMPAGNA = "021";
        public const string STATUS_DESC_NULL_OWNER_CAMPAGNA = "Valore NULL owner all'interno della campagna";
        public const string STATUS_CODE_CHIAMANTE_CAMPAGNA_NOT_FOUND = "022";
        public const string STATUS_DESC_CHIAMANTE_CAMPAGNA_NOT_FOUND = "Chiamante campagna non trovato per id passato";
        public const string STATUS_CODE_LEAD_NOT_FOUND = "023";
        public const string STATUS_DESC_LEAD_NOT_FOUND = "Partecipante non trovato";
        public const string STATUS_CODE_MISS_CODICE_PROMOZIONALE = "024";
        public const string STATUS_DESC_MISS_CODICE_PROMOZIONALE = "Nessun codice promozionale presente";
        public const string STATUS_CODE_TIPOLOGIA_PREMIO_NON_GESTITA = "025";
        public const string STATUS_DESC_TIPOLOGIA_PREMIO_NON_GESTITA = "Tipologia premio non gestita";
        public const string STATUS_CODE_NULL_DATA_ACTUALSTART = "026";
        public const string STATUS_DESC_NULL_DATA_ACTUALSTART = "Valore NULL per la data actualstart all'interno della campagna";
        public const string STATUS_CODE_NULL_DATA_ACTUALEND = "027";
        public const string STATUS_DESC_NULL_DATA_ACTUALEND = "Valore NULL per la data actualend all'interno della campagna";
        public const string STATUS_CODE_NULL_QUANTITA = "028";
        public const string STATUS_DESC_NULL_QUANTITA = "Valore NULL per la quantita all'interno del template premio";
        public const string STATUS_CODE_NULL_GIORNI_VALIDITA = "029";
        public const string STATUS_DESC_NULL_GIORNI_VALIDITA = "Valore NULL per i giorni di validità all'interno del template premio";
        public const string STATUS_CODE_VINCITE_RIMANENTI_NEGATIVO = "031";
        public const string STATUS_DESC_VINCITE_RIMANENTI_NEGATIVO = "Il valore delle vincite rimanenti è uguale a 0 ma lo stato del chimante campagna è diverso da vincitore";
        public const string STATUS_CODE_VINCITORE_MANUALE = "032";
        public const string STATUS_DESC_VINCITORE_MANUALE = "Vincitore del premio in modalità manuale";

        //public const string STATUS_CODE_PREMIO_CONCORSO = "033";
        //public const string STATUS_DESC_PREMIO_CONCORSO = "Premio concorso vinto";
        public const string STATUS_CODE_POTENZIALE_VINCITORE_PREMIO_CONCORSO = "034";
        public const string STATUS_DESC_POTENZIALE_VINCITORE_PREMIO_CONCORSO = "Uscito come potenziale vincitore";
        public const string STATUS_CODE_PREMIO_CONCORSO = "035";
        public const string STATUS_DESC_PREMIO_CONCORSO = "Premio concorso vinto";
        public const string STATUS_CODE_MISS_CHIAMANTE_CAMPAGNA = "036";
        public const string STATUS_DESC_MISS_CHIAMANTE_CAMPAGNA = "Chiamante campagna non trovato";
        public const string STATUS_CODE_NO_PRIVACY_CHIAMANTE_CAMPAGNA = "037";
        public const string STATUS_DESC_NO_PRIVACY_CHIAMANTE_CAMPAGNA = "Flag privacy su chiamante campagna impostato a NO";
        public const string STATUS_CODE_PHONENUMBER_NON_PERMESSO = "038";
        public const string STATUS_DESC_PHONENUMBER_NON_PERMESSO = "Il numero di telefono passato non è previsto dal template premio come tipologia di telefono valida.";
        public const string STATUS_CODE_TIPO_TARGET_NOT_FOUND = "039";
        public const string STATUS_DESC_TIPO_TARGET_NOT_FOUND = "Tipo target della campagna non trovato";
        public const string STATUS_CODE_CHIMANTE_CAMPAGNA_NOT_FOUND = "040";
        public const string STATUS_DESC_CHIMANTE_CAMPAGNA_NOT_FOUND = "Chiamante Campagna non trovato per l'ID passato come parametro";
        public const string STATUS_CODE_NULL_DATA_ATTIVAZIONE = "041";
        public const string STATUS_DESC_NULL_DATA_ATTIVAZIONE = "Valore NULL per la data attivazione all'interno del template premio";
        public const string STATUS_CODE_NULL_DATA_ATTIVAZIONE_PREMIO = "042";
        public const string STATUS_DESC_NULL_DATA_ATTIVAZIONE_PREMIO = "Valore NULL per la data attivazione premio all'interno del template premio";
        public const string STATUS_CODE_ID_CAMPAGNA_NOT_FOUND = "043";
        public const string STATUS_DESC_ID_CAMPAGNA_NOT_FOUND = "Id Campagna non presente nel chiamante campagna";
        public const string STATUS_CODE_CAMPAGNA_NOT_FOUND = "044";
        public const string STATUS_DESC_CAMPAGNA_NOT_FOUND = "Campagna non presente per Id Campagna passato";
        public const string STATUS_CODE_CAMPAGNA_NOT_ACTIVE = "045";
        public const string STATUS_DESC_CAMPAGNA_NOT_ACTIVE = "La campagna trovata non è attiva per la data attuale";

    }
}
