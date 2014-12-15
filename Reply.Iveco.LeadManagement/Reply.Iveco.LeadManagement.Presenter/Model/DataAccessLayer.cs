using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Data.Linq;

namespace Reply.Iveco.LeadManagement.Presenter.Model
{
    public partial class DataAccessLayer : BaseDataAccessLayer, IDisposable
    {
        #region PUBLIC MEMBERS

        #endregion

        #region PRIVATE MEMBERS
        private static String PATHLOG = System.Environment.CurrentDirectory + "\\Logs";
        private Guid _systemUserId;
        private Guid _businessUnitId;
        private Guid _baseCurrencyId;
        private Guid _organizationId;
        #endregion

        #region PROTECTED PROPERTY
        /// <summary>
        /// current organization id
        /// </summary>
        private Guid OrganizationId
        {
            get { return _organizationId; }
            set { _organizationId = value; }
        }
        /// <summary>
        /// Current system user id
        /// </summary>
        private Guid SystemUserId
        {
            get { return _systemUserId; }
            set { _systemUserId = value; }
        }
        /// <summary>
        /// Current business unit id
        /// </summary>
        private Guid BusinessUnitId
        {
            get { return _businessUnitId; }
            set { _businessUnitId = value; }
        }
        /// <summary>
        /// Current currency id
        /// </summary>
        private Guid BaseCurrencyId
        {
            get { return _baseCurrencyId; }
            set { _baseCurrencyId = value; }
        }
        #endregion

        #region CTOR
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="organizationName"></param>
        public DataAccessLayer(string organizationName,HttpContext context)
            : base(organizationName, context)
        {

        }
        #endregion

        #region PRIVATE COMPILED QUERY
        ///// <summary>
        ///// Ottiene il delay rispetto a greenwich
        ///// </summary>
        //private static Func<LeadManagementModelDataContext,string,IQueryable<int>> GetFuncTimeZoneDelayByCountryName = 
        //    CompiledQuery.Compile((LeadManagementModelDataContext model,string country) =>
        //        from tmz in model.TimeZoneDefinitions
        //         join tmr in model.TimeZoneRules
        //                on tmz.TimeZoneDefinitionId equals tmr.TimeZoneDefinitionId
        //         where tmz.DeletionStateCode == 0 && tmz.TimeZoneCode == (model.New_countries.Where(c => c.New_name.ToUpper() == country && c.DeletionStateCode == 0).Select(c => c.New_timezone).SingleOrDefault())
        //         select tmr.Bias);
        ///// <summary>
        ///// Ottiene il time zone code dato il country name
        ///// </summary>
        //private static Func<LeadManagementModelDataContext, string, IQueryable<int>> GetFuncTimeZoneCodeByCountryName =
        //            CompiledQuery.Compile((LeadManagementModelDataContext model,string country) =>
        //                model.New_countries.Where(c => c.New_name.ToUpper() == country && c.DeletionStateCode == 0).Select(c => c.New_timezone.Value));
        #endregion

        #region PUBLIC METHODS      
        /// <summary>
        /// Ottiene il time zon code dato il country name
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public int GetTimeZoneCodeByCountryName(string country)
        {
            throw new NotImplementedException("GetTimeZoneCodeByCountryName");
            //return DataAccessLayer.GetFuncTimeZoneCodeByCountryName(base.CurrentDataContext, country).SingleOrDefault();
        }
        ///// <summary>
        ///// Ottiene il delay rispetto a greenwich
        ///// </summary>
        ///// <param name="country"></param>
        ///// <returns></returns>
        //public int GetTimeZoneDelayByCountryName(string country)
        //{
        //    try
        //    {
        //        throw new NotImplementedException("GetTimeZoneDelayByCountryName");

