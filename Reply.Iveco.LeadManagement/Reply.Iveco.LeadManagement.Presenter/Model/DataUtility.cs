using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using System.Data.SqlClient;
using System.Web;
using Microsoft.Win32;
using Microsoft.Crm.SdkTypeProxy;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using Microsoft.Crm.Sdk.Query;
using System.Configuration;

namespace Reply.Iveco.LeadManagement.Presenter.Model
{
    /// <summary>
    /// Classe di utility per l'accesso ai dati e il consumo dei webservice
    /// </summary>
    public static class DataUtility
    {

        public static List<BusinessEntity> RetrieveMultipleDynamic(CrmService service, string[] attributeNames, object[] values, string searchEntityName, string sort)
        {
            int numCondizioni = attributeNames.Length;

            //Preparo la query con le info di base
            QueryExpression query = new QueryExpression();
            query.EntityName = searchEntityName;
            query.ColumnSet = new AllColumns();

            if (!string.IsNullOrEmpty(sort))
            {
                OrderExpression ordine = new OrderExpression(sort, OrderType.Ascending);
                query.Orders.Add(ordine);
            }

            //Preparo il filtro
            FilterExpression filter = new FilterExpression();
            filter.FilterOperator = LogicalOperator.And;
            //filter.Conditions.Add(numCondizioni);// { condition };

            for (int i = 0; i < numCondizioni; i++)
            {
                ConditionExpression condition = new ConditionExpression();
                condition.AttributeName = attributeNames[i];
                condition.Operator = ConditionOperator.Equal;
                condition.Values = new object[] { values[i] };
                filter.Conditions.Add(condition);
            }

            //Imposto il filtro creato sulla query
            query.Criteria = filter;

            RetrieveMultipleRequest richiesta = new RetrieveMultipleRequest();
            richiesta.ReturnDynamicEntities = true;
            richiesta.Query = query;
            RetrieveMultipleResponse risposta = (RetrieveMultipleResponse)service.Execute(richiesta);

            return risposta.BusinessEntityCollection.BusinessEntities;
        }
        /// <summary>
        /// Deserializza la string xml in una classe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T DeserializeXmlToClass<T>(this string xml)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (XmlTextReader reader = new XmlTextReader(new StreamReader(new MemoryStream(ASCIIEncoding.ASCII.GetBytes(xml)))))
            {
                return (T)ser.Deserialize(reader);
            }
        }
        /// <summary>
        /// Ottiene una guid dall'oggetto reader
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Guid GetGuidFromReader(object obj)
        {
            if (obj == null || !(obj is Guid))
                return Guid.Empty;
            return new Guid(obj.ToString());
        }
        /// <summary>
        /// imposta la guid per i parametri sql
        /// </summary>
        /// <param name="_self"></param>
        /// <returns></returns>
        public static string ToSqlGuid(this Guid _self)
        {
            return _self.ToString().Replace("{", "").Replace("}", "");
        }
        /// <summary>
        /// imposta la guid per i parametri sql
        /// </summary>
        /// <param name="_self"></param>
        /// <returns></returns>
        public static string ToSqlGuid(this string _self)
        {
            return _self.Replace("{", "").Replace("}", "");
        }
        /// <summary>
        /// Imposta i numeri negati per entità crm
        /// </summary>
        /// <param name="_self"></param>
        /// <returns></returns>
        public static string CorrectNegativeNumber(this string _self)
        {
            if (_self.Contains("-"))
            {
                return ("-" + _self.Replace("-", ""));
            }
            else
                return _self;
        }
        /// <summary>
        /// Proprietà money per entità crm
        /// </summary>
        /// <param name="_self"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Property ToCrmMoneyProperty(this decimal _self, string propertyName)
        {
            return new CrmMoneyProperty(propertyName, new CrmMoney(_self));
        }
        /// <summary>
        /// Compone la connection string al db
        /// </summary>
        /// <param name="_self"></param>
        /// <param name="httpContext"></param>
        /// <param name="orgName"></param>
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
                            _sqlCommand.CommandText = DataUtility.GetQueryForGetConnectionString(orgName);
                            SqlDataReader _sqlReader = _sqlCommand.ExecuteReader();

                            //-- In un ciclo while per leggere l'ultimo (in teoria solo uno) e chiudere in automatico il reader
                            while (_sqlReader.Read())
                            {
                                _connectionString = _sqlReader[0].ToString().Replace("Provider=SQLOLEDB;", ""); ;
                            }

