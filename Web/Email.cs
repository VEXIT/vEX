/*
 * Author:			Vex Tatarevic 
 * Date Created:	2007-06-14
 * Copyright:       VEXIT Pty Ltd - www.vexit.com
 *
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

namespace vEX.Web
{

    public class Email
    {

        // Put this in web.Config
        //<system.net>
        //  <mailSettings>
        //    <smtp>
        //      <network host="mail.ihostasp.net"/>
        //    </smtp>
        //  </mailSettings>
        //</system.net>
        public static void Send(string fromAddress, string fromDisplayName,
                               string[] toReceivers, string[] ccReceivers, string[] bccReceivers,
                               string subject, string body, bool isBodyHtml, string[] attachmentPaths, MailPriority priority = MailPriority.Normal )
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromAddress, fromDisplayName);
            //------------------------------------------
            if (toReceivers == null)
            {
                toReceivers = new string[0];
            }
            if (ccReceivers == null)
            {
                ccReceivers = new string[0];
            }
            if (bccReceivers == null)
            {
                bccReceivers = new string[0];
            }
            //------------------------------------------
            foreach (string toReceiver in toReceivers)
            {
                message.To.Add(new MailAddress(toReceiver));
            }
            foreach (string ccReceiver in ccReceivers)
            {
                message.CC.Add(new MailAddress(ccReceiver));
            }
            foreach (string bccReceiver in bccReceivers)
            {
                message.Bcc.Add(new MailAddress(bccReceiver));
            }
            //------------------------------------------   
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isBodyHtml;
            message.Priority = priority;

            if(attachmentPaths != null) {
                foreach(var file in attachmentPaths) { 
                    message.Attachments.Add(new Attachment(file)); 
                }
            }
            
            var client = new SmtpClient();
            // You must configure your SMTP host settings in Web.config
            //client.Host = "mail.ihostasp.net"; 
            // this line only if your web.config doesnt contain <system.net><mailSettings>
            //<configuration>
            //   <system.net>
            //      <mailSettings>
            //         <smtp from="defaultEmail@yourdomain.com">
            //            <network host="smtp.yourdomain.com" port="25" userName="yourUserName" password="yourPassword"/>
            //         </smtp>
            //      </mailSettings>
            //   </system.net>
            //</configuration>
            client.Send(message);
        }
        /// <summary>
        ///  Send to one recepient
        /// </summary>
        public static void Send(string fromEmail, string fromDisplayName, string toEmail, string subject, string body, bool isBodyHtml = true)
        {
            string[] toReceivers = { toEmail };
            Send(fromEmail, fromDisplayName, toReceivers, null, null, subject, body, isBodyHtml, null);
        }
        
        /// <summary>
        ///  Send to one recepient - WITH HIGH PRIORITY
        /// </summary>
        public static void SendAsImportant(string fromEmail, string fromDisplayName, string toEmail, string subject, string body, bool isBodyHtml = true)
        {
            string[] toReceivers = { toEmail };
            Send(fromEmail, fromDisplayName, toReceivers, null, null, subject, body, isBodyHtml, null, MailPriority.High);
        }

        public static void Send(string fromEmail, string fromDisplayName, string toEmail, string subject, string body, bool isBodyHtml, MailPriority priority)
        {
            string[] toReceivers = { toEmail };
            Send(fromEmail, fromDisplayName, toReceivers, null, null, subject, body, isBodyHtml,null, priority);
        }

        public static void SendWithAttachments(string fromEmail, string fromDisplayName, string toEmail, string subject, string body, bool isBodyHtml, string[] attachmentPaths, MailPriority priority = MailPriority.Normal)
        {
            string[] toReceivers = { toEmail };
            Send(fromEmail, fromDisplayName, toReceivers, null, null, subject, body, isBodyHtml, attachmentPaths, priority );
        }

        public static void SendWithAttachmentsAsImportant(string fromEmail, string fromDisplayName,string toEmail, string subject, string body, bool isBodyHtml, string[] attachmentPaths){
            SendWithAttachments(fromEmail, fromDisplayName, toEmail, subject, body, isBodyHtml, attachmentPaths, MailPriority.High);
        }


    }

}