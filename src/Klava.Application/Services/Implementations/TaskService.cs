namespace Klava.Application.Services.Implementations;

using Microsoft.EntityFrameworkCore;
using Klava.Application.Services.Interfaces;
using Klava.Domain.Entities;
using Klava.Infrastructure.Data;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;
    
    public TaskService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Klava.Domain.Entities.Task> CreateTaskAsync(int subjectId, string name, string? description, DateTime? deadline)
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
        
        return task;
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
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null)
            return false;
        
        task.Name = name;
        task.Description = description;
        task.Deadline = deadline;
        
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null)
            return false;
        
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        
        return true;
    }
}
