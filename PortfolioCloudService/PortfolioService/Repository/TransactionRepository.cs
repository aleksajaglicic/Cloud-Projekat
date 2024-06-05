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
using System.Diagnostics;

namespace PortfolioService.Repository
{
    public class TransactionRepository
    {
        private DifferenceInWorthRepository _diffRepository = new DifferenceInWorthRepository();
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
                transaction.RowKey = transaction.User_id;
                transaction.PartitionKey = "Transaction";
                transaction.Id = Guid.NewGuid();
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
                var partitionKeyCondition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Transaction");
                var rowKeyCondition = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, userId);
                var combinedCondition = TableQuery.CombineFilters(partitionKeyCondition, TableOperators.And, rowKeyCondition);

                var query = new TableQuery<Transaction>().Where(combinedCondition);

                var transactions = _table.ExecuteQuery(query).ToList();
                return transactions;
            }
            catch (Exception ex)
            {
                // Log the exception
                Trace.TraceError($"Error retrieving transactions for user {userId}: {ex.Message}");
                return null;
            }
        }


        public List<Transaction> GetCryptoCurrenciesByUser(string userId, bool isYesterday)
        {
            try
            {
                var transactions = _table.ExecuteQuery(new TableQuery<Transaction>()).Where(t => t.User_id == userId).ToList();

                var groupedTransactions = transactions
                    .GroupBy(t => new { t.Currency, t.Type })
                    .Select(group => new
                    {
                        Currency = group.Key.Currency,
                        Type = group.Key.Type,
                        TotalAmount = group.Sum(t => decimal.Parse(t.Amount_paid_dollars))
                    });

                var portfolio = new List<Transaction>();
                foreach (var transactionGroup in groupedTransactions)
                {
                    decimal totalAmount = transactionGroup.TotalAmount;

                    // Convert to euros
                    //totalAmount = convert_currency(totalAmount, "USD", "EUR");

                    //// Convert to cryptocurrency (if needed)
                    //if (!isYesterday)
                    //{
                    //    totalAmount = convert_crypto_currency(totalAmount, transactionGroup.Currency);
                    //}
                    //else
                    //{
                    //    totalAmount = convert_crypto_currency_yesterday(totalAmount, transactionGroup.Currency);
                    //}

                    // Get difference from another source
                    var difference = _diffRepository.GetDifferenceByUserIdCurrency(userId, transactionGroup.Currency);

                    // Create a transaction object for the portfolio
                    var portfolioTransaction = new Transaction
                    {
                        Currency = transactionGroup.Currency,
                        Amount_paid_dollars = totalAmount.ToString(),
                        Type = transactionGroup.Type,
                        
                    };

                    portfolio.Add(portfolioTransaction);
                }

                return portfolio;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public bool DeleteTransactionById(Guid transactionId)
        {
            try
            {
                // Create a table query to find the transaction with the given ID
                var query = new TableQuery<Transaction>().Where(
                    TableQuery.GenerateFilterConditionForGuid("Id", QueryComparisons.Equal, transactionId));

                var result = _table.ExecuteQuery(query).FirstOrDefault();

                if (result == null)
                {
                    // Transaction with the given ID not found
                    return false;
                }

                // Create a delete operation for the found transaction
                var deleteOperation = TableOperation.Delete(result);
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