namespace Klava.Application.Services.Implementations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Klava.Application.Services.Interfaces;
using Klava.Domain.Entities;
using Klava.Domain.Enums;
using Klava.Infrastructure.Data;

public class SubjectService : ISubjectService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SubjectService> _logger;
    
    public SubjectService(AppDbContext context, ILogger<SubjectService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<Subject> CreateSubjectAsync(int teamId, string title, string? subjectInfo, SubjectStatus status)
    {
        _logger.LogInformation("Creating subject {Title} for team {TeamId}", title, teamId);
        
        try
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
            
            _logger.LogInformation("Subject created: {Title} (ID: {SubjectId}) for team {TeamId}", title, subject.Id, teamId);
            return subject;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create subject {Title} for team {TeamId}", title, teamId);
            throw;
        }
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
        _logger.LogInformation("Updating subject {SubjectId}", subjectId);
        
        var subject = await _context.Subjects.FindAsync(subjectId);
        if (subject == null)
        {
            _logger.LogWarning("Update subject failed: Subject {SubjectId} not found", subjectId);
            return false;
        }
        
        subject.Title = title;
        subject.SubjectInfo = subjectInfo;
        subject.Status = status;
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Subject updated: {SubjectId} ({Title})", subjectId, title);
        return true;
    }
    
    public async Task<bool> DeleteSubjectAsync(int subjectId)
    {
        _logger.LogInformation("Deleting subject {SubjectId}", subjectId);
        
        var subject = await _context.Subjects.FindAsync(subjectId);
        if (subject == null)
        {
            _logger.LogWarning("Delete subject failed: Subject {SubjectId} not found", subjectId);
            return false;
        }
        
        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Subject deleted: {SubjectId}", subjectId);
        return true;
    }
}
