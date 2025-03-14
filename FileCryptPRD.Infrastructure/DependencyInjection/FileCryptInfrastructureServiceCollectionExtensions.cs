using FileCryptPRD.Infrastructure.ScraperApiKeyManagement;
using Microsoft.Extensions.DependencyInjection;

namespace FileCryptPRD.Infrastructure.DependencyInjection
{
    public static class FileCryptInfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddFileCryptInfrastructure(this IServiceCollection services)
        {
            services.AddHttpClient("Default", client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.0.0 Safari/537.36");
            });

            services.AddHttpClient<ScraperApiClient>();
            services.AddSingleton<IScraperApiKeyManager, ScraperApiKeyManager>();
            //services.AddHttpClient<FileCryptClient>(client =>
            //{
            //    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.0.0 Safari/537.36");
            //});
            //services.AddSingleton<IFileCryptClient, FileCryptClient>();

            return services;
        }
    }
}
