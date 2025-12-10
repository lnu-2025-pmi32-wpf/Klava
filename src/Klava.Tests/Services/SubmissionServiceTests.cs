using Microsoft.EntityFrameworkCore;
using Klava.Application.Services.Implementations;
using Klava.Domain.Entities;
using Klava.Domain.Enums;
using Klava.Infrastructure.Data;
using Task = System.Threading.Tasks.Task;

namespace Klava.Tests.Services;

public class SubmissionServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly SubmissionService _submissionService;

    public SubmissionServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _submissionService = new SubmissionService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task ToggleStatusAsync_WhenNoSubmissionExists_CreatesNewWithDoneStatus()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Math", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var task = new Klava.Domain.Entities.Task { SubjectId = subject.Id, Name = "Task 1" };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Act
        var result = await _submissionService.ToggleStatusAsync(task.Id, 1);

        // Assert
        Assert.True(result);
        var submission = await _context.Submissions.FirstOrDefaultAsync(s => s.TaskId == task.Id && s.UserId == 1);
        Assert.NotNull(submission);
        Assert.Equal(SubmissionStatus.Done, submission.Status);
    }

    [Fact]
    public async Task ToggleStatusAsync_WhenSubmissionIsWait_ChangesToDone()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Math", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var task = new Klava.Domain.Entities.Task { SubjectId = subject.Id, Name = "Task 1" };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var submission = new Submission { TaskId = task.Id, UserId = 1, Status = SubmissionStatus.Wait, SubmittedAt = DateTime.UtcNow };
        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();

        // Act
        var result = await _submissionService.ToggleStatusAsync(task.Id, 1);

        // Assert
        Assert.True(result);
        var updated = await _context.Submissions.FirstOrDefaultAsync(s => s.TaskId == task.Id && s.UserId == 1);
        Assert.Equal(SubmissionStatus.Done, updated.Status);
    }

    [Fact]
    public async Task ToggleStatusAsync_WhenSubmissionIsDone_ChangesToWait()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Math", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var task = new Klava.Domain.Entities.Task { SubjectId = subject.Id, Name = "Task 1" };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var submission = new Submission { TaskId = task.Id, UserId = 1, Status = SubmissionStatus.Done, SubmittedAt = DateTime.UtcNow };
        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();

        // Act
        var result = await _submissionService.ToggleStatusAsync(task.Id, 1);

        // Assert
        Assert.True(result);
        var updated = await _context.Submissions.FirstOrDefaultAsync(s => s.TaskId == task.Id && s.UserId == 1);
        Assert.Equal(SubmissionStatus.Wait, updated.Status);
    }

    [Fact]
    public async Task GetTeamTasksWithStatusAsync_ReturnsTasksWithUserStatus()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Math", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var task1 = new Klava.Domain.Entities.Task { SubjectId = subject.Id, Name = "Task 1", Deadline = DateTime.UtcNow.AddDays(7) };
        var task2 = new Klava.Domain.Entities.Task { SubjectId = subject.Id, Name = "Task 2", Deadline = DateTime.UtcNow.AddDays(3) };
        _context.Tasks.AddRange(task1, task2);
        await _context.SaveChangesAsync();

        var submission = new Submission { TaskId = task1.Id, UserId = 1, Status = SubmissionStatus.Done, SubmittedAt = DateTime.UtcNow };
        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();

        // Act
        var result = await _submissionService.GetTeamTasksWithStatusAsync(1, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        var resultTask1 = result.First(t => t.Name == "Task 1");
        Assert.Equal(SubmissionStatus.Done, resultTask1.CurrentUserStatus);
        Assert.NotNull(resultTask1.SubmittedAt);

        var resultTask2 = result.First(t => t.Name == "Task 2");
        Assert.Null(resultTask2.CurrentUserStatus);
        Assert.Null(resultTask2.SubmittedAt);
    }

    [Fact]
    public async Task GetTeamTasksWithStatusAsync_OrdersByDeadline()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Math", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var task1 = new Klava.Domain.Entities.Task { SubjectId = subject.Id, Name = "Task 1", Deadline = DateTime.UtcNow.AddDays(7) };
        var task2 = new Klava.Domain.Entities.Task { SubjectId = subject.Id, Name = "Task 2", Deadline = DateTime.UtcNow.AddDays(3) };
        _context.Tasks.AddRange(task1, task2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _submissionService.GetTeamTasksWithStatusAsync(1, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Task 2", result[0].Name);
        Assert.Equal("Task 1", result[1].Name);
    }

    [Fact]
    public async Task GetTeamTasksWithStatusAsync_WithNoTasks_ReturnsEmptyList()
    {
        // Act
        var result = await _submissionService.GetTeamTasksWithStatusAsync(999, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserTaskStatusAsync_WithExistingSubmission_ReturnsStatus()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Math", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var task = new Klava.Domain.Entities.Task { SubjectId = subject.Id, Name = "Task 1" };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var submission = new Submission { TaskId = task.Id, UserId = 1, Status = SubmissionStatus.Done, SubmittedAt = DateTime.UtcNow };
        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();

        // Act
        var result = await _submissionService.GetUserTaskStatusAsync(task.Id, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(SubmissionStatus.Done, result.Value);
    }

    [Fact]
    public async Task GetUserTaskStatusAsync_WithNoSubmission_ReturnsNull()
    {
        // Act
        var result = await _submissionService.GetUserTaskStatusAsync(1, 1);

        // Assert
        Assert.Null(result);
    }
}
