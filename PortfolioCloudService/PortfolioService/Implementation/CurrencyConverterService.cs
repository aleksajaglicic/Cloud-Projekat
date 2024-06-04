using Common.Interfaces;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System;

public class CurrencyConverterService
{
    private const string StaticFolder = "static";
    private const string CurrencyApiUrl = "https://currency-exchange-api-six.vercel.app/";

    private readonly ICachingService _cachingService;
    private readonly HttpClient _httpClient;

    public CurrencyConverterService(ICachingService cachingService)
    {
        _cachingService = cachingService;
        _httpClient = new HttpClient();
    }

    public async Task<Dictionary<string, JObject>> GetAvailableCurrenciesCourseAsync(bool forceRefresh = false)
    {
        string cachedFile = Path.Combine(StaticFolder, "available_currencies_course.json");

        if (!forceRefresh && _cachingService.IsCacheValid(cachedFile))
        {
            var cachedData = _cachingService.ReadCachedData(cachedFile);
            if (cachedData != null)
            {
                var currencies = JObject.Parse(cachedData)["currencies"];
                return currencies.ToObject<Dictionary<string, JObject>>();
            }
        }

        var response = await _httpClient.GetStringAsync($"{CurrencyApiUrl}/api/v1/currencies/today");
        _cachingService.WriteCachedData(cachedFile, response);

        var data = JObject.Parse(response)["currencies"];
        return data.ToObject<Dictionary<string, JObject>>();
    }

    public async Task<double> ConvertCurrencyAsync(double amount, string fromCurrency, string toCurrency)
    {
        var exchangeRates = await GetAvailableCurrenciesCourseAsync();

        if (exchangeRates == null || !exchangeRates.ContainsKey(fromCurrency) || !exchangeRates.ContainsKey(toCurrency))
        {
            return -1;
        }

        var fromRateData = exchangeRates[fromCurrency];
        var toRateData = exchangeRates[toCurrency];

        double fromRate = double.Parse(fromRateData["course"].ToString().Replace(",", ".")) /
                          double.Parse(fromRateData["number"].ToString());
        double toRate = double.Parse(toRateData["course"].ToString().Replace(",", ".")) /
                        double.Parse(toRateData["number"].ToString());

        double converted = fromRate * amount / toRate;
        return Math.Round(converted, 4);
    }
}