using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class EmailService
    {
        private static SmtpClient smtpClient;

        public static void SetSmtpClient(SmtpClient client)
        {
            smtpClient = client;
        }

        public static void SendEmail(string to, string subject, string body)
        {
            MailMessage mail = new MailMessage("aleksajaglicic@gmail.com", to, subject, body);

            smtpClient = new SmtpClient();
            smtpClient.Host = "http://127.0.0.1:5000/sendEmails";
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential("aleksajaglicic070@gmail.com", "mdfn geem ogav qsue");
            smtpClient.EnableSsl = true;

            MailMessage message = new MailMessage();
            message.From = new MailAddress("aleksajaglicic@gmail.com");
            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.Body = body;

            smtpClient.Send(message);
        }
    }
}