using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Conversations.Commands.StartConversation;
public sealed class StartConversationCommand : IRequest<Result<string>>
{
    public string ReceiverId { get; set; } = null!;
}
