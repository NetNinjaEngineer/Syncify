using AutoMapper;
using FluentValidation;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Auth;
using Syncify.Application.Features.Auth.Commands.ConfirmEmail;
using Syncify.Application.Features.Auth.Commands.ConfirmEnable2FaCommand;
using Syncify.Application.Features.Auth.Commands.ConfirmForgotPasswordCode;
using Syncify.Application.Features.Auth.Commands.Disable2Fa;
using Syncify.Application.Features.Auth.Commands.Enable2Fa;
using Syncify.Application.Features.Auth.Commands.ForgetPassword;
using Syncify.Application.Features.Auth.Commands.LoginUser;
using Syncify.Application.Features.Auth.Commands.RefreshToken;
using Syncify.Application.Features.Auth.Commands.Register;
using Syncify.Application.Features.Auth.Commands.RevokeToken;
using Syncify.Application.Features.Auth.Commands.SendConfirmEmailCode;
using Syncify.Application.Features.Auth.Commands.SignInGoogle;
using Syncify.Application.Features.Auth.Commands.ValidateToken;
using Syncify.Application.Features.Auth.Commands.Verify2FaCode;
using Syncify.Application.Helpers;
using Syncify.Application.Interfaces.Services;
using Syncify.Application.Interfaces.Services.Models;
using Syncify.Domain.Entities.Identity;
using Syncify.Domain.Enums;
using Syncify.Infrastructure.Persistence;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Syncify.Services.Services;

