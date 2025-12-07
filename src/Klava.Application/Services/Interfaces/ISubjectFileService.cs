namespace Klava.Application.Services.Interfaces;

using Klava.Application.DTOs;
using Klava.Domain.Entities;

public interface ISubjectFileService
{
    Task<SubjectFile?> UploadFileAsync(UploadFileRequest request);
    Task<SubjectFile?> GetFileByIdAsync(int fileId);
    Task<List<SubjectFile>> GetFilesBySubjectAsync(int subjectId);
    Task<Stream?> DownloadFileAsync(int fileId);
    Task<bool> DeleteFileAsync(int fileId);
}
