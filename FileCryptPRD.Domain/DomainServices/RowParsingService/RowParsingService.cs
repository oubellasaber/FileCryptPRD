using FileCryptPRD.Domain.Entities.Abstractions;
using FileCryptPRD.Domain.Entities.FileCryptHeader;
using FileCryptPRD.Domain.Entities.Rows;
using FileCryptPRD.Domain.Entities.Rows.Enums;
using FileCryptPRD.Domain.Entities.Rows.ValueObjects;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace FileCryptPRD.Domain.DomainServices.RowParsingService;

public sealed class RowParsingService
{
    private readonly LinkResolvingService.LinkResolvingService _linkResolvingService;

    public RowParsingService(LinkResolvingService.LinkResolvingService linkResolvingService)
    {
        _linkResolvingService = linkResolvingService;
    }

    public async Task<Result<Row>> ParseRowAsync(HtmlNode row, FileCryptHeader header)
    {
        var id = GetFirstDataAttribute(row)?.Value;
        var fileName = row.SelectSingleNode(".//td/@title").GetAttributeValue("title", "");
        var status = row
            .SelectSingleNode(".//td[@class = 'status']/i")
            !.GetAttributeValue("class", "")
            .Split(" ")
            .FirstOrDefault();
        var directLink = await _linkResolvingService.Resolve(id!, header);

        var link = new Link(directLink.Value, status switch
        {
            "online" => Status.Online,
            "offline" => Status.Offline,
            _ => Status.Unknown
        });

        string rawFileSize = row.SelectSingleNode("./td[3]").InnerText;
        string pattern = @"^(?<size>\d+(\.\d+)?)\s?(?<unit>GB|MB)$";

        FileSize? fileSize = null;

        Match match = Regex.Match(rawFileSize, pattern);

        if (match.Success)
        {
            double size = double.Parse(match.Groups["size"].Value);
            DataMeasurement unit = Enum.Parse<DataMeasurement>(match.Groups["unit"].Value);

            fileSize = new FileSize(size, unit);
        }

        var parsedRow = new Row(fileName, fileSize, link);
        return parsedRow;
    }

    private static HtmlAttribute? GetFirstDataAttribute(HtmlNode node)
    {
        return node.SelectSingleNode(".//button").Attributes
                   .FirstOrDefault(attr => attr.Name.StartsWith("data-"));
    }
}
