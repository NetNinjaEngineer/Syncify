using Syncify.Api.Middleware;

namespace Syncify.Api.Extensions;

public static class ApiMiddlewareExtensions
{
    public static IApplicationBuilder UseApiMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<MigrateDatabaseMiddleware>();
        app.UseMiddleware<JwtValidationMiddleware>();

        return app;
    }
}
