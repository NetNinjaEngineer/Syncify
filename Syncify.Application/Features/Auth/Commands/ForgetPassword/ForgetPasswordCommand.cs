using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Auth.Commands.ForgetPassword;
public sealed class ForgetPasswordCommand : IRequest<Result<string>>
{
    public string Email { get; set; } = string.Empty;
}
