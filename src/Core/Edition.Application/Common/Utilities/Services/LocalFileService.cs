using Microsoft.AspNetCore.Http;
using Edition.Application.Common.Utilities.Contracts;

namespace Edition.Application.Common.Utilities.Services;

public class LocalFileService : ILocalFileService
{
    public void DeleteDirectory(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
            Directory.Delete(directoryPath, true);
    }

    public void DeleteFile(string path, string fileName)
    {
        var filePath = Path
            .Combine(Directory
            .GetCurrentDirectory(), path, fileName);

        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    public void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    public async Task SaveFile(IFormFile file, string directoryPath)
    {
        if (file is null)
            throw new InvalidDataException("file is Null");

        var fileName = file.FileName;
        var folderName = Path
            .Combine(Directory
            .GetCurrentDirectory(), directoryPath.Replace("/", "\\"));

        if (!Directory.Exists(folderName))
            Directory.CreateDirectory(folderName);

        var path = Path.Combine(folderName, fileName);
        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);
    }
    public async Task SaveFileAsync(IFormFile file, string directoryPath, string fileName)
    {
        if (file is null)
            throw new InvalidDataException("file is Null");

        var folderName = Path.Combine(Directory.GetCurrentDirectory(), directoryPath.Replace("/", "\\"));
        if (!Directory.Exists(folderName))
            Directory.CreateDirectory(folderName);

        var path = Path.Combine(folderName, fileName);
        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);
    }
    public async Task<string> SaveFileAsync(IFormFile file, string directoryPath)
    {
        if (file is null)
            throw new InvalidDataException("file is Null");

        var fileName = file.FileName;
        fileName = $"{Guid.NewGuid()}{DateTime.Now.TimeOfDay}"
            .Replace(":", "")
            .Replace(".", "") + Path.GetExtension(fileName);

        var folderName = Path
            .Combine(Directory
            .GetCurrentDirectory(), directoryPath.Replace("/", "\\"));

        if (!Directory.Exists(folderName))
            Directory.CreateDirectory(folderName);

        var path = Path.Combine(folderName, fileName);
        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);
        return fileName;
    }
}