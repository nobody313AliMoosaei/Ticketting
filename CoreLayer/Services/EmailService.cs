using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Services
{
    public class EmailService : IEmailService
    {
        public Task SendEmail(string To, string Title, string Body)
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp.mail.yahoo.com",25);
                //client.Port = 587;
                //client.Host = "smtp.mail.yahoo.com";
                client.EnableSsl = true;
                client.Timeout = 1000000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                
                //در خط بعدی ایمیل  خود و پسورد ایمیل خود  را جایگزین کنید

                client.Credentials = new NetworkCredential("tickettinggroup@yahoo.com", "bhedkhjtszmogxsp");
                MailMessage message = new MailMessage("tickettinggroup@yahoo.com", To, Title, Body);
                message.IsBodyHtml = true;
                message.BodyEncoding = UTF8Encoding.UTF8;
                message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
                client.Send(message);

                return Task.CompletedTask;
            }
            catch(Exception ex)
            {
                var message = ex.Message;
                var data = ex.Data;
                var mes = ex.Source;

                return null;
            }
        }
    }
}
