using AutoMapper;
using Syncify.Application.DTOs.Messages;
using Syncify.Domain.Entities;

namespace Syncify.Application.Mapping;
public sealed class MessageProfile : Profile
{
    public MessageProfile()
    {
        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.SenderName, options => options.MapFrom(
                src => string.Concat(src.Conversation.SenderUser.FirstName, " ", src.Conversation.SenderUser.LastName)))
            .ForMember(dest => dest.ReceiverName, options => options.MapFrom(
                src => string.Concat(src.Conversation.ReceiverUser.FirstName, " ", src.Conversation.ReceiverUser.LastName)));
    }
}
