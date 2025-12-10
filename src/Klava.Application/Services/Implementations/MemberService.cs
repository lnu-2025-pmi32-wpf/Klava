namespace Klava.Application.Services.Implementations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Klava.Application.Services.Interfaces;
using Klava.Domain.Entities;
using Klava.Domain.Enums;
using Klava.Infrastructure.Data;

public class MemberService : IMemberService
{
    private readonly AppDbContext _context;
    private readonly ILogger<MemberService> _logger;
    
    public MemberService(AppDbContext context, ILogger<MemberService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<List<TeamMember>> GetTeamMembersAsync(int teamId)
    {
        return await _context.TeamMembers
            .Where(tm => tm.TeamId == teamId)
            .Include(tm => tm.User)
            .ToListAsync();
    }
    
    public async Task<bool> UpdateMemberRoleAsync(int teamId, int userId, TeamMemberRole newRole)
    {
        _logger.LogInformation("Updating role for user {UserId} in team {TeamId} to {NewRole}", userId, teamId, newRole);
        
        var member = await _context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);
        
        if (member == null)
        {
            _logger.LogWarning("Update role failed: Member not found (User {UserId}, Team {TeamId})", userId, teamId);
            return false;
        }
        
        var oldRole = member.Role;
        member.Role = newRole;
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Role updated for user {UserId} in team {TeamId}: {OldRole} -> {NewRole}", userId, teamId, oldRole, newRole);
        return true;
    }
    
    public async Task<bool> RemoveMemberAsync(int teamId, int userId)
    {
        _logger.LogInformation("Removing user {UserId} from team {TeamId}", userId, teamId);
        
        var member = await _context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);
        
        if (member == null)
        {
            _logger.LogWarning("Remove member failed: Member not found (User {UserId}, Team {TeamId})", userId, teamId);
            return false;
        }
        
        _context.TeamMembers.Remove(member);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("User {UserId} removed from team {TeamId}", userId, teamId);
        return true;
    }
    
    public async Task<bool> IsHeadmanAsync(int userId, int teamId)
    {
        var member = await _context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.UserId == userId && tm.TeamId == teamId);
        
        return member?.Role == TeamMemberRole.Headman;
    }
}
