using Microsoft.AspNetCore.Identity;
using Syncify.Application;
using Syncify.Domain.Entities.Identity;
using Syncify.Infrastructure;
using Syncify.Infrastructure.Persistence;
using Syncify.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services
    .AddInfrastructureDependencies(builder.Configuration)
    .AddApplicationDependecies()
    .AddServicesDependecies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
