namespace Klava.Application.Services.Implementations;

using Microsoft.EntityFrameworkCore;
using Klava.Application.Services.Interfaces;
using Klava.Application.DTOs;
using Klava.Domain.Entities;
using Klava.Infrastructure.Data;
using Klava.Infrastructure.Interfaces;

public class SubjectFileService : ISubjectFileService
{
    private readonly AppDbContext _context;
    private readonly IFileStorageService _fileStorage;

    public SubjectFileService(AppDbContext context, IFileStorageService fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<SubjectFile?> UploadFileAsync(UploadFileRequest request)
    {
        try
        {
            // Save file in subject-specific folder
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

            return subjectFile;
        }
        catch
        {
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
        var file = await GetFileByIdAsync(fileId);
        
        if (file == null)
            return null;

        // Get file from subject-specific folder
        return await _fileStorage.GetFileAsync(file.StorageName, file.SubjectId);
    }

    public async Task<bool> DeleteFileAsync(int fileId)
    {
        var file = await _context.SubjectFiles.FindAsync(fileId);
        
        if (file == null)
            return false;

        // Delete file from subject-specific folder
        var fileDeleted = await _fileStorage.DeleteFileAsync(file.StorageName, file.SubjectId);
        
        if (!fileDeleted)
            return false;

        _context.SubjectFiles.Remove(file);
        await _context.SaveChangesAsync();

        return true;
    }
}
