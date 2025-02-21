using FileCryptPRD.Domain.Entities.Rows;

namespace FileCryptPRD.Domain.Entities.FileCryptContainer.ValueObjects;

public record RowVersion(string FileName, List<Row> Rows);