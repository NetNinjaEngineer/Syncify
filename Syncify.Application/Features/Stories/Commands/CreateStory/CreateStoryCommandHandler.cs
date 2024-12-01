using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Stories;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Stories.Commands.CreateStory;
public sealed class CreateStoryCommandHandler
    (IStoryService storyService) : IRequestHandler<CreateStoryCommand, Result<StoryDto>>
{
    public async Task<Result<StoryDto>> Handle(CreateStoryCommand request, CancellationToken cancellationToken)
        => await storyService.CreateStoryAsync(request);
}
