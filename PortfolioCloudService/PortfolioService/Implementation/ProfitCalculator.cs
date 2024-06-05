using PortfolioService.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PortfolioService
{
    public class ProfitCalculator
    {
        private const string CurrencyApiBaseUrl = "https://currency-exchange-api-six.vercel.app/api/v2/currencies/";

        public static async Task<double> CalculateProfit(Transaction transaction)
        {
            var todayRates = await GetAvailableCurrencies();
            var yesterdayRates = await GetAvailableCurrenciesYesterday();

            if (todayRates == null || yesterdayRates == null)
            {
                throw new Exception("Failed to fetch currency rates.");
            }

            if (transaction.Type == TypeTransaction.BOUGHT)
            {
                double yesterdayRate;
                if (!yesterdayRates.TryGetValue(transaction.Currency, out yesterdayRate))
                {
                    throw new Exception($"Yesterday rate for {transaction.Currency} not found.");
                }

                double todayRate;
                if (!todayRates.TryGetValue(transaction.Currency, out todayRate))
                {
                    throw new Exception($"Today rate for {transaction.Currency} not found.");
                }

                double purchaseAmount = double.Parse(transaction.Amount_paid_dollars);

                return (todayRate - yesterdayRate) * purchaseAmount;
            }
            else if (transaction.Type == TypeTransaction.SOLD)
            {
                return double.Parse(transaction.Amount_paid_dollars); // For sold transactions, profit is the amount sold
            }

            return 0;
        }

        private static async Task<Dictionary<string, double>> GetAvailableCurrencies()
        {
            string apiUrl = $"{CurrencyApiBaseUrl}today";
            return await FetchCurrencyRates(apiUrl);
        }

        private static async Task<Dictionary<string, double>> GetAvailableCurrenciesYesterday()
        {
            string apiUrl = $"{CurrencyApiBaseUrl}yesterday";
            return await FetchCurrencyRates(apiUrl);
        }

        private static async Task<Dictionary<string, double>> FetchCurrencyRates(string apiUrl)
        {
            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<Dictionary<string, double>>();
                    return data;
                }
                else
                {
                    throw new Exception($"Failed to fetch currency rates. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching currency rates: {ex.Message}");
            }
        }
    }
}
