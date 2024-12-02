using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Syncify.Application.Helpers;
using Syncify.Application.Interfaces.Services;
using Syncify.Application.Interfaces.Services.Models;

namespace Syncify.Services.Services;
public sealed class FileService(
    IConfiguration configuration,
    IHttpContextAccessor contextAccessor) : IFileService
{
    public async Task<(bool uploaded, string fileName)> UploadFileAsync(IFormFile? file, string folderName)
    {
        if (file is null || file.Length == 0)
            return (false, string.Empty); // return default file path

        var uniqueFileName = $"{DateTimeOffset.Now:yyyyMMdd_HHmmssfff}_{file.FileName}"; // 20241127_153025123_example.jpg

        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", folderName);

        if (!File.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        var filePathToCreate = Path.Combine(directoryPath, uniqueFileName);

        await using var fileStream = new FileStream(filePathToCreate, FileMode.CreateNew);
        await file.CopyToAsync(fileStream);

        return (true, uniqueFileName);
    }

    public async Task<IEnumerable<FileUploadResult>> UploadFilesParallelAsync(
      IEnumerable<IFormFile> files,
      string? folderName = null,
      CancellationToken cancellationToken = default)
    {
        var formFiles = files.ToList();

        if (formFiles.Count == 0)
            return [];

        var uploadResults = formFiles.Select(file =>
        {
            return Task.Run(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                string directoryPath = string.Empty;
                string locationPath = string.Empty;

                try
                {
                    if (!string.IsNullOrEmpty(folderName))
                    {
                        directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", folderName);
                        locationPath = $"/Uploads/{folderName}";
                    }
                    else
                    {
                        var isImage = FileFormats.AllowedImageFormats.Contains(fileExtension);
                        var isVideo = FileFormats.AllowedVideoFormats.Contains(fileExtension);
                        if (isImage)
                        {
                            directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Images");
                            locationPath = "Uploads/Images";
                        }
                        else if (isVideo)
                        {
                            directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Videos");
                            locationPath = "Uploads/Videos";
                        }
                        else
                        {
                            directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Other");
                            locationPath = "Uploads/Other";
                        }
                    }

                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    var uniqueFileName = $"{DateTimeOffset.Now:yyyyMMdd_HHmmssfff}_{file.FileName}";
                    var fullPathToCreate = Path.Combine(directoryPath, uniqueFileName);

                    await using var fileStream = new FileStream(fullPathToCreate, FileMode.Create, FileAccess.ReadWrite);
                    await file.CopyToAsync(fileStream, cancellationToken);

                    return new FileUploadResult
                    {
                        OriginalFileName = file.FileName,
                        SavedFileName = uniqueFileName,
                        Size = file.Length,
                        Type = GetFileType(fileExtension),
                        Url = contextAccessor.HttpContext!.Request.IsHttps ?
                            $"{configuration["BaseApiUrl"]}/{locationPath}/{uniqueFileName}" :
                            $"{configuration["FullbackUrl"]}/{locationPath}/{uniqueFileName}"
                    };
                }
                catch (Exception)
                {
                    return new FileUploadResult
                    {
                        OriginalFileName = file.FileName,
                        SavedFileName = string.Empty,
                        Size = 0,
                        Url = string.Empty,
                    };
                }
            });
        });


        return await Task.WhenAll(uploadResults);
    }


    private static FileType GetFileType(string fileExtension)
    {
        if (string.IsNullOrEmpty(fileExtension))
            throw new ArgumentException("File extension cannot be null or empty", nameof(fileExtension));

        if (FileFormats.AllowedVideoFormats.Contains(fileExtension))
            return FileType.Video;

        if (FileFormats.AllowedImageFormats.Contains(fileExtension))
            return FileType.Image;

        if (FileFormats.AllowedDocumentFormats.Contains(fileExtension))
            return FileType.Document;

        if (FileFormats.AllowedTextFormats.Contains(fileExtension))
            return FileType.Text;

        if (FileFormats.AllowedAudioFormats.Contains(fileExtension))
            return FileType.Audio;

        throw new NotSupportedException($"File extension '{fileExtension}' is not supported");
    }
}
