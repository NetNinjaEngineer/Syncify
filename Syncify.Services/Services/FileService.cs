using Microsoft.AspNetCore.Http;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Services.Services;
public sealed class FileService : IFileService
{
    public async Task<(bool uploaded, string fileName)> UploadFileAsync(IFormFile? file)
    {
        if (file is null || file.Length == 0)
            return (false, string.Empty); // return default file path

        var uniqueFileName = $"{DateTimeOffset.Now:yyyyMMdd_HHmmssfff}_{file.FileName}"; // 20241127_153025123_example.jpg

        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Images");

        if (!File.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        var filePathToCreate = Path.Combine(directoryPath, uniqueFileName);

        await using var fileStream = new FileStream(filePathToCreate, FileMode.CreateNew);
        await file.CopyToAsync(fileStream);

        return (true, uniqueFileName);
    }
}
