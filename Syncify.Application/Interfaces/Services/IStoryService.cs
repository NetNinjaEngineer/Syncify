using Syncify.Application.Bases;
using Syncify.Application.DTOs.Stories;
using Syncify.Application.Features.Stories.Commands.CreateStory;

namespace Syncify.Application.Interfaces.Services;
public interface IStoryService
{
    Task<Result<StoryDto>> CreateStoryAsync(CreateStoryCommand command);
}
