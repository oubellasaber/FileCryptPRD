using FileCryptPRD.Domain.Entities.Rows.ValueObjects;

namespace FileCryptPRD.Domain.Entities.Rows;

public class Row
{
    public string RowId { get; private set; }
    public string? FileName { get; private set; }
    public double? FileSize { get; private set; }
    public Link Link { get; private set; }

    public Row(string rowId, string? fileName, double? fileSize, Link link)
    {
        if (string.IsNullOrEmpty(rowId))
            throw new ArgumentException("RowId cannot be empty.", nameof(rowId));
        if (fileSize < 0)
            throw new ArgumentException("FileSize must be greater than or equal to zero.", nameof(fileSize));

        if (fileName == "n/a")
        {
            fileName = null;
        }

        RowId = rowId;
        FileName = fileName;
        FileSize = fileSize;
        Link = link;
    }
}