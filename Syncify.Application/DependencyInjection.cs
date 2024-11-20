using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Syncify.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependecies(this IServiceCollection services)
    {
        services.AddMediatR(options =>
            options.RegisterServicesFromAssembly(assembly: Assembly.GetExecutingAssembly()));

        return services;
    }
}
