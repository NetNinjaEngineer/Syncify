using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Auth;

namespace Syncify.Application.Features.Auth.Commands.Verify2FaCode;
public sealed class Verify2FaCodeCommand : IRequest<Result<SignInResponseDto>>
{
    public string Code { get; set; } = null!;
}