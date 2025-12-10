namespace Klava.Application.Services.Implementations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Klava.Application.Services.Interfaces;
using Klava.Application.DTOs;
using Klava.Domain.Entities;
using Klava.Domain.Enums;
using Klava.Infrastructure.Data;

public class SubmissionService : ISubmissionService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SubmissionService> _logger;

    public SubmissionService(AppDbContext context, ILogger<SubmissionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> ToggleStatusAsync(int taskId, int userId)
    {
        _logger.LogInformation("Toggling submission status for task {TaskId} by user {UserId}", taskId, userId);
        
        var submission = await _context.Submissions
            .FirstOrDefaultAsync(s => s.TaskId == taskId && s.UserId == userId);

        if (submission == null)
        {
            submission = new Submission
            {
                TaskId = taskId,
                UserId = userId,
                Status = SubmissionStatus.Done,
                SubmittedAt = DateTime.UtcNow
            };
            _context.Submissions.Add(submission);
            _logger.LogInformation("Created new submission for task {TaskId} by user {UserId} with status Done", taskId, userId);
        }
        else
        {
            var oldStatus = submission.Status;
            submission.Status = submission.Status == SubmissionStatus.Wait 
                ? SubmissionStatus.Done 
                : SubmissionStatus.Wait;
            submission.SubmittedAt = DateTime.UtcNow;
            _logger.LogInformation("Toggled submission status for task {TaskId} by user {UserId}: {OldStatus} -> {NewStatus}", taskId, userId, oldStatus, submission.Status);
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
