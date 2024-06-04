using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace HealthMonitoringService
{
    public class HealthCheckEntity : TableEntity
    {
        public HealthCheckEntity(DateTime timestamp, string message)
        {
            this.PartitionKey = timestamp.ToString("yyyyMMdd");
            this.RowKey = timestamp.ToString("HHmmssfff");
            this.Timestamp = timestamp;
            this.Message = message;
        }

        public HealthCheckEntity() { }

        public string Message { get; set; }
    }
}
