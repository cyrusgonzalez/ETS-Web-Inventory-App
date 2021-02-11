using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using Inventory_WebApp.DatabaseInterface;
using log4net;

namespace Inventory_WebApp.Pages
{
    public class EMailHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public bool sendEmail()
        {
            DBOps db = new DBOps();
            try
            {

                DataSet from = db.GetConfig("senderEmail");
                DataSet to = db.GetConfig("receiverEmail");


                var fromAddress = new MailAddress("etsinventoryalert@gmail.com", "ETS_Inventory_Alerts");
                var toAddress = new MailAddress("sanketmehrotra102@gmail.com", "Recievers");
                const string fromPassword = "ETS_Inventory_2021";
                const string subject = "Subject";
                const string body = "Body";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }

                return true;
            } catch(Exception ex)
            {
                log.Error("An error happened", ex);
                return false;
            }
            
        }

        public string prepItemEmail(string item, string lab, int quantity)
        {
            return $"The Item: {item} in lab {lab} is low in quantity. It's current quantity is {quantity.ToString()}";
        }

        public bool sendItemEmail(string item, string lab, int quantity)
        {
            DBOps db = new DBOps();
            try
            {
                string from = db.GetConfig("senderEmail").Tables[0].Rows[0]["configval1"].ToString();
                string to = db.GetConfig("receiverEmail").Tables[0].Rows[0]["configval1"].ToString();
                MailAddress fromAddress = new MailAddress(from, "ETS_Inventory");
                MailAddress toAddress = new MailAddress(to, "ETS Team");
                string fromPassword = db.GetConfig("senderPassword").Tables[0].Rows[0]["configval1"].ToString();
                string body = prepItemEmail(item, lab, quantity);
                string subject = "ETS Inventory: Low Item Quantity Alert";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false, //true for multiple row table i think.
                    BodyEncoding = System.Text.Encoding.UTF8,
                })
                {

                    smtp.Send(message); //trySend() in case of failure.
                }

                return true;

            }
            catch (Exception ex)
            {
                log.Error("Error in sendItem", ex);
                return false;
            }
        }
    }
}