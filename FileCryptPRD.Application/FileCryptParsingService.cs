using FileCryptPRD.Domain.DomainServices.RowParsingService;
using FileCryptPRD.Domain.Entities.Abstractions;
using FileCryptPRD.Domain.Entities.FileCryptContainer;
using FileCryptPRD.Domain.Entities.FileCryptHeader;
using FileCryptPRD.Infrastructure;
using HtmlAgilityPack;
using System.Text;

namespace FileCryptPRD.Application;

public class FileCryptParsingService
{
    private readonly HttpClient _httpClient;
    private readonly ScraperApiClient _scraperApiClient;
    private readonly RowParsingService _rowParsingService;
    private readonly FileCryptHeaderExtractionService _headerExtractionService;

    public FileCryptParsingService(
        IHttpClientFactory httpClientFactory,
        ScraperApiClient scraperApiClient,
        RowParsingService rowParsingService,
        FileCryptHeaderExtractionService headerExtractionService)
    {
        _httpClient = httpClientFactory.CreateClient("Default");
        _scraperApiClient = scraperApiClient;
        _rowParsingService = rowParsingService;
        _headerExtractionService = headerExtractionService;
    }

    public async Task<Result<FileCryptContainer>> ParseAsync(Uri url, string? password = null)
    {
        var content = password is not null
            ? new StringContent($"pssw={password}", Encoding.UTF8, "application/x-www-form-urlencoded")
            : null;

        // Try scraping with primary HttpClient
        //var result = await TryScrapeWithHttpClientAsync(url, content);
        //if (result.IsSuccess) return result;

        // Try again with ScraperApiClient if captcha was detected
        for (var i = 0; i < 3; i++)
        {
            var result = await TryScrapeWithScraperApiAsync(url, content);
            if (result.IsSuccess) return result;

            if (result.Error != Error.CaptchaDetected)
                return result;
        }

        return Result.Failure<FileCryptContainer>(Error.CaptchaDetected);
    }

    private async Task<Result<FileCryptContainer>> TryScrapeWithHttpClientAsync(Uri url, StringContent? content)
    {
        var response = await _httpClient.PostAsync(url.ToString(), content);
        return await ProcessResponse(url, response);
    }

    private async Task<Result<FileCryptContainer>> TryScrapeWithScraperApiAsync(Uri url, StringContent? content)
    {
        var response = await _scraperApiClient.GetAsync(url.ToString(), content);

        if (response.IsFailure)
            return Result.Failure<FileCryptContainer>(response.Error);

        return await ProcessResponse(url, response.Value);
    }

    private async Task<Result<FileCryptContainer>> ProcessResponse(Uri url, HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();

        if (ContainsCaptcha(html))
            return Result.Failure<FileCryptContainer>(Error.CaptchaDetected);

        var header = _headerExtractionService.GetFileCryptHeader(response);

        HtmlDocument document = new();
        document.LoadHtml(html);
        FileCryptContainer container = new FileCryptContainer(url, document);

        await container.ParseAsync(_rowParsingService, header);
        return container;
    }

    private bool ContainsCaptcha(string responseBody) =>
        responseBody.Contains("Please confirm that you are no robot", StringComparison.OrdinalIgnoreCase);
}
