using AutoMapper;
using Syncify.Application.DTOs.FriendshipRequests;
using Syncify.Domain.Entities;

namespace Syncify.Application.Mapping;
public sealed class FriendshipsProfile : Profile
{
    public FriendshipsProfile()
    {
        CreateMap<Friendship, PendingFriendshipRequest>()
            .ForMember(dest => dest.ReceiverName,
                options => options.MapFrom(
                    src => string.Concat(src.Receiver.FirstName, " ", src.Receiver.LastName)))
            .ForMember(dest => dest.RequesterName,
                options => options.MapFrom(
                    src => string.Concat(src.Requester.FirstName, " ", src.Requester.LastName)))
            .ForMember(dest => dest.RequestedAt, options => options.MapFrom(src => src.CreatedAt));

        CreateMap<Friendship, GetUserAcceptedFriendshipDto>()
            .ForMember(dest => dest.FriendName, options =>
                options.MapFrom(src => string.Concat(src.Requester.FirstName, " ", src.Requester.LastName)))
            .ForMember(dest => dest.AcceptedAt, options =>
                options.MapFrom(src => src.CreatedAt));
    }
}
