using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using PortfolioService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure;
using System.Configuration;

namespace PortfolioService.Repository
{
    public class ProfitRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public ProfitRepository()
        {
            var connectionString = CloudConfigurationManager.GetSetting("DataConnectionString") ??
                                   ConfigurationManager.ConnectionStrings["DataConnectionString"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("DataConnectionString is not set in the configuration.");
            }

            _storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = _storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference("ProfitTable");
            _table.CreateIfNotExists();
        }

        public bool CreateOrUpdate(Profit profit)
        {
            try
            {
                var insertOrReplaceOperation = TableOperation.InsertOrReplace(profit);
                _table.Execute(insertOrReplaceOperation);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<Profit> GetAllProfits()
        {
            try
            {
                var query = new TableQuery<Profit>();
                return _table.ExecuteQuery(query).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Profit GetProfitByUserId(string userId)
        {
            try
            {
                var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId);
                var query = new TableQuery<Profit>().Where(filter);
                return _table.ExecuteQuery(query).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
