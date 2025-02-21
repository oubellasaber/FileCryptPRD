using FileCryptPRD.Domain.Entities.FileCryptContainer.ValueObjects;
using FileCryptPRD.Domain.Entities.Rows;
using System.Linq;

namespace FileCryptPRD.Domain.Entities.FileCryptContainer;

public class FileCryptContainer
{
    private readonly Dictionary<string, RowVersion> _rowVersions = new();
    private readonly List<Row> _unknownRows = new();

    public string Title { get; private set; }
    public Uri Url { get; private set; }

    public FileCryptContainer(string title, Uri url)
    {
        Title = title;
        Url = url;
    }

    public void Add(string? fileName, Row row)
    {
        if (fileName is null)
        {
            _unknownRows.Add(row);
            return;
        }

        if (_rowVersions.TryGetValue(fileName, out var rowVersion))
        {
            rowVersion.Rows.Add(row);
        }
        else
        {
            _rowVersions[fileName] = new RowVersion(fileName, new List<Row> { row });
        }
    }

    public IEnumerable<RowVersion> Rows => _rowVersions.Values.Union(_unknownRows.Select(x => new RowVersion(null!, [x])));
}