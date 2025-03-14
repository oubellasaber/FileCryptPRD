using FileCryptPRD.Domain.DomainServices.RowParsingService;
using FileCryptPRD.Domain.Entities.Rows;
using HtmlAgilityPack;

namespace FileCryptPRD.Domain.Entities.FileCryptContainer;

public class FileCryptContainer
{
    private readonly List<Row> _rows = new();

    public Uri Url { get; private set; }
    public string Title { get; private set; }
    private readonly HtmlDocument _doc;

    public IEnumerable<Row> Rows => _rows;

    public FileCryptContainer(Uri url, HtmlDocument doc)
    {
        Url = url;
        _doc = doc;
        Title = string.Empty;
    }

    public async Task<FileCryptContainer> ParseAsync(
        RowParsingService rowParsingService, 
        FileCryptHeader.FileCryptHeader header)
    {
        var title = _doc
            .DocumentNode
            .SelectSingleNode("//*[@id='page']/div[2]/div/div/h2")
            ?.InnerText ?? string.Empty;

        Title = title;

        var rows = _doc.DocumentNode.SelectNodes("//table//tr");

        if (rows is null)
            return this;

        foreach (var row in rows)
        {
            var parsedRow = await rowParsingService.ParseRowAsync(row, header);

            if (parsedRow.IsSuccess)
            {
                Add(parsedRow.Value);
            }
        }

        return this;
    }

    private void Add(Row row)
        => _rows.Add(row);
}