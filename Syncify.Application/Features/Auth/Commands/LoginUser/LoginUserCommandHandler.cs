using MediatR;
using Microsoft.AspNetCore.Http;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Auth;
using Syncify.Application.Interfaces.Services;
using System.Text;

namespace Syncify.Application.Features.Auth.Commands.LoginUser;
public sealed class LoginUserCommandHandler(
    IAuthService authService,
    IHttpContextAccessor contextAccessor) : IRequestHandler<LoginUserCommand, Result<SignInResponseDto>>
{
    public async Task<Result<SignInResponseDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request);
        if (response is not { } loginResponse) return response;
        if (!string.IsNullOrEmpty(loginResponse.Value?.RefreshToken))
            SetRefreshTokenInCookie(Convert.ToBase64String(Encoding.UTF8.GetBytes(loginResponse.Value.RefreshToken)), loginResponse.Value.RefreshTokenExpiration);
        return response;
    }

    private void SetRefreshTokenInCookie(string refreshToken, DateTimeOffset expiresOn)
    {
        var cookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            Expires = expiresOn,
        };

        contextAccessor.HttpContext!.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}