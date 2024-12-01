using MediatR;
using Microsoft.AspNetCore.Http;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Stories;
using Syncify.Domain.Enums;

namespace Syncify.Application.Features.Stories.Commands.CreateStory;
public sealed class CreateStoryCommand : IRequest<Result<StoryDto>>
{
    public IFormFile Media { get; set; } = null!;

    public MediaType MediaType { get; set; }

    public string? Caption { get; set; }

    public List<string> HashTags { get; set; } = [];
}
