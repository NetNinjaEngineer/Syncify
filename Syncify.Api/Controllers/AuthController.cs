using MediatR;
using Microsoft.AspNetCore.Mvc;
using Syncify.Api.Base;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Auth;
using Syncify.Application.Features.Auth.Commands.ConfirmEmail;
using Syncify.Application.Features.Auth.Commands.ConfirmEnable2FaCommand;
using Syncify.Application.Features.Auth.Commands.ConfirmForgotPasswordCode;
using Syncify.Application.Features.Auth.Commands.Disable2Fa;
using Syncify.Application.Features.Auth.Commands.Enable2Fa;
using Syncify.Application.Features.Auth.Commands.ForgetPassword;
using Syncify.Application.Features.Auth.Commands.LoginUser;
using Syncify.Application.Features.Auth.Commands.Logout;
using Syncify.Application.Features.Auth.Commands.RefreshToken;
using Syncify.Application.Features.Auth.Commands.Register;
using Syncify.Application.Features.Auth.Commands.RevokeToken;
using Syncify.Application.Features.Auth.Commands.SendConfirmEmailCode;
using Syncify.Application.Features.Auth.Commands.SignInGoogle;
using Syncify.Application.Features.Auth.Commands.ValidateToken;
using Syncify.Application.Features.Auth.Commands.Verify2FaCode;
using Syncify.Application.Helpers;
using System.Text;

namespace Syncify.Api.Controllers;
[Route("api/auth")]
public class AuthController(IMediator mediator) : ApiBaseController(mediator)
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(Result<RegisterResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<RegisterResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<RegisterResponseDto>>> RegisterUserAsync([FromForm] RegisterCommand command)
        => CustomResult(await Mediator.Send(command));

    [HttpPost("signin-google")]
    [ProducesResponseType(typeof(Result<GoogleUserProfile?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GlobalErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(Result<string>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<GoogleUserProfile>>> SigninWithGoogleAsync(SignInGoogleCommand command)
        => CustomResult(await Mediator.Send(command));

    [HttpPost("sendConfirmEmailCode")]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(Result<SendCodeConfirmEmailResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<SendCodeConfirmEmailResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<SendCodeConfirmEmailResponseDto>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Result<string>>> SendConfirmEmailCodeAsync(SendConfirmEmailCodeCommand command)
        => CustomResult(await Mediator.Send(command));

    [HttpPost("confirm-email")]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<string>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<string>>> ConfirmEmailAsync(ConfirmEmailCommand command)
        => CustomResult(await Mediator.Send(command));

    [HttpPost("signOut")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> SignOutAsync()
    {
        await Mediator.Send(new LogoutCommand());
        return Ok();
    }

    [HttpPost("login-user")]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(Result<SignInResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<SignInResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<SignInResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<SignInResponseDto>>> LoginUserAsync(LoginUserCommand command)
        => CustomResult(await Mediator.Send(command));

    [HttpGet("refresh-token")]
    [ProducesResponseType(typeof(Result<SignInResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<SignInResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<SignInResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<SignInResponseDto>>> RefreshTokenAsync()
    {
        var refreshToken = Encoding.UTF8.GetString(Convert.FromBase64String(Request.Cookies["refreshToken"]!));
        var authResponseResult = await Mediator.Send(new RefreshTokenCommand { Token = refreshToken! });
        if (authResponseResult.Value?.IsAuthenticated == true)
            SetRefreshTokenInCookie(
                Convert.ToBase64String(Encoding.UTF8.GetBytes(authResponseResult.Value.RefreshToken!)),
                authResponseResult.Value.RefreshTokenExpiration);

        return CustomResult(authResponseResult);
    }

    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeTokenAsync(RevokeTokenCommand command)
    {
        if (command.Token is not null) return CustomResult(await Mediator.Send(command));
        command = new RevokeTokenCommand
        { Token = Encoding.UTF8.GetString(Convert.FromBase64String(Request.Cookies["refreshToken"]!)) };
        return CustomResult(await Mediator.Send(command));
    }

    private void SetRefreshTokenInCookie(string valueRefreshToken, DateTimeOffset valueRefreshTokenExpiration)
    {
        var cookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            Expires = valueRefreshTokenExpiration,
        };

        Response.Cookies.Append("refreshToken", valueRefreshToken, cookieOptions);
    }


    [HttpPost("forgot-password")]
    public async Task<ActionResult<Result<string>>> ForgotPasswordAsync(ForgetPasswordCommand command) => CustomResult(await Mediator.Send(command));

    [HttpPost("reset-password")]
    public async Task<ActionResult<Result<string>>> ConfirmForgotPasswordCodeAsync(
        ConfirmForgotPasswordCodeCommand command) =>
        CustomResult(await Mediator.Send(command));


    [HttpPost("enable-2fa")]
    public async Task<ActionResult<Result<string>>> Enable2FaAsync(Enable2FaCommand command)
        => CustomResult(await Mediator.Send(command));

    [HttpPost("confirm-enable-2fa")]
    public async Task<ActionResult<Result<string>>> ConfirmEnable2FaAsync(ConfirmEnable2FaCommand command)
        => CustomResult(await Mediator.Send(command));

    [HttpPost("verify-2fa")]
    public async Task<ActionResult<Result<string>>> Verify2FaAsync(Verify2FaCodeCommand command)
        => CustomResult(await Mediator.Send(command));

    [HttpPost("disable-2fa")]
    public async Task<ActionResult<Result<string>>> Disable2FaAsync(Disable2FaCommand command)
        => CustomResult(await Mediator.Send(command));

    [HttpPost("validate-token")]
    public async Task<ActionResult<Result<ValidateTokenResponseDto>>> ValidateTokenAsync([FromQuery] ValidateTokenCommand command)
        => CustomResult(await Mediator.Send(command));
}
