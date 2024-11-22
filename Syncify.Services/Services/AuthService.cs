using FluentValidation;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Auth;
using Syncify.Application.Features.Auth.Commands.SignInGoogle;
using Syncify.Application.Helpers;
using Syncify.Application.Interfaces.Services;
using Syncify.Domain.Entities.Identity;
using Syncify.Domain.Events;
using System.Net;
using System.Text.Json;

namespace Syncify.Services.Services;

public sealed class AuthService(
    IMemoryCache memoryCache,
    ILogger<AuthService> logger,
    IOptions<AuthOptions> authOptions,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IMediator mediator) : IAuthService
{
    private const string CacheKeyPrefix = "GoogleToken_";
    private readonly AuthOptions _authenticationOptions = authOptions.Value;

    public async Task<Result<GoogleUserProfile?>> VerifyAndGetUserProfileAsync(SignInGoogleCommand command)
    {
        var validator = new SignInGoogleCommandValidator();
        await validator.ValidateAndThrowAsync(command);

        GoogleJsonWebSignature.ValidationSettings validationSettings = new()
        {
            Audience = [_authenticationOptions.GoogleOptions.ClientId]
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(command.IdToken, validationSettings);

        var cacheKey = $"{CacheKeyPrefix}{payload.Subject}";
        if (memoryCache.TryGetValue(cacheKey, out GoogleUserProfile? userProfile))
        {
            logger.LogInformation($"Get GoogleUser Info From Cache: {JsonSerializer.Serialize(userProfile)}");
            await mediator.Publish(
                new GoogleUserRegisteredEvent(payload.Email, payload.Name),
                CancellationToken.None);

            return Result<GoogleUserProfile?>.Success(userProfile);
        }

        var profile = new GoogleUserProfile(
            Email: payload.Email,
            Name: payload.Name,
            Picture: payload.Picture,
            FirstName: payload.GivenName,
            LastName: payload.FamilyName,
            GoogleId: payload.Subject,
            Locale: payload.Locale,
            EmailVerified: payload.EmailVerified,
            HostedDomain: payload.HostedDomain,
            Expires: TimeSpan.FromSeconds(Convert.ToDouble(payload.ExpirationTimeSeconds)));

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

        memoryCache.Set(cacheKey, profile, cacheOptions);

        var existingUser = await userManager.FindByEmailAsync(payload.Email);

        if (existingUser != null)
        {
            await mediator.Publish(
                new GoogleUserRegisteredEvent(payload.Email, payload.Name),
                CancellationToken.None);

            return Result<GoogleUserProfile?>.Success(profile);
        }

        existingUser = new ApplicationUser()
        {
            Email = payload.Email,
            FirstName = payload.GivenName,
            LastName = payload.FamilyName,
            EmailConfirmed = payload.EmailVerified,
            ProfilePictureUrl = payload.Picture,
            UserName = payload.Email,
            CreatedAt = DateTimeOffset.Now
        };

        var createResult = await userManager.CreateAsync(existingUser);

        if (!createResult.Succeeded)
            return Result<GoogleUserProfile?>.Failure(
                statusCode: HttpStatusCode.UnprocessableEntity,
                message: "One or more errors happened",
                errors: createResult.Errors.Select(e => $"{e.Code}: {e.Description}").ToList());

        var loginResult = await userManager.AddLoginAsync(existingUser,
            new UserLoginInfo("Google", existingUser.Email, existingUser.FirstName));

        if (loginResult.Succeeded)
        {
            await mediator.Publish(
                new GoogleUserRegisteredEvent(payload.Email, payload.Name),
                CancellationToken.None);
            return Result<GoogleUserProfile?>.Success(profile);
        }

        return Result<GoogleUserProfile?>.Failure(
            statusCode: HttpStatusCode.UnprocessableEntity,
            message: "One or more errors happened when tring to login !!!",
            errors: loginResult.Errors.Select(e => $"{e.Code} : {e.Description}").ToList());
    }
}
