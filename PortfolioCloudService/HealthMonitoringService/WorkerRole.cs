using Microsoft.Azure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace HealthMonitoringService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private Timer timer;
        private CloudTable healthCheckTable;

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

            bool result = base.OnStart();

            Trace.TraceInformation("HealthMonitoringService has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("HealthMonitoringService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("HealthMonitoringService has stopped");
        }

        private void CheckHealth(object state)
        {
            // Perform health check by sending requests to PortfolioService and NotificationService
            bool isPortfolioServiceHealthy = PerformHealthCheck("http://127.0.0.1:10201/health-monitoring");
            bool isNotificationServiceHealthy = PerformHealthCheck("http://127.0.0.1:10301/health-monitoring");

            string message = $"{DateTime.UtcNow:yyyy-MM-dd:HH:mm:ss.fffffff}_{(isPortfolioServiceHealthy ? "OK" : "NOT_OK")}";
            Trace.TraceInformation(message);
            // Log the health check result
            LogHealthCheck(message);
        }

        private bool PerformHealthCheck(string url)
        {
            try
            {
                // Perform a web request to check the health status
                using (var client = new WebClient())
                {
                    string response = client.DownloadString(url);
                    // Check if the response indicates a healthy status
                    return response.Trim().Equals("OK", StringComparison.OrdinalIgnoreCase);
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
            // Retrieve the storage account from the connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));

            // Create the table client
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create or reference an existing table
            healthCheckTable = tableClient.GetTableReference("HealthCheck");
            healthCheckTable.CreateIfNotExists();
        }

        private void LogHealthCheck(string message)
        {
            // Create a new health check entity
            HealthCheckEntity healthCheckEntity = new HealthCheckEntity(DateTime.UtcNow, message);

            // Create the TableOperation object that inserts the health check entity
            TableOperation insertOperation = TableOperation.Insert(healthCheckEntity);

            // Execute the insert operation
            healthCheckTable.Execute(insertOperation);
        }
    }
}
