using FileCryptPRD.Domain.Entities.Abstractions;
using FileCryptPRD.Domain.Entities.FileCryptContainer;

namespace FileCryptPRD.Application.UseCases;

public class FileCryptParsingService
{
    private readonly FileCryptContainerParsingService _parsingService;

    public FileCryptParsingService(FileCryptContainerParsingService parsingService)
    {
        _parsingService = parsingService;
    }

    public async Task<Result<FileCryptContainer>> ExecuteAsync(Uri url, string? password = null)
    {
        // Application logic, like validation, could go here

        // Call the domain service to parse the container       
        var result = await _parsingService.ScrapeAsync(url, password);

        // Additional handling for success/failure, e.g., logging, error mapping, etc.

        return result;
    }
}
