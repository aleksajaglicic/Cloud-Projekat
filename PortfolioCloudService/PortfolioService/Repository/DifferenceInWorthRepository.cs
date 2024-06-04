using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortfolioService.Model;

namespace PortfolioService.Repository
{
    public class DifferenceInWorthRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public DifferenceInWorthRepository()
        {
            var connectionString = CloudConfigurationManager.GetSetting("DataConnectionString") ??
                                   ConfigurationManager.ConnectionStrings["DataConnectionString"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("DataConnectionString is not set in the configuration.");
            }

            _storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = _storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference("DifferenceInWorthTable");
            _table.CreateIfNotExists();
        }

        public void CreateDifferenceEntry(DifferenceInWorth differenceInWorth)
        {

        }
    }
}
