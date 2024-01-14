using Microsoft.AspNetCore.Http;

namespace Edition.Application.Common.Utilities;

public static class FileValidation
{
    public static bool IsValidFile(this IFormFile file)
    {
        if (file is null) return false;

        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        string[] validExtensions = 
            [".bmp", ".wmf", ".gif", ".log", ".jpg",
            ".png", ".tif", ".wmv", ".ppt", ".pptx", 
            ".gif", ".xls", ".xla", ".xlsx", ".doc", 
            ".pdf", ".txt", ".ogg", ".mp4", ".mp3", 
            ".zip", ".rar", ".wav", ".docx", ".mmf", ".m4a"];
        return validExtensions.Contains(fileExtension);
    }
    public static bool IsValidCompressFile(this IFormFile file)
    {
        if (file is null) return false;

        var path = Path.GetExtension(file.FileName).ToLower();        
        return path is ".zip" or ".rar";
    }
    public static bool IsValidMp4File(this IFormFile file)
    {
        if (file is null) return false;

        var path = Path.GetExtension(file.FileName).ToLower();        
        return path is ".mp4";
    }
    public static bool IsValidImageFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return false;

        var fileExtension = Path.GetExtension(fileName).ToLower();
        string[] validExtensions = [".jpg", ".png", ".bmp", ".svg", ".jpeg", ".webp"];
        return validExtensions.Contains(fileExtension);
    }
}