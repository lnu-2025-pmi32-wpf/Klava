namespace Klava.Application.Services.Interfaces;

using Klava.Domain.Entities;
using Klava.Domain.Enums;

public interface ITeamService
{
    Task<Team> CreateTeamAsync(string name, int ownerId);
    Task<Team?> GetTeamByIdAsync(int teamId);
    Task<Team?> GetTeamByCodeAsync(string code);
    Task<List<Team>> GetUserTeamsAsync(int userId);
    Task<bool> JoinTeamAsync(int userId, string teamCode);
    Task<bool> IsUserInTeamAsync(int userId, int teamId);
    Task<TeamMemberRole?> GetUserRoleInTeamAsync(int userId, int teamId);
}
