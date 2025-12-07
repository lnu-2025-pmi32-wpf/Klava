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

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, int? subjectId = null)
    {
        // Create subject-specific folder if subjectId is provided
        var targetPath = _storagePath;
        if (subjectId.HasValue)
        {
            targetPath = Path.Combine(_storagePath, "subjects", $"subject_{subjectId.Value}");
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
        }

        // Generate unique filename with timestamp and GUID
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var guid = Guid.NewGuid().ToString("N")[..8]; // First 8 chars of GUID
        var extension = Path.GetExtension(fileName);
        var storageName = $"{timestamp}_{guid}{extension}";
        
        var filePath = Path.Combine(targetPath, storageName);

        using (var fileStreamOutput = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            await fileStream.CopyToAsync(fileStreamOutput);
        }

        return storageName;
    }

    public async Task<Stream?> GetFileAsync(string storageName, int? subjectId = null)
    {
        // Try subject-specific folder first if subjectId is provided
        string? filePath = null;
        
        if (subjectId.HasValue)
        {
            filePath = Path.Combine(_storagePath, "subjects", $"subject_{subjectId.Value}", storageName);
        }
        
        // Fallback to root storage path
        if (filePath == null || !File.Exists(filePath))
        {
            filePath = Path.Combine(_storagePath, storageName);
        }

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

    public Task<bool> DeleteFileAsync(string storageName, int? subjectId = null)
    {
        // Try subject-specific folder first if subjectId is provided
        string? filePath = null;
        
        if (subjectId.HasValue)
        {
            filePath = Path.Combine(_storagePath, "subjects", $"subject_{subjectId.Value}", storageName);
        }
        
        // Fallback to root storage path
        if (filePath == null || !File.Exists(filePath))
        {
            filePath = Path.Combine(_storagePath, storageName);
        }

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

    public Task<bool> FileExistsAsync(string storageName, int? subjectId = null)
    {
        // Try subject-specific folder first if subjectId is provided
        string? filePath = null;
        
        if (subjectId.HasValue)
        {
            filePath = Path.Combine(_storagePath, "subjects", $"subject_{subjectId.Value}", storageName);
        }
        
        // Fallback to root storage path
        if (filePath == null || !File.Exists(filePath))
        {
            filePath = Path.Combine(_storagePath, storageName);
        }
        
        return Task.FromResult(File.Exists(filePath));
    }
}
