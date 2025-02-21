namespace FileCryptPRD.Domain.Entities.FileCryptHeader.ValueObjects;

public record CookieHeader(string Value) : HttpHeader("Cookie", Value);