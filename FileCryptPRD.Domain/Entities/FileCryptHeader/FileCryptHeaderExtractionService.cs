using Microsoft.Extensions.Options;

namespace FileCryptPRD.Domain.Entities.FileCryptHeader;

public class FileCryptHeaderExtractionService
{
    private readonly FileCryptSettings _settings;
    private readonly FileCryptHeaderConfig _config;

    public FileCryptHeaderExtractionService(
        IOptions<FileCryptSettings> settings,
        IOptions<FileCryptHeaderConfig> config)
    {
        _settings = settings.Value;
        _config = config.Value;
    }

    public FileCryptHeader GetFileCryptHeader(HttpResponseMessage httpRequestMessage)
    {
        string requiredHeaders = httpRequestMessage.Headers.GetValues("Set-Cookie").First();

        var headersSeperated = requiredHeaders.Split(';');

        var phpSessid = headersSeperated[0].Split('=')[1];
        var expirationDate = headersSeperated[1].Split('=')[1];

        return new FileCryptHeader(
            phpSessid,
            expirationDate,
            _config);
    }
}