                            _sqlConn.Close();
                        }
                        catch
                        {
                            throw;
                        }
                    }
                    _self.ConnectionString = _connectionString;
                }
            }
        }
        /// <summary>
        /// Ottiene la string di connessione
        /// </summary>
        /// <param name="orgName"></param>
        /// <returns></returns>
        public static string GetSqlConnection(string orgName)
        {
            string _connectionString = string.Empty;
            bool _isOffline = false;

            //-- Se non sono in offline 
            if (!_isOffline)
            {
                try
                {
                    //-- Creo la connessione e la query da eseguire
                    using (SqlConnection _sqlConn = new SqlConnection(Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\MSCRM").GetValue("configdb").ToString()))
                    {
                        _sqlConn.Open();
                        using (SqlCommand _sqlCommand = _sqlConn.CreateCommand())
                        {
                            _sqlCommand.CommandText = DataUtility.GetQueryForGetConnectionString(orgName);
                            using (SqlDataReader _sqlReader = _sqlCommand.ExecuteReader())
                            {
                                //-- In un ciclo while per leggere l'ultimo (in teoria solo uno) e chiudere in automatico il reader
                                while (_sqlReader.Read())
                                    _connectionString = _sqlReader[0].ToString().Replace("Provider=SQLOLEDB;", ""); ;
                            }
                        }
                        _sqlConn.Close();
                    }
                }
                catch
                {
                    throw;
                }
            }
            return _connectionString;
        }
        /// <summary>
        /// Compone la connection string al db
        /// </summary>
        /// <param name="_self"></param>
        /// <param name="orgName"></param>
        public static void SetCrmConnectionString(this SqlConnection _self, string orgName)
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
                    _sqlCommand.CommandText = DataUtility.GetQueryForGetConnectionString(orgName);
                    SqlDataReader _sqlReader = _sqlCommand.ExecuteReader();

                    //-- In un ciclo while per leggere l'ultimo (in teoria solo uno) e chiudere in automatico il reader
                    while (_sqlReader.Read())
                    {
                        _connectionString = _sqlReader[0].ToString().Replace("Provider=SQLOLEDB;", ""); ;
                    }

                    _sqlConn.Close();

                }
                catch
                {
                    throw;
                }

                _self.ConnectionString = _connectionString;
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
        /// <summary>
        /// Compone la connection string al db
        /// </summary>
        /// <param name="orgName"></param>
        /// <returns></returns>
        public static string GetQueryForGetConnectionString(string orgName)
        {
            string _res = @"
                SELECT ConnectionString
                FROM Organization O
                WHERE
	                O.State = 1
	                AND O.IsDeleted = 0
	                AND O.UniqueName= '" + orgName + @"'
                ";
            return _res;
        }
        /// <summary>
        /// Restituisce un'instanza di CrmService
        /// </summary>
        /// <param name="orgName"></param>
        /// <returns></returns>
        public static CrmService LoadCrmService(string orgName,HttpContext context)
        {
#if DEBUG
            CrmService crmService = null;
            if (!string.IsNullOrEmpty(orgName) && orgName.ToLower().Contains("ivecosvilitalia"))
            {
                IFDConnection ifdc = new IFDConnection("IvecoSvilItalia", "http://192.168.90.9:5555", "ivecocloudsvil", "crmdnsadmin", "pass@word1");
                crmService = ifdc.GetCrmService();
                System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
            }
            else
            {
                crmService = new CrmService();
                crmService.CrmAuthenticationTokenValue = new CrmAuthenticationToken() { OrganizationName = DataConstant.ORGANIZATION_NAME_TEST, AuthenticationType = 0 };
                crmService.Url = DataConstant.URL_WS_LM;
                crmService.UseDefaultCredentials = true;
                crmService.Credentials = new System.Net.NetworkCredential("crmdnsadmin", "CrmReply100", "IVECOCLOUD");
                //IFDConnection ifdc = new IFDConnection("IvecoLeadManagement", "http://to0crm03/", "REPLYNET", "d.trotta", "80.David");
                //crmService = ifdc.GetCrmService();
                //crmService.Credentials = new System.Net.NetworkCredential("d.trotta", "80.David", "REPLYNET");
            }
            return crmService;
#else

            CrmService crmService = new CrmService();
            crmService.CrmAuthenticationTokenValue = new CrmAuthenticationToken() { OrganizationName = orgName, AuthenticationType = 0 };
            crmService.Url = DataUtility.GetBaseServiceUrl() + "/2007/CrmService.asmx";
            crmService.UseDefaultCredentials = true;
            //crmService.Credentials = System.Net.CredentialCache.DefaultCredentials;

            //if (context != null)
            //{
            //    if (context.User.Identity.AuthenticationType.ToUpper().Equals("CRMPOSTAUTHENTICATION"))
            //    {
            //        using (new CrmImpersonator())
            //        {
            //            crmService.CrmAuthenticationTokenValue = CrmAuthenticationToken.ExtractCrmAuthenticationToken(context, orgName);
            //        }
            //    }
            //}

            //crmService.CrmAuthenticationTokenValue.CallerId = new Guid("AD4BE077-2E01-E011-882D-005056326A2D");
            return crmService;
            
#endif
        }

        /// <summary>
        /// Restituisce un'instanza di CrmService
        /// </summary>
        /// <param name="orgName"></param>
        /// <returns></returns>
        public static CrmService LoadCrmServiceMail(string orgName, HttpContext context)
        {
#if DEBUG
            CrmService crmService = null;
            if (!string.IsNullOrEmpty(orgName) && orgName.ToLower().Contains("ivecosvilitalia"))
            {
                IFDConnection ifdc = new IFDConnection("IvecoSvilItalia", "http://192.168.90.9:5555", "ivecocloudsvil", "crmdnsadmin", "pass@word1");
                crmService = ifdc.GetCrmService();
                System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
            }
            else
            {
                crmService = new CrmService();
                crmService.CrmAuthenticationTokenValue = new CrmAuthenticationToken() { OrganizationName = DataConstant.ORGANIZATION_NAME_TEST, AuthenticationType = 0 };
                crmService.Url = "http://TO0CRM03/MSCRMServices/2007/CrmService.asmx";
                crmService.UseDefaultCredentials = true;
                crmService.Credentials = new System.Net.NetworkCredential("d.trotta", "80.David", "REPLYNET");
                //IFDConnection ifdc = new IFDConnection("IvecoLeadManagement", "http://to0crm03/", "REPLYNET", "d.trotta", "80.David");
                //crmService = ifdc.GetCrmService();
                //crmService.Credentials = new System.Net.NetworkCredential("d.trotta", "80.David", "REPLYNET");
            }
            return crmService;
#else

            CrmService crmService = new CrmService();
            crmService.CrmAuthenticationTokenValue = new CrmAuthenticationToken() { OrganizationName = orgName, AuthenticationType = 0 };
            crmService.Url = DataUtility.GetBaseServiceUrl() + "/2007/CrmService.asmx";
            //crmService.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //crmService.UseDefaultCredentials = true;
            crmService.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SendMailCrmAdminUsername"],
                ConfigurationManager.AppSettings["SendMailCrmAdminPassword"],
                ConfigurationManager.AppSettings["SendMailCrmAdminDomain"]);
            //if (context != null)
            //{
            //    //if (context.User.Identity.AuthenticationType.ToUpper().Equals("CRMPOSTAUTHENTICATION"))
            //    //{
            //        using (new CrmImpersonator())
            //        {
            //            crmService.CrmAuthenticationTokenValue = CrmAuthenticationToken.ExtractCrmAuthenticationToken(context, orgName);
            //        }
            //    //}
            //}

            //crmService.CrmAuthenticationTokenValue.CallerId = new Guid("AD4BE077-2E01-E011-882D-005056326A2D");
            return crmService;
#endif
        }

        /// <summary>
        /// stringa di connessione al web service
        /// </summary>
        /// <returns></returns>
        public static string GetBaseServiceUrl()
        {
            try
            {
                //Get the server url from the registry 
                string CRM_REG_DIRECTORY = @"Software\Microsoft\MSCRM\";
                Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(CRM_REG_DIRECTORY);
                string crmServerURL = regkey.GetValue("ServerUrl").ToString();
                return crmServerURL.TrimEnd(new char[] { '/' });
            }
            catch
            {
                return "http://localhost/MSCRMServices";
            }
        }
        /// <summary>
        /// Assegnazione di un'entità
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entityId"></param>
        /// <param name="ownerId"></param>
        /// <param name="ownerType"></param>
        /// <param name="crmService"></param>
        /// <returns></returns>
        public static AssignResponse Assign(string entityName, Guid entityId, Guid ownerId, SecurityPrincipalType ownerType, CrmService crmService)
        {
            AssignRequest richiesta = new AssignRequest()
            {
                Target = new TargetOwnedDynamic()
                {
                    EntityId = entityId,
                    EntityName = entityName
                },
                Assignee = new SecurityPrincipal()
                {
                    PrincipalId = ownerId,
                    Type = ownerType
                }
            };
            return crmService.Execute(richiesta) as AssignResponse;

        }
        /// <summary>
        /// Assegna statecode e status code ad un entità 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entityId"></param>
        /// <param name="stateCode"></param>
        /// <param name="statusCode"></param>
        /// <param name="crmService"></param>
        public static void SetState(string entityName, Guid entityId, string stateCode, int statusCode, CrmService crmService)
        {
            SetStateDynamicEntityRequest request = new SetStateDynamicEntityRequest();
            request.Entity = new Moniker() { Name = entityName, Id = entityId };
            request.State = stateCode;
            request.Status = statusCode;
            crmService.Execute(request);


        }


    }
}
