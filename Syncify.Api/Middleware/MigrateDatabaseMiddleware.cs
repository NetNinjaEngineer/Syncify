using Microsoft.EntityFrameworkCore;
using Syncify.Infrastructure.Persistence;

namespace Syncify.Api.Middleware;

public sealed class MigrateDatabaseMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var scopeFactory = context.RequestServices.GetRequiredService<IServiceScopeFactory>();
        var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
        await next(context);
    }
}
