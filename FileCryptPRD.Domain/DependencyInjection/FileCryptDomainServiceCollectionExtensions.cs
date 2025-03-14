using FileCryptPRD.Domain.DomainServices.LinkResolvingService;
using FileCryptPRD.Domain.DomainServices.RowParsingService;
using FileCryptPRD.Domain.Entities.FileCryptContainer;
using FileCryptPRD.Domain.Entities.FileCryptHeader;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileCryptPRD.Domain.DependencyInjection;

public static class FileCryptDomainServiceCollectionExtensions
{
    public static IServiceCollection AddFileCryptDomain(
        this IServiceCollection services)
    {
        services.AddSingleton<LinkResolvingService>();
        services.AddSingleton<RowParsingService>();

        return services;
    }
}