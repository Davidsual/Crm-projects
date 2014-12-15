using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Microsoft.Win32;
using Microsoft.Crm.Sdk;

namespace Reply.Iveco.LeadManagement.Utils.Classes
{
    public static class Extensions
    {
        public static bool ContainsId(this List<BusinessEntity> lista, Guid searchId, string keyName)
        {
            List<BusinessEntity> trovate = lista.FindAll(ent => ((Key)((DynamicEntity)ent)[keyName]).Value.Equals(searchId));
            return trovate.Count > 0;
        }

        public static Guid GetLookupValue(this DynamicEntity _self, string attributeName)
        {
            if (!_self.Properties.Contains(attributeName))
                return Guid.Empty;
            else
                return ((Lookup)_self[attributeName]).Value;

        }

        public static bool GetBooleanValue(this DynamicEntity _self, string attributeName)
        {
            if (!_self.Properties.Contains(attributeName))
                return false;
            else
                return ((CrmBoolean)_self[attributeName]).Value;

        }

        public static Guid GetPrimaryKey(this DynamicEntity _self)
        {
            KeyProperty _chiave = (KeyProperty)_self.Properties.ToList().Find(p => p.GetType().Equals(typeof(KeyProperty)));
            if (_chiave != null)
                return _chiave.Value.Value;
            else
                return Guid.Empty;
        }

        public static string ToSqlGuid(this Guid _self)
        {
            return _self.ToString().Replace("{", "").Replace("}", "");
        }

        public static string ToSqlGuid(this string _self)
        {
            return _self.Replace("{", "").Replace("}", "");
        }

        public static void SetCrmConnectionString(this SqlConnection _self, string orgName)
        {
            string _connectionString = string.Empty;
            try
            {
                //-- Database di configurazione con l'elenco organizzazioni
                string _configDbConnectionString = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\MSCRM").GetValue("configdb").ToString().Replace("Provider=SQLOLEDB;", "");

                //-- Creo la connessione e la query da eseguire
                SqlConnection _sqlConn = new SqlConnection(_configDbConnectionString);
                _sqlConn.Open();
                SqlCommand _sqlCommand = _sqlConn.CreateCommand();
                _sqlCommand.CommandText = SqlQueryStatement.GetConnectionString(orgName);
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

            _self.ConnectionString = _connectionString;

        }

    }
}
