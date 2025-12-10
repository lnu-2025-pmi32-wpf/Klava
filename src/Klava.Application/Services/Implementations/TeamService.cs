namespace Klava.Application.Services.Implementations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Klava.Application.Services.Interfaces;
using Klava.Domain.Entities;
using Klava.Domain.Enums;
using Klava.Infrastructure.Data;

public class TeamService : ITeamService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TeamService> _logger;
    
    public TeamService(AppDbContext context, ILogger<TeamService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<Team> CreateTeamAsync(string name, int ownerId)
    {
        _logger.LogInformation("Creating team {TeamName} for owner {OwnerId}", name, ownerId);
        
        try
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
            
            _logger.LogInformation("Team created: {TeamName} (ID: {TeamId}, Code: {TeamCode})", name, team.Id, code);
            return team;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create team {TeamName} for owner {OwnerId}", name, ownerId);
            throw;
        }
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
        _logger.LogInformation("User {UserId} attempting to join team with code {TeamCode}", userId, teamCode);
        
        var team = await GetTeamByCodeAsync(teamCode);
        if (team == null)
        {
            _logger.LogWarning("Join team failed: Team code {TeamCode} not found", teamCode);
            return false;
        }
        
        var existingMember = await _context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.TeamId == team.Id && tm.UserId == userId);
        
        if (existingMember != null)
        {
            _logger.LogWarning("Join team failed: User {UserId} already member of team {TeamId}", userId, team.Id);
            return false;
        }
        
        var teamMember = new TeamMember
        {
            TeamId = team.Id,
            UserId = userId,
            Role = TeamMemberRole.Student
        };
        
        _context.TeamMembers.Add(teamMember);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("User {UserId} joined team {TeamId} ({TeamName})", userId, team.Id, team.Name);
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
