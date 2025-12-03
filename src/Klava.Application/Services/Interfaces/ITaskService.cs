namespace Klava.Application.Services.Interfaces;

using Klava.Domain.Entities;

public interface ITaskService
{
    Task<Klava.Domain.Entities.Task> CreateTaskAsync(int subjectId, string name, string? description, DateTime? deadline);
    Task<Klava.Domain.Entities.Task?> GetTaskByIdAsync(int taskId);
    Task<List<Klava.Domain.Entities.Task>> GetTasksBySubjectAsync(int subjectId);
    Task<List<Klava.Domain.Entities.Task>> GetTasksByTeamAsync(int teamId);
    Task<bool> UpdateTaskAsync(int taskId, string name, string? description, DateTime? deadline);
    Task<bool> DeleteTaskAsync(int taskId);
}
