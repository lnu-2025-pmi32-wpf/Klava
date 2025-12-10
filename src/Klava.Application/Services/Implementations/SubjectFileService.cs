namespace Klava.Application.Services.Implementations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Klava.Application.Services.Interfaces;
using Klava.Application.DTOs;
using Klava.Domain.Entities;
using Klava.Infrastructure.Data;
using Klava.Infrastructure.Interfaces;

public class SubjectFileService : ISubjectFileService
{
    private readonly AppDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<SubjectFileService> _logger;

    public SubjectFileService(AppDbContext context, IFileStorageService fileStorage, ILogger<SubjectFileService> logger)
    {
        _context = context;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    public async Task<SubjectFile?> UploadFileAsync(UploadFileRequest request)
    {
        _logger.LogInformation("Uploading file {FileName} for subject {SubjectId}", request.FileName, request.SubjectId);
        
        try
        {
            var storageName = await _fileStorage.SaveFileAsync(
                request.FileStream, 
                request.FileName, 
                request.SubjectId);

            var subjectFile = new SubjectFile
            {
                SubjectId = request.SubjectId,
                DisplayName = request.FileName,
                StorageName = storageName,
                ContentType = request.ContentType,
                Size = request.Size,
                UploadedAt = DateTime.UtcNow
            };

            _context.SubjectFiles.Add(subjectFile);
            await _context.SaveChangesAsync();

            _logger.LogInformation("File uploaded: {FileName} (ID: {FileId}) for subject {SubjectId}", request.FileName, subjectFile.Id, request.SubjectId);
            return subjectFile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file {FileName} for subject {SubjectId}", request.FileName, request.SubjectId);
            return null;
        }
    }

    public async Task<SubjectFile?> GetFileByIdAsync(int fileId)
    {
        return await _context.SubjectFiles
            .Include(f => f.Subject)
            .FirstOrDefaultAsync(f => f.Id == fileId);
    }

    public async Task<List<SubjectFile>> GetFilesBySubjectAsync(int subjectId)
    {
        return await _context.SubjectFiles
            .Where(f => f.SubjectId == subjectId)
            .OrderByDescending(f => f.UploadedAt)
            .ToListAsync();
    }

    public async Task<Stream?> DownloadFileAsync(int fileId)
    {
        _logger.LogInformation("Downloading file {FileId}", fileId);
        
        var file = await GetFileByIdAsync(fileId);
        
        if (file == null)
        {
            _logger.LogWarning("Download file failed: File {FileId} not found", fileId);
            return null;
        }

        return await _fileStorage.GetFileAsync(file.StorageName, file.SubjectId);
    }

    public async Task<bool> DeleteFileAsync(int fileId)
    {
        _logger.LogInformation("Deleting file {FileId}", fileId);
        
        var file = await _context.SubjectFiles.FindAsync(fileId);
        
        if (file == null)
        {
            _logger.LogWarning("Delete file failed: File {FileId} not found", fileId);
            return false;
        }

        var fileDeleted = await _fileStorage.DeleteFileAsync(file.StorageName, file.SubjectId);
        
        if (!fileDeleted)
        {
            _logger.LogError("Failed to delete file from storage: {FileId}", fileId);
            return false;
        }

        _context.SubjectFiles.Remove(file);
        await _context.SaveChangesAsync();

        _logger.LogInformation("File deleted: {FileId}", fileId);
        return true;
    }
}
