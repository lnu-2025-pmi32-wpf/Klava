using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Klava.Application.Services.Implementations;
using Klava.Domain.Entities;
using Klava.Domain.Enums;
using Klava.Infrastructure.Data;
using Task = System.Threading.Tasks.Task;

namespace Klava.Tests.Services;

public class SubjectServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly SubjectService _subjectService;

    public SubjectServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        var mockLogger = new Mock<ILogger<SubjectService>>();
        _subjectService = new SubjectService(_context, mockLogger.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task CreateSubjectAsync_CreatesSubject()
    {
        // Act
        var result = await _subjectService.CreateSubjectAsync(1, "Mathematics", "Advanced calculus", SubjectStatus.Exam);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TeamId);
        Assert.Equal("Mathematics", result.Title);
        Assert.Equal("Advanced calculus", result.SubjectInfo);
        Assert.Equal(SubjectStatus.Exam, result.Status);
    }

    [Fact]
    public async Task CreateSubjectAsync_WithNullInfo_CreatesSubject()
    {
        // Act
        var result = await _subjectService.CreateSubjectAsync(1, "Physics", null, SubjectStatus.Test);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Physics", result.Title);
        Assert.Null(result.SubjectInfo);
        Assert.Equal(SubjectStatus.Test, result.Status);
    }

    [Fact]
    public async Task GetSubjectByIdAsync_WithExistingSubject_ReturnsSubject()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Mathematics", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        // Act
        var result = await _subjectService.GetSubjectByIdAsync(subject.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Mathematics", result.Title);
        Assert.NotNull(result.Tasks);
    }

    [Fact]
    public async Task GetSubjectByIdAsync_WithNonExistentSubject_ReturnsNull()
    {
        // Act
        var result = await _subjectService.GetSubjectByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetSubjectsByTeamAsync_ReturnsSubjectsForTeam()
    {
        // Arrange
        _context.Subjects.AddRange(
            new Subject { TeamId = 1, Title = "Mathematics", Status = SubjectStatus.Exam },
            new Subject { TeamId = 1, Title = "Physics", Status = SubjectStatus.Test },
            new Subject { TeamId = 2, Title = "Chemistry", Status = SubjectStatus.Exam }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _subjectService.GetSubjectsByTeamAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.Title == "Mathematics");
        Assert.Contains(result, s => s.Title == "Physics");
        Assert.DoesNotContain(result, s => s.Title == "Chemistry");
    }

    [Fact]
    public async Task GetSubjectsByTeamAsync_WithNoSubjects_ReturnsEmptyList()
    {
        // Act
        var result = await _subjectService.GetSubjectsByTeamAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateSubjectAsync_WithExistingSubject_ReturnsTrue()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Old Title", Status = SubjectStatus.Test };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        // Act
        var result = await _subjectService.UpdateSubjectAsync(subject.Id, "New Title", "New Info", SubjectStatus.Exam);

        // Assert
        Assert.True(result);
        var updated = await _context.Subjects.FindAsync(subject.Id);
        Assert.Equal("New Title", updated.Title);
        Assert.Equal("New Info", updated.SubjectInfo);
        Assert.Equal(SubjectStatus.Exam, updated.Status);
    }

    [Fact]
    public async Task UpdateSubjectAsync_WithNonExistentSubject_ReturnsFalse()
    {
        // Act
        var result = await _subjectService.UpdateSubjectAsync(999, "New Title", "New Info", SubjectStatus.Exam);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteSubjectAsync_WithExistingSubject_ReturnsTrue()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Mathematics", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        // Act
        var result = await _subjectService.DeleteSubjectAsync(subject.Id);

        // Assert
        Assert.True(result);
        var deleted = await _context.Subjects.FindAsync(subject.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteSubjectAsync_WithNonExistentSubject_ReturnsFalse()
    {
        // Act
        var result = await _subjectService.DeleteSubjectAsync(999);

        // Assert
        Assert.False(result);
    }
}
