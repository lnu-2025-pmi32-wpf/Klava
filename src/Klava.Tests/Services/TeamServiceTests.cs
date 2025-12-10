using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Klava.Application.Services.Implementations;
using Klava.Domain.Entities;
using Klava.Domain.Enums;
using Klava.Infrastructure.Data;
using Task = System.Threading.Tasks.Task;

namespace Klava.Tests.Services;

public class TeamServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly TeamService _teamService;

    public TeamServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        var mockLogger = new Mock<ILogger<TeamService>>();
        _teamService = new TeamService(_context, mockLogger.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task CreateTeamAsync_CreatesTeamAndAddsOwnerAsHeadman()
    {
        // Arrange
        _context.Users.Add(new User { Id = 1, Firstname = "John", Lastname = "Doe", Password = "hash" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _teamService.CreateTeamAsync("Test Team", 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Team", result.Name);
        Assert.NotNull(result.Code);
        Assert.Equal(8, result.Code.Length);
        Assert.Equal(1, result.OwnerId);
        Assert.Matches("^[A-Z0-9]{8}$", result.Code);

        var member = await _context.TeamMembers.FirstOrDefaultAsync(tm => tm.TeamId == result.Id && tm.UserId == 1);
        Assert.NotNull(member);
        Assert.Equal(TeamMemberRole.Headman, member.Role);
    }

    [Fact]
    public async Task GetTeamByIdAsync_WithExistingTeam_ReturnsTeam()
    {
        // Arrange
        var team = new Team { Name = "Test Team", Code = "ABC12345", OwnerId = 1 };
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        // Act
        var result = await _teamService.GetTeamByIdAsync(team.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Team", result.Name);
    }

    [Fact]
    public async Task GetTeamByIdAsync_WithNonExistentTeam_ReturnsNull()
    {
        // Act
        var result = await _teamService.GetTeamByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTeamByCodeAsync_WithExistingCode_ReturnsTeam()
    {
        // Arrange
        var team = new Team { Name = "Test Team", Code = "ABC12345", OwnerId = 1 };
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        // Act
        var result = await _teamService.GetTeamByCodeAsync("ABC12345");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ABC12345", result.Code);
    }

    [Fact]
    public async Task GetTeamByCodeAsync_WithNonExistentCode_ReturnsNull()
    {
        // Act
        var result = await _teamService.GetTeamByCodeAsync("INVALID1");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserTeamsAsync_ReturnsUserTeams()
    {
        // Arrange
        var team1 = new Team { Name = "Team 1", Code = "CODE1111", OwnerId = 1 };
        var team2 = new Team { Name = "Team 2", Code = "CODE2222", OwnerId = 2 };
        _context.Teams.AddRange(team1, team2);
        await _context.SaveChangesAsync();

        _context.TeamMembers.AddRange(
            new TeamMember { TeamId = team1.Id, UserId = 1, Role = TeamMemberRole.Headman },
            new TeamMember { TeamId = team2.Id, UserId = 1, Role = TeamMemberRole.Student }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _teamService.GetUserTeamsAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, t => t.Name == "Team 1");
        Assert.Contains(result, t => t.Name == "Team 2");
    }

    [Fact]
    public async Task JoinTeamAsync_WithValidCode_ReturnsTrue()
    {
        // Arrange
        var team = new Team { Name = "Test Team", Code = "ABC12345", OwnerId = 1 };
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        // Act
        var result = await _teamService.JoinTeamAsync(2, "ABC12345");

        // Assert
        Assert.True(result);
        var member = await _context.TeamMembers.FirstOrDefaultAsync(tm => tm.TeamId == team.Id && tm.UserId == 2);
        Assert.NotNull(member);
        Assert.Equal(TeamMemberRole.Student, member.Role);
    }

    [Fact]
    public async Task JoinTeamAsync_WithInvalidCode_ReturnsFalse()
    {
        // Act
        var result = await _teamService.JoinTeamAsync(2, "INVALID1");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task JoinTeamAsync_WhenAlreadyMember_ReturnsFalse()
    {
        // Arrange
        var team = new Team { Name = "Test Team", Code = "ABC12345", OwnerId = 1 };
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();
        
        _context.TeamMembers.Add(new TeamMember { TeamId = team.Id, UserId = 2, Role = TeamMemberRole.Student });
        await _context.SaveChangesAsync();

        // Act
        var result = await _teamService.JoinTeamAsync(2, "ABC12345");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsUserInTeamAsync_WhenUserIsMember_ReturnsTrue()
    {
        // Arrange
        var team = new Team { Name = "Test Team", Code = "ABC12345", OwnerId = 1 };
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();
        
        _context.TeamMembers.Add(new TeamMember { TeamId = team.Id, UserId = 1, Role = TeamMemberRole.Student });
        await _context.SaveChangesAsync();

        // Act
        var result = await _teamService.IsUserInTeamAsync(1, team.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsUserInTeamAsync_WhenUserIsNotMember_ReturnsFalse()
    {
        // Act
        var result = await _teamService.IsUserInTeamAsync(1, 1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetUserRoleInTeamAsync_WhenUserIsMember_ReturnsRole()
    {
        // Arrange
        var team = new Team { Name = "Test Team", Code = "ABC12345", OwnerId = 1 };
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();
        
        _context.TeamMembers.Add(new TeamMember { TeamId = team.Id, UserId = 1, Role = TeamMemberRole.Headman });
        await _context.SaveChangesAsync();

        // Act
        var result = await _teamService.GetUserRoleInTeamAsync(1, team.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TeamMemberRole.Headman, result.Value);
    }

    [Fact]
    public async Task GetUserRoleInTeamAsync_WhenUserIsNotMember_ReturnsNull()
    {
        // Act
        var result = await _teamService.GetUserRoleInTeamAsync(1, 1);

        // Assert
        Assert.Null(result);
    }
}
