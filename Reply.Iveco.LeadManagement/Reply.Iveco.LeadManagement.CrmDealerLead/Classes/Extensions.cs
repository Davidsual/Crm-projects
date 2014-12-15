using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Crm.Sdk;
using Microsoft.Win32;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;

namespace Reply.Iveco.LeadManagement.CrmDealerLead.Classes
{
    public static class Extensions
    {
        public static string ToSqlGuid(this Guid _self)
        {
            return _self.ToString().Replace("{", "").Replace("}", "");
        }

        public static string ToSqlGuid(this string _self)
        {
            return _self.Replace("{", "").Replace("}", "");
        }

        public static string ToSqlInCondition(this string[] _self)
        {
            if (_self.Length == 0)
                return "('" + Guid.Empty.ToSqlGuid() + "')";

            StringBuilder _sb = new StringBuilder(" (");
            foreach (string _valId in _self)
            {
                _sb.Append("'" + _valId.ToSqlGuid() + "',");
            }

            return _sb.ToString().TrimEnd(new char[] { ',' }) + ") ";
        }

        public static string CorrectNegativeNumber(this string _self)
        {
            if (_self.Contains("-"))
            {
                return ("-" + _self.Replace("-", ""));
            }
            else
                return _self;
        }

        public static string ToJson(this object _self)
        {
            System.Web.Script.Serialization.JavaScriptSerializer jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return jsonSerializer.Serialize(_self);
        }

        public static string GetString(this DynamicEntity _self, string propertyName)
        {
            if (_self.Properties.Contains(propertyName))
                return (string)_self[propertyName];
            else
                return string.Empty;
        }
        public static void SetString(this DynamicEntity _self, string propertyName, string propertyValue)
        {
            if (_self.Properties.Contains(propertyName))
                _self[propertyName] = propertyValue;
            else
                _self.Properties.Add(new StringProperty(propertyName, propertyValue));
        }
    
        public static void SetDecimal(this DynamicEntity _self, string propertyName, decimal propertyValue)
        {
            if (_self.Properties.Contains(propertyName))
                ((CrmDecimal)_self[propertyName]).Value = propertyValue;
            else
                _self.Properties.Add(propertyValue.ToCrmDecimalProperty(propertyName));
        }
        public static decimal GetDecimal(this DynamicEntity _self, string propertyName)
        {
            if (_self.Properties.Contains(propertyName))
                return ((CrmDecimal)_self[propertyName]).Value;
            else
                return 0;
        }
        
        public static decimal GetMoney(this DynamicEntity _self, string propertyName)
        {
            if (_self.Properties.Contains(propertyName))
                return ((CrmMoney)_self[propertyName]).Value;
            else
                return 0;
        }
        public static void SetMoney(this DynamicEntity _self, string propertyName, decimal propertyValue)
        {
            if (_self.Properties.Contains(propertyName))
                ((CrmMoney)_self[propertyName]).Value = propertyValue;
            else
                _self.Properties.Add(propertyValue.ToCrmMoneyProperty(propertyName));
        }
        
        public static Guid GetPrimaryKey(this DynamicEntity _self)
        {
            string propertyName = _self.Name + "id";
            if (_self.Properties.Contains(propertyName))
                return ((Key)_self[propertyName]).Value;
            else
                return Guid.Empty;
        }
        public static void SetPrimaryKey(this DynamicEntity _self, Guid keyValue)
        {
            string propertyName = _self.Name + "id";
            if (_self.Properties.Contains(propertyName))
                ((Key)_self[propertyName]).Value = keyValue;
            else
                _self.Properties.Add(new KeyProperty(propertyName, new Key(keyValue)));
        }
        
        public static bool GetBoolean(this DynamicEntity _self, string propertyName)
        {
            if (_self.Properties.Contains(propertyName))
                return ((CrmBoolean)_self[propertyName]).Value;
            else
                return false;
        }
        public static void SetBoolean(this DynamicEntity _self, string propertyName, bool propertyValue)
        {
            if (_self.Properties.Contains(propertyName))
                ((CrmBoolean)_self[propertyName]).Value = propertyValue;
            else
                _self.Properties.Add(propertyValue.ToCrmBooleanProperty(propertyName));
        }
        
        public static Guid GetLookup(this DynamicEntity _self, string propertyName)
        {
            if (_self.Properties.Contains(propertyName))
                return ((Lookup)_self[propertyName]).Value;
            else
                return Guid.Empty;
        }
        public static void SetLookup(this DynamicEntity _self, string propertyName, string targetEntityName,Guid propertyValue)
        {
            if (_self.Properties.Contains(propertyName))
                ((Lookup)_self[propertyName]).Value = propertyValue;
            else
                _self.Properties.Add(new LookupProperty(propertyName, new Lookup(targetEntityName,propertyValue)));

        }

