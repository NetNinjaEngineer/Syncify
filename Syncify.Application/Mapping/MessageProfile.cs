using AutoMapper;
using Syncify.Application.DTOs.Messages;
using Syncify.Domain.Entities;

namespace Syncify.Application.Mapping;
public sealed class MessageProfile : Profile
{
    public MessageProfile()
    {
        CreateMap<Attachment, AttachmentDto>()
            .ForMember(dest => dest.Size, options => options.MapFrom(src => src.AttachmentSize))
            .ForMember(dest => dest.Type, options => options.MapFrom(src => src.AttachmentType));
    }
}
