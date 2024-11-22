using Syncify.Application.Bases;
using Syncify.Application.DTOs.Auth;
using Syncify.Application.Features.Auth.Commands.SignInGoogle;

namespace Syncify.Application.Interfaces.Services;
public interface IAuthService
{
    Task<Result<GoogleUserProfile?>> VerifyAndGetUserProfileAsync(SignInGoogleCommand command);
}
