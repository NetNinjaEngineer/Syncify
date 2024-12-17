using Microsoft.AspNetCore.Http.Features;
using Syncify.Api.Extensions;
using Syncify.Api.Filters;
using Syncify.Api.Middleware;
using System.Text.Json.Serialization;

namespace Syncify.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiDependencies(
        this IServiceCollection services,
        IConfiguration configuration,
        ConfigureWebHostBuilder webHostBuilder)
    {
        services.AddControllers()
          .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.AddGlobalExceptionHandler();

        services.AddSwaggerDocumentation();

        services.AddSignalR(options => options.EnableDetailedErrors = true);

        webHostBuilder.ConfigureKestrel(serverOptions =>
            serverOptions.Limits.MaxRequestBodySize = Convert.ToInt64(configuration["FormOptionsSize"]));

        services.Configure<IISServerOptions>(options =>
            options.MaxRequestBodySize = Convert.ToInt64(configuration["FormOptionsSize"]));

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = Convert.ToInt64(configuration["FormOptionsSize"]);
            options.ValueLengthLimit = int.MaxValue;
            options.MemoryBufferThreshold = int.MaxValue;
        });

        services.AddScoped<MigrateDatabaseMiddleware>();

        services.AddSingleton<ApiKeyFilter>();

        services.AddSingleton<GuardFilter>();

        return services;
    }
}
