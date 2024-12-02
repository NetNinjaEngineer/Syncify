using Microsoft.AspNetCore.Identity;
using Syncify.Application.Bases;
using Syncify.Application.Features.Conversations.Commands.StartConversation;
using Syncify.Application.Helpers;
using Syncify.Application.Interfaces.Services;
using Syncify.Domain.Entities;
using Syncify.Domain.Entities.Identity;
using Syncify.Domain.Interfaces;
using System.Net;

namespace Syncify.Services.Services;
public sealed class ConversationService(
    IUnitOfWork unitOfWork,
    UserManager<ApplicationUser> userManager,
    ICurrentUser currentUser) : IConversationService
{
    public async Task<Result<string>> StartConversationAsync(StartConversationCommand command)
    {
        var sender = await userManager.FindByIdAsync(currentUser.Id);
        if (sender == null)
            return Result<string>.Failure(HttpStatusCode.Unauthorized, DomainErrors.Users.UserUnauthorized);

        var receiver = await userManager.FindByIdAsync(command.ReceiverId);
        if (receiver == null)
            return Result<string>.Failure(HttpStatusCode.NotFound, DomainErrors.Users.UserNotExists);

        if (currentUser.Id == command.ReceiverId)
            return Result<string>.Failure(HttpStatusCode.Conflict,
                DomainErrors.Conversation.CanNotStartConversationToSelf);

        var conversation = new Conversation()
        {
            Id = Guid.NewGuid(),
            ReceiverUserId = command.ReceiverId,
            SenderUserId = currentUser.Id
        };

        unitOfWork.Repository<Conversation>()?.Create(conversation);

        await unitOfWork.SaveChangesAsync();

        return Result<string>.Success(conversation.Id.ToString());
    }
}
