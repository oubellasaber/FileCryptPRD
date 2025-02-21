using FileCryptPRD.Domain.DomainServices.RowParsingService;
using FileCryptPRD.Domain.Entities.Abstractions;
using HtmlAgilityPack;
using System.Text;

namespace FileCryptPRD.Domain.Entities.FileCryptContainer;

public class FileCryptContainerParsingService
{
    private readonly IFileCryptClient _htmlFetcher;
    private readonly RowParsingService _rowParsingService;

    public FileCryptContainerParsingService(
        IFileCryptClient htmlFetcher, 
        RowParsingService rowParsingService)
    {
        _htmlFetcher = htmlFetcher;
        _rowParsingService = rowParsingService;
    }

    public async Task<Result<FileCryptContainer>> ScrapeAsync(Uri url, string? password = null)
    {
        StringContent? content = null;

        if (password is not null)
        {
            var body = $"pssw={password}";
            content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
        }

        var html = await _htmlFetcher.GetAsync(url.ToString(), content);

        if (ContainsCaptcha(html))
        {
            return Result.Failure<FileCryptContainer>(
                new Error("FileCryptContainerParsingService.CaptchaDetected", "The page contains a CAPTCHA and cannot be scraped automatically.")
                );
        }

        HtmlDocument document = new();
        document.LoadHtml(html);

        var title = document
            .DocumentNode
            .SelectSingleNode("//*[@id='page']/div[2]/div/div/h2")
            ?.InnerText ?? string.Empty;

        FileCryptContainer container = new FileCryptContainer(title, url);

        var rows = document.DocumentNode.SelectNodes("//table//tr");

        if (rows != null)
        {
            foreach (var row in rows)
            {
                var parsedRow = await _rowParsingService.ParseRowAsync(row);
                container.Add(parsedRow.Value.FileName, parsedRow.Value);
            }
        }

        return container;
    }

    private bool ContainsCaptcha(string responseBody)
        => responseBody.Contains("Please confirm that you are no robot", StringComparison.OrdinalIgnoreCase);
}