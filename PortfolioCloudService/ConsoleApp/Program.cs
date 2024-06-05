using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string receiveEmailsEndpoint = "http://127.0.0.1:10800/api/User/get";
        string sendEmailsEndpoint = "http://127.0.0.1:5000/api/sendEmails";

        HttpClient httpClient = new HttpClient();

        while (true)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(receiveEmailsEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string jsonData = await response.Content.ReadAsStringAsync();

                    List<string> emails = JsonConvert.DeserializeObject<List<string>>(jsonData);

                    foreach (var email in emails)
                    {
                        Console.WriteLine($"Sending email to: {email}");
                        SendEmail(email, "Subject", "Body");
                    }

                    await SendProcessedEmails(httpClient, sendEmailsEndpoint, emails);
                }
                else
                {
                    Console.WriteLine($"Failed to receive data. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while receiving data: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }

    static async Task SendProcessedEmails(HttpClient httpClient, string endpoint, List<string> emails)
    {
        try
        {
            string jsonData = JsonConvert.SerializeObject(emails);

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Processed emails sent successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to send processed emails. Status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while sending processed emails: {ex.Message}");
        }
    }

    static void SendEmail(string recipient, string subject, string body)
    {
        using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
        {
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential("aleksajaglicic070@gmail.com", "mdfn geem ogav qsue");
            smtpClient.EnableSsl = true;

            MailMessage message = new MailMessage();
            message.From = new MailAddress("aleksajaglicic@gmail.com");
            message.To.Add(new MailAddress(recipient));
            message.Subject = subject;
            message.Body = body;

            smtpClient.Send(message);
        }
    }
}
