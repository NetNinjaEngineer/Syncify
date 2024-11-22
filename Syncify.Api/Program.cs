using Microsoft.AspNetCore.Identity;
using Syncify.Api.Extensions;
using Syncify.Application;
using Syncify.Domain.Entities.Identity;
using Syncify.Infrastructure;
using Syncify.Infrastructure.Persistence;
using Syncify.Persistence;
using Syncify.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services
    .AddInfrastructureDependencies(builder.Configuration)
    .AddApplicationDependencies(builder.Configuration)
    .AddServicesDependencies(builder.Configuration)
    .AddPersistenceDependencies(builder.Configuration);

builder.Services.AddGlobalExceptionHandler();

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
