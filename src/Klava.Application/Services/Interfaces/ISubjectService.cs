namespace Klava.Application.Services.Interfaces;

using Klava.Domain.Entities;
using Klava.Domain.Enums;

public interface ISubjectService
{
    Task<Subject> CreateSubjectAsync(int teamId, string title, string? subjectInfo, SubjectStatus status);
    Task<Subject?> GetSubjectByIdAsync(int subjectId);
    Task<List<Subject>> GetSubjectsByTeamAsync(int teamId);
    Task<bool> UpdateSubjectAsync(int subjectId, string title, string? subjectInfo, SubjectStatus status);
    Task<bool> DeleteSubjectAsync(int subjectId);
}
