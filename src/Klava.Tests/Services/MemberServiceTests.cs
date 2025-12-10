using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Klava.Application.Services.Implementations;
using Klava.Domain.Entities;
using Klava.Domain.Enums;
using Klava.Infrastructure.Data;
using Task = System.Threading.Tasks.Task;

namespace Klava.Tests.Services;

public class MemberServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly MemberService _memberService;

    public MemberServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        var mockLogger = new Mock<ILogger<MemberService>>();
        _memberService = new MemberService(_context, mockLogger.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetTeamMembersAsync_ReturnsAllMembersWithUsers()
    {
        // Arrange
        var team = new Team { Name = "Test Team", Code = "ABC12345", OwnerId = 1 };
        var user1 = new User { Firstname = "John", Lastname = "Doe", Password = "hash" };
        var user2 = new User { Firstname = "Jane", Lastname = "Smith", Password = "hash" };
        _context.Teams.Add(team);
        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();

        _context.TeamMembers.AddRange(
            new TeamMember { TeamId = team.Id, UserId = user1.Id, Role = TeamMemberRole.Headman },
            new TeamMember { TeamId = team.Id, UserId = user2.Id, Role = TeamMemberRole.Student }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _memberService.GetTeamMembersAsync(team.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, m => m.User.Firstname == "John");
        Assert.Contains(result, m => m.User.Firstname == "Jane");
    }

    [Fact]
    public async Task GetTeamMembersAsync_WithNoMembers_ReturnsEmptyList()
    {
        // Act
        var result = await _memberService.GetTeamMembersAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateMemberRoleAsync_WithExistingMember_ReturnsTrue()
    {
        // Arrange
        var team = new Team { Name = "Test Team", Code = "ABC12345", OwnerId = 1 };
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var member = new TeamMember { TeamId = team.Id, UserId = 1, Role = TeamMemberRole.Student };
        _context.TeamMembers.Add(member);
        await _context.SaveChangesAsync();

        // Act
        var result = await _memberService.UpdateMemberRoleAsync(team.Id, 1, TeamMemberRole.Headman);

        // Assert
        Assert.True(result);
        var updated = await _context.TeamMembers.FindAsync(team.Id, 1);
        Assert.Equal(TeamMemberRole.Headman, updated.Role);
    }

    [Fact]
    public async Task UpdateMemberRoleAsync_WithNonExistentMember_ReturnsFalse()
    {
        // Act
        var result = await _memberService.UpdateMemberRoleAsync(1, 1, TeamMemberRole.Headman);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveMemberAsync_WithExistingMember_ReturnsTrue()
    {
        // Arrange
        var team = new Team { Name = "Test Team", Code = "ABC12345", OwnerId = 1 };
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var member = new TeamMember { TeamId = team.Id, UserId = 1, Role = TeamMemberRole.Student };
        _context.TeamMembers.Add(member);
        await _context.SaveChangesAsync();

        // Act
        var result = await _memberService.RemoveMemberAsync(team.Id, 1);

        // Assert
        Assert.True(result);
        var removed = await _context.TeamMembers.FindAsync(team.Id, 1);
        Assert.Null(removed);
    }

    [Fact]
    public async Task RemoveMemberAsync_WithNonExistentMember_ReturnsFalse()
    {
        // Act
        var result = await _memberService.RemoveMemberAsync(1, 1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsHeadmanAsync_WhenUserIsHeadman_ReturnsTrue()
    {
        // Arrange
        var team = new Team { Name = "Test Team", Code = "ABC12345", OwnerId = 1 };
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var member = new TeamMember { TeamId = team.Id, UserId = 1, Role = TeamMemberRole.Headman };
        _context.TeamMembers.Add(member);
        await _context.SaveChangesAsync();

        // Act
        var result = await _memberService.IsHeadmanAsync(1, team.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsHeadmanAsync_WhenUserIsStudent_ReturnsFalse()
    {
        // Arrange
        var team = new Team { Name = "Test Team", Code = "ABC12345", OwnerId = 1 };
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var member = new TeamMember { TeamId = team.Id, UserId = 1, Role = TeamMemberRole.Student };
        _context.TeamMembers.Add(member);
        await _context.SaveChangesAsync();

        // Act
        var result = await _memberService.IsHeadmanAsync(1, team.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsHeadmanAsync_WhenUserNotMember_ReturnsFalse()
    {
        // Act
        var result = await _memberService.IsHeadmanAsync(1, 1);

        // Assert
        Assert.False(result);
    }
}
