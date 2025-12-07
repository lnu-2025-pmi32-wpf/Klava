namespace Klava.Infrastructure.Services;

using Klava.Infrastructure.Interfaces;

public class FileStorageService : IFileStorageService
{
    private readonly string _storagePath;

    public FileStorageService(string storagePath)
    {
        _storagePath = storagePath;
        
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName)
    {
        var storageName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var filePath = Path.Combine(_storagePath, storageName);

        using (var fileStreamOutput = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            await fileStream.CopyToAsync(fileStreamOutput);
        }

        return storageName;
    }

    public async Task<Stream?> GetFileAsync(string storageName)
    {
        var filePath = Path.Combine(_storagePath, storageName);

        if (!File.Exists(filePath))
        {
            return null;
        }

        var memoryStream = new MemoryStream();
        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            await fileStream.CopyToAsync(memoryStream);
        }
        
        memoryStream.Position = 0;
        return memoryStream;
    }

    public Task<bool> DeleteFileAsync(string storageName)
    {
        var filePath = Path.Combine(_storagePath, storageName);

        if (!File.Exists(filePath))
        {
            return Task.FromResult(false);
        }

        try
        {
            File.Delete(filePath);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<bool> FileExistsAsync(string storageName)
    {
        var filePath = Path.Combine(_storagePath, storageName);
        return Task.FromResult(File.Exists(filePath));
    }
}
