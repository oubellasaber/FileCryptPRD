using FileCryptPRD.Domain.Entities.Abstractions;
using FileCryptPRD.Domain.Entities.Rows;
using FileCryptPRD.Domain.Entities.Rows.Enums;
using FileCryptPRD.Domain.Entities.Rows.ValueObjects;
using HtmlAgilityPack;

namespace FileCryptPRD.Domain.DomainServices.RowParsingService;

public sealed class RowParsingService
{
    private readonly IFileCryptClient _htmlFetcher;
    private readonly LinkResolvingService.LinkResolvingService _linkResolvingService;

    public RowParsingService(IFileCryptClient htmlFetcher, LinkResolvingService.LinkResolvingService linkResolvingService)
    {
        _htmlFetcher = htmlFetcher;
        _linkResolvingService = linkResolvingService;
    }

    public async Task<Result<Row>> ParseRowAsync(HtmlNode row)
    {
        var id = GetFirstDataAttribute(row)?.Value;
        var fileName = row.SelectSingleNode(".//td/@title").GetAttributeValue("title", "");
        var status = row
            .SelectSingleNode(".//td[@class = 'status']/i")
            !.GetAttributeValue("class", "")
            .Split(" ")
            .FirstOrDefault();
        double.TryParse(row.SelectSingleNode("./td[3]")!.InnerText.Split(' ').FirstOrDefault(), out var fileSize);
        var directLink = await _linkResolvingService.Resolve(id!);

        var link = new Link(directLink.Value, status switch
        {
            "online" => Status.Online,
            "offline" => Status.Offline, 
            _ => Status.Unknown
        });

        var parsedRow = new Row(id!, fileName, fileSize, link);
        return parsedRow;
    }

    private static HtmlAttribute? GetFirstDataAttribute(HtmlNode node)
    {
        return node.SelectSingleNode(".//button").Attributes
                   .FirstOrDefault(attr => attr.Name.StartsWith("data-"));
    }
}
