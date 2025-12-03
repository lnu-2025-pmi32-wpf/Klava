namespace Klava.Application.Services.Implementations;

using Microsoft.EntityFrameworkCore;
using Klava.Application.Services.Interfaces;
using Klava.Domain.Entities;
using Klava.Domain.Enums;
using Klava.Infrastructure.Data;

public class MemberService : IMemberService
{
    private readonly AppDbContext _context;
    
    public MemberService(AppDbContext context)
    {
        _context = context;
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
        var member = await _context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);
        
        if (member == null)
            return false;
        
        member.Role = newRole;
        await _context.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> RemoveMemberAsync(int teamId, int userId)
    {
        var member = await _context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);
        
        if (member == null)
            return false;
        
        _context.TeamMembers.Remove(member);
        await _context.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> IsHeadmanAsync(int userId, int teamId)
    {
        var member = await _context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.UserId == userId && tm.TeamId == teamId);
        
        return member?.Role == TeamMemberRole.Headman;
    }
}
