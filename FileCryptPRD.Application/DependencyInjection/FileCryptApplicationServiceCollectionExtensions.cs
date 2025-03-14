using FileCryptPRD.Domain.DependencyInjection;
using FileCryptPRD.Domain.Entities.FileCryptHeader;
using FileCryptPRD.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace FileCryptPRD.Application.DependencyInjection
{
    public static class FileCryptApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddFileCryptApplication(this IServiceCollection services)
        {
            services.AddSingleton<FileCryptHeaderExtractionService>();
            services.AddFileCryptInfrastructure();
            services.AddFileCryptDomain();
            services.AddSingleton<FileCryptParsingService>();

            return services;
        }
    }
}
