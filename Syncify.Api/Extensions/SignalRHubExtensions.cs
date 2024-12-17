using Syncify.Application.Hubs;

namespace Syncify.Api.Extensions;

public static class SignalRHubExtensions
{
    public static IApplicationBuilder UseHubs(this WebApplication app)
    {
        app.MapHub<NotificationHub>("/hubs/notifications");

        app.MapHub<MessageHub>("/hubs/messages");

        app.MapHub<FriendRequestHub>("/hubs/friendRequests");

        return app;
    }
}
