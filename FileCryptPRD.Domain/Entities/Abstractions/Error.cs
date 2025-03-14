namespace FileCryptPRD.Domain.Entities.Abstractions;

public record Error(string Code, string Name)
{
    public static Error None = new(string.Empty, string.Empty);
    public static Error NullValue = new("Error.NullValue", "Null value was provided");
    public static Error NoApiKeyAvailable = new("ScraperApi.NoApiKeyAvailable", "No Api key has credits at the moment");
    public static Error CaptchaDetected = new ("FileCryptContainerParsingService.CaptchaDetected", "The page contains a CAPTCHA and cannot be scraped automatically.");
}