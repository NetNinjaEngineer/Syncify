using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Auth.Commands.ConfirmEnable2FaCommand;
public sealed class ConfirmEnable2FaCommand : IRequest<Result<string>>
{
    public string Email { get; set; } = null!;
    public string Code { get; set; } = null!;
}
