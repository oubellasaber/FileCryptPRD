using FileCryptPRD.Domain.DomainServices.RowParsingService;
using Microsoft.Extensions.Logging;

namespace FileCryptPRD.Infrastructure;

public class FileCryptClient : IFileCryptClient
{
    private readonly HttpClient _httpClient;
    private readonly ScraperApiClient _scraperApiClient;
    private readonly ILogger<FileCryptClient> _logger;

    public FileCryptClient(
        IHttpClientFactory httpClientFactory,
        ScraperApiClient scraperApiClient,
        ILogger<FileCryptClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("Default");
        _scraperApiClient = scraperApiClient;
        _logger = logger;
    }

    public async Task<string> GetAsync(string url, StringContent? content)
    {
        try
        {
            // First attempt with regular HttpClient
            var response = await (content is null ?
                _httpClient.GetAsync(url) :
                _httpClient.PostAsync(url, content));

            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseBody))
            {
                _logger.LogWarning("Received empty response from {Url}", url);
                throw new HttpRequestException("Received empty response from server");
            }

            // If no captcha, return the response
            if (!ContainsCaptcha(responseBody))
            {
                return responseBody;
            }

            // If captcha detected, try with ScraperAPI
            _logger.LogInformation("Captcha detected, attempting with ScraperAPI for {Url}", url);

            var scraperResponse = await _scraperApiClient.GetAsync(url, content);
            scraperResponse.EnsureSuccessStatusCode();

            var scraperResponseBody = await scraperResponse.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(scraperResponseBody))
            {
                _logger.LogWarning("Received empty response from ScraperAPI for {Url}", url);
                throw new HttpRequestException("Received empty response from ScraperAPI");
            }

            return scraperResponseBody;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for {Url}", url);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching {Url}", url);
            throw;
        }
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage)
    {
        return await _httpClient.SendAsync(httpRequestMessage);
    }

    public async Task<HttpResponseMessage> GetHeadersAsync(Uri url)
        => await _httpClient.GetAsync(url);

    private bool ContainsCaptcha(string responseBody)
    => responseBody.Contains("Please confirm that you are no robot", StringComparison.OrdinalIgnoreCase);
}