        //        ///Ottengo il delay in ore del fusorario
        //        //return DataAccessLayer.GetFuncTimeZoneDelayByCountryName(base.CurrentDataContext, country).SingleOrDefault();
        //        //return (from tmz in base.Model.TimeZoneDefinitions
        //        //        join tmr in base.Model.TimeZoneRules
        //        //               on tmz.TimeZoneDefinitionId equals tmr.TimeZoneDefinitionId
        //        //        where tmz.DeletionStateCode == 0 && tmz.TimeZoneCode == (Model.New_countries.Where(c => c.New_name.ToUpper() == country && c.DeletionStateCode == 0).Select(c => c.New_timezone).SingleOrDefault())
        //        //               select tmr.Bias).SingleOrDefault();
                
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Log sul file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="createFile"></param>
        public static void logFile(String message, bool createFile)
        {
            StreamWriter sw;
            if (!Directory.Exists(PATHLOG))
                Directory.CreateDirectory(PATHLOG);
            if (!createFile)
                sw = File.AppendText(PATHLOG + "\\Log" + DateTime.Today.Day + "-" + DateTime.Today.Month + ".txt");
            else
                sw = File.CreateText(PATHLOG + "\\Log" + DateTime.Today.Day + "-" + DateTime.Today.Month + ".txt");
            sw.WriteLine(message);
            sw.Close();
            sw.Dispose();
        }
        /// <summary>
        /// Set Data
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int SetData(string sqlQuery, IList<SqlParameter> param)
        {
            return SetData(sqlQuery, param, this.CurrentSqlConnection);
        }
        /// <summary>
        /// Set data
        /// </summary>
        /// <returns></returns>
        public int SetData(string sqlQuery, IList<SqlParameter> param, SqlConnection conn)
        {
            try
            {
                ///Creo il command
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddRange(param.ToArray());
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ///this.SqlTransaction.Rollback();
                //this.CurrentSqlConnection.Close();
                /////this.SqlTransaction.Dispose();
                //this.CurrentSqlConnection.Dispose();
                //this.SqlTransaction = null;
                throw;

            }
        }
        /// <summary>
        /// Ottiene una guid data la query ed eventuali parametri
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Guid GetData(string sqlQuery, IList<SqlParameter> param, SqlConnection conn)
        {
            try
            {
                ///Creo il command
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddRange(param.ToArray());
                    object _result = cmd.ExecuteScalar();
                    return (_result == null) ? Guid.Empty : (Guid)_result;
                }
            }
            catch (Exception ex)
            {
                ///this.SqlTransaction.Rollback();
                this.CurrentSqlConnection.Close();
                ///this.SqlTransaction.Dispose();
                this.CurrentSqlConnection.Dispose();
                throw;
            }
        }
        /// <summary>
        /// Ottiene una guid data la query ed eventuali parametri
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Guid GetData(string sqlQuery, IList<SqlParameter> param)
        {
            try
            {
                ///Creo il command
                using (SqlCommand cmd = new SqlCommand(sqlQuery, this.CurrentSqlConnection))
                {

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddRange(param.ToArray());
                    object _result = cmd.ExecuteScalar();
                    return (_result == null) ? Guid.Empty : (Guid)_result;
                }
            }
            catch (Exception ex)
            {
                ///this.SqlTransaction.Rollback();
                this.CurrentSqlConnection.Close();
                ///this.SqlTransaction.Dispose();
                this.CurrentSqlConnection.Dispose();
                throw;
            }
        }
        /// <summary>
        /// Ottiene i dati
        /// </summary>
        /// <returns></returns>
        public Guid GetData(string sqlQuery)
        {
            try
            {
                ///Creo il command
                using (SqlCommand cmd = new SqlCommand(sqlQuery, this.CurrentSqlConnection))
                {
                    return (Guid)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                ///this.SqlTransaction.Rollback();
                this.CurrentSqlConnection.Close();
                ///this.SqlTransaction.Dispose();
                this.CurrentSqlConnection.Dispose();
                throw;
            }
        }
        /// <summary>
        /// Ottiene i dati
        /// </summary>
        /// <returns></returns>
        public Guid GetData(string sqlQuery, SqlConnection conn)
        {
            try
            {
                ///Creo il command
                using (SqlCommand cmd = new SqlCommand(sqlQuery, this.CurrentSqlConnection))
                {
                    return (Guid)cmd.ExecuteScalar();
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Restituisce un dictionary key/value guid-descr
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        public Dictionary<Guid, string> GetMultipleData(string sqlQuery)
        {
            SqlDataReader _reader = null;
            Dictionary<Guid, string> _return = null;
            try
            {
                ///Creo il command
                using (SqlCommand cmd = new SqlCommand(sqlQuery, this.CurrentSqlConnection))
                {
                    _return = new Dictionary<Guid, string>();
                    _reader = cmd.ExecuteReader();
                    while (_reader.Read())
                    {
                        _return.Add((Guid)_reader[0], _reader[1] as string);
                    }
                    _reader.Close();
                    _reader.Dispose();
                }
                return _return;
            }
            catch (Exception ex)
            {
                ///this.SqlTransaction.Rollback();
                this.CurrentSqlConnection.Close();
                ///this.SqlTransaction.Dispose();
                this.CurrentSqlConnection.Dispose();
                throw;
            }
        }
        /// <summary>
        /// Ottiene lista chiave valore
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Dictionary<Guid, string> GetMultipleData(string sqlQuery, IList<SqlParameter> param)
        {
            SqlDataReader _reader = null;
            Dictionary<Guid, string> _return = null;
            try
            {
                ///Creo il command
                using (SqlCommand cmd = new SqlCommand(sqlQuery, this.CurrentSqlConnection))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddRange(param.ToArray());
                    _return = new Dictionary<Guid, string>();
                    _reader = cmd.ExecuteReader();
                    while (_reader.Read())
                    {
                        _return.Add((Guid)_reader[0], _reader[1] as string);
                    }
                    _reader.Close();
                    _reader.Dispose();
                }
                return _return;
            }
            catch (Exception ex)
            {
                /////this.SqlTransaction.Rollback();
                //this.CurrentSqlConnection.Close();
                /////this.SqlTransaction.Dispose();
                //this.CurrentSqlConnection.Dispose();
                //this.SqlTransaction = null;
                throw;
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Rilascio delle risorse       
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion
    }
}
