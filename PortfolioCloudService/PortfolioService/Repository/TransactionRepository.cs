using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PortfolioService.Model;

namespace PortfolioService.Repository
{
    public class TransactionRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        public TransactionRepository()
        {
            var connectionString = CloudConfigurationManager.GetSetting("DataConnectionString") ??
                                   ConfigurationManager.ConnectionStrings["DataConnectionString"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("DataConnectionString is not set in the configuration.");
            }

            _storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = _storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference("TransactionTable");
            _table.CreateIfNotExists();
        }
        public bool Create(Transaction transaction)
        {
            try
            {
                var insertOperation = TableOperation.Insert(transaction);
                _table.Execute(insertOperation);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteCrypto(string userId, string currency)
        {
            try
            {
                var query = new TableQuery<Transaction>().Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("Currency", QueryComparisons.Equal, currency)
                    ));

                var transactions = _table.ExecuteQuery(query).ToList();

                foreach (var transaction in transactions)
                {
                    var deleteOperation = TableOperation.Delete(transaction);
                    _table.Execute(deleteOperation);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CreateSaleTransaction(Transaction transaction)
        {
            try
            {
                // Implement logic similar to the Python code here

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<Transaction> GetTransactionByUser(string userId)
        {
            try
            {
                var query = new TableQuery<Transaction>().Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId));

                var transactions = _table.ExecuteQuery(query).ToList();
                return transactions;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Transaction> GetCryptoCurrenciesByUser(string userId, bool isYesterday)
        {
            try
            {
                // Implement logic similar to the Python code here
                return new List<Transaction>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool DeleteTransactionById(string transactionId)
        {
            try
            {
                var retrieveOperation = TableOperation.Retrieve<Transaction>(transactionId, transactionId);
                var result = _table.Execute(retrieveOperation);
                var transaction = result.Result as Transaction;

                if (transaction == null)
                {
                    return false;
                }

                var deleteOperation = TableOperation.Delete(transaction);
                _table.Execute(deleteOperation);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int FindMaxId()
        {
            try
            {
                var query = new TableQuery<Transaction>().Select(new[] { "RowKey" });
                var transactions = _table.ExecuteQuery(query).ToList();
                var maxId = transactions.Max(t => int.Parse(t.RowKey));
                return maxId;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}