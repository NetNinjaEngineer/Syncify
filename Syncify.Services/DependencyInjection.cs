using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Syncify.Application.Helpers;
using Syncify.Application.Interfaces.Services;
using Syncify.Services.Services;
using System.Text;

namespace Syncify.Services;
public static class DependencyInjection
{
    public static IServiceCollection AddServicesDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IFriendshipService, FriendshipService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddMemoryCache();

        var jwtSettings = new JwtSettings();
        configuration.GetSection(nameof(JwtSettings)).Bind(jwtSettings);

        services.Configure<AuthOptions>(configuration.GetSection("Authentication"));

        var authOptions = new AuthOptions();
        configuration.GetSection("Authentication").Bind(authOptions);

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
            .AddCookie(options =>
            {
                options.LoginPath = "/api/auth/login";
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = jwtSettings.Audience,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            })
            //.AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
            //{
            //    options.AppId = authOptions.FacebookOptions.AppId;
            //    options.AppSecret = authOptions.FacebookOptions.AppSecret;
            //})
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = authOptions.GoogleOptions.ClientId;
                options.ClientSecret = authOptions.GoogleOptions.ClientSecret;
            });


        services.Configure<SmtpSettings>(configuration.GetSection(nameof(SmtpSettings)));

        services.AddTransient<IMailService, MailService>();

        services.AddScoped<IFollowingService, FollowingService>();

        return services;
    }
}
