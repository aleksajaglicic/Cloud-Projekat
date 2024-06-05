using Common;
using Microsoft.Owin.Hosting;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using NotificationService.Implementation;
using PortfolioService.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private Timer timer;
        private CloudQueue alarmsDoneQueue;
        private IDisposable _app = null;

        public override void Run()
        {
            Trace.TraceInformation("NotificationService is running");

            var storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("DataConnectionString"));
            var queueClient = storageAccount.CreateCloudQueueClient();
            alarmsDoneQueue = queueClient.GetQueueReference("alarmsdone");
            alarmsDoneQueue.CreateIfNotExists();

            timer = new Timer(SendNotifications, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

            while (true)
            {
                Thread.Sleep(10000);
            }

        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["InputData"];
            string baseUri = $"{endpoint.Protocol}://{endpoint.IPEndpoint}";

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            _app = WebApp.Start<Startup>(new StartOptions(url: baseUri));
            return base.OnStart();
        }

        public override void OnStop()
        {
            Trace.TraceInformation("NotificationService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("NotificationService has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }

        private void SendNotifications(object state)
        {
            // Define your SMTP server settings
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587; // Gmail SMTP port

            // Specify sender credentials
            string senderEmail = "aleksajaglicic070@gmail.com";
            string senderPassword = "mdfn geem ogav qsue";

            using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                smtpClient.EnableSsl = true;

                for (int i = 1; i <= 20; i++)
                {
                    if (i % 5 == 0)
                    {
                        UserDataRepository userDataRepository = new UserDataRepository();
                        List<string> failedHealthCheckEmails = userDataRepository.GetFailedHealthCheckEmails();
                        foreach (var email in failedHealthCheckEmails)
                        {
                            MailMessage message = new MailMessage(senderEmail, email, "Profit Alert", "Congratulations! Your desired profit is reached.");

                            try
                            {
                                smtpClient.SendMailAsync(message);
                                Console.WriteLine($"Email sent to: {email}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Failed to send email to {email}: {ex.Message}");
                            }
                        }
                    }
                }
            }
        }
    }
}
