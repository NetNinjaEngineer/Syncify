using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Messages.Commands.EditMessage;
public sealed class EditMessageCommand : IRequest<Result<bool>>
{
    public Guid MessageId { get; set; }
    public string NewContent { get; set; } = null!;
}
