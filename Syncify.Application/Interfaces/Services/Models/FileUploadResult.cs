namespace Syncify.Application.Interfaces.Services.Models;
public class FileUploadResult
{
    public long Size { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string SavedFileName { get; set; } = string.Empty;
    public FileType Type { get; set; }
    public string Url { get; set; } = string.Empty;
}
