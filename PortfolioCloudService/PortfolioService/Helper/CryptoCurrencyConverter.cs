using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Common.Interfaces;
using Newtonsoft.Json.Linq;

//[RoutePrefix("api/currencies")]
public class CryptoCurrencyConverter
{
    private const string StaticFolder = "static";
    private const string CurrencyApiBaseUrl = "https://currency-exchange-api-six.vercel.app/api/v2/currencies/";

    private readonly ICachingService _cachingService;
    private readonly HttpClient _httpClient;
    private readonly CurrencyConverterService _currencyConverterService;

    public CryptoCurrencyConverter(ICachingService cachingService, CurrencyConverterService currencyConverterService)
    {
        _cachingService = cachingService;
        _currencyConverterService = currencyConverterService;
        _httpClient = new HttpClient();
    }

    //[HttpGet]
    //[Route("get/rates")]
    public async Task<Dictionary<string, double>> GetAvailableCryptoCurrenciesAsync(bool forceRefresh = false)
    {
        string cachedFile = Path.Combine(StaticFolder, "available_crypto_currencies.json");

        if (!forceRefresh && _cachingService.IsCacheValid(cachedFile))
        {
            var cachedData = _cachingService.ReadCachedData(cachedFile);
            if (cachedData != null)
            {
                var rates = JObject.Parse(cachedData)["rates"];
                return rates.ToObject<Dictionary<string, double>>();
            }
        }

        var response = await _httpClient.GetStringAsync($"{CurrencyApiBaseUrl}today");
        _cachingService.WriteCachedData(cachedFile, response);

        var data = JObject.Parse(response)["rates"];
        return data.ToObject<Dictionary<string, double>>();
    }

    public async Task<double> ConvertCryptoCurrencyAsync(double amount, string currency)
    {
        var exchangeRates = await GetAvailableCryptoCurrenciesAsync();

        if (exchangeRates == null || !exchangeRates.ContainsKey(currency))
        {
            return -1;
        }

        double converted = exchangeRates[currency] * amount;
        return Math.Round(converted, 10);
    }

    public async Task<double> ConvertCryptoToDollarsAsync(double amount, string currency)
    {
        var exchangeRates = await GetAvailableCryptoCurrenciesAsync();

        if (exchangeRates == null || !exchangeRates.ContainsKey(currency))
        {
            return -1;
        }

        double converted = amount / exchangeRates[currency];

        // Convert from EUR to USD
        converted = await _currencyConverterService.ConvertCurrencyAsync(converted, "EUR", "USD");

        return Math.Round(converted, 10);
    }

    public async Task<Dictionary<string, double>> GetAvailableCryptoCurrenciesYesterdayAsync(bool forceRefresh = false)
    {
        string cachedFile = Path.Combine(StaticFolder, "available_crypto_currencies_yesterday.json");

        if (!forceRefresh && _cachingService.IsCacheValid(cachedFile))
        {
            var cachedData = _cachingService.ReadCachedData(cachedFile);
            if (cachedData != null)
            {
                var rates = JObject.Parse(cachedData)["rates"];
                return rates.ToObject<Dictionary<string, double>>();
            }
        }

        var response = await _httpClient.GetStringAsync($"{CurrencyApiBaseUrl}yesterday");
        _cachingService.WriteCachedData(cachedFile, response);

        var data = JObject.Parse(response)["rates"];
        return data.ToObject<Dictionary<string, double>>();
    }

    public async Task<double> ConvertCryptoCurrencyYesterdayAsync(double amount, string currency)
    {
        var exchangeRates = await GetAvailableCryptoCurrenciesYesterdayAsync();

        if (exchangeRates == null || !exchangeRates.ContainsKey(currency))
        {
            return -1;
        }

        double converted = exchangeRates[currency] * amount;
        return Math.Round(converted, 10);
    }
}