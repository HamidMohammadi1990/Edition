using Microsoft.AspNetCore.Http;

namespace Edition.Application.Common.Utilities.Contracts;

public interface ILocalFileService
{
    void DeleteDirectory(string directoryPath);
    void DeleteFile(string path, string fileName);
    void DeleteFile(string filePath);
    Task SaveFile(IFormFile file, string directoryPath);
    Task SaveFileAsync(IFormFile file, string directoryPath, string fileName);
    Task<string> SaveFileAsync(IFormFile file, string directoryPath);
}