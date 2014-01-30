/*
 * Author:			Vex Tatarevic 
 * Date Created:	2007-06-10
 * Copyright:       VEX IT Pty Ltd - www.vexit.com
 *
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;
using vEX.Web.Security;

namespace vEX.Web
{
    public class Cookie
    {
        private static bool UseEncryption = Convert.ToBoolean(ConfigurationManager.AppSettings["AppSecurity_EncryptCookies"]);
        /// <summary>
        ///  if UseEncryption is ON, this method will html encode and encrypt passed string and return the encrypted text. Otherwise it will return the same string back
        /// </summary>
        private static string set(string val) { return (UseEncryption ? WebCrypto.EncryptEncode(val) : val); }
        /// <summary>
        ///  if UseEncryption is ON, this method will html decode and decrypt passed string and return the plain text. Otherwise it will return the same string back
        /// </summary>
        private static string get(string val) { return (UseEncryption ? WebCrypto.DecodeDecrypt(val) : val); }

        /// <summary>
        ///  Creates new HttpCookie object OR Updates existing one in Response.Cookies collection. 
        ///  Save cookie and set it's duration in days.
        ///  NOTE: If days not set or 0,  Cookie will last only for duration of SESSION
        /// </summary>        
        public static HttpCookie Save(string cookieName, int days = 0)
        {
            HttpCookie cookie = RequestGet(cookieName);
            // add cookie if it is null and duration set to more than -1. If it is negative it means kill cookie. We don't need to kill cookie if it doesnt exist.
            cookieName = set(cookieName);
            if (cookie == null)
                cookie = new HttpCookie(cookieName);
            if (days != 0) cookie.Expires = DateTime.Now.AddDays(days);
            ResponseAdd(cookie);            
            return cookie;
        }

        /// <summary>
        ///  Returns a new empty HttpCookie object with a given name
        ///  NOTE: the object is not added to the Response.Cookies collection. Use Save() for that
        /// </summary>
        public static HttpCookie New(string cookieName)
        {
            cookieName = set(cookieName);
            var cookie = new HttpCookie(cookieName); 
            cookie.HttpOnly = true; 
            return cookie; 
        }

        /// <summary>
        ///  Check if cookie exists
        /// </summary>        
        public static bool Exists(string cookieName) { return (RequestGet(cookieName) != null); }
        
        /// <summary>
        ///  Kill Cookie by setting its expiry date to day before today
        /// </summary>
        public static void Kill(HttpCookie cookie)
        {
            cookie.Expires = DateTime.Now.AddDays(-1);
            ResponseAdd(cookie);
        }
        public static void Kill(string cookieName) { Save(cookieName, -1); }     
        
        /// <summary>
        ///  Get value of HttpCookie. If cookie is encrypted automatically decrypts its value
        /// </summary>
        public static string GetVal(HttpCookie cookie, string key = null)
        {
            if (key == null) return get(cookie.Value);
            return get(cookie[set(key)]);
        }

        /// <summary>
        ///  
        /// </summary>
        public static void SetVal(HttpCookie cookie, string key, string value)
        {
            key = set(key);
            value = set(value);
            if (key == null) cookie.Value = value;
            else cookie[key] = value;
        }


        //--------------------------------
        //          REQUEST
        //--------------------------------


        public static string RequestGet(string cookieName, string key)
        {
            cookieName = set(cookieName);
            key = set(key);
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            return (cookie != null && cookie[key] != null ? get(cookie[key]) : string.Empty);
        }

        /// <summary>
        ///  Reads cookie from Request.Cookies[CookieName] array
        ///  Cookie name is encrypted the same way as when the cookie was created in order to be able to read it
        /// </summary>        
        public static HttpCookie RequestGet(string cookieName)
        {
            cookieName = set(cookieName);
            return HttpContext.Current.Request.Cookies[cookieName];
        }

        //--------------------------------
        //          RESPONSE
        //--------------------------------

        /// <summary>
        ///  Sets a Response cookie's value and if cookie is null adds it to the response collection.
        /// </summary>
        public static HttpCookie ResponseSet(string cookieName, string key, string value)
        {
            cookieName = set(cookieName);
            value = set(value);
            HttpCookie cookie = HttpContext.Current.Response.Cookies[cookieName];
            // create new if doesnt exist
            if (cookie == null)
            {
                cookie = new HttpCookie(cookieName);
                cookie.HttpOnly = true;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            if (key != null)
            {
                key = set(key);
                cookie[key] = value;
            }
            else
                cookie.Value = value;
            return cookie;
        }

        /// <summary>
        ///  Gets a cookie from the response collection
        /// </summary>
        public static string ResponseGet(string cookieName, string key = null)
        {
            cookieName = set(cookieName);
            key = set(key);
            HttpCookie cookie = HttpContext.Current.Response.Cookies[cookieName];
            if (key == null) return (cookie != null ? get(cookie.Value) : string.Empty);
            return (cookie != null ? get(cookie[key]) : string.Empty);
        }

        /// <summary>
        ///  Writes cookie using Response.Cookies.Add method
        ///  Creates SESSION Cookie because Expiry duration is not set
        /// </summary>        
        private static HttpCookie ResponseAdd(HttpCookie cookie)
        {   //--------------------------------------------------------------------
            // SET to HTTP-ONLY to Prevent XSS - Cross Site Scripting - Attacks
            //--------------------------------------------------------------------
            cookie.HttpOnly = true;
            HttpContext.Current.Response.Cookies.Add(cookie);
            return cookie;
        }
        /// <summary>
        ///  Removes all cookies with given name from the collection
        /// </summary>
        public static void ResponseRemove(string cookieName)
        {
            cookieName = set(cookieName);
            HttpContext.Current.Response.Cookies.Remove(cookieName);
        }
        /// <summary>
        ///  Removes existing cookie in Response.Cookies collection and creates a fresh new cookie with that name.
        ///  Return newely created HttpCookie object
        /// </summary>
        public static HttpCookie ResponseNew(string cookieName)
        {
            ResponseRemove(cookieName);
            cookieName = set(cookieName);
            return ResponseAdd(new HttpCookie(cookieName));
        }


    }
}
