using Microsoft.AspNetCore.Http.Features;
using Syncify.Api.Extensions;
using Syncify.Api.Middleware;
using Syncify.Application;
using Syncify.Application.Hubs;
using Syncify.Infrastructure;
using Syncify.Persistence;
using Syncify.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi();

builder.Services
    .AddInfrastructureDependencies(builder.Configuration)
    .AddApplicationDependencies(builder.Configuration)
    .AddServicesDependencies(builder.Configuration)
    .AddPersistenceDependencies(builder.Configuration);

builder.Services.AddGlobalExceptionHandler();

builder.Services.AddSwaggerDocumentation();

builder.Services.AddSignalR(options => options.EnableDetailedErrors = true);

builder.WebHost.ConfigureKestrel(serverOptions =>
    serverOptions.Limits.MaxRequestBodySize = Convert.ToInt64(builder.Configuration["FormOptionsSize"]));

builder.Services.Configure<IISServerOptions>(options =>
    options.MaxRequestBodySize = Convert.ToInt64(builder.Configuration["FormOptionsSize"]));

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = Convert.ToInt64(builder.Configuration["FormOptionsSize"]);
    options.ValueLengthLimit = int.MaxValue;
    options.MemoryBufferThreshold = int.MaxValue;
});

builder.Services.AddScoped<MigrateDatabaseMiddleware>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerDocumentation();
}

app.UseMiddleware<MigrateDatabaseMiddleware>();

app.UseStaticFiles();

app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHub>("/hubs/notifications");

app.MapHub<MessageHub>("/hubs/messages");

app.MapHub<FriendRequestHub>("/hubs/friendRequests");

app.Run();
