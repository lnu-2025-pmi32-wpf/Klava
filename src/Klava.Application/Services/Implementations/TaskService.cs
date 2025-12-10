namespace Klava.Application.Services.Implementations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Klava.Application.Services.Interfaces;
using Klava.Domain.Entities;
using Klava.Infrastructure.Data;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TaskService> _logger;
    
    public TaskService(AppDbContext context, ILogger<TaskService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<Klava.Domain.Entities.Task> CreateTaskAsync(int subjectId, string name, string? description, DateTime? deadline)
    {
        _logger.LogInformation("Creating task {TaskName} for subject {SubjectId}", name, subjectId);
        
        try
        {
            var task = new Klava.Domain.Entities.Task
            {
                SubjectId = subjectId,
                Name = name,
                Description = description,
                Deadline = deadline
            };
            
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Task created: {TaskName} (ID: {TaskId}) for subject {SubjectId}", name, task.Id, subjectId);
            return task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create task {TaskName} for subject {SubjectId}", name, subjectId);
            throw;
        }
    }
    
    public async Task<Klava.Domain.Entities.Task?> GetTaskByIdAsync(int taskId)
    {
        return await _context.Tasks
            .Include(t => t.Subject)
            .FirstOrDefaultAsync(t => t.Id == taskId);
    }
    
    public async Task<List<Klava.Domain.Entities.Task>> GetTasksBySubjectAsync(int subjectId)
    {
        return await _context.Tasks
            .Include(t => t.Subject)
            .Where(t => t.SubjectId == subjectId)
            .OrderBy(t => t.Deadline)
            .ToListAsync();
    }
    
    public async Task<List<Klava.Domain.Entities.Task>> GetTasksByTeamAsync(int teamId)
    {
        return await _context.Tasks
            .Include(t => t.Subject)
            .Where(t => t.Subject.TeamId == teamId)
            .OrderBy(t => t.Deadline)
            .ToListAsync();
    }
    
    public async Task<bool> UpdateTaskAsync(int taskId, string name, string? description, DateTime? deadline)
    {
        _logger.LogInformation("Updating task {TaskId}", taskId);
        
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null)
        {
            _logger.LogWarning("Update task failed: Task {TaskId} not found", taskId);
            return false;
        }
        
        task.Name = name;
        task.Description = description;
        task.Deadline = deadline;
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Task updated: {TaskId} ({TaskName})", taskId, name);
        return true;
    }
    
    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        _logger.LogInformation("Deleting task {TaskId}", taskId);
        
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null)
        {
            _logger.LogWarning("Delete task failed: Task {TaskId} not found", taskId);
            return false;
        }
        
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Task deleted: {TaskId}", taskId);
        return true;
    }
}
