using System;
using System.Collections.Generic;
using System.Linq;
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

            //try
            //{
            //    smtpClient.Send(mail);
            //    Console.WriteLine("Email sent successfully.");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Failed to send email: " + ex.Message);
            //}
        }
    }
}