namespace Klava.Application.Services.Implementations;

using Microsoft.EntityFrameworkCore;
using Klava.Application.Services.Interfaces;
using Klava.Domain.Entities;
using Klava.Domain.Enums;
using Klava.Infrastructure.Data;

public class SubjectService : ISubjectService
{
    private readonly AppDbContext _context;
    
    public SubjectService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Subject> CreateSubjectAsync(int teamId, string title, string? subjectInfo, SubjectStatus status)
    {
        var subject = new Subject
        {
            TeamId = teamId,
            Title = title,
            SubjectInfo = subjectInfo,
            Status = status
        };
        
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();
        
        return subject;
    }
    
    public async Task<Subject?> GetSubjectByIdAsync(int subjectId)
    {
        return await _context.Subjects
            .Include(s => s.Tasks)
            .FirstOrDefaultAsync(s => s.Id == subjectId);
    }
    
    public async Task<List<Subject>> GetSubjectsByTeamAsync(int teamId)
    {
        return await _context.Subjects
            .Where(s => s.TeamId == teamId)
            .Include(s => s.Tasks)
            .ToListAsync();
    }
    
    public async Task<bool> UpdateSubjectAsync(int subjectId, string title, string? subjectInfo, SubjectStatus status)
    {
        var subject = await _context.Subjects.FindAsync(subjectId);
        if (subject == null)
            return false;
        
        subject.Title = title;
        subject.SubjectInfo = subjectInfo;
        subject.Status = status;
        
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> DeleteSubjectAsync(int subjectId)
    {
        var subject = await _context.Subjects.FindAsync(subjectId);
        if (subject == null)
            return false;
        
        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();
        
        return true;
    }
}
