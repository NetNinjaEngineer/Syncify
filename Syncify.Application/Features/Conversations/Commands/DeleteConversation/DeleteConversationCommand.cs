using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Conversations.Commands.DeleteConversation;
public sealed class DeleteConversationCommand : IRequest<Result<bool>>
{
    public required Guid ConversationId { get; set; }
}
