using AutoMapper;
using Syncify.Application.DTOs.Stories;
using Syncify.Application.Features.Stories.Commands.CreateStory;
using Syncify.Application.Resolvers;
using Syncify.Domain.Entities;

namespace Syncify.Application.Mapping;
public sealed class StoryProfile : Profile
{
    public StoryProfile()
    {
        CreateMap<CreateStoryCommand, Story>();
        CreateMap<Story, StoryDto>()
            .ForMember(dest => dest.MediaType, opt => opt.MapFrom(src => src.MediaType.ToString()))
            .ForMember(dest => dest.MediaUrl, opt => opt.MapFrom<StoryMediaUrlValueResolver>());
    }
}
