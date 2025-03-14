using System.Text.Json.Serialization;

namespace FileCryptPRD.Infrastructure.ScraperApiKeyManagement;

public class AccountInfo
{
    [JsonPropertyName("requestCount")]
    public int RequestCount { get; set; }

    [JsonPropertyName("requestLimit")]
    public int RequestLimit { get; set; }
}