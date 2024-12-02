using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Conversation;
using Syncify.Application.DTOs.Messages;
using Syncify.Application.Features.Conversations.Queries.GetPagedConversationMessages;
using Syncify.Application.Features.Messages.Commands.SendPrivateMessage;
using Syncify.Application.Features.Messages.Commands.SendPrivateMessageByCurrentUser;
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
    ICurrentUser currentUser,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IMessageRepository messageRepository) : IMessageService
{
    public Task<Result<bool>> DeleteMessageAsync(Guid messageId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> EditMessageAsync(Guid messageId, string newContent)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<ConversationDto>> GetConversationMessagesAsync(GetPagedConversationMessagesQuery query)
    {
        var pagedConversationMessages = await messageRepository.GetPagedConversationMessagesAsync(
            query.ConversationId, query.PageNumber, query.PageSize);
        if (pagedConversationMessages == null)
            return Result<ConversationDto>.Failure(HttpStatusCode.NotFound);
        var mappedConversationMessages = mapper.Map<ConversationDto>(pagedConversationMessages);
        return Result<ConversationDto>.Success(mappedConversationMessages);
    }

    public async Task<Result<ConversationDto>> GetConversationMessagesAsync(Guid conversationId)
    {
        var existedConversation = await messageRepository.GetConversationMessagesAsync(conversationId);

        if (existedConversation == null)
            return Result<ConversationDto>.Failure(HttpStatusCode.NotFound);

        var mappedConversation = mapper.Map<ConversationDto>(existedConversation);
        return Result<ConversationDto>.Success(mappedConversation);
    }

    public async Task<Result<MessageDto>> GetMessageByIdAsync(Guid messageId)
    {
        var specification = new GetExistedMessageSpecification(messageId);
        var existedMessage = await unitOfWork.Repository<Message>()!.GetBySpecificationAsync(specification);

        if (existedMessage == null)
            return Result<MessageDto>.Failure(HttpStatusCode.NotFound);

        var mappedMessage = mapper.Map<MessageDto>(existedMessage);
        return Result<MessageDto>.Success(mappedMessage);
    }

    public Task<Result<IEnumerable<MessageDto>>> GetMessagesByDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate, Guid? conversationId = null)
    {
        throw new NotImplementedException();
    }

    public Task<Result<int>> GetUnreadMessageCountAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ConversationDto>> GetUserConversationsAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> MarkMessageAsReadAsync(Guid messageId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<MessageDto>>> SearchMessagesAsync(string searchTerm, Guid? conversationId = null)
    {
        throw new NotImplementedException();
    }

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
                message.Attachments.Add(new Attachment
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

        conversation.LastMessageAt = message.CreatedAt;

        unitOfWork.Repository<Conversation>()?.Update(conversation);

        await unitOfWork.SaveChangesAsync();

        var messageResult = MessageDto.Create(
            string.Concat(sender.FirstName, " ", sender.LastName),
            string.Concat(receiver.FirstName, " ", receiver.LastName),
            MessageStatus.Delivered,
            message.Content,
            message.Attachments?.Count > 0 ? mapper.Map<List<AttachmentDto>>(message.Attachments) : [],
            message.CreatedAt, null);

        await hubContext.Clients.User(command.ReceiverId).ReceivePrivateMessage(messageResult);

        return Result<MessageDto>.Success(messageResult);
    }

    public async Task<Result<MessageDto>> SendPrivateMessageByCurrentUserAsync(SendPrivateMessageByCurrentUserCommand command)
    {
        if (string.Equals(currentUser.Id, command.ReceiverId))
            return Result<MessageDto>.Failure(HttpStatusCode.Conflict, DomainErrors.Messages.CanNotSendMessagesToSelf);

        var sender = await userManager.FindByIdAsync(currentUser.Id);
        var receiver = await userManager.FindByIdAsync(command.ReceiverId);

        if (sender == null || receiver == null)
            return Result<MessageDto>.Failure(HttpStatusCode.NotFound, DomainErrors.Users.UnkownUser);

        var specification = new GetExistedConversationBetweenSenderAndReceiverSpecification(
            currentUser.Id,
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
                message.Attachments.Add(new Attachment
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

        conversation.LastMessageAt = message.CreatedAt;

        unitOfWork.Repository<Conversation>()?.Update(conversation);

        await unitOfWork.SaveChangesAsync();

        var messageResult = MessageDto.Create(
            string.Concat(sender.FirstName, " ", sender.LastName),
            string.Concat(receiver.FirstName, " ", receiver.LastName),
            MessageStatus.Delivered,
            message.Content,
            message.Attachments?.Count > 0 ? mapper.Map<List<AttachmentDto>>(message.Attachments) : [],
            message.CreatedAt, null);

        await hubContext.Clients.User(command.ReceiverId).ReceivePrivateMessage(messageResult);

        return Result<MessageDto>.Success(messageResult);
    }
}
