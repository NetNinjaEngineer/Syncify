using AutoMapper;
using FluentValidation;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Stories;
using Syncify.Application.Features.Stories.Commands.CreateStory;
using Syncify.Application.Interfaces.Services;
using Syncify.Domain.Entities;
using Syncify.Domain.Enums;
using Syncify.Domain.Interfaces;

namespace Syncify.Services.Services;
public sealed class StoryService(
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IFileService fileService) : IStoryService
{
    public async Task<Result<StoryDto>> CreateStoryAsync(CreateStoryCommand command)
    {
        var validator = new CreateStoryCommandValidator();
        await validator.ValidateAndThrowAsync(command);

        var subFolder = command.MediaType == MediaType.Image ? "Images" : "Videos";

        var filePath = Path.Combine("Stories", subFolder);
        var (uploaded, fileName) = await fileService.UploadFileAsync(command.Media, filePath);

        var mappedStory = mapper.Map<Story>(command);
        mappedStory.MediaUrl = fileName;
        mappedStory.UserId = currentUser.Id;
        mappedStory.ExpiresAt = DateTimeOffset.Now.AddHours(24);

        unitOfWork.Repository<Story>()?.Create(mappedStory);
        await unitOfWork.SaveChangesAsync();

        return Result<StoryDto>.Success(mapper.Map<StoryDto>(mappedStory));

    }
}
