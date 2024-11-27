using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Auth;

namespace Syncify.Application.Features.Auth.Commands.SendConfirmEmailCode;
public sealed class SendConfirmEmailCodeCommand : IRequest<Result<SendCodeConfirmEmailResponseDto>>
{
    public string Email { get; set; } = string.Empty;
}
