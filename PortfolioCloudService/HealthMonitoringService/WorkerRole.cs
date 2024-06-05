using HealthMonitoringService.Implementation;
using Microsoft.Azure;
using Microsoft.Owin.Hosting;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using PortfolioService.Model;
using PortfolioService.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HealthMonitoringService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        UserDataRepository userDataRepository = new UserDataRepository();
        private Timer timer;
        private CloudTable healthCheckTable;
        private IDisposable _app = null;

        public override void Run()
        {
            Trace.TraceInformation("HealthMonitoringService is running");

            InitializeTable();

            timer = new Timer(CheckHealth, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            while (true)
            {
                Thread.Sleep(10000);
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["InputDataD"];
            string baseUri = $"{endpoint.Protocol}://{endpoint.IPEndpoint}";

            Trace.TraceInformation($"Starting OWIN at {baseUri}", "Information");

            _app = WebApp.Start<Startup>(new StartOptions(url: baseUri));
            return base.OnStart();
        }

        public override void OnStop()
        {
            Trace.TraceInformation("HealthMonitoringService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("HealthMonitoringService has stopped");
        }

        private async void CheckHealth(object state)
        {
            bool isPortfolioServiceHealthy = await PerformHealthCheckAsync("http://127.0.0.1:10200/api/check/isAlive");
            bool isNotificationServiceHealthy = await PerformHealthCheckAsync("http://127.0.0.1:10100/api/check/isAlive");

            string message = $"{DateTime.UtcNow:yyyy-MM-dd:HH:mm:ss.fffffff}_{(isPortfolioServiceHealthy ? "OK" : "NOT_OK")}";
            Trace.TraceInformation(message);

            string message1 = $"{DateTime.UtcNow:yyyy-MM-dd:HH:mm:ss.fffffff}_{(isNotificationServiceHealthy ? "OK" : "NOT_OK")}";
            Trace.TraceInformation(message1);

            LogHealthCheck(message);
            LogHealthCheck(message1);

            if(!isNotificationServiceHealthy || !isPortfolioServiceHealthy)
            {
                IEnumerable<User> users = userDataRepository.RetrieveAllUsers();
                Console.WriteLine("Failed health check detected. Sending emails to console application for further processing...");

                SendEmailsToConsoleApp(users.ToList());
            }
        }

        private async void SendEmailsToConsoleApp(List<User> users)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string consoleAppEndpoint = "http://127.0.0.1:10600/api/sendEmails";

                string jsonData = JsonConvert.SerializeObject(users);

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(consoleAppEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Emails data sent to console application successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to send emails data to console application. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while sending emails data to console application: {ex.Message}");
            }
        }


        private async Task<bool> PerformHealthCheckAsync(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseBody = await response.Content.ReadAsStringAsync();
                    if(response.StatusCode == HttpStatusCode.OK)
                    {

                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Health check failed for {url}: {ex.Message}");
                return false;
            }
        }


        private void InitializeTable()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            healthCheckTable = tableClient.GetTableReference("HealthCheck");
            healthCheckTable.CreateIfNotExists();
        }

        private void LogHealthCheck(string message)
        {
            HealthCheckEntity healthCheckEntity = new HealthCheckEntity(DateTime.UtcNow, message);

            TableOperation insertOperation = TableOperation.InsertOrReplace(healthCheckEntity);

            healthCheckTable.Execute(insertOperation);
        }
    }
}
