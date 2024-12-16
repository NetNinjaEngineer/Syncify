using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Syncify.Application.Helpers;
using System.Reflection;

namespace Syncify.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencies(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(options =>
            options.RegisterServicesFromAssembly(assembly: Assembly.GetExecutingAssembly()));

        services.Configure<JwtSettings>(jwtOptions =>
        {
            var jwtSettingsSection = configuration.GetSection(nameof(JwtSettings));
            jwtOptions.Key = jwtSettingsSection["Key"]!;
            jwtOptions.Issuer = jwtSettingsSection["Issuer"]!;
            jwtOptions.Audience = jwtSettingsSection["Audience"]!;
            jwtOptions.ExpirationInDays = Convert.ToInt32(jwtSettingsSection["ExpirationInDays"]);
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}
