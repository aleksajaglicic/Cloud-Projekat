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
using System.Diagnostics;

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

        public bool CreateDifferenceEntry(DifferenceInWorth differenceInWorth)
        {
            try
            {
                var insertOperation = TableOperation.InsertOrMerge(differenceInWorth);
                _table.Execute(insertOperation);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public decimal GetDifferenceByUserIdCurrency(string userId, string currency)
        {
            try
            {
                var query = new TableQuery<DifferenceInWorth>()
                    .Where(TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("Currency", QueryComparisons.Equal, currency)));

                //var result = null; /*_table.ExecuteQuery(query).FirstOrDefault();*/

                //return result != null ? result.Difference : 0.0m;
                return 0.0M;
            }
            catch (Exception ex)
            {
                // Log the exception
                Trace.TraceError($"Error retrieving difference entry: {ex.Message}");
                return 0.0m;
            }
        }
    }
}
