using FileCryptPRD.Domain.Entities.Rows.Enums;

namespace FileCryptPRD.Domain.Entities.Rows.ValueObjects;

public record Link(Uri Url, Status Status);