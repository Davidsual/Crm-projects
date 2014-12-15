using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reply.Iveco.LeadManagement.Web.UI
{
    /// <summary>
    /// UtilityGeneraly
    /// </summary>
    public static class Utility
    {

    }
}
namespace System
{
    /// <summary>
    /// Extension Methods
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// Parsifica la stringa e controlla se è un datetime
        /// </summary>
        /// <param name="myData"></param>
        /// <returns></returns>
        public static DateTime ParseDateTime(this string myData)
        {
            DateTime _result = DateTime.MinValue;
            if(string.IsNullOrEmpty(myData))
                return _result;

            if(DateTime.TryParse(myData,out _result))
                return _result;

            return _result;
        }
        /// <summary>
        /// Parsifica un eventuale valore booleano
        /// </summary>
        /// <param name="myBool"></param>
        /// <returns></returns>
        public static bool? ParseBoolean(this string myBool)
        {
            bool _myReturn;
            if (bool.TryParse(myBool, out _myReturn))
                return _myReturn;

            return null;

        }
    }
}

