using FileCryptPRD.Domain.DomainServices.RowParsingService;
using Microsoft.Extensions.DependencyInjection;

namespace FileCryptPRD.Infrastructure.DependencyInjection
{
    public static class FileCryptInfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddFileCryptInfrastructure(this IServiceCollection services)
        {
            services.AddHttpClient("Default", client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
            });

            services.AddHttpClient<ScraperApiClient>();
            services.AddHttpClient<FileCryptClient>(client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
            });
            services.AddSingleton<IFileCryptClient, FileCryptClient>();

            return services;
        }
    }
}
