using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FileCryptPRD.Infrastructure.ScraperApiKeyManagement;

internal class ScraperApiKeyManager : IScraperApiKeyManager
{
    private readonly List<ApiKeyInfo> _apiKeys;
    private readonly HttpClient _httpClient;
    private readonly object _lock = new();

    public ScraperApiKeyManager(IHttpClientFactory httpClientFactory, IOptions<ScraperApiSettings> settings)
    {
        _httpClient = httpClientFactory.CreateClient("Default");
        _apiKeys = settings.Value.ApiKeys.Select(apiKey => new ApiKeyInfo { ApiKey = apiKey }).ToList();
        InitializeApiKeys().Wait();
    }

    private async Task InitializeApiKeys()
    {
        foreach (var key in _apiKeys)
        {
            await FetchApiKeyInfo(key);
        }
    }

    private async Task FetchApiKeyInfo(ApiKeyInfo key)
    {
        try
        {
            var response = await _httpClient.GetAsync($"https://api.scraperapi.com/account?api_key={key.ApiKey}");
            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync();
            var accountInfo = JsonSerializer.Deserialize<AccountInfo>(json);

            if (accountInfo == null) return;

            // Mark the key as exhausted if it has exceeded the request limit
            key.IsExhausted = accountInfo.RequestCount >= accountInfo.RequestLimit;
        }
        catch
        {
            key.IsExhausted = false;
        }
    }

    private DateTime GetNextResetDate()
    {
        DateTime now = DateTime.UtcNow;
        // Get the first day of the next month
        return new DateTime(now.Year, now.Month, 1).AddMonths(1);
    }

    public string? GetBestApiKey()
    {
        lock (_lock)
        {
            DateTime now = DateTime.UtcNow;
            DateTime nextResetDate = GetNextResetDate();

            // Get the first available key that is not exhausted or has reached the reset date
            // Order by the next reset date to prioritize keys that are about to reset
            var availableKey = _apiKeys
                .Where(k => !k.IsExhausted || nextResetDate <= now)
                .OrderBy(k => nextResetDate)
                .FirstOrDefault();

            return availableKey?.ApiKey;
        }
    }

    public void MarkAsExhausted(string apiKey)
    {
        lock (_lock)
        {
            var key = _apiKeys.FirstOrDefault(k => k.ApiKey == apiKey);
            if (key != null)
            {
                key.IsExhausted = true;
            }
        }
    }
}