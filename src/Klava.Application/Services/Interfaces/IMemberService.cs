namespace Klava.Application.Services.Interfaces;

using Klava.Domain.Entities;
using Klava.Domain.Enums;

public interface IMemberService
{
    Task<List<TeamMember>> GetTeamMembersAsync(int teamId);
    Task<bool> UpdateMemberRoleAsync(int teamId, int userId, TeamMemberRole newRole);
    Task<bool> RemoveMemberAsync(int teamId, int userId);
    Task<bool> IsHeadmanAsync(int userId, int teamId);
}
