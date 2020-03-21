using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace Msdfa.Core.Tools
{
    public class Mailer
    {
        SmtpClient SmtpServer;
        MailMessage Mail;
        public bool IsEnabled { get; set; } = true;
        public List<string> RedirectionAddressList { get; set; } = new List<string>();

        public bool HasRecipients => Mail.To.Count > 0;

        public Mailer(string smtpServerAddress, string login, string password, int port = 587, bool authorization = true)
        {
            this.Mail = new MailMessage();
            this.SmtpServer = new SmtpClient(smtpServerAddress);
            this.SmtpServer.Port = port;
            if (authorization)
            {
                SmtpServer.Credentials = new System.Net.NetworkCredential(login, password);
                SmtpServer.EnableSsl = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }
        }

        public Mailer NewMail(string subject = null)
        {
            this.Mail = new MailMessage();
            this.Mail.Subject = subject;
            return this;
        }

        public Mailer SetFrom(string from, string displayName = null)
        {
            this.Mail.From = new MailAddress(from, displayName);
            return this;
        }

        public Mailer SetSender(string from)
        {
            this.Mail.Sender = new MailAddress(from);
            return this;
        }

        public Mailer SetReplyTo(string replyTo)
        {
            this.Mail.ReplyToList.Add(replyTo);
            return this;
        }

        public Mailer SetReplyTo(List<string> replyToList)
        {
            replyToList.ForEach(this.Mail.ReplyToList.Add);
            return this;
        }

        public Mailer AddRecipent(string toAddress)
        {
            if (!this.Mail.To.Select(x => x.Address).Contains(toAddress))
                if (!String.IsNullOrWhiteSpace(toAddress))
                    this.Mail.To.Add(new MailAddress(toAddress));

            return this;
        }

        public Mailer SetUseHtml(bool value = true)
        {
            this.Mail.IsBodyHtml = value;
            return this;
        }

        public Mailer SetRecipent(string toAddress)
        {
            this.Mail.To.Clear();
            this.Mail.To.Add(new MailAddress(toAddress));
            return this;
        }

        public Mailer SetRecipents(string[] toAddressArray)
        {
            this.Mail.To.Clear();
            foreach (var item in toAddressArray.Where(x => x.Trim().Length > 0).Distinct())
            {
                if (!this.Mail.To.Select(x => x.Address).Contains(item))
                {
                    this.Mail.To.Add(GetMailAddress(item));
                }
            }
            return this;
        }

        public Mailer SetRecipents(List<string> toAddressList)
        {
            return this.SetRecipents(toAddressList.ToArray());
        }

        public Mailer SetCcRecipients(List<string> toAddressList)
        {
            foreach (var item in toAddressList.Distinct())
            {
                if (!this.Mail.CC.Select(x => x.Address).Contains(item))
                {
                    this.Mail.CC.Add(GetMailAddress(item));
                }
            }
            return this;
        }

        private MailAddress GetMailAddress(string mail)
        {
            mail = mail.Trim().TrimEnd(';');
            return new MailAddress(mail);
        }

        public Mailer SetSubject(string subject)
        {
            this.Mail.Subject = subject;
            return this;
        }

        public Mailer SetBody(string body)
        {
            this.Mail.Body = body;
            return this;
        }

        public Mailer ClearAttachments()
        {
            this.Mail.Attachments.Clear();
            return this;
        }

        public Mailer ClearRecipents()
        {
            this.Mail.To.Clear();
            this.Mail.CC.Clear();
            return this;
        }

        public Mailer AddAttachment(string fileName)
        {
            string fName = null;
            if (fileName.Contains("/")) fName = fileName.Split('/').Last();
            else if (fileName.Contains(@"\")) fName = fileName.Split('\\').Last();
            else fName = fileName;

            var att = new Attachment(fileName);
            att.Name = fName;
            this.Mail.Attachments.Add(att);
            return this;
        }

        //public Mailer SetDeleteAttachmentsAfterSending(bool value)
        //{
        //   if (value == true)
        //   {
        //      this.SmtpServer.SendCompleted += new SendCompletedEventHandler(SmtpServer_SendCompleted);
        //   }
        //   return this;
        //}

        //void SmtpServer_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        //{
        //   foreach (var att in this.Attachments)
        //   {
        //      var fileName = att.ContentDisposition.FileName;
        //      att.Dispose();
        //      System.IO.File.Delete(fileName);
        //   }
        //}

        public Mailer Send()
        {
            if (this.IsEnabled == false) return this;

            // Jeżeli mail jest przekierowany na inny adres, zmieniamy adresata, a informacje dodajemy do treści maila.
            if (this.RedirectionAddressList.Any())
            {
                var toList = this.Mail.To.Select(x => x.Address);
                var ccList = this.Mail.CC.Select(x => $"(CC): {x.Address}");

                if (this.Mail.IsBodyHtml)
                {
                    var msg = $@"<hr><b>Mail has been redirected to: {string.Join(", ", this.RedirectionAddressList)}</b><br>
Original To: {string.Join(", ", toList.Union(ccList))}<br>
<hr>";
                    this.Mail.Body = $@"{msg}<br>
{this.Mail.Body}";
                }

                else
                {
                    var msg = $@"---------------------[ Mail redirection message ]---------------------   
Mail has been redirected to: {string.Join(", ", this.RedirectionAddressList)}   
Original To: {string.Join(", ", toList.Union(ccList))}   
---------------------[ End of redirection message ]---------------------   
   ";
                    this.Mail.Body = $@"{msg}
{this.Mail.Body}";
                }

                this.ClearRecipents();
                this.SetRecipents(this.RedirectionAddressList);
            }
            
            this.SmtpServer.Send(this.Mail);
            this.Mail.Dispose();
            return this;
        }

        //public void Dispose()
        //{
        //   this.SmtpServer.SendCompleted -= new SendCompletedEventHandler(SmtpServer_SendCompleted);
        //}
    }
}