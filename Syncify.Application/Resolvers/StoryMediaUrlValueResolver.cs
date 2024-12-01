using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Syncify.Application.DTOs.Stories;
using Syncify.Domain.Entities;
using Syncify.Domain.Enums;

namespace Syncify.Application.Resolvers;
public sealed class StoryMediaUrlValueResolver(
    IConfiguration configuration,
    IHttpContextAccessor contextAccessor) : IValueResolver<Story, StoryDto, string?>
{
    public string Resolve(Story source, StoryDto destination, string? destMember, ResolutionContext context)
    {
        if (string.IsNullOrEmpty(source.MediaUrl))
            return string.Empty;

        if (source.MediaType is not (MediaType.Image or MediaType.Video))
            return string.Empty;

        var subFolder = source.MediaType == MediaType.Image ? "Images" : "Videos";

        return contextAccessor.HttpContext.Request.IsHttps
            ? $"{configuration["BaseApiUrl"]}/Uploads/Stories/{subFolder}/{source.MediaUrl}"
            : $"{configuration["FullbackUrl"]}/Uploads/Stories/{subFolder}/{source.MediaUrl}";
    }
}
