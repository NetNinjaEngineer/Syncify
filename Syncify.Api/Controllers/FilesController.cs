using Microsoft.AspNetCore.Mvc;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Api.Controllers;
[Route("api/files")]
[ApiController]
public class FilesController(IFileService fileService) : ControllerBase
{
    [HttpPost("upload-multiple-files")]
    public async Task<IActionResult> UploadMultipleWithParallelAsync(
        string? folderName,
        [FromForm] List<IFormFile> files)
    {
        var results = await fileService.UploadFilesParallelAsync(files, folderName);
        return Ok(results);
    }
}
