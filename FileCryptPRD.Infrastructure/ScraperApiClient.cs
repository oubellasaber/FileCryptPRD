using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace FileCryptPRD.Infrastructure;

public class ScraperApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ScraperApiSettings _scraperApiSettings;

    public ScraperApiClient(HttpClient httpClient, IOptions<ScraperApiSettings> scraperApiSettings)
    {
        _httpClient = httpClient;
        _scraperApiSettings = scraperApiSettings.Value;
    }

    public async Task<HttpResponseMessage> GetAsync(string url, StringContent? content)
    {
        var uriBuilder = new UriBuilder(_scraperApiSettings.ApiBaseUrl);
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["api_key"] = _scraperApiSettings.ApiKey;
        query["url"] = url;
        query["premium"] = "true";

        uriBuilder.Query = query.ToString();
        var requestUrl = uriBuilder.ToString();

        if (content == null)
        {
            return await _httpClient.GetAsync(requestUrl);
        }

        return await _httpClient.PostAsync(requestUrl, content);
    }
}