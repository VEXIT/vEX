/*
 * Author:			vEX - Vedran Tatarevic
 * Date Created:	2007-06-14
 * Copyright:       VEXIT Pty Ltd - www.vexit.com
 *
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace vEX.Web
{
    /// <summary>
    /// Generic Web utility class that provides static methods
    /// </summary>
    public class Util
    {

        #region [ Session ]

        /// <summary>
        ///  Gets Session object
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetSession(string key)
        {
            return (HttpContext.Current.Session == null ? null : HttpContext.Current.Session[key]);
        }
        public static void SetSession(string key, object value)
        {
            HttpContext.Current.Session[key] = value;
        }

        public static void RemoveSession(string key)
        {
            HttpContext.Current.Session.Remove(key);
        }

        #endregion

        #region [ GUID ]

        public static string GUID()
        {
            return System.Guid.NewGuid().ToString();
        }

        public static string GUID_UniqueName()
        {
            // Get the GUID
            string guidResult = System.Guid.NewGuid().ToString();
            // Remove the hyphens
            guidResult = guidResult.Replace("-", string.Empty);
            return guidResult;
        }

        #endregion

        #region [ IP Address ]

        public static string IPAdress()
        {
            return HttpContext.Current.Request.UserHostAddress;
        }

        /// <summary>
        ///  IP ADDRESS    - GetIPAddress
        /// </summary>        
        public static string IPAddress_Get(HttpContext httpContext)
        {
            string strIpAddress = "";
            strIpAddress = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (strIpAddress == "" || strIpAddress == null)
                strIpAddress = httpContext.Request.ServerVariables["REMOTE_ADDR"];
            return strIpAddress;
        }

        /// <summary>
        ///  IP NUMBER    - GetIPNumber
        /// </summary>        
        public static long IPNumber_Get(HttpContext httpContext)
        {
            long ipNumber = 0;
            string ipAddress = IPAddress_Get(System.Web.HttpContext.Current);
            if (ipAddress != null && ipAddress != string.Empty)
            {
                ipNumber = IPAddress_ToNumber(ipAddress);
            }
            return ipNumber;
        }

        /// <summary>
        ///  Convert IP Address from string format to number format
        /// </summary>
        public static long IPAddress_ToNumber(string IPAddress)
        {
            //return System.Net.IPAddress.Parse(IPAddress).Address; // OBSOLETE
            string[] arrDec;
            int num = 0;
            if (IPAddress != "")
            {
                arrDec = IPAddress.Split('.');
                num = (int.Parse(arrDec[3])) + (int.Parse(arrDec[2]) * 256) + (int.Parse(arrDec[1]) * 65536) + (int.Parse(arrDec[0]) * 16777216);
            }
            return num;
        }

        /// <summary>
        ///  Convert IP Address from string format to number format
        /// </summary>
        public static long IPAddress_ToNumber(HttpContext httpContext)
        {
            return IPAddress_ToNumber(IPAddress_Get(httpContext));
        }

        #endregion

    }
}
