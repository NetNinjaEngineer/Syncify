using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Conversation;
using Syncify.Application.DTOs.Messages;
using Syncify.Application.Features.Conversations.Queries.GetPagedConversationMessages;
using Syncify.Application.Features.Conversations.Queries.GetUserConversation;
using Syncify.Application.Features.Messages.Commands.DeleteMessageInConversation;
using Syncify.Application.Features.Messages.Commands.MarkMessageAsRead;
using Syncify.Application.Features.Messages.Commands.SendPrivateMessage;
using Syncify.Application.Features.Messages.Commands.SendPrivateMessageByCurrentUser;
using Syncify.Application.Features.Messages.Queries.GetMessagesByDateRange;
using Syncify.Application.Features.Messages.Queries.GetUnreadMessages;
using Syncify.Application.Features.Messages.Queries.GetUnreadMessagesCount;
using Syncify.Application.Features.Messages.Queries.SearchMessages;
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
    IHubContext<MessageHub, IMessageClient> hubContext) : IMessageService
{
    public async Task<Result<bool>> DeleteMessageAsync(Guid messageId)
    {
        var existedMessage = await unitOfWork.MessageRepository.GetByIdAsync(messageId);
        if (existedMessage == null)
            return Result<bool>.Failure(HttpStatusCode.NotFound);
        unitOfWork.MessageRepository.Delete(existedMessage);
        await unitOfWork.SaveChangesAsync();
        return Result<bool>.Success(true, AppConstants.Messages.MessageDeleted);
    }

    public async Task<Result<bool>> DeleteMessageInConversationAsync(DeleteMessageInConversationCommand command)
    {
        var existedConversation = await unitOfWork.ConversationRepository.GetByIdAsync(command.ConversationId);
        if (existedConversation != null)
        {
            var specification = new GetExistedMessageSpecification(command.MessageId, command.ConversationId);
            var existedMessage = await unitOfWork.MessageRepository.GetBySpecificationAsync(specification);
            if (existedMessage == null)
                return Result<bool>.Failure(HttpStatusCode.NotFound, DomainErrors.Messages.MessageNotFound);
            unitOfWork.MessageRepository.Delete(existedMessage);
            await unitOfWork.SaveChangesAsync();
            return Result<bool>.Success(true, AppConstants.Messages.MessageDeleted);
        }

        return Result<bool>.Failure(HttpStatusCode.NotFound,
            string.Format(DomainErrors.Conversation.ConversationNotExisted, command.ConversationId.ToString()));
    }

    public async Task<Result<bool>> EditMessageAsync(Guid messageId, string newContent)
    {
        var existedMessage = await unitOfWork.MessageRepository.GetByIdAsync(messageId);
        if (existedMessage == null)
            return Result<bool>.Failure(HttpStatusCode.NotFound);
        existedMessage.Content = newContent;
        existedMessage.UpdatedAt = DateTimeOffset.Now;
        unitOfWork.MessageRepository.Update(existedMessage);
        await unitOfWork.SaveChangesAsync();
        return Result<bool>.Success(true, AppConstants.Messages.MessageUpdated);
    }

    public async Task<Result<ConversationDto>> GetConversationMessagesAsync(GetPagedConversationMessagesQuery query)
    {
        var pagedConversationMessages = await unitOfWork.MessageRepository.GetPagedConversationMessagesAsync(
            query.ConversationId, query.PageNumber, query.PageSize);
        if (pagedConversationMessages == null)
            return Result<ConversationDto>.Failure(HttpStatusCode.NotFound);
        var mappedConversationMessages = mapper.Map<ConversationDto>(pagedConversationMessages);
        return Result<ConversationDto>.Success(mappedConversationMessages);
    }

    public async Task<Result<ConversationDto>> GetConversationMessagesAsync(Guid conversationId)
    {
        var existedConversation = await unitOfWork.MessageRepository.GetConversationMessagesAsync(conversationId);

        if (existedConversation == null)
            return Result<ConversationDto>.Failure(HttpStatusCode.NotFound);

        var mappedConversation = mapper.Map<ConversationDto>(existedConversation);
        return Result<ConversationDto>.Success(mappedConversation);
    }

    public async Task<Result<MessageDto>> GetMessageByIdAsync(Guid messageId)
    {
        var specification = new GetExistedMessageSpecification(messageId);
        var existedMessage = await unitOfWork.MessageRepository.GetBySpecificationAsync(specification);

        if (existedMessage == null)
            return Result<MessageDto>.Failure(HttpStatusCode.NotFound);

        var mappedMessage = mapper.Map<MessageDto>(existedMessage);
        return Result<MessageDto>.Success(mappedMessage);
    }

    public async Task<Result<IEnumerable<MessageDto>>> GetMessagesByDateRangeAsync(GetMessagesByDateRangeQuery query)
    {
        var messagesInRange = await unitOfWork.MessageRepository.GetMessagesByDateRangeAsync(
            query.StartDate, query.EndDate, query.ConversationId);

        var mappedMessages = mapper.Map<IEnumerable<MessageDto>>(messagesInRange);

        return Result<IEnumerable<MessageDto>>.Success(mappedMessages);
    }

    public async Task<Result<int>> GetUnreadMessageCountAsync(GetUnreadMessagesCountQuery query)
    {
        var validator = new GetUnreadMessagesCountQueryValidator();
        await validator.ValidateAndThrowAsync(query);

        return Result<int>.Success(await unitOfWork.MessageRepository.GetUnreadMessagesCountAsync(query.UserId));
    }

    public async Task<Result<IEnumerable<MessageDto>>> GetUnreadMessagesAsync(GetUnreadMessagesQuery query)
    {
        var validator = new GetUnreadMessagesQueryValidator();
        await validator.ValidateAndThrowAsync(query);

        var mappedMessages = mapper.Map<IEnumerable<MessageDto>>(
            await unitOfWork.MessageRepository.GetUnreadMessagesAsync(query.UserId));

        return Result<IEnumerable<MessageDto>>.Success(mappedMessages);
    }

    public async Task<Result<ConversationDto>> GetUserConversationsAsync(GetUserConversationQuery query)
    {
        var existedUser = await userManager.FindByIdAsync(query.UserId);
        if (existedUser == null)
            return Result<ConversationDto>.Failure(HttpStatusCode.NotFound, DomainErrors.Users.UnkownUser);
        var conversation = await unitOfWork.ConversationRepository.GetUserConversationsAsync(query.UserId);
        if (conversation == null)
            return Result<ConversationDto>.Failure(HttpStatusCode.NotFound);
        var mappedConversation = mapper.Map<ConversationDto>(conversation);
        return Result<ConversationDto>.Success(mappedConversation);
    }

    public async Task<Result<bool>> MarkMessageAsReadAsync(MarkMessageAsReadCommand command)
    {
        var existedMessage = await unitOfWork.MessageRepository.GetByIdAsync(command.MessageId);
        if (existedMessage is not null)
        {
            existedMessage.MessageStatus = MessageStatus.Read;
            existedMessage.UpdatedAt = DateTimeOffset.Now;
            unitOfWork.MessageRepository.Update(existedMessage);
            await unitOfWork.SaveChangesAsync();
            return Result<bool>.Success(true, successMessage: AppConstants.Messages.MessageStatusUpdated);
        }

        return Result<bool>.Failure(HttpStatusCode.NotFound, DomainErrors.Messages.MessageNotFound);
    }

    public async Task<Result<IEnumerable<MessageDto>>> SearchMessagesAsync(SearchMessagesQuery searchMessagesQuery)
    {
        var searchedMessages = await unitOfWork.MessageRepository.SearchMessagesAsync(
            searchTerm: searchMessagesQuery.SearchTerm,
            conversationId: searchMessagesQuery.ConversationId);

        var mappedResult = mapper.Map<IEnumerable<MessageDto>>(searchedMessages);
        return Result<IEnumerable<MessageDto>>.Success(mappedResult);
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
            MessageStatus = MessageStatus.Sent,
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
            MessageStatus.Sent,
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
            MessageStatus = MessageStatus.Sent,
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
            MessageStatus.Sent,
            message.Content,
            message.Attachments?.Count > 0 ? mapper.Map<List<AttachmentDto>>(message.Attachments) : [],
            message.CreatedAt, null);

        await hubContext.Clients.User(command.ReceiverId).ReceivePrivateMessage(messageResult);

        return Result<MessageDto>.Success(messageResult);
    }
}
