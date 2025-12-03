namespace Klava.Application.Services.Implementations;

using Microsoft.EntityFrameworkCore;
using Klava.Application.Services.Interfaces;
using Klava.Domain.Entities;
using Klava.Domain.Enums;
using Klava.Infrastructure.Data;

public class TeamService : ITeamService
{
    private readonly AppDbContext _context;
    
    public TeamService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Team> CreateTeamAsync(string name, int ownerId)
    {
        var code = await GenerateUniqueTeamCodeAsync();
        
        var team = new Team
        {
            Name = name,
            Code = code,
            OwnerId = ownerId
        };
        
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();
        
        var teamMember = new TeamMember
        {
            TeamId = team.Id,
            UserId = ownerId,
            Role = TeamMemberRole.Headman
        };
        
        _context.TeamMembers.Add(teamMember);
        await _context.SaveChangesAsync();
        
        return team;
    }
    
    public async Task<Team?> GetTeamByIdAsync(int teamId)
    {
        return await _context.Teams
            .Include(t => t.Members)
            .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(t => t.Id == teamId);
    }
    
    public async Task<Team?> GetTeamByCodeAsync(string code)
    {
        return await _context.Teams
            .FirstOrDefaultAsync(t => t.Code == code);
    }
    
    public async Task<List<Team>> GetUserTeamsAsync(int userId)
    {
        return await _context.TeamMembers
            .Where(tm => tm.UserId == userId)
            .Include(tm => tm.Team)
            .Select(tm => tm.Team)
            .ToListAsync();
    }
    
    public async Task<bool> JoinTeamAsync(int userId, string teamCode)
    {
        var team = await GetTeamByCodeAsync(teamCode);
        if (team == null)
            return false;
        
        var existingMember = await _context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.TeamId == team.Id && tm.UserId == userId);
        
        if (existingMember != null)
            return false; 
        
        var teamMember = new TeamMember
        {
            TeamId = team.Id,
            UserId = userId,
            Role = TeamMemberRole.Student
        };
        
        _context.TeamMembers.Add(teamMember);
        await _context.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> IsUserInTeamAsync(int userId, int teamId)
    {
        return await _context.TeamMembers
            .AnyAsync(tm => tm.UserId == userId && tm.TeamId == teamId);
    }
    
    public async Task<TeamMemberRole?> GetUserRoleInTeamAsync(int userId, int teamId)
    {
        var member = await _context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.UserId == userId && tm.TeamId == teamId);
        
        return member?.Role;
    }
    
    private async Task<string> GenerateUniqueTeamCodeAsync()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        string code;
        
        do
        {
            code = new string(Enumerable.Range(0, 8)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
        }
        while (await _context.Teams.AnyAsync(t => t.Code == code));
        
        return code;
    }
}
