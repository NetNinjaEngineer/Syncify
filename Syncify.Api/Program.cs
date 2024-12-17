using Syncify.Api;
using Syncify.Api.Extensions;
using Syncify.Application;
using Syncify.Infrastructure;
using Syncify.Persistence;
using Syncify.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services
    .AddInfrastructureDependencies(builder.Configuration)
    .AddApplicationDependencies(builder.Configuration)
    .AddServicesDependencies(builder.Configuration)
    .AddPersistenceDependencies(builder.Configuration)
    .AddApiDependencies(
        builder.Configuration,
        builder.WebHost);

var app = builder.Build();

app.UseSwaggerDocumentation();

app.UseApiMiddlewares();

app.UseStaticFiles();

app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseHubs();

app.Run();
