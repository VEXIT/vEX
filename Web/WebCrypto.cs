/*
 * Author:			Vex Tatarevic 
 * Date Created:	2012-03-29
 * Copyright:       VEX IT Pty Ltd - www.vexit.com
 *
 */

using System;
using System.Collections.Generic;
using System.Text;

using System.Web;
using vEX.Security;

namespace vEX.Web.Security
{
    /// <summary>
    ///     Used for encoding and encryption of cookies, urls and other objects/texts that are moved through http.
    ///     It helps encode/decode http reserved characters. Specifically, the characters '=' and ';' are reserved and must be escaped; 
    ///     Encryption algorithm will append "=" to fill the block with the allocated block size.
    /// </summary>
    public class WebCrypto
    {
        /// <summary>
        /// Url Encode the keys and values of the Cookies and URLs
        /// </summary>
        public static string Encode(string value) { return HttpContext.Current.Server.UrlEncode(value); }
        /// <summary>
        ///  Url Decode text
        /// </summary>
        public static string Decode(string value) { return HttpContext.Current.Server.UrlDecode(value); }
        /// <summary>
        ///  Encrypt , then URL Encode for http transport inside url string or a cookie
        /// </summary>
        public static string EncryptEncode(string value) { return (string.IsNullOrEmpty(value) ? "" : Encode(Cryptograph.Encrypt(value))); }
        /// <summary>
        ///   URL Decode from http transport inside url string or a cookie, then Decrypt 
        /// </summary>
        public static string DecodeDecrypt(string value) { return (string.IsNullOrEmpty(value) ? "" : Cryptograph.Decrypt(Decode(value))); }
    }
}
