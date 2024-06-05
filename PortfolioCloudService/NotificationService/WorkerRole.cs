using Common;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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

        public override void Run()
        {
            Trace.TraceInformation("NotificationService is running");

            var storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("DataConnectionString"));
            var queueClient = storageAccount.CreateCloudQueueClient();
            alarmsDoneQueue = queueClient.GetQueueReference("alarmsdone");
            alarmsDoneQueue.CreateIfNotExists();

            // Initialize and start the timer
            timer = new Timer(SendNotifications, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

            while (true)
            {
                Thread.Sleep(10000); // Sleep to prevent the thread from exiting
            }

            //try
            //{
            //    this.RunAsync(this.cancellationTokenSource.Token).Wait();
            //}
            //finally
            //{
            //    this.runCompleteEvent.Set();
            //}
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("NotificationService has been started");

            return result;
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
            // Retrieve up to 20 alarms and check if desired profit is reached
            // If reached, send emails to subscribed users
            // Dummy implementation for demonstration purposes

            // Simulate sending notification emails
            for (int i = 1; i <= 20; i++)
            {
                //if (i % 5 == 0) // Example condition for reaching desired profit
                //{
                //    // Send email notifications
                //    string userEmail = $"user{i}@example.com";
                //    EmailService.SendEmail(userEmail, "Profit Alert", $"Congratulations! Your desired profit is reached.");
                //}
                



                //Ako je profit dobar
                // salji email
            }

            // After sending notifications, send information to the alarmsdone queue
            SendToAlarmsDoneQueue();
        }

        private void SendToAlarmsDoneQueue()
        {
            // Send information about completed alarms to the alarmsdone queue
            // Dummy implementation for demonstration purposes
            for (int i = 1; i <= 20; i++)
            {
                string alarmId = $"alarm{i}";
                CloudQueueMessage message = new CloudQueueMessage(alarmId);
                alarmsDoneQueue.AddMessage(message);
            }
        }
    }
}
