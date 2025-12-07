namespace Klava.Infrastructure.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName);
    Task<Stream?> GetFileAsync(string storageName);
    Task<bool> DeleteFileAsync(string storageName);
    Task<bool> FileExistsAsync(string storageName);
}
