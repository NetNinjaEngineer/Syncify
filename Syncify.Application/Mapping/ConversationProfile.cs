using AutoMapper;
using Syncify.Application.DTOs.Conversation;
using Syncify.Domain.Entities;

namespace Syncify.Application.Mapping;
public sealed class ConversationProfile : Profile
{
    public ConversationProfile()
    {
        CreateMap<Conversation, ConversationDto>()
            .ForMember(dest => dest.Sender, options => options.MapFrom(src => string.Concat(src.SenderUser.FirstName, " ", src.SenderUser.LastName)))
            .ForMember(dest => dest.Receiver, options => options.MapFrom(src => string.Concat(src.ReceiverUser.FirstName, " ", src.ReceiverUser.LastName)));

    }
}
