/*
 * Author:			Vex Tatarevic 
 * Date Created:	2007-06-10
 * Copyright:       VEX IT Pty Ltd
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
    ///  Convenience Class that wraps around vEX.Web.Cookie
    ///  
    /// </summary>
    public abstract class CookieBase
    {
        protected string CookieName;
        /// <summary>
        ///  DOES NOT create new HttpCookie object. Just initializes CookieBase object by setting CookieName.
        /// </summary>
        public CookieBase(string cookieName) { CookieName = cookieName; }
        /// <summary>
        ///  Create new HttpCookie object
        ///  Returns a new empty HttpCookie object with a given name
        ///  NOTE: the object is not added to the Response.Cookies collection. Use Save() for that
        /// </summary>
        public HttpCookie New() { return Cookie.New(CookieName); }
        /// <summary>
        ///  Sets cookie expiry period in days and adds it to Response.Cookies collection.
        ///  NOTE: If days not set or 0, Cookie will only last until SESSION ends/browser closed
        /// </summary>
        public void Save(int days = 0) { Cookie.Save(CookieName, days); }
        /// <summary>
        /// Check if HttpCookie object exists on client computer
        /// </summary>
        public bool Exists() { return Cookie.Exists(CookieName); }
        /// <summary>
        /// Delete HttpCookie object from client computer
        /// </summary>
        public void Kill() { Cookie.Kill(CookieName); }
    }
}
