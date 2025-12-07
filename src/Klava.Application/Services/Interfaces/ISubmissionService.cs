namespace Klava.Application.Services.Interfaces;

using Klava.Application.DTOs;
using Klava.Domain.Entities;
using Klava.Domain.Enums;

public interface ISubmissionService
{
    Task<bool> ToggleStatusAsync(int taskId, int userId);
    Task<List<TaskWithUserStatusDto>> GetTeamTasksWithStatusAsync(int teamId, int userId);
    Task<SubmissionStatus?> GetUserTaskStatusAsync(int taskId, int userId);
}
