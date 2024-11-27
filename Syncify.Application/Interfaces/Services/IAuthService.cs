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

namespace Syncify.Application.Interfaces.Services;
public interface IAuthService
{
    Task<Result<RegisterResponseDto>> RegisterAsync(RegisterCommand command);
    Task<Result<GoogleUserProfile?>> VerifyAndGetUserProfileAsync(SignInGoogleCommand command);
    Task<Result<SignInResponseDto>> LoginAsync(LoginUserCommand command);
    Task<Result<SignInResponseDto>> RefreshTokenAsync(RefreshTokenCommand command);
    Task<Result<bool>> RevokeTokenAsync(RevokeTokenCommand command);
    Task<Result<string>> ForgotPasswordAsync(ForgetPasswordCommand command);
    Task<Result<string>> ConfirmForgotPasswordCodeAsync(ConfirmForgotPasswordCodeCommand command);
    Task<Result<string>> Enable2FaAsync(Enable2FaCommand command);
    Task<Result<string>> ConfirmEnable2FaAsync(ConfirmEnable2FaCommand command);
    Task<Result<SignInResponseDto>> Verify2FaCodeAsync(Verify2FaCodeCommand command);
    Task<Result<string>> Disable2FaAsync(Disable2FaCommand command);
    Task<Result<SendCodeConfirmEmailResponseDto>> SendConfirmEmailCodeAsync(SendConfirmEmailCodeCommand command);
    Task<Result<string>> ConfirmEmailAsync(ConfirmEmailCommand command);
    Task LogoutAsync();
    Task<Result<ValidateTokenResponseDto>> ValidateTokenAsync(ValidateTokenCommand command);
}
