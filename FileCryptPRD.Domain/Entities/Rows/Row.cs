using FileCryptPRD.Domain.Entities.Rows.ValueObjects;

namespace FileCryptPRD.Domain.Entities.Rows;

public class Row
{
    public string? FileName { get; private set; }
    public FileSize? FileSize { get; private set; }
    public Link Link { get; private set; }

    public Row(string? fileName, FileSize? fileSize, Link link)
    {
        if (fileName == "n/a")
        {
            fileName = null;
        }

        FileName = fileName;
        FileSize = fileSize;
        Link = link;
    }
}