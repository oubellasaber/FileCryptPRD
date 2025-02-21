using FileCryptPRD.Domain.Entities.Rows;

namespace FileCryptPRD.Domain.Entities.FileCryptContainer;

public class FileCryptContainer
{
    private readonly List<Row> _rows = new();

    public string Title { get; private set; }
    public Uri Url { get; private set; }

    public FileCryptContainer(string title, Uri url)
    {
        Title = title;
        Url = url;
    }

    public void Add(Row row)
        => _rows.Add(row);

    public IEnumerable<Row> Rows
        => _rows;
}