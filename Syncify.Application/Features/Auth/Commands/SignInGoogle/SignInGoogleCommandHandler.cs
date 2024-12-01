using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Auth;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Auth.Commands.SignInGoogle;
public sealed class SignInGoogleCommandHandler(IAuthService authService) : IRequestHandler<SignInGoogleCommand, Result<GoogleUserProfile?>>
{
    public async Task<Result<GoogleUserProfile?>> Handle(SignInGoogleCommand request, CancellationToken cancellationToken)
    {
        return await authService.VerifyAndGetUserProfileAsync(request);
    }
}