public sealed class AuthService(
    IMemoryCache memoryCache,
    ILogger<AuthService> logger,
    IOptions<AuthOptions> authOptions,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ApplicationDbContext context,
    IMapper mapper,
    IMailService mailService,
    IConfiguration configuration,
    ITokenService tokenService,
    IHttpContextAccessor contextAccessor,
    IFileService fileService) : IAuthService
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
            return Result<GoogleUserProfile?>.Success(profile);
        }

        return Result<GoogleUserProfile?>.Failure(
            statusCode: HttpStatusCode.UnprocessableEntity,
            message: "One or more errors happened when tring to login !!!",
            errors: loginResult.Errors.Select(e => $"{e.Code} : {e.Description}").ToList());
    }

    public async Task<Result<RegisterResponseDto>> RegisterAsync(RegisterCommand command)
    {
        var registerCommandValidator = new RegisterCommandValidator();
        await registerCommandValidator.ValidateAndThrowAsync(command);

        var user = mapper.Map<ApplicationUser>(command);

        var (uploaded, uploadedFileName) = await fileService.UploadFileAsync(command.Picture, "Images");
        user.ProfilePictureUrl = uploadedFileName;

        var result = await userManager.CreateAsync(user, command.Password);

        await userManager.AddToRoleAsync(user, AppConstants.Roles.User);

        return !result.Succeeded
            ? Result<RegisterResponseDto>.Failure(
                HttpStatusCode.BadRequest,
                DomainErrors.Users.UnableToCreateAccount,
                [result.Errors.Select(e => e.Description).FirstOrDefault() ?? string.Empty])
            : Result<RegisterResponseDto>.Success(new RegisterResponseDto(user.Id, true));
    }

    public async Task<Result<SendCodeConfirmEmailResponseDto>> SendConfirmEmailCodeAsync(
        SendConfirmEmailCodeCommand command)
    {
        var confirmEmailValidator = new SendConfirmEmailCodeCommandValidator();
        await confirmEmailValidator.ValidateAndThrowAsync(command);

        var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var user = await userManager.FindByEmailAsync(command.Email);

            if (user is null)
                return Result<SendCodeConfirmEmailResponseDto>.Failure(
                    HttpStatusCode.NotFound, DomainErrors.Users.UnkownUser);

            if (await userManager.IsEmailConfirmedAsync(user))
                return Result<SendCodeConfirmEmailResponseDto>.Failure(
                    HttpStatusCode.Conflict,
                    DomainErrors.Users.AlreadyEmailConfirmed);

            var authenticationCode = await userManager.GenerateUserTokenAsync(user, "Email", "Confirm User Email");
            var encodedAuthenticationCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationCode));

            user.Code = encodedAuthenticationCode;
            user.CodeExpiration = DateTimeOffset.Now.AddMinutes(
                minutes: Convert.ToDouble(configuration[AppConstants.AuthCodeExpireKey]!)
            );

            var identityResult = await userManager.UpdateAsync(user);

            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors
                    .Select(e => e.Description)
                    .ToList();

                return Result<SendCodeConfirmEmailResponseDto>.Failure(
                    HttpStatusCode.BadRequest,
                    DomainErrors.Users.UnableToUpdateUser, errors);
            }

            var emailMessage = new EmailMessage()
            {
                To = command.Email,
                Subject = "Activate your Syncify Account",
                Message = @$"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <h1 style='color: #4CAF50; font-size: 24px; margin-top: 20px;'>Welcome to Syncify</h1>
                        <p style='font-size: 16px; line-height: 1.5;'>
                            Thank you for registering with Syncify. To activate your account, please use the following code:
                        </p>
                        <div style='text-align: center; margin: 20px 0;'>
                            <span style='font-size: 32px; font-weight: 500; color: #4CAF50;'>{authenticationCode}</span>
                        </div>
                        <p style='font-size: 16px; line-height: 1.5;'>
                            This code will expire in <strong>{configuration[AppConstants.AuthCodeExpireKey]} minutes</strong>.
                        </p>
                        <p style='font-size: 16px; line-height: 1.5; color: #888;'>
                            If you did not request this registration, please ignore this email.
                        </p>
                        <p style='font-size: 16px; line-height: 1.5; color: #333;'>
                            Best regards,<br>
                            <strong>The Syncify Team</strong>
                        </p>
                    </div>"
            };

            await mailService.SendEmailAsync(emailMessage);


            await transaction.CommitAsync();
            return Result<SendCodeConfirmEmailResponseDto>.Success(
                SendCodeConfirmEmailResponseDto.ToResponse(user.CodeExpiration.Value),
                AppConstants.ConfirmEmailCodeSentSuccessfully
            );
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Result<SendCodeConfirmEmailResponseDto>.Failure(HttpStatusCode.BadRequest, ex.Message);
        }
    }


    private static RefreshToken GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = new RNGCryptoServiceProvider();
        rng.GetBytes(randomNumber);
        return new RefreshToken()
        {
            Token = Convert.ToBase64String(randomNumber),
            ExpiresOn = DateTimeOffset.Now.AddDays(10),
            CreatedOn = DateTimeOffset.Now
        };
    }

    private async Task<Result<ApplicationUser>> CheckIfUserHasAssignedToRefreshToken(
        string refreshToken)
    {
        var user = await userManager.Users.SingleOrDefaultAsync(x =>
            x.RefreshTokens != null && x.RefreshTokens.Any(x => x.Token == refreshToken));
        return user is null
            ? Result<ApplicationUser>.Failure(HttpStatusCode.NotFound, "Invalid Token")
            : Result<ApplicationUser>.Success(user);
    }


    public async Task<Result<string>> ForgotPasswordAsync(ForgetPasswordCommand command)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user == null)
            return Result<string>.Failure(
                HttpStatusCode.NotFound, "User not found");

        //code and Expiration 
        var decoded = await userManager.GenerateUserTokenAsync(user, "Email", "Generate Code");
        var authCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(decoded));
        user.Code = authCode;
        user.CodeExpiration =
            DateTimeOffset.Now.AddMinutes(Convert.ToDouble(configuration[AppConstants.AuthCodeExpireKey]));

        //Update user
        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Result<string>.Failure(HttpStatusCode.BadRequest, DomainErrors.Users.UnableToUpdateUser);

        var emailMessage = new EmailMessage()
        {
            To = command.Email,
            Subject = "Syncify - Reset Password Code",
            Message = @$"<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h1 style='color: #4CAF50; font-size: 24px; margin-top: 20px;'>Password Reset Code</h1>
                    <p style='font-size: 16px; line-height: 1.5;'>
                        Hello,
                    </p>
                    <p style='font-size: 16px; line-height: 1.5;'>
                        To reset your Syncify account password, please use the following code:
                    </p>
                    <div style='text-align: center; margin: 20px 0;'>
                        <span style='font-size: 32px; font-weight: 500; color: #4CAF50;'>{{decoded}}</span>
                    </div>
                    <p style='font-size: 16px; line-height: 1.5;'>
                        This code will expire in {configuration[AppConstants.AuthCodeExpireKey]} minutes.
                    </p>
                    <p style='font-size: 16px; line-height: 1.5; color: #555;'>
                        If you did not request a password reset, please ignore this email.
                    </p>
                    <p style='font-size: 16px; line-height: 1.5; color: #333;'>
                        Best regards,<br>
                        <strong>The Syncify Team</strong>
                    </p>
                </div>"
        };

        await mailService.SendEmailAsync(emailMessage);

        return Result<string>.Success("Password reset code has been sent to your email.");
    }

    public async Task<Result<string>> ConfirmForgotPasswordCodeAsync(ConfirmForgotPasswordCodeCommand command)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user == null)
            return Result<string>.Failure(HttpStatusCode.BadRequest, DomainErrors.Users.UserNotFound);

        var decodedAuthCode = Encoding.UTF8.GetString(Convert.FromBase64String(user.Code!));
        if (decodedAuthCode != command.Code)
            return Result<string>.Failure(HttpStatusCode.BadRequest, DomainErrors.Users.InvalidAuthCode);

        if (DateTimeOffset.Now > user.CodeExpiration)
            return Result<string>.Failure(HttpStatusCode.BadRequest, DomainErrors.Users.CodeExpired);

        await userManager.RemovePasswordAsync(user);
        var result = await userManager.AddPasswordAsync(user, command.NewPassword);

        if (!result.Succeeded)
        {
            var err = result.Errors.Select(x => x.Description).FirstOrDefault();
            return Result<string>.Failure(HttpStatusCode.UnprocessableEntity, error: err);
        }


        return Result<string>.Success("Reset Password Successfuly.");
    }

    public async Task<Result<string>> Enable2FaAsync(Enable2FaCommand command)
    {
        var user = await userManager.FindByEmailAsync(command.Email);

        if (user == null)
            return Result<string>.Failure(HttpStatusCode.BadRequest,
                string.Format(DomainErrors.Users.UserNotFound, command.Email));

        var code = await userManager.GenerateTwoFactorTokenAsync(user, command.TokenProvider.ToString());

        await signInManager.TwoFactorSignInAsync(code, command.TokenProvider.ToString(), false, true);

        if (command.TokenProvider == TokenProvider.Email)
            // send code via user email
            await mailService.SendEmailAsync(
                new EmailMessage()
                {
                    To = command.Email,
                    Subject = "Enable Syncify 2FA",
                    Message = @$"<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h1 style='color: #4CAF50; font-size: 24px; margin-top: 20px;'>Enable 2-Factor Authentication</h1>
                    <p style='font-size: 16px; line-height: 1.5;'>
                        To enhance the security of your Syncify account, we recommend enabling 2-factor authentication (2FA).
                    </p>
                    <div style='text-align: center; margin: 20px 0;'>
                        <span style='font-size: 32px; font-weight: 500; color: #4CAF50;'>{code}</span>
                    </div>
                    <p style='font-size: 16px; line-height: 1.5;'>
                        Use this code to enable 2FA on your account. This code will expire in 5 minutes.
                    </p>
                    <p style='font-size: 16px; line-height: 1.5; color: #888;'>
                        If you did not request this 2FA activation, please ignore this email.
                    </p>
                    <p style='font-size: 16px; line-height: 1.5; color: #333;'>
                        Best regards,<br>
                        <strong>The Syncify Team</strong>
                    </p>
                </div>"
                });


        else if (command.TokenProvider == TokenProvider.Phone)
        {
            // handle send via phone
        }

        return Result<string>.Success(AppConstants.TwoFactorCodeSent);
    }

    public async Task<Result<string>> ConfirmEnable2FaAsync(ConfirmEnable2FaCommand command)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user == null)
            return Result<string>.Failure(HttpStatusCode.NotFound, DomainErrors.Users.UnkownUser);

        // verify 2fa code
        var providers = await userManager.GetValidTwoFactorProvidersAsync(user);

        if (providers.Contains(TokenProvider.Email.ToString()))
        {
            var verified =
                await userManager.VerifyTwoFactorTokenAsync(user, TokenProvider.Email.ToString(), command.Code);

            if (!verified)
                return Result<string>.Failure(HttpStatusCode.BadRequest, DomainErrors.Users.Invalid2FaCode);

            // code is verified update status of 2FA

            await userManager.SetTwoFactorEnabledAsync(user, true);

            await mailService.SendEmailAsync(
                new EmailMessage()
                {
                    Subject = "Syncify 2FA Setup Complete",
                    To = user.Email!,
                    Message = @$"<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <h1 style='color: #4CAF50; font-size: 24px; margin-top: 20px;'>2-Factor Authentication Enabled</h1>
                        <p style='font-size: 16px; line-height: 1.5;'>
                            Congratulations! Your 2-factor authentication (2FA) has been successfully enabled for your Syncify account.
                        </p>
                        <p style='font-size: 16px; line-height: 1.5;'>
                            Your account is now more secure, and you'll be required to enter a one-time code when logging in.
                        </p>
                        <p style='font-size: 16px; line-height: 1.5; color: #333;'>
                            Best regards,<br>
                            <strong>The Syncify Team</strong>
                        </p>
                    </div>"
                });


            return Result<string>.Success(AppConstants.TwoFactorEnabled);
        }

        return Result<string>.Failure(HttpStatusCode.BadRequest, DomainErrors.Users.InvalidTokenProvider);
    }

    public async Task<Result<SignInResponseDto>> Verify2FaCodeAsync(Verify2FaCodeCommand command)
    {
        var userEmail =
            Encoding.UTF8.GetString(
                Convert.FromBase64String(contextAccessor.HttpContext!.Request.Cookies["userEmail"]!));
        var appUser = await userManager.FindByEmailAsync(userEmail);

        if (appUser == null) return Result<SignInResponseDto>.Failure(HttpStatusCode.Unauthorized);

        var verified = await userManager.VerifyTwoFactorTokenAsync(appUser, "Email", command.Code);

        if (!verified)
            return Result<SignInResponseDto>.Failure(HttpStatusCode.BadRequest,
                DomainErrors.Users.Invalid2FaCode);

        await signInManager.SignInAsync(appUser, isPersistent: true);

        var response = await CreateLoginResponseAsync(userManager, appUser);

        return Result<SignInResponseDto>.Success(response);
    }

    public async Task<Result<string>> Disable2FaAsync(Disable2FaCommand command)
    {
        var user = await userManager.FindByEmailAsync(command.Email);

        if (user == null)
            return Result<string>.Failure(HttpStatusCode.NotFound, string.Format(DomainErrors.Users.UserNotFound, command.Email));

        if (!await userManager.GetTwoFactorEnabledAsync(user))
            return Result<string>.Failure(HttpStatusCode.BadRequest, DomainErrors.Users.TwoFactorAlreadyDisabled);

        var result = await userManager.SetTwoFactorEnabledAsync(user, false);

        if (!result.Succeeded)
            return Result<string>.Failure(HttpStatusCode.BadRequest, DomainErrors.Users.Disable2FaFailed);

        await mailService.SendEmailAsync(
            new EmailMessage()
            {
                To = user.Email!,
                Subject = "Syncify 2FA Disabled",
                Message = @$"<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <h1 style='color: #4CAF50; font-size: 24px; margin-top: 20px;'>2-Factor Authentication Disabled</h1>
                        <p style='font-size: 16px; line-height: 1.5;'>
                            Your two-factor authentication (2FA) has been successfully disabled for your Syncify account.
                        </p>
                        <p style='font-size: 16px; line-height: 1.5;'>
                            Please note that disabling 2FA may decrease the security of your account. It is recommended to keep 2FA enabled whenever possible.
                        </p>
                        <p style='font-size: 16px; line-height: 1.5; color: #333;'>
                            Best regards,<br>
                            <strong>The Syncify Team</strong>
                        </p>
                    </div>"
            });

        return Result<string>.Success(AppConstants.Disable2FaSuccess);
    }

    public async Task<Result<string>> ConfirmEmailAsync(ConfirmEmailCommand command)
    {
        var validator = new ConfirmEmailCommandValidator();
        await validator.ValidateAndThrowAsync(command);

        var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var user = await userManager.FindByEmailAsync(command.Email);

            if (user is null)
                return Result<string>.Failure(HttpStatusCode.NotFound, DomainErrors.Users.UnkownUser);

            var decodedAuthenticationCode = Encoding.UTF8.GetString(Convert.FromBase64String(user.Code!));

            if (decodedAuthenticationCode == command.Token)
            {
                // check if the token is expired
                if (DateTimeOffset.Now > user.CodeExpiration)
                    return Result<string>.Failure(HttpStatusCode.BadRequest, DomainErrors.Users.AuthCodeExpired);

                // confirm the email for the user
                user.EmailConfirmed = true;
                var identityResult = await userManager.UpdateAsync(user);

                if (!identityResult.Succeeded)
                {
                    var errors = identityResult.Errors
                        .Select(e => e.Description)
                        .ToList();

                    return Result<string>.Failure(HttpStatusCode.BadRequest, DomainErrors.Users.UnableToUpdateUser, errors);
                }

                var emailMessage = new EmailMessage()
                {
                    To = command.Email,
                    Subject = "Welcome to Syncify - Email Confirmed",
                    Message = @$"<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <h1 style='color: #4CAF50; font-size: 24px; margin-top: 20px;'>Congratulations, Your Email is Confirmed!</h1>
                        <p style='font-size: 16px; line-height: 1.5;'>
                            Hello, and welcome to Syncify. Your email address has been successfully confirmed.
                        </p>
                        <p style='font-size: 16px; line-height: 1.5;'>
                            You can now access all the features and benefits of your Syncify account.
                        </p>
                        <p style='font-size: 16px; line-height: 1.5; color: #555;'>
                            If you encounter any issues or have any questions, feel free to reach out to our support team.
                        </p>
                        <p style='font-size: 16px; line-height: 1.5; color: #333;'>
                            Best regards,<br>
                            <strong>The Syncify Team</strong>
                        </p>
                    </div>"
                };

                await mailService.SendEmailAsync(emailMessage);


                await transaction.CommitAsync();
                return Result<string>.Success(AppConstants.EmailConfirmed);
            }

            return Result<string>.Failure(HttpStatusCode.BadRequest, DomainErrors.Users.InvalidAuthCode);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Result<string>.Failure(HttpStatusCode.BadRequest, ex.Message);
        }
    }

    public async Task LogoutAsync() => await signInManager.SignOutAsync();

    public async Task<Result<ValidateTokenResponseDto>> ValidateTokenAsync(ValidateTokenCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.JwtToken))
            return Result<ValidateTokenResponseDto>.Failure(HttpStatusCode.BadRequest, "Token cannot be null or empty.");

        var claimsPrincipal = await tokenService.ValidateToken(command.JwtToken);

        var response = new ValidateTokenResponseDto();
        foreach (var claim in claimsPrincipal.Claims)
            response.Claims.Add(new ClaimsResponse() { ClaimType = claim.Type, ClaimValue = claim.Value });

        return Result<ValidateTokenResponseDto>.Success(response, "token is valid.");
    }



    public async Task<Result<SignInResponseDto>> LoginAsync(LoginUserCommand command)
    {
        var loggedInUser = await userManager.FindByEmailAsync(command.Email);

        if (loggedInUser is null)
            return Result<SignInResponseDto>.Failure(HttpStatusCode.NotFound, DomainErrors.Users.UnkownUser);

        if (!await userManager.IsEmailConfirmedAsync(loggedInUser))
            return Result<SignInResponseDto>.Failure(HttpStatusCode.BadRequest, DomainErrors.Users.EmailNotConfirmed);

        if (await userManager.GetTwoFactorEnabledAsync(loggedInUser))
        {
            // if two factor is enabled send code to user
            var twoFactorCode = await userManager.GenerateTwoFactorTokenAsync(loggedInUser, "Email");

            await signInManager.TwoFactorSignInAsync("Email", twoFactorCode, false, true);

            contextAccessor.HttpContext!.Response.Cookies.Append(
                "userEmail",
                Convert.ToBase64String(Encoding.UTF8.GetBytes(loggedInUser.Email!)),
                new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict
                });

            await mailService.SendEmailAsync(
                new EmailMessage()
                {
                    To = command.Email,
                    Subject = "Syncify 2FA Code Required",
                    Message = @$"<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <h1 style='color: #4CAF50; font-size: 24px; margin-top: 20px;'>2-Factor Authentication Required</h1>
                        <p style='font-size: 16px; line-height: 1.5;'>
                            To complete the login process, you'll need to enter a 2-factor authentication (2FA) code.
                        </p>
                        <div style='text-align: center; margin: 20px 0;'>
                            <span style='font-size: 32px; font-weight: 500; color: #4CAF50;'>{twoFactorCode}</span>
                        </div>
                        <p style='font-size: 16px; line-height: 1.5;'>
                            Please enter this code to verify your identity and access your Syncify account.
                        </p>
                        <p style='font-size: 16px; line-height: 1.5; color: #333;'>
                            Best regards,<br>
                            <strong>The Syncify Team</strong>
                        </p>
                    </div>"
                });



            return Result<SignInResponseDto>.Failure(HttpStatusCode.BadRequest, DomainErrors.Users.TwoFactorRequired);
        }

        // check account is locked
        if (await userManager.IsLockedOutAsync(loggedInUser))
        {
            return Result<SignInResponseDto>.Failure(
                HttpStatusCode.Unauthorized,
                $"Your account is locked until {loggedInUser.LockoutEnd!.Value.ToLocalTime()}");
        }

        var result = await signInManager.PasswordSignInAsync(
            user: loggedInUser,
            password: command.Password,
            isPersistent: true,
            lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

            var lockoutEndTime = TimeZoneInfo.ConvertTimeFromUtc(loggedInUser.LockoutEnd!.Value.UtcDateTime, timeZone);

            return Result<SignInResponseDto>.Failure(
                 HttpStatusCode.Unauthorized, $"Your account is locked until {lockoutEndTime}");
        }

        if (result.Succeeded)
        {
            var response = await CreateLoginResponseAsync(userManager, loggedInUser);
            return Result<SignInResponseDto>.Success(response);
        }

        return Result<SignInResponseDto>.Failure(
            HttpStatusCode.Unauthorized, DomainErrors.Users.InvalidCredientials);
    }

    private async Task<SignInResponseDto> CreateLoginResponseAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationUser loggedInUser)
    {
        var token = await tokenService.GenerateJwtTokenAsync(loggedInUser);
        var userRoles = await userManager.GetRolesAsync(loggedInUser);
        var response = new SignInResponseDto()
        {
            Email = loggedInUser.Email!,
            UserName = loggedInUser.UserName!,
            IsAuthenticated = true,
            Roles = [.. userRoles],
            Token = token,
        };

        if (loggedInUser.RefreshTokens != null && loggedInUser.RefreshTokens.Any(x => x.IsActive))
        {
            var activeRefreshToken = loggedInUser.RefreshTokens.FirstOrDefault(x => x.IsActive);
            if (activeRefreshToken != null)
            {
                response.RefreshToken = activeRefreshToken.Token;
                response.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
        }
        else
        {
            // user not have active refresh token
            var refreshToken = GenerateRefreshToken();
            response.RefreshToken = refreshToken.Token;
            response.RefreshTokenExpiration = refreshToken.ExpiresOn;
            loggedInUser.RefreshTokens?.Add(refreshToken);
            await userManager.UpdateAsync(loggedInUser);
        }

        return response;
    }

    public async Task<Result<SignInResponseDto>> RefreshTokenAsync(RefreshTokenCommand command)
        => (await (await (await (await CheckIfUserHasAssignedToRefreshToken(command.Token))
                        .Bind(user => SelectRefreshTokenAssignedToUser(user, command.Token))
                        .Bind(CheckIfTokenIsActive)
                        .Bind(RevokeUserTokenAndReturnsAppUser)
                        .BindAsync(GenerateNewRefreshToken))
                    .BindAsync(GenerateNewJwtToken))
                .BindAsync(CreateSignInResponse))
            .Map(authResponse => authResponse);

    public async Task<Result<bool>> RevokeTokenAsync(RevokeTokenCommand command)
        => (await (await CheckIfUserHasAssignedToRefreshToken(command.Token!))
                .Bind(appUser => SelectRefreshTokenAssignedToUser(appUser, command.Token!))
                .Bind(CheckIfTokenIsActive)
                .Bind(RevokeUserTokenAndReturnsAppUser)
                .BindAsync(UpdateApplicationUser))
            .Map(userUpdated => userUpdated);

    private async Task<Result<bool>> UpdateApplicationUser(ApplicationUser appUser)
    {
        await userManager.UpdateAsync(appUser);
        return Result<bool>.Success(true);
    }


    private async Task<Result<SignInResponseDto>> CreateSignInResponse(
        (ApplicationUser appUser, string jwtToken) appUserWithJwt)
    {
        var userRoles = await userManager.GetRolesAsync(appUserWithJwt.appUser);
        var newRefreshToken = appUserWithJwt.appUser.RefreshTokens?.FirstOrDefault(x => x.IsActive);

        var response = new SignInResponseDto
        {
            IsAuthenticated = true,
            UserName = appUserWithJwt.appUser.UserName!,
            Email = appUserWithJwt.appUser.Email!,
            Token = appUserWithJwt.jwtToken,
            Roles = [.. userRoles],
            RefreshToken = newRefreshToken?.Token,
            RefreshTokenExpiration = newRefreshToken!.ExpiresOn
        };

        return Result<SignInResponseDto>.Success(response);
    }

    private async Task<Result<(ApplicationUser appUser, string jwtToken)>>
        GenerateNewJwtToken(ApplicationUser appUser)
    {
        var jwtToken = await tokenService.GenerateJwtTokenAsync(appUser);
        return Result<(ApplicationUser appUser, string jwtToken)>.Success((appUser, jwtToken));
    }

    private async Task<Result<ApplicationUser>> GenerateNewRefreshToken(ApplicationUser appUser)
    {
        var newRefreshToken = GenerateRefreshToken();
        appUser.RefreshTokens?.Add(newRefreshToken);
        await userManager.UpdateAsync(appUser);
        return Result<ApplicationUser>.Success(appUser);
    }

    private Result<ApplicationUser> RevokeUserTokenAndReturnsAppUser(RefreshToken userRefreshToken)
    {
        userRefreshToken.RevokedOn = DateTimeOffset.Now;
        var user = userManager.Users.SingleOrDefault(x =>
            x.RefreshTokens != null && x.RefreshTokens.Any(x => x.Token == userRefreshToken.Token));
        return Result<ApplicationUser>.Success(user!);
    }

    private static Result<RefreshToken> CheckIfTokenIsActive(RefreshToken userRefreshToken)
    {
        if (!userRefreshToken.IsActive)
            return Result<RefreshToken>.Failure(HttpStatusCode.BadRequest, "Inactive token");
        return Result<RefreshToken>.Success(userRefreshToken);
    }

    private static Result<RefreshToken> SelectRefreshTokenAssignedToUser(ApplicationUser user,
        string token)
    {
        var refreshToken = user.RefreshTokens?.Single(x => x.Token == token);
        if (refreshToken is not null)
            return Result<RefreshToken>.Success(refreshToken);
        return Result<RefreshToken>.Failure(HttpStatusCode.NotFound, "Token not found");
    }
}
