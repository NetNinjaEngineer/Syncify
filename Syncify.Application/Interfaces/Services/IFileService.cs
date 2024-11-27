using Microsoft.AspNetCore.Http;

namespace Syncify.Application.Interfaces.Services;
public interface IFileService
{
    Task<(bool uploaded, string fileName)> UploadFileAsync(IFormFile? file);
}
