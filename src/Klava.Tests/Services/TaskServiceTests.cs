using Microsoft.EntityFrameworkCore;
using Klava.Application.Services.Implementations;
using Klava.Domain.Entities;
using Klava.Domain.Enums;
using Klava.Infrastructure.Data;
using Task = System.Threading.Tasks.Task;

namespace Klava.Tests.Services;

public class TaskServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _taskService = new TaskService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task CreateTaskAsync_CreatesTask()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Math", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var deadline = DateTime.UtcNow.AddDays(7);

        // Act
        var result = await _taskService.CreateTaskAsync(subject.Id, "Homework 1", "Complete exercises", deadline);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subject.Id, result.SubjectId);
        Assert.Equal("Homework 1", result.Name);
        Assert.Equal("Complete exercises", result.Description);
        Assert.Equal(deadline, result.Deadline);
    }

    [Fact]
    public async Task CreateTaskAsync_WithNullDescription_CreatesTask()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Math", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskService.CreateTaskAsync(subject.Id, "Homework 1", null, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Homework 1", result.Name);
        Assert.Null(result.Description);
        Assert.Null(result.Deadline);
    }

    [Fact]
    public async Task GetTaskByIdAsync_WithExistingTask_ReturnsTask()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Math", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var task = new Klava.Domain.Entities.Task { SubjectId = subject.Id, Name = "Task 1" };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskService.GetTaskByIdAsync(task.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Task 1", result.Name);
        Assert.NotNull(result.Subject);
    }

    [Fact]
    public async Task GetTaskByIdAsync_WithNonExistentTask_ReturnsNull()
    {
        // Act
        var result = await _taskService.GetTaskByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTasksBySubjectAsync_ReturnsTasksOrderedByDeadline()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Math", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        _context.Tasks.AddRange(
            new Klava.Domain.Entities.Task { SubjectId = subject.Id, Name = "Task 1", Deadline = DateTime.UtcNow.AddDays(5) },
            new Klava.Domain.Entities.Task { SubjectId = subject.Id, Name = "Task 2", Deadline = DateTime.UtcNow.AddDays(2) }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskService.GetTasksBySubjectAsync(subject.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Task 2", result[0].Name); // Earlier deadline first
        Assert.Equal("Task 1", result[1].Name);
    }

    [Fact]
    public async Task GetTasksByTeamAsync_ReturnsTasksForTeam()
    {
        // Arrange
        var subject1 = new Subject { TeamId = 1, Title = "Math", Status = SubjectStatus.Exam };
        var subject2 = new Subject { TeamId = 2, Title = "Physics", Status = SubjectStatus.Test };
        _context.Subjects.AddRange(subject1, subject2);
        await _context.SaveChangesAsync();

        _context.Tasks.AddRange(
            new Klava.Domain.Entities.Task { SubjectId = subject1.Id, Name = "Task 1" },
            new Klava.Domain.Entities.Task { SubjectId = subject2.Id, Name = "Task 2" }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskService.GetTasksByTeamAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Task 1", result[0].Name);
    }

    [Fact]
    public async Task UpdateTaskAsync_WithExistingTask_ReturnsTrue()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Math", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var task = new Klava.Domain.Entities.Task { SubjectId = subject.Id, Name = "Old Name" };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var newDeadline = DateTime.UtcNow.AddDays(10);

        // Act
        var result = await _taskService.UpdateTaskAsync(task.Id, "New Name", "New Description", newDeadline);

        // Assert
        Assert.True(result);
        var updated = await _context.Tasks.FindAsync(task.Id);
        Assert.Equal("New Name", updated.Name);
        Assert.Equal("New Description", updated.Description);
        Assert.Equal(newDeadline, updated.Deadline);
    }

    [Fact]
    public async Task UpdateTaskAsync_WithNonExistentTask_ReturnsFalse()
    {
        // Act
        var result = await _taskService.UpdateTaskAsync(999, "New Name", "New Description", null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteTaskAsync_WithExistingTask_ReturnsTrue()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Math", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var task = new Klava.Domain.Entities.Task { SubjectId = subject.Id, Name = "Task 1" };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskService.DeleteTaskAsync(task.Id);

        // Assert
        Assert.True(result);
        var deleted = await _context.Tasks.FindAsync(task.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteTaskAsync_WithNonExistentTask_ReturnsFalse()
    {
        // Act
        var result = await _taskService.DeleteTaskAsync(999);

        // Assert
        Assert.False(result);
    }
}
