/*
 * Author:			Vex Tatarevic
 * Date Created:	2010-09-17 : LoadList, LoadEntity 
 *                  2012-09-17 : Map
 * Copyright:       VEX IT Pty Ltd 2013 - www.vexit.com
 */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace vEX.DataAccess
{
    /// <summary>
    ///  This utility class provides data parsing, loading and mapping operations
    /// </summary>
    public class Data
    {
        
        #region [ Data Parsing ]

        public const string StringNull = "";
        public const int IntNull = 0;
        public const long LongNull = 0;
        public const decimal DecimalNull = 0;
        public static DateTime DateTimeNull = DateTime.MinValue.ToUniversalTime(); // must convert mintime to UTC otherwise json serializer craps itself

        public static string ParseString(object value)
        {
            return (value == null || value.ToString().Replace(" ", "") == string.Empty ? StringNull : value.ToString());
        }

        /// <summary>
        ///  Converts  Null to application Int Null
        /// </summary>
        public static int ParseInt(object value)
        {
            return (value == null || value.ToString().Replace(" ", "") == string.Empty ? IntNull : Convert.ToInt32(value));
        }

        /// <summary>
        ///  Converts  Null to application Long Null
        /// </summary>
        public static long ParseLong(object value)
        {
            return (value == null || value.ToString().Replace(" ", "") == string.Empty ? LongNull : Convert.ToInt64(value));
        }

        /// <summary>
        ///  Converts  Null to application Decimal Null
        /// </summary>
        public static decimal ParseDecimal(object value)
        {
            return (value == null || value.ToString().Replace(" ", "") == string.Empty ? DecimalNull : Convert.ToDecimal(value));
        }

        /// <summary>
        ///  Converts null, empty string ,  Null to application DateTime Null
        /// </summary>
        public static DateTime ParseDateTime(object value)
        {
            DateTime returnDate = DateTimeNull;
            if (value == null || value.ToString() == string.Empty || value.ToString() == "/Date(-62135596800000)/")
                return returnDate;
            else
            {
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

        public static bool ParseBool(object value)
        {
            return (value == null || value.ToString().ToLower() == "true" ? true : false);
        }

        #endregion

        /// <summary>
        ///  Loads Object array (typically received as web service prameter) into a list of expected object type
        /// </summary>
        public static List<T> LoadList<T>(object[] Entities)
        {
            List<T> lst = new List<T>();
            foreach (object ent in Entities)
            {
                Dictionary<string, object> fields = (Dictionary<string, object>)ent;
                T o = (T)Activator.CreateInstance(typeof(T));
                LoadEntity(ref o, fields);
                lst.Add(o);
            }
            return lst;
        }

        public static T LoadEntity<T>(ref T obj, Dictionary<string, object> fields)
        {
            Type oClassType = obj.GetType();
            PropertyInfo[] oClassProperties = oClassType.GetProperties();
            foreach (PropertyInfo prop in oClassProperties)
            {
                if (fields.ContainsKey(prop.Name))
                {
                    object fld = fields[prop.Name];
                    var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.String: prop.SetValue(obj, NULL.ParseString(fld), null); break;
                        case TypeCode.Int32: prop.SetValue(obj, NULL.ParseInt(fld), null); break;
                        case TypeCode.Int64: prop.SetValue(obj, NULL.ParseLong(fld), null); break;
                        case TypeCode.Decimal: prop.SetValue(obj, NULL.ParseDecimal(fld), null); break;
                        case TypeCode.DateTime: prop.SetValue(obj, NULL.ParseDateTime(fld), null); break;
                        case TypeCode.Boolean: prop.SetValue(obj, NULL.ParseBool(fld), null); break;
                        //case TypeCode.Object: prop.SetValue(obj, GetBool(prop.Name), null); break;
                    }
                }
            }
            return obj;
        }

        /// <summary>
        ///     Maps values of properties of sourceObject to properties of targetObject
        ///     You can use this in MVC programming when mapping Entity to ViewModel and back
        ///     Limitations: Only maps flat properties (first level/no nesting) that have matching names and types
        /// </summary>
        public static TTarget Map<TTarget, TSource>(TSource sourceObject)
        {
            TTarget targetObject = (TTarget)Activator.CreateInstance(typeof(TTarget));
            Map<TTarget, TSource>(ref targetObject, sourceObject);
            return targetObject;
        }
        public static void Map<TTarget, TSource>(ref TTarget targetObject, TSource sourceObject)
        {
            Type sourceType = sourceObject.GetType();
            Type targetType = targetObject.GetType();
            foreach (var property in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var targetProperty = targetType.GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance);

                if (targetProperty != null
                      && targetProperty.CanWrite
                      && targetProperty.PropertyType.IsAssignableFrom(GetUnderlyingType(property.PropertyType)))
                {
                    //if (property.GetValue(sourceObject, null) != null)
                    targetProperty.SetValue(targetObject, property.GetValue(sourceObject, null), null);
                }
            }
        }

        public static Type GetUnderlyingType(Type type)
        {
            bool isNullable = (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
            return (isNullable ? Nullable.GetUnderlyingType(type) : type);
        }

        public static bool IsNullable<T>(T obj)
        {
            if (obj == null) return true; // obvious
            Type type = typeof(T);
            if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }


    }
}
