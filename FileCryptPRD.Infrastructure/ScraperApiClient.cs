using FileCryptPRD.Domain.Entities.Abstractions;
using FileCryptPRD.Infrastructure.ScraperApiKeyManagement;
using Microsoft.Extensions.Options;
using System.Web;

namespace FileCryptPRD.Infrastructure;

public class ScraperApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ScraperApiSettings _scraperApiSettings;
    private readonly IScraperApiKeyManager _scraperApiKeyManager;

    public ScraperApiClient(
        HttpClient httpClient,
        IOptions<ScraperApiSettings> scraperApiSettings,
        IScraperApiKeyManager scraperApiKeyManager)
    {
        _httpClient = httpClient;
        _scraperApiSettings = scraperApiSettings.Value;
        _scraperApiKeyManager = scraperApiKeyManager;
    }

    public async Task<Result<HttpResponseMessage>> GetAsync(string url, StringContent? content)
    {
        var uriBuilder = new UriBuilder(_scraperApiSettings.ApiBaseUrl);
        var query = HttpUtility.ParseQueryString(string.Empty);

        var bestApiKey = _scraperApiKeyManager.GetBestApiKey();

        if (bestApiKey is null)
        {
            return Result.Failure<HttpResponseMessage>(Error.NoApiKeyAvailable);
        }

        query["api_key"] = bestApiKey;
        query["url"] = url;
        //query["premium"] = "true";

        uriBuilder.Query = query.ToString();
        var requestUrl = uriBuilder.ToString();

        if (content == null)
        {
            return await _httpClient.GetAsync(requestUrl);
        }

        var response = await _httpClient.PostAsync(requestUrl, content);

        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            _scraperApiKeyManager.MarkAsExhausted(bestApiKey);
            return Result.Failure<HttpResponseMessage>(Error.NoApiKeyAvailable);
        }

        return response;
    }
}