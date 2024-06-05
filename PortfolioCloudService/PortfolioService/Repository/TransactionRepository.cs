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
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                decimal currentCurrencyValue = GetTodaysCurrencyValue(transaction.Currency).Result;


                decimal remainingAmount = 600.0M - decimal.Parse(transaction.Amount_paid_dollars);

                if (remainingAmount < 0)
                {
                    return false;
                }

                decimal profit = remainingAmount * currentCurrencyValue;

                ProfitRepository profitRepository = new ProfitRepository();
                Profit profitRecord = new Profit
                {
                    PartitionKey = "Profit",
                    RowKey = transaction.Currency,
                    Id = Guid.NewGuid().ToString(), // Generate new ID for the profit record
                    User_id = transaction.User_id,
                    Type = profit >= 0 ? TypeProfit.PROFIT : TypeProfit.LOSS,
                    Summary = (double)Math.Abs(profit),
                    Net_worth = (double)remainingAmount
                };

                profitRepository.CreateOrUpdate(profitRecord);

                transaction.Amount_paid_dollars = remainingAmount.ToString();



                return CreateOrUpdate(transaction); ; 
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CreateOrUpdate(Transaction transaction)
        {
            try
            {
                var retrieveOperation = TableOperation.Retrieve<Transaction>(transaction.PartitionKey, transaction.RowKey);
                var retrieveResult = _table.Execute(retrieveOperation);
                var existingEntity = (Transaction)retrieveResult.Result;

                if (existingEntity != null)
                {
                    existingEntity.Amount_paid_dollars = transaction.Amount_paid_dollars;
                    existingEntity.Date_and_time = transaction.Date_and_time;

                    var updateOperation = TableOperation.Replace(existingEntity);
                    _table.Execute(updateOperation);
                }
                else
                {
                    var insertOperation = TableOperation.Insert(transaction);
                    _table.Execute(insertOperation);
                }

                return true;
            }
            catch (Exception)
            {
                return false; 
            }
        }


        public async Task<decimal> GetTodaysCurrencyValue(string currency)
        {
            string url = "https://currency-exchange-api-six.vercel.app/api/v2/currencies/today";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var exchangeRates = await response.Content.ReadAsStringAsync(); // Read the response as string
                    JObject ratesObject = JObject.Parse(exchangeRates); // Parse JSON string to JObject

                    // Get the value corresponding to the currency key
                    decimal currencyValue = (decimal)ratesObject["rates"][currency];

                    return currencyValue;

                    // If currency not found, throw an exception or return a default value
                   // throw new KeyNotFoundException($"Currency value for {currency} not found in the response.");
                }
                catch (HttpRequestException httpRequestException)
                {
                    // Log detailed HttpRequestException
                    if (httpRequestException.InnerException != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Inner Exception: {httpRequestException.InnerException.Message}");
                    }
                    System.Diagnostics.Debug.WriteLine($"HttpRequestException: {httpRequestException.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    // Log detailed Exception
                    System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                    throw;
                }
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

                    var difference = _diffRepository.GetDifferenceByUserIdCurrency(userId, transactionGroup.Currency);

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
                var query = new TableQuery<Transaction>().Where(
                    TableQuery.GenerateFilterConditionForGuid("Id", QueryComparisons.Equal, transactionId));

                var result = _table.ExecuteQuery(query).FirstOrDefault();

                if (result == null)
                {
                   
                    return false;
                }

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