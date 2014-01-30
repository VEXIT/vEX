/*
 * Author:			vEX - Vedran Tatarevic
 * Date Created:	2011-06-10
 * Copyright:       VEXIT Pty Ltd - www.vexit.com
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using vEX.DataAccess;

namespace vEX.Web
{
    /// <summary>
    ///  Http Request object wraper and convenience utility class
    /// </summary>
    public class Req
    {
        private static HttpRequest req { get { return HttpContext.Current.Request; } }

        public static string Get(string parameterName) { return req.Params[parameterName]; }

        public static string GetString(string name) { return Data.ParseString(req.Params[name]); }
        public static void SetString(string obj, string name) { object val = req.Params[name]; if (val != null) obj = GetString(name); }

        public static int GetInt(string name) { return Data.ParseInt(req.Params[name]); }
        public static void SetInt(int obj, string name) { object val = req.Params[name]; if (val != null) obj = GetInt(name); }

        public static long GetLong(string name) { return Data.ParseLong(req.Params[name]); }
        public static void SetLong(long obj, string name) { object val = req.Params[name]; if (val != null) obj = GetLong(name); }

        public static decimal GetDecimal(string name) { return Data.ParseDecimal(req.Params[name]); }
        public static void SetDecimal(decimal obj, string name) { object val = req.Params[name]; if (val != null) obj = GetDecimal(name); }

        public static DateTime GetDateTime(string name) { return Data.ParseDateTime(req.Params[name]); }
        public static void SetDateTime(DateTime obj, string name) { object val = req.Params[name]; if (val != null) obj = GetDateTime(name); }

        public static bool GetBool(string name) { return Data.ParseBool(req.Params[name]); }
        public static void SetBool(bool obj, string name) { object val = req.Params[name]; if (val != null) obj = GetBool(name); }

        


        /// <summary>
        /// Check if request value received and if it equals the given value
        /// </summary>
        /// <param name="name">Case Insensitive</param>
        /// <param name="value">Case Insensitive</param>
        /// <returns></returns>
        public static bool Is(string name, string value) { return GetString(name).ToLower() == value.ToLower(); }


        /// <summary>
        ///  Loads Entity object with values extraced from Http Request from fields whose names match Entity field names
        ///  NOTE: You must not use the following field names as they are already used by HttpRequest object: 
        ///  URL
        /// </summary>
        public static T LoadInto<T>(T obj)
        {
            Type oClassType = obj.GetType();
            PropertyInfo[] oClassProperties = oClassType.GetProperties();
            foreach (PropertyInfo prop in oClassProperties)
            {
                if (req.Params[prop.Name] != null)
                {
                    var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.String: prop.SetValue(obj, GetString(prop.Name), null); break;
                        case TypeCode.Int32: prop.SetValue(obj, GetInt(prop.Name), null); break;
                        case TypeCode.Int64: prop.SetValue(obj, GetLong(prop.Name), null); break;
                        case TypeCode.Decimal: prop.SetValue(obj, GetDecimal(prop.Name), null); break;
                        case TypeCode.DateTime: prop.SetValue(obj, GetDateTime(prop.Name), null); break;
                        case TypeCode.Boolean: prop.SetValue(obj, GetBool(prop.Name), null); break;
                        //case TypeCode.Object: prop.SetValue(obj, GetBool(prop.Name), null); break;
                    }
                }
            }
            return obj;
        }

        public static T LoadInto<T>(ref T obj) { return LoadInto(obj); }

        //public static object LoadInto(string typeName, string asembleyName = null)
        //{
        //  return LoadInto(Activator.CreateInstance(asembleyName,typeName));
        //}


    }
}