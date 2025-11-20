using Integration.Dtos;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace Integration
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly CurrencySettings _settings;

        public CurrencyService(HttpClient httpClient, IOptions<CurrencySettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;

            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        }

        public async Task<decimal> GetExchangeToAsync(string toCurrency)
        {
            var uri = $"latest?access_key={_settings.ApiKey}&symbols={_settings.DefaultCurrency},{toCurrency}";

            using var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<CurrencyRateDTO>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (result?.Rates == null ||
                !result.Rates.TryGetValue(_settings.DefaultCurrency, out var fromRate) ||
                !result.Rates.TryGetValue(toCurrency, out var toRate))
            {
                throw new InvalidOperationException("Missing expected currency rates.");
            }

            return toRate / fromRate;
        }

        public async Task<decimal> GetExchangeFromAsync(string fromCurrency)
        {
            var forwardRate = await GetExchangeToAsync(fromCurrency);
            return 1 / forwardRate;
        }
    }
}