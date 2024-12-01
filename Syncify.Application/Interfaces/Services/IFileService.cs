using Microsoft.AspNetCore.Http;
using Syncify.Application.Interfaces.Services.Models;

namespace Syncify.Application.Interfaces.Services;
public interface IFileService
{
    Task<(bool uploaded, string fileName)> UploadFileAsync(IFormFile? file, string folderName);
    Task<IEnumerable<FileUploadResult>> UploadFilesParallelAsync(
        IEnumerable<IFormFile> files,
        string? folderName = null, CancellationToken cancellationToken = default);
}
