namespace Klava.Application.Services.Implementations;

using Microsoft.EntityFrameworkCore;
using Klava.Application.Services.Interfaces;
using Klava.Application.DTOs;
using Klava.Domain.Entities;
using Klava.Domain.Enums;
using Klava.Infrastructure.Data;

public class SubmissionService : ISubmissionService
{
    private readonly AppDbContext _context;

    public SubmissionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ToggleStatusAsync(int taskId, int userId)
    {
        var submission = await _context.Submissions
            .FirstOrDefaultAsync(s => s.TaskId == taskId && s.UserId == userId);

        if (submission == null)
        {
            // Create new submission with Done status
            submission = new Submission
            {
                TaskId = taskId,
                UserId = userId,
                Status = SubmissionStatus.Done,
                SubmittedAt = DateTime.UtcNow
            };
            _context.Submissions.Add(submission);
        }
        else
        {
            // Toggle between Wait and Done
            submission.Status = submission.Status == SubmissionStatus.Wait 
                ? SubmissionStatus.Done 
                : SubmissionStatus.Wait;
            submission.SubmittedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<TaskWithUserStatusDto>> GetTeamTasksWithStatusAsync(int teamId, int userId)
    {
        var tasks = await _context.Tasks
            .Include(t => t.Subject)
            .Where(t => t.Subject.TeamId == teamId)
            .GroupJoin(
                _context.Submissions.Where(s => s.UserId == userId),
                task => task.Id,
                submission => submission.TaskId,
                (task, submissions) => new { task, submission = submissions.FirstOrDefault() })
            .Select(x => new TaskWithUserStatusDto
            {
                Id = x.task.Id,
                SubjectId = x.task.SubjectId,
                SubjectTitle = x.task.Subject.Title,
                Name = x.task.Name,
                Description = x.task.Description,
                Deadline = x.task.Deadline,
                CurrentUserStatus = x.submission != null ? x.submission.Status : (SubmissionStatus?)null,
                SubmittedAt = x.submission != null ? x.submission.SubmittedAt : (DateTime?)null
            })
            .OrderBy(t => t.Deadline)
            .ToListAsync();

        return tasks;
    }

    public async Task<SubmissionStatus?> GetUserTaskStatusAsync(int taskId, int userId)
    {
        var submission = await _context.Submissions
            .FirstOrDefaultAsync(s => s.TaskId == taskId && s.UserId == userId);

        return submission?.Status;
    }
}
