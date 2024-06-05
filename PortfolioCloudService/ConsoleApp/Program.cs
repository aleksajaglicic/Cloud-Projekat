using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string healthMonitoringServiceUrl = "http://127.0.0.1:10401/health-check"; // Update with the actual endpoint of the health monitoring system
        string failedHealthCheckEmailsUrl = "http://127.0.0.1:10401/api/users/failedHealthCheckEmails";

        HttpClient httpClient = new HttpClient();

        while (true)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(healthMonitoringServiceUrl);
                response.EnsureSuccessStatusCode(); // Throws exception if not successful

                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Health Check Result: {result}");

                if (result != "OK")
                {
                    List<string> failedHealthCheckEmails = await GetFailedHealthCheckEmails(httpClient, failedHealthCheckEmailsUrl);
                    foreach (var email in failedHealthCheckEmails)
                    {
                        SendEmail(email, "FAIL - Health Check", "The HealthCheck for the Portfolio Service has failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching health check results: {ex.Message}");
            }

            // Wait for a few seconds before the next health check
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }

    public static async Task<List<string>> GetFailedHealthCheckEmails(HttpClient httpClient, string url)
    {
        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode(); // Throws exception if not successful

            // Read the response content as a string
            string content = await response.Content.ReadAsStringAsync();

            // Parse the content as a list of strings (assuming each string represents an email)
            List<string> emails = ParseEmails(content);

            // Return the list of emails
            return emails;
        }
        catch (HttpRequestException ex)
        {
            // Log any HTTP request exceptions
            Console.WriteLine($"HTTP request error: {ex.Message}");
            throw; // Rethrow the exception to handle it at a higher level if necessary
        }
        catch (Exception ex)
        {
            // Log any other exceptions that occur
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw; // Rethrow the exception to handle it at a higher level if necessary
        }
    }

    private static List<string> ParseEmails(string content)
    {
        // Implement logic to parse the content string and extract email addresses
        // Here's a placeholder implementation assuming the content is a comma-separated list of emails
        string[] emailArray = content.Split(',');
        List<string> emails = new List<string>(emailArray);
        return emails;
    }

    static void SendEmail(string recipient, string subject, string body)
    {
        // Set up the SMTP client
        using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
        {
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential("aleksajaglicic070@gmail.com", "mdfn geem ogav qsue");
            smtpClient.EnableSsl = true;

            // Create the email message
            MailMessage message = new MailMessage();
            message.From = new MailAddress("aleksajaglicic@gmail.com");
            message.To.Add(new MailAddress(recipient));
            message.Subject = subject;
            message.Body = body;

            // Send the email
            smtpClient.Send(message);
        }
    }

}
