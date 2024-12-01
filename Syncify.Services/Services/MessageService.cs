using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Messages;
using Syncify.Application.Features.Messages.Commands.SendPrivateMessage;
using Syncify.Application.Helpers;
using Syncify.Application.Hubs;
using Syncify.Application.Hubs.Interfaces;
using Syncify.Application.Interfaces.Services;
using Syncify.Domain.Entities;
using Syncify.Domain.Entities.Identity;
using Syncify.Domain.Enums;
using Syncify.Domain.Interfaces;
using Syncify.Domain.Specifications;
using System.Net;

namespace Syncify.Services.Services;
public sealed class MessageService(
    UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork,
    IFileService fileService,
    IMapper mapper,
    IHubContext<MessageHub, IMessageClient> hubContext) : IMessageService
{
    public async Task<Result<MessageDto>> SendPrivateMessageAsync(SendPrivateMessageCommand command)
    {
        if (string.Equals(command.SenderId, command.ReceiverId))
            return Result<MessageDto>.Failure(HttpStatusCode.Conflict, DomainErrors.Messages.CanNotSendMessagesToSelf);

        var sender = await userManager.FindByIdAsync(command.SenderId);
        var receiver = await userManager.FindByIdAsync(command.ReceiverId);

        if (sender == null || receiver == null)
            return Result<MessageDto>.Failure(HttpStatusCode.NotFound, DomainErrors.Users.UnkownUser);

        var specification = new GetExistedConversationBetweenSenderAndReceiverSpecification(
            command.SenderId,
            command.ReceiverId);

        var conversation = await unitOfWork.Repository<Conversation>()!.GetBySpecificationAndIdAsync(specification, command.ConversationId);

        if (conversation == null)
            return Result<MessageDto>.Failure(HttpStatusCode.NotFound,
                DomainErrors.Conversation.ShouldStartConversation);

        var message = new Message
        {
            Id = Guid.NewGuid(),
            Content = command.Content,
            MessageStatus = MessageStatus.Delivered,
            ConversationId = conversation.Id,
        };

        if (command.Attachments?.Count() > 0)
        {
            var uploadResults = await fileService.UploadFilesParallelAsync(command.Attachments);

            foreach (var result in uploadResults)
            {
                _ = message.Attachments.Select(x =>
                    new Attachment()
                    {
                        Id = Guid.NewGuid(),
                        AttachmentSize = result.Size,
                        MessageId = message.Id,
                        Name = result.SavedFileName,
                        AttachmentType = Enum.Parse<AttachmentType>(result.Type.ToString()),
                        Url = result.Url
                    });
            }
        }

        unitOfWork.Repository<Message>()?.Create(message);

        await unitOfWork.SaveChangesAsync();

        var messageResult = new MessageDto(
            string.Concat(sender.FirstName, " ", sender.LastName),
            string.Concat(receiver.FirstName, " ", receiver.LastName),
            MessageStatus.Delivered,
            message.Content,
            message.Attachments?.Count > 0 ? mapper.Map<List<AttachmentDto>>(message.Attachments) : []);

        await hubContext.Clients.User(command.ReceiverId).ReceivePrivateMessage(messageResult);

        return Result<MessageDto>.Success(messageResult);
    }
}
