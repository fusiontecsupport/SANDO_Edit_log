using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace scfs_erp.Models
{
    [SessionExpire]
    public class CommonEmailFunction
    {
        public string PasswordRecovery(int qmbtype, string str_email, string type, string str_subject, string str_link, string str_firstname, string CC, string BCC, HttpPostedFileBase[] Attachedfile, FileInfo[] DefaultFileNames, string strInvoiceFullPath, string strLOGFullPath, string strSCFullPath)
        {
            string str_msg = "success";
            string str_template = "";
            //if (type == "registration")
            //    str_template = "AddMember.txt";

            SmtpClient smtp = new SmtpClient();
            smtp.Host = HttpContext.Current.Session["SMTPNAME"].ToString();
            smtp.Port = Convert.ToInt16(HttpContext.Current.Session["SMTPPORT"].ToString());
            smtp.EnableSsl = Convert.ToBoolean(HttpContext.Current.Session["SMTPEnableSsl"]);
            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(HttpContext.Current.Session["SMTPCredentialsUser"].ToString(), HttpContext.Current.Session["SMTPCredentialsPassword"].ToString()); //new NetworkCredential(HttpContext.Current.Session["COMPEMAIL"].ToString(), HttpContext.Current.Session["EMAILPWD"].ToString());
            smtp.Timeout = 50000;
            MailMessage mail = new MailMessage();

            //Setting From , To and CC
            mail.From = new MailAddress(HttpContext.Current.Session["FromMailIDs"].ToString(), HttpContext.Current.Session["FromMailDisplays"].ToString());//new MailAddress(HttpContext.Current.Session["COMPEMAIL"].ToString(), HttpContext.Current.Session["COMPNAME"].ToString());
            foreach (var address in str_email.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                mail.To.Add(new MailAddress(address));
            }
            string subject = str_subject;
            string body = str_link;

            mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html));
            mail.IsBodyHtml = true;
            mail.Subject = subject;
            
            if (CC != null && CC != "")
            {
                foreach (var CCaddress in CC.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mail.CC.Add(new MailAddress(CCaddress));
                }
            }
            
            if (BCC != null && BCC != "")
            {
                foreach (var BCCaddress in BCC.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mail.Bcc.Add(new MailAddress(BCCaddress));
                }
            }
            //Default File Name
            if (qmbtype == 0)
            {
                if (DefaultFileNames != null)
                {
                    foreach (FileInfo DefaultFileName in DefaultFileNames)
                    {
                        if (DefaultFileName != null)
                        {
                            string fileName = Path.GetFileName(DefaultFileName.FullName);
                            mail.Attachments.Add(new Attachment(DefaultFileName.FullName));
                        }
                    }
                }
            }

            //InvoiceFullPath File Name
            if (strInvoiceFullPath != string.Empty)
            {
                string strInvoiceFullPathValuue = HttpContext.Current.Server.MapPath(strInvoiceFullPath);
                mail.Attachments.Add(new Attachment(strInvoiceFullPathValuue));
            }

            //LOGFullPath File Name
           // if (strLOGFullPath != string.Empty)
            //{
            //    string strLOGFullPathValue = HttpContext.Current.Server.MapPath(strLOGFullPath);
            //    mail.Attachments.Add(new Attachment(strLOGFullPathValue));
            //}

            //LOGFullPath File Name
           // if (strSCFullPath != string.Empty)
            //{
             //   string strSCFullPathValue = HttpContext.Current.Server.MapPath(strSCFullPath);
              //  mail.Attachments.Add(new Attachment(strSCFullPathValue));
           // }

            //Attchedfile
            if (Attachedfile != null)
            {
                foreach (HttpPostedFileBase fileimg in Attachedfile)
                {
                    if (fileimg != null)
                    {
                        string fileName = Path.GetFileName(fileimg.FileName);
                        mail.Attachments.Add(new Attachment(fileimg.InputStream, fileName));
                    }
                }
            }
            smtp.Send(mail);

            foreach (Attachment attachment in mail.Attachments)
            {
                attachment.Dispose();
            }

            mail.Attachments.Clear();
            mail.Attachments.Dispose();
            //client.Dispose();


            string InvoicePath = HttpContext.Current.Server.MapPath(strInvoiceFullPath);

            if (Directory.Exists(Path.GetDirectoryName(InvoicePath)))
            {
                using (FileStream fileStream = System.IO.File.Open(InvoicePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    if (fileStream != null) fileStream.Close();  //This line is me being overly cautious, fileStream will never be null unless an exception occurs... and I know the "using" does it but its helpful to be explicit - especially when we encounter errors - at least for me anyway!
                }

                System.IO.FileStream fs;
                fs = System.IO.File.Open(InvoicePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Read, System.IO.FileShare.None);
                fs.Close();

                System.IO.File.Delete(InvoicePath);
            }

            return str_msg;
        }
        public string Replyemail(string str_email, string type, string str_subject, string str_link, string str_firstname, string CC, HttpPostedFileBase[] Attachedfile, string msgid, string strInvoiceFullPath)
        {
            string str_msg = "success";

            SmtpClient smtp = new SmtpClient();
            smtp.Host = HttpContext.Current.Session["SMTPNAME"].ToString();
            smtp.Port = Convert.ToInt16(HttpContext.Current.Session["SMTPPORT"].ToString());
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(HttpContext.Current.Session["USEREMAIL"].ToString(), HttpContext.Current.Session["EMAILPWD"].ToString());
            smtp.Timeout = 50000;
            MailMessage mail = new MailMessage();

            //Setting From , To and CC
            mail.From = new MailAddress(HttpContext.Current.Session["USEREMAIL"].ToString(), "Sanco CFS");
            foreach (var address in str_email.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                mail.To.Add(new MailAddress(address));

            }
            mail.Headers.Add("In-Reply-To", msgid);
            mail.Headers.Add("References", msgid);
            string subject = "Re:" + str_subject;
            string body = str_link;

            mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html));
            mail.IsBodyHtml = true;
            mail.Subject = subject;
            if (CC != null && CC != "")
            {
                foreach (var CCaddress in CC.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mail.CC.Add(new MailAddress(CCaddress));
                }
            }

            //InvoiceFullPath File Name
            if (strInvoiceFullPath != string.Empty)
            {
                string strInvoiceFullPathValuue = HttpContext.Current.Server.MapPath(strInvoiceFullPath);
                mail.Attachments.Add(new Attachment(strInvoiceFullPathValuue));
            }

            if (Attachedfile != null)
            {
                foreach (HttpPostedFileBase fileimg in Attachedfile)
                {
                    if (fileimg != null)
                    {
                        string fileName = Path.GetFileName(fileimg.FileName);
                        mail.Attachments.Add(new Attachment(fileimg.InputStream, fileName));
                    }
                }
            }
            smtp.Send(mail);
            mail.Attachments.Clear();
            mail.Attachments.Dispose();
            return str_msg;
        }

        public string Replyallemail(string str_email, string type, string str_subject, string str_link, string str_firstname, string CC, HttpPostedFileBase[] Attachedfile, string msgid, string strInvoiceFullPath)
        {
            string str_msg = "success";
            string str_template = "";


            SmtpClient smtp = new SmtpClient();
            smtp.Host = HttpContext.Current.Session["SMTPNAME"].ToString();
            smtp.Port = Convert.ToInt16(HttpContext.Current.Session["SMTPPORT"].ToString());
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(HttpContext.Current.Session["USEREMAIL"].ToString(), HttpContext.Current.Session["EMAILPWD"].ToString());
            smtp.Timeout = 50000;
            MailMessage mail = new MailMessage();

            //Setting From , To and CC
            mail.From = new MailAddress(HttpContext.Current.Session["USEREMAIL"].ToString(), "KGK Jet India");
            foreach (var address in str_email.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                mail.To.Add(new MailAddress(address));

            }
            mail.Headers.Add("In-Reply-To", msgid);
            mail.Headers.Add("References", msgid);
            string subject = "Re:" + str_subject;
            string body = str_link;

            mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html));
            mail.IsBodyHtml = true;
            mail.Subject = subject;
            if (CC != null && CC != "")
            {
                foreach (var CCaddress in CC.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mail.CC.Add(new MailAddress(CCaddress));
                }
            }

            //InvoiceFullPath File Name
            if (strInvoiceFullPath != string.Empty)
            {
                string strInvoiceFullPathValuue = HttpContext.Current.Server.MapPath(strInvoiceFullPath);
                mail.Attachments.Add(new Attachment(strInvoiceFullPathValuue));
            }


            if (Attachedfile != null)
            {
                foreach (HttpPostedFileBase fileimg in Attachedfile)
                {
                    if (fileimg != null)
                    {
                        string fileName = Path.GetFileName(fileimg.FileName);
                        mail.Attachments.Add(new Attachment(fileimg.InputStream, fileName));
                    }
                }
            }
            smtp.Send(mail);
            mail.Attachments.Clear();
            mail.Attachments.Dispose();
            return str_msg;
        }

        public string Logisticsmail(string str_email, string str_subject, string str_link, string CC, HttpPostedFileBase[] Attachedfile, string invfilename, string pslipfilename)
        {
            string str_msg = "success";

            SmtpClient smtp = new SmtpClient();
            smtp.Host = HttpContext.Current.Session["SMTPNAME"].ToString();
            smtp.Port = Convert.ToInt16(HttpContext.Current.Session["SMTPPORT"].ToString());
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(HttpContext.Current.Session["USEREMAIL"].ToString(), HttpContext.Current.Session["EMAILPWD"].ToString());
            smtp.Timeout = 50000;
            MailMessage mail = new MailMessage();

            //Setting From , To and CC
            mail.From = new MailAddress(HttpContext.Current.Session["USEREMAIL"].ToString(), "KGK Jet India");
            foreach (var address in str_email.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                mail.To.Add(new MailAddress(address));
            }
            string subject = str_subject;
            string body = str_link;

            mail.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html));
            mail.IsBodyHtml = true;
            mail.Subject = subject;
            if (CC != null && CC != "")
            {
                foreach (var CCaddress in CC.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mail.CC.Add(new MailAddress(CCaddress));
                }
            }


            //Invoice File Name
            if (invfilename != string.Empty)
            {
                //  string strinvfilenamePathValuue = HttpContext.Current.Server.MapPath(invfilename);
                mail.Attachments.Add(new Attachment(invfilename));
            }

            //Packing File Name
            if (pslipfilename != string.Empty)
            {
                // string strpslipfilenamehValue = HttpContext.Current.Server.MapPath(pslipfilename);
                mail.Attachments.Add(new Attachment(pslipfilename));
            }


            //Attchedfile
            if (Attachedfile != null)
            {
                foreach (HttpPostedFileBase fileimg in Attachedfile)
                {
                    if (fileimg != null)
                    {
                        string fileName = Path.GetFileName(fileimg.FileName);
                        mail.Attachments.Add(new Attachment(fileimg.InputStream, fileName));
                    }
                }
            }
            smtp.Send(mail);

            foreach (Attachment attachment in mail.Attachments)
            {
                attachment.Dispose();
            }

            mail.Attachments.Clear();
            mail.Attachments.Dispose();
            //client.Dispose();


            //string InvoicePath = HttpContext.Current.Server.MapPath(strInvoiceFullPath);

            //if (Directory.Exists(Path.GetDirectoryName(InvoicePath)))
            //{
            //    using (FileStream fileStream = System.IO.File.Open(InvoicePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            //    {
            //        if (fileStream != null) fileStream.Close();  //This line is me being overly cautious, fileStream will never be null unless an exception occurs... and I know the "using" does it but its helpful to be explicit - especially when we encounter errors - at least for me anyway!
            //    }

            //    System.IO.FileStream fs;
            //    fs = System.IO.File.Open(InvoicePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Read, System.IO.FileShare.None);
            //    fs.Close();

            //    System.IO.File.Delete(InvoicePath);
            //}

            return str_msg;
        }


    }

}