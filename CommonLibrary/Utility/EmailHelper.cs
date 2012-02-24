using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace CommonLibrary.Utility
{
    public class EmailHelper
    {
        public static bool SendEmailByCredential(string host, string user_name, string password, string[] mailto, string subject, string body, bool is_body_html, string[] attachments)
        {
            return SendEmailByCredential(host, MailPriority.Normal, user_name, password, mailto, null, null, subject, body, is_body_html, attachments, false);
        }
        public static bool SendEmailByCredential(string host, string user_name, string password, string[] mailto, string subject, string body, bool is_body_html)
        {
            return SendEmailByCredential(host, MailPriority.Normal, user_name, password, mailto, null, null, subject, body, is_body_html, null, false);
        }
        public static bool SendEmailByCredential(string host, string user_name, string password, string mailto, string subject, string body, bool is_body_html)
        {
            return SendEmailByCredential(host, MailPriority.Normal, user_name, password, new string[] { mailto }, null, null, subject, body, is_body_html, null, false);
        }
        public static bool SendEmailByCredential(string host, MailPriority priority, string user_name, string password, string[] mailto, string[] cc, string[] bcc, string subject, string body, bool is_body_html, string[] attachments)
        {
            return SendEmailByCredential(host, MailPriority.Normal, user_name, password, mailto, cc, bcc, subject, body, is_body_html, attachments, false);
        }
        public static bool SendEmailByCredential(string host, MailPriority priority, string user_name, string password, string[] mailto, string[] cc, string[] bcc, string subject, string body, bool is_body_html, string[] attachments, bool enableSSL)
        {
            return SendEmailByCredential(host, priority, user_name, password, mailto, cc, bcc, subject, body, is_body_html, attachments, enableSSL, null);
        }
        public static bool SendEmailByCredential(string host, MailPriority priority, string user_name, string password, string[] mailto, string[] cc, string[] bcc, string subject, string body, bool is_body_html, string[] attachments, bool enableSSL, string reply_to)
        {
            return SendEmailByCredential(host, priority, user_name, password, mailto, cc, bcc, subject, body, is_body_html, attachments, enableSSL, null, reply_to);
        }
        public static bool SendEmailByCredential(string host, MailPriority priority, string user_name, string password, string[] mailto, string[] cc, string[] bcc, string subject, string body, bool is_body_html, string[] attachments, bool enableSSL, System.Collections.Specialized.NameValueCollection messageHeaders, string reply_to)
        {
            SmtpClient mail = new SmtpClient();
            mail.DeliveryMethod = SmtpDeliveryMethod.Network;
            mail.Host = host;// "smtp.163.com";
            mail.UseDefaultCredentials = false;
            mail.Credentials = new System.Net.NetworkCredential(user_name, password);
            mail.EnableSsl = enableSSL;
            MailMessage message = new MailMessage();
            if (messageHeaders != null && messageHeaders.Count > 0)
            {
                message.Headers.Add(messageHeaders);
            }
            message.From = new MailAddress(user_name + "@" + host.Substring(host.IndexOf(".") + 1));
            if (mailto != null)
            {
                foreach (string s in mailto)
                {
                    if (!string.IsNullOrEmpty(s))
                        message.To.Add(s);
                }
            }
            if (cc != null)
            {
                foreach (string s in cc)
                {
                    if (!string.IsNullOrEmpty(s))
                        message.CC.Add(s);
                }
            }
            if (bcc != null)
            {
                foreach (string s in bcc)
                {
                    if (!string.IsNullOrEmpty(s))
                        message.Bcc.Add(s);
                }
            }
            if (!string.IsNullOrEmpty(reply_to)) message.ReplyTo = new MailAddress(reply_to);
            message.Subject = subject;
            message.Body = body;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = is_body_html;
            message.Priority = priority;
            if (attachments != null)
            {
                foreach (string s in attachments)
                {
                    message.Attachments.Add(new Attachment(s));
                }
            }
            mail.Send(message);
            return true;
        }
        private static System.Net.Mail.SmtpClient GetSmtpClient(string smtpServer)
        {
            System.Net.Mail.SmtpClient newmail = new System.Net.Mail.SmtpClient(smtpServer);
            newmail.Port = 25;
            newmail.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            newmail.Timeout = 100000;
            newmail.UseDefaultCredentials = false;
            return newmail;
        }
        public static bool SendMail(string smtp_server, string from, string[] mailto, string subject, string body, bool is_body_html, string[] attachments)
        {
            return SendMail(smtp_server, MailPriority.Normal, from, mailto, null, null, subject, body, is_body_html, attachments);
        }
        public static bool SendMail(string smtp_server, string from, string[] mailto, string subject, string body, bool is_body_html)
        {
            return SendMail(smtp_server, MailPriority.Normal, from, mailto, null, null, subject, body, is_body_html, null);
        }
        public static bool SendMail(string smtp_server, string from, string mailto, string subject, string body, bool is_body_html)
        {
            return SendMail(smtp_server, MailPriority.Normal, from, new string[] { mailto }, null, null, subject, body, is_body_html, null);
        }
        public static bool SendMail(string smtp_server, MailPriority priority, string from, string[] mailto, string[] cc, string[] bcc, string subject, string body, bool is_body_html, string[] attachments)
        {
            return SendMail(smtp_server, priority, from, mailto, cc, bcc, subject, body, is_body_html, attachments, null, "");
        }
        public static bool SendMail(string smtp_server, MailPriority priority, string from, string[] mailto, string[] cc, string[] bcc, string subject, string body, bool is_body_html, string[] attachments, string reply_to)
        {
            return SendMail(smtp_server, priority, from, mailto, cc, bcc, subject, body, is_body_html, attachments, null, reply_to);
        }
        public static bool SendMail(string smtp_server, MailPriority priority, string from, string[] mailto, string[] cc, string[] bcc, string subject, string body, bool is_body_html, string[] attachments, System.Collections.Specialized.NameValueCollection messageHeaders, string reply_to)
        {
            System.Net.Mail.SmtpClient newmail = GetSmtpClient(smtp_server);
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            if (messageHeaders != null && messageHeaders.Count > 0)
            {
                message.Headers.Add(messageHeaders);
            }
            message.From = new MailAddress(from);
            if (mailto != null)
            {
                foreach (string s in mailto)
                {
                    if (!string.IsNullOrEmpty(s))
                        message.To.Add(s);
                }
            }
            message.IsBodyHtml = is_body_html;
            if (cc != null)
            {
                foreach (string s in cc)
                {
                    if (!string.IsNullOrEmpty(s))
                        message.CC.Add(s);
                }
            }
            if (bcc != null)
            {
                foreach (string s in bcc)
                {
                    if (!string.IsNullOrEmpty(s))
                        message.Bcc.Add(s);
                }
            }
            if (attachments != null)
            {
                foreach (string s in attachments)
                {
                    message.Attachments.Add(new Attachment(s));
                }
            }
            if (!string.IsNullOrEmpty(reply_to)) message.ReplyTo = new MailAddress(reply_to);
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.Priority = priority;
            message.Body = body;
            message.Subject = subject;
            newmail.Send(message);
            return true;
        }

        public static bool isValidEmail(string xEmailAddress)
        {
            bool myIsEmail = false;
            string myRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,50}\.[0-9]{1,50}\.[0-9]{1,50}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{1,50}|[0-9]{1,50})(\]?)$";
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(myRegex);
            if (reg.IsMatch(xEmailAddress))
            {
                myIsEmail = true;
            }
            return myIsEmail;
        }
        public static bool IsValidEmails(string emailAddresses, char separator)
        {
            string[] addresses = emailAddresses.Split(separator);
            foreach (string addr in addresses)
            {
                if (!isValidEmail(addr))
                    return false;
            }
            return true;
        }
    }
}
