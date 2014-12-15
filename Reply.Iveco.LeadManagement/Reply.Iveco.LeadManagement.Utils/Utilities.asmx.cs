using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Data.SqlClient;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy;
using Reply.Iveco.LeadManagement.Utils.Classes;
using Reply.Iveco.LeadManagement.Utils.Entities;
using System.Xml;

namespace Reply.Iveco.LeadManagement.Utils
{
    /// <summary>
    /// Summary description for Utilities
    /// </summary>
    [WebService(Namespace = "http://schemas.microsoft.com/crm/2006/WebServices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [ScriptService]
    public class Utilities : System.Web.Services.WebService
    {
        #region Labels

        [WebMethod]
        public string GetUILabel(string LabelId, string LangCode)
        {
            string filename = Server.MapPath("labels.xml");

            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            try
            {
                XmlNode nodo = doc.SelectSingleNode("//" + LabelId + "//Text[@lang='" + LangCode + "']");
                return nodo.InnerText;
            }
            catch (Exception ex)
            {
                return LabelId;
            }
        }

        [WebMethod]
        public string[] GetUILabelArray(string[] LabelIds, string LangCode)
        {
            string filename = Server.MapPath("labels.xml");

            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            string[] res = new string[LabelIds.Length];
            for (int i = 0; i < res.Length; i++)
            {
                try
                {
                    XmlNode nodo = doc.SelectSingleNode("//" + LabelIds[i] + "//Text[@lang='" + LangCode + "']");
                    res[i] = nodo.InnerText;
                }
                catch (Exception ex)
                {
                    res[i] = LabelIds[i];
                }
            }

            return res;
        }


        #endregion

        #region Gestione QueueItem

        Dictionary<string, string> GetQueueItemInfo(string orgName, string userId, string[] data, string[] valoriPicklist)
        {
            Dictionary<string, string> _res = new Dictionary<string, string>();
            try
            {
                List<string> _casi = data.ToList().FindAll(d => d.EndsWith(",112"));
                List<string> _activity = data.ToList().FindAll(d => !d.EndsWith(",112"));
                
                //string _queryTotale = SqlQueryStatement.GetIncidentInfo(_casi) + " union " + SqlQueryStatement.GetActivityInfo(_activity);
                string _queryTotale = SqlQueryStatement.GetActivityInfo(_activity, userId);

                using (new CrmImpersonator())
                {
                    using (SqlConnection _sqlConn = new SqlConnection())
                    {
                        _sqlConn.SetCrmConnectionString(orgName);
                        _sqlConn.Open();

                        using (SqlCommand _sqlCommand = _sqlConn.CreateCommand())
                        {
                            _sqlCommand.CommandText = _queryTotale;

                            SqlDataReader _reader = _sqlCommand.ExecuteReader();
                            List<QueueItemInfo> _parsedReader = QueueItemInfo.FromSqlReader(_reader, valoriPicklist);
                            foreach (QueueItemInfo _queueItem in _parsedReader)
                            {
                                //Log.Debug("Aggiungo '" + _queueItem.QueueItemKey + "'");
                                _res.Add(_queueItem.QueueItemKey, _queueItem.ToString());
                            }

                        }

                        _sqlConn.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return _res;
        }


        [WebMethod]
        public string[] GetQueueItemListHTML(string orgName, string userId, string langCode, string[] data)
        {
            // "Creato da;Creato da BU;Argomento;Proprietario;Numero caso";
            string _intestazione = GetUILabel("QUEUEITEMS_HEADER", langCode);
            string[] _valoriPicklist = GetUILabelArray(new string[]{
                    "RESULT_CONTACT_0",
                    "RESULT_CONTACT_1",
                    "RESULT_CONTACT_2",
                    "RESULT_CONTACT_3",
                    "RESULT_CONTACT_4",
                    "RESULT_CONTACT_5",
                }, langCode);

            using (new Microsoft.Crm.Sdk.CrmImpersonator())
            {
                List<string> result = new List<string>();

                //PRIMA RIGA = INTESTAZIONI COLONNE
                result.Add(_intestazione);

                try
                {
                    Dictionary<string, string> _itemInfo = GetQueueItemInfo(orgName, userId, data, _valoriPicklist);

                    foreach (string element in data)
                    {
                        string td = new QueueItemInfo(element).ToString();
                        try
                        {
                            //SE E' UNA DELLE ENTITY SUPPORTATE
                            if (_itemInfo.ContainsKey(element))
                            {
                                td = _itemInfo[element];
                            }
                        }
                        catch (Exception ex) { }
                        result.Add(td);
                    }
                }
                catch (Exception ex)
                {
                    string[] resErr = new string[data.Length];
                    resErr[0] = _intestazione;
                    return resErr;
                }

                return result.ToArray();
            }
        }


        #endregion
    }
}
