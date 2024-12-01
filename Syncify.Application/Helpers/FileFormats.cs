namespace Syncify.Application.Helpers;
public static class FileFormats
{
    public readonly static List<string> AllowedImageFormats = [".jpg", ".jpeg", ".png", ".gif"];
    public readonly static List<string> AllowedVideoFormats = [".mp4", ".avi", ".mov", ".mkv"];
    public readonly static List<string> AllowedAudioFormats = [".mp3", ".wav", ".aac", ".flac"];
    public readonly static List<string> AllowedDocumentFormats = [".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx"];
    public readonly static List<string> AllowedTextFormats = [".txt", ".rtf", ".html", ".xml", ".json"];
}
