using FileCryptPRD.Domain.DomainServices.RowParsingService;
using Microsoft.Extensions.Options;

namespace FileCryptPRD.Domain.Entities.FileCryptHeader;

public class FileCryptHeaderProviderService
{
    private readonly IFileCryptClient _client;
    private readonly FileCryptSettings _settings;
    private readonly FileCryptHeaderConfig _config;

    public FileCryptHeaderProviderService(
        IFileCryptClient client,
        IOptions<FileCryptSettings> settings, 
        IOptions<FileCryptHeaderConfig> config)
    {
        _client = client;
        _settings = settings.Value;
        _config = config.Value;
    }

    public async Task<FileCryptHeader> GetFileCryptHeaderAsync()
    {
        var response = await _client.GetHeadersAsync(_settings.CreateEndpointFullUrl);
        string requiredHeaders = response.Headers.GetValues("Set-Cookie").First();

        var headersSeperated = requiredHeaders.Split(';');

        var phpSessid = headersSeperated[0].Split('=')[1];
        var expirationDate = headersSeperated[1].Split('=')[1];

        return new FileCryptHeader(
            phpSessid,
            expirationDate,
            _config);
    }

    public FileCryptHeader GetFileCryptHeader()
        => GetFileCryptHeaderAsync().Result;
}