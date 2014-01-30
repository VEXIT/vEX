/*
 * Author:			vEX - Vedran Tatarevic
 * Date Created:	2007-06-14
 * Copyright:       VEXIT Pty Ltd - www.vexit.com
 *
 */


using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace vEX.Web
{
    /// <summary>
    ///   Utility class for manipulation of URLs
    /// </summary>
    public class Urls
    {

        /// <summary>
        ///  Check if url passed is the current url in the context
        /// </summary>
        public static bool IsCurrent(string url)
        {
            return HttpContext.Current.Request.Url.AbsoluteUri.Contains(url);
        }

        /// <summary>
        ///  Check if current url is root url without any slashes
        /// </summary>
        public static bool IsCurrentRoot()
        {
            var parts = GetPartsCount(HttpContext.Current.Request.Url.AbsoluteUri);
            return (parts == 1);
        }

        /// <summary>
        ///  Check if this url requires user to log in or is open to any visitor
        /// </summary>
        public static bool IsUrlOpenAccess(string url)
        {
            string rootPath = GetRoot();
            if (url.Contains(rootPath))
                url = url.Substring(rootPath.Length, (url.Length - rootPath.Length));

            //Must explicitly Declare in Web.Config a name value collection of pages accessable without logging in
            // <configuration>
            //      <UrlOpenAccessList>
            //          <add key="Home" value="Default.aspx"/>
            //      </UrlOpenAccessList>

            return IsInWebConfigNameValueCollection(url, "UrlOpenAccessList");
        }

        /// <summary>
        ///  Check if this url is unrestricted when the rest of the environment is restricted/locked by password
        /// </summary>
        public static bool IsUrlUnrestricted(string url)
        {
            string rootPath = GetRoot();
            if (url.Contains(rootPath))
                url = url.Substring(rootPath.Length, (url.Length - rootPath.Length));

            //Must explicitly Declare in Web.Config a name value collection of pages accessable without logging in
            // <configuration>
            //      <UrlUnrestricted>
            //          <add key="Home" value="Default.aspx"/>
            //      </UrlUnrestricted>

            return IsInWebConfigNameValueCollection(url, "UrlUnrestricted");
        }

        public static bool IsInWebConfigNameValueCollection(string token, string configSection)
        {
            NameValueCollection theCollection = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection(configSection);
            //Loop through the collection		
            for (int i = 0; i < theCollection.Count; i++)
            {
                //Get Key name into a string
                string urlName = theCollection.Keys[i].ToString();
                //Get Key value into a string
                string urlInTheList = theCollection.Get(i);
                //Check if token is same as one in open access list
                if (token.Trim().ToLower().Contains(urlInTheList.Trim().ToLower()))
                    return true;
            }
            return false;
        }

        /// <summary>
        ///   Get Domain Name of the application
        /// </summary>
        public static string GetDomain(HttpContext context)
        {
            HttpRequest r = context.Request;
            return (r.Url.Scheme + System.Uri.SchemeDelimiter + r.Url.Host + (r.Url.IsDefaultPort ? "" : ":" + r.Url.Port)).Replace("http://", "").Replace("www.", "");
        }

        /// <summary>
        ///   Get URL Root of the application
        /// </summary>
        public static string GetRoot(string appDomain = null)
        {
            //Return variable declaration
            var appPath = string.Empty;

            //Getting the current context of HTTP request
            var context = HttpContext.Current;

            //Checking the current context content
            if (context != null)
            {
                //Formatting the fully qualified website url/name
                appPath = string.Format("{0}://{1}{2}{3}",
                                        context.Request.Url.Scheme,
                                        (string.IsNullOrEmpty(appDomain) ? context.Request.Url.Host : appDomain),
                                        context.Request.Url.Port == 80
                                            ? string.Empty
                                            : ":" + context.Request.Url.Port,
                                        context.Request.ApplicationPath);
            }

            if (!appPath.EndsWith("/"))
                appPath += "/";

            return appPath;
        }


        /// <summary>
        ///   Get URL Root of the application
        /// </summary>
        //public static string GetRoot()
        //{
        //    string APP_PATH = HttpContext.Current.Request.ApplicationPath.ToLower();
        //    if (APP_PATH == "/")
        //    {
        //        APP_PATH = "/";
        //    }
        //    else if (!APP_PATH.EndsWith("/"))
        //    {
        //        APP_PATH = (APP_PATH + "/");
        //    }
        //    string URL = HttpContext.Current.Request.Url.AbsoluteUri;
        //    string VirtualPath = HttpContext.Current.Request.Url.AbsolutePath;
        //    int rootLength = URL.IndexOf(VirtualPath);
        //    string rootURL = URL.Substring(0, rootLength);
        //    return rootURL + APP_PATH;
        //}


        public static string GetRoot(System.Web.UI.Page page)
        {
            string fullUrl = page.Request.Url.AbsoluteUri;
            string fullPagePath = page.Request.Url.AbsolutePath;
            string applicationUrl = fullUrl.Replace(fullPagePath, string.Empty);
            if (applicationUrl.IndexOf("?") > -1)
                applicationUrl = applicationUrl.Substring(0, applicationUrl.IndexOf("?"));
            string applicationPath = page.Request.ApplicationPath;
            return applicationUrl + applicationPath;
        }

        /// <summary>
        ///  Get Virtual from Full URL
        /// </summary>
        public static string GetVirtual(string URL, string URLRoot)
        {
            return URL.Substring(URLRoot.Length);
        }
        /// <summary>
        ///  Get Virtual from Full URL
        /// </summary>
        public static string GetVirtual(string URL)
        {
            return GetVirtual(URL, GetRoot());
        }
        /// <summary>
        ///  Get URL Text after last forward slash
        /// </summary>
        public static string GetTextAfterLastSlash(string URL)
        {
            string virtualPath = GetVirtual(URL);
            int startIndex = (virtualPath.LastIndexOf("/") == -1 ? 0 : virtualPath.LastIndexOf("/") + 1);
            return virtualPath.Substring(startIndex);
        }

        /// <summary>
        ///  Get number of parts after root in URL
        /// </summary>
        public static int GetPartsCount(string URL)
        {
            string afterRoot = URL.Substring(GetRoot().Length);
            string[] parts = afterRoot.Split('/');
            return parts.Length;
        }


    }
}
