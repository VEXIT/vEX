/*
 * Author:			Vex Tatarevic
 * Date Created:	2007-09-09
 * Copyright:       VEX IT Pty Ltd 2013 - www.vexit.com
 * 
 * Description:     Convenience class for null object representation
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace vEX.DataAccess
{
    public class NULL
    {

        #region [ PROPERTIES ]

        public const string StringNull = "";
        public const int IntNull = -1;
        public const long LongNull = -1;
        public const decimal DecimalNull = -1;
        public static DateTime DateTimeNull = DateTime.MinValue.ToUniversalTime(); // must convert mintime to UTC otherwise json serializer craps itself

        #endregion

        /// <summary>
        ///  Convert DB Null to code null
        /// </summary>
        public static object ParseFromDB(object val)
        {
            return (object)((val == System.DBNull.Value ? val = null : val));
        }

        #region [ APPLICATION NULLS ]

        public static string ParseString(object value)
        {
            return (value == null || value.ToString().Replace(" ", "") == string.Empty ? StringNull : value.ToString());
        }

        /// <summary>
        ///  Converts DB Null to application Int Null
        /// </summary>
        public static int ParseInt(object value)
        {
            return (value == null || value.ToString().Replace(" ", "") == string.Empty ? IntNull : Convert.ToInt32(value));
        }

        /// <summary>
        ///  Converts DB Null to application Long Null
        /// </summary>
        public static long ParseLong(object value)
        {
            return (value == null || value.ToString().Replace(" ", "") == string.Empty ? LongNull : Convert.ToInt64(value));
        }

        /// <summary>
        ///  Converts DB Null to application Decimal Null
        /// </summary>
        public static decimal ParseDecimal(object value)
        {
            return (value == null || value.ToString().Replace(" ", "") == string.Empty ? DecimalNull : Convert.ToDecimal(value));
        }

        /// <summary>
        ///  Converts null, empty string , DB Null to application DateTime Null
        /// </summary>
        public static DateTime ParseDateTime(object value)
        {
            DateTime returnDate = DateTimeNull;
            if (value == null || value.ToString() == string.Empty || value.ToString() == "/Date(-62135596800000)/" )
            return returnDate;
            else{
                try
                {
                    returnDate = DateTime.SpecifyKind(Convert.ToDateTime(value), DateTimeKind.Unspecified);// must be unspecified to prevent c# from changing date by adding timezone offset
                
                }
                catch (Exception)
                {   // value supplied in milliseconds from 1970
                    long milliseconds;
                    bool isTicks = long.TryParse(value.ToString(), out milliseconds);
                    if (isTicks)
                        returnDate = new DateTime(1970, 01, 01).AddMilliseconds(milliseconds).ToUniversalTime();
                    return returnDate;
                    
                }
            }
            return returnDate;
        }

        /// <summary>
        ///  
        /// </summary>
        public static bool ParseBool(object value)
        {
            return (value == null || value.ToString().ToLower() == "false" ? false : true);
        }

        #endregion

        #region [ DB NULL ]

        /// <summary>
        ///   Converts string null to DBNull
        /// </summary>
        public static object DB_STRING(string value)
        {
            return (value != StringNull && value != null ? (object)(value) : DBNull.Value);
        }

        /// <summary>
        ///   Converts int null to DBNull
        /// </summary>
        public static object DB_INT(int value)
        {
            return (value != IntNull  ? (object)(value) : DBNull.Value);
        }

        /// <summary>
        ///   Converts long null to DBNull
        /// </summary>
        public static object DB_LONG(long value)
        {
            return (value != LongNull  ? (object)(value) : DBNull.Value);
        }

        /// <summary>
        ///   Converts decimal null to DBNull
        /// </summary>
        public static object DB_DECIMAL(decimal value)
        {
            return (value != DecimalNull  ? (object)(value) : DBNull.Value);
        }
        /// <summary>
        ///   Converts DateTime null to DBNull
        /// </summary>
        public static object DB_DATETIME(DateTime value)
        {
            return (value != DateTimeNull && value != null && !value.ToString().Contains("1/1/0001") ? (object)(value) : DBNull.Value);
        }

        #endregion

    }
}
