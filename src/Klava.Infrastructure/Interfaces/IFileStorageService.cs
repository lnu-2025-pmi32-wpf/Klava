namespace Klava.Infrastructure.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, int? subjectId = null);
    Task<Stream?> GetFileAsync(string storageName, int? subjectId = null);
    Task<bool> DeleteFileAsync(string storageName, int? subjectId = null);
    Task<bool> FileExistsAsync(string storageName, int? subjectId = null);
}
