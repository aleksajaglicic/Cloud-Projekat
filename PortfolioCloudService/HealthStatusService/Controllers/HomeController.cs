using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HealthMonitoringService;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace HealthStatusService.Controllers
{
    public class HealthCheckController : Controller
    {
        public ActionResult Index()
        {
            List<HealthCheckEntity> healthCheckData = GetHealthCheckData();
            double uptimePercentage = CalculateUptimePercentage(healthCheckData);
            ViewBag.UptimePercentage = uptimePercentage;
            return View();
        }

        private List<HealthCheckEntity> GetHealthCheckData()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DataConnectionString");
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable healthCheckTable = tableClient.GetTableReference("HealthCheck");

            TableQuery<HealthCheckEntity> query = new TableQuery<HealthCheckEntity>()
                .Where(TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, DateTimeOffset.UtcNow.AddHours(-1)));

            List<HealthCheckEntity> healthCheckData = healthCheckTable.ExecuteQuery(query).ToList();
            return healthCheckData;
        }

        private double CalculateUptimePercentage(List<HealthCheckEntity> healthCheckData)
        {
            int totalChecks = healthCheckData.Count;
            int successfulChecks = healthCheckData.Count(e => e.Message.Contains("_OK"));

            if (totalChecks == 0)
            {
                return 0;
            }

            double uptimePercentage = (double)successfulChecks / totalChecks * 100;
            return uptimePercentage;
        }
    }
}
