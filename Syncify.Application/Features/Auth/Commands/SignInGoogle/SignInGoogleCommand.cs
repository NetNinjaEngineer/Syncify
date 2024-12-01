using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Auth;

namespace Syncify.Application.Features.Auth.Commands.SignInGoogle;
public sealed class SignInGoogleCommand : IRequest<Result<GoogleUserProfile?>>
{
    public string IdToken { get; set; } = null!;
}
