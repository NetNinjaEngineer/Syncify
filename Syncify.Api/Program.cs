using Syncify.Api.Extensions;
using Syncify.Api.Hubs;
using Syncify.Application;
using Syncify.Infrastructure;
using Syncify.Persistence;
using Syncify.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services
    .AddInfrastructureDependencies(builder.Configuration)
    .AddApplicationDependencies(builder.Configuration)
    .AddServicesDependencies(builder.Configuration)
    .AddPersistenceDependencies(builder.Configuration);

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddGlobalExceptionHandler();

builder.Services.AddSwaggerDocumentation();

builder.Services.AddSignalR(options => options.EnableDetailedErrors = true);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerDocumentation();
}

app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<FriendRequestsHub>("/signalRHub/friendRequestsHub");

app.Run();
