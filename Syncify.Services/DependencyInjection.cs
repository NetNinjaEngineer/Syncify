using Microsoft.Extensions.DependencyInjection;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Services;
public static class DependencyInjection
{
    public static IServiceCollection AddServicesDependecies(this IServiceCollection services)
    {
        services.AddScoped<IFriendshipService, FriendshipService>();

        return services;
    }
}
