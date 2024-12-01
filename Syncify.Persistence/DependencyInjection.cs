using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Syncify.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {

        return services;
    }
}
