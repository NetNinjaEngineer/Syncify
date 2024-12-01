using Syncify.Domain.Entities;

namespace Syncify.Domain.Specifications;
public sealed class GetExistedConversationBetweenSenderAndReceiverSpecification(
    string senderId,
    string receiverId) : BaseSpecification<Conversation>(c =>
        c.SenderUserId == senderId &&
        c.ReceiverUserId == receiverId);