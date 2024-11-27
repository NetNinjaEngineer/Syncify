using AutoMapper;
using Syncify.Application.Features.Auth.Commands.Register;
using Syncify.Domain.Entities.Identity;

namespace Syncify.Application.Mapping;
public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<RegisterCommand, ApplicationUser>();
    }
}