        public static int GetPicklist(this DynamicEntity _self, string propertyName)
        {
            if (_self.Properties.Contains(propertyName))
                return ((Picklist)_self[propertyName]).Value;
            else
                return -1;
        }
        

        public static Property ToCrmMoneyProperty(this decimal _self, string propertyName)
        {
            return new CrmMoneyProperty(propertyName, new CrmMoney(_self));
        }

        public static Property ToCrmDecimalProperty(this decimal _self, string propertyName)
        {
            return new CrmDecimalProperty(propertyName, new CrmDecimal(_self));
        }

        public static Property ToCrmBooleanProperty(this bool _self, string propertyName)
        {
            return new CrmBooleanProperty(propertyName, new CrmBoolean(_self));
        }

        public static void SetCrmConnectionString(this SqlConnection _self, HttpContext httpContext, string orgName)
        {
            if (httpContext == null)
            {
                _self.SetCrmConnectionString(orgName);
            }
            else
            {
                using (new CrmImpersonator())
                {
                    string _connectionString = string.Empty;
                    string _reqUrl = httpContext.Request.Url.ToString().ToUpperInvariant();
                    bool _isOffline = false;

                    //-- Provo a recuperare la connection string
                    try
                    {
                        //30 Novembre 2010: cambiato il criterio di valutazione offline / online

                        //-- Se sono offline
                        //string _cassiniPort = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\MSCRMClient").GetValue("CassiniPort").ToString();
                        string _offlineRegStatus = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\MSCRMClient").GetValue("SMOffline").ToString();

                        //if (_reqUrl.Contains("//localhost:" + _cassiniPort))
                        if (_offlineRegStatus.Equals("1"))
                        {
                            _connectionString = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\MSCRMClient").GetValue("Database").ToString().Replace("Provider=SQLOLEDB;", "");
                            _isOffline = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        //-- Il webserver online non ha la chiave CassiniPort...
                    }

                    //-- Se non sono in offline 
                    if (!_isOffline)
                    {
                        try
                        {
                            //-- Database di configurazione con l'elenco organizzazioni
                            string _configDbConnectionString = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\MSCRM").GetValue("configdb").ToString();

                            //-- Creo la connessione e la query da eseguire
                            SqlConnection _sqlConn = new SqlConnection(_configDbConnectionString);
                            _sqlConn.Open();
                            SqlCommand _sqlCommand = _sqlConn.CreateCommand();
                            _sqlCommand.CommandText = SqlQueryGenerator.GetConnectionString(orgName);
                            SqlDataReader _sqlReader = _sqlCommand.ExecuteReader();

                            //-- In un ciclo while per leggere l'ultimo (in teoria solo uno) e chiudere in automatico il reader
                            while (_sqlReader.Read())
                            {
                                _connectionString = _sqlReader[0].ToString().Replace("Provider=SQLOLEDB;", ""); ;
                            }

                            _sqlConn.Close();
                        }
                        catch (Exception ex2)
                        {

                        }
                    }

                    _self.ConnectionString = _connectionString;
                }
            }
        }

        public static void SetCrmConnectionString(this SqlConnection self, string orgName)
        {
            string _connectionString = string.Empty;
            bool _isOffline = false;

            //-- Se non sono in offline 
            if (!_isOffline)
            {
                try
                {
                    //-- Database di configurazione con l'elenco organizzazioni
                    string _configDbConnectionString = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\MSCRM").GetValue("configdb").ToString();

                    //-- Creo la connessione e la query da eseguire
                    SqlConnection _sqlConn = new SqlConnection(_configDbConnectionString);
                    _sqlConn.Open();
                    SqlCommand _sqlCommand = _sqlConn.CreateCommand();
                    _sqlCommand.CommandText = SqlQueryGenerator.GetConnectionString(orgName);
                    SqlDataReader _sqlReader = _sqlCommand.ExecuteReader();

                    //-- In un ciclo while per leggere l'ultimo (in teoria solo uno) e chiudere in automatico il reader
                    while (_sqlReader.Read())
                    {
                        _connectionString = _sqlReader[0].ToString().Replace("Provider=SQLOLEDB;", ""); ;
                    }

                    _sqlConn.Close();
                }
                catch (Exception)
                {

                }

                self.ConnectionString = _connectionString;
            }
        }


        /// <summary>
        /// Restituisce una stringa con il trim anche se è null (xml listino)
        /// </summary>
        /// <param name="_self"></param>
        /// <returns></returns>
        public static string MyTrim(this string _self)
        {
            if (_self == null)
                return string.Empty;
            else
                return _self.Trim();
        }
    }
}
