using FileCryptPRD.Application;
using FileCryptPRD.Application.DependencyInjection;
using FileCryptPRD.Domain.Entities.FileCryptHeader;
using FileCryptPRD.Infrastructure;
using System.Text.Json.Serialization;

namespace FileCryptPRD.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            builder.Services.Configure<FileCryptHeaderConfig>(builder.Configuration.GetSection("FileCryptHeaderConfig"));
            builder.Services.Configure<FileCryptSettings>(builder.Configuration.GetSection("FileCryptSettings"));
            builder.Services.Configure<ScraperApiSettings>(builder.Configuration.GetSection("ScraperApiSettings"));
            builder.Services.AddFileCryptApplication();

            builder.Services.AddProblemDetails();

            var app = builder.Build();

            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            app.MapGet("/api/filecrypt", async (FileCryptParsingService service, string url, string? password) =>
            {
                var result = await service.ParseAsync(new Uri(url), password);
                return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
            });

            app.UseHttpsRedirection();

            app.Run();
        }
    }
}
