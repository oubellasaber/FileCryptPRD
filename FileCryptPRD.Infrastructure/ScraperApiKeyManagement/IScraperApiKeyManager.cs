namespace FileCryptPRD.Infrastructure.ScraperApiKeyManagement;

public interface IScraperApiKeyManager
{
    string? GetBestApiKey();
    void MarkAsExhausted(string apiKey);
}