using Microsoft.EntityFrameworkCore;
using Moq;
using Klava.Application.Services.Implementations;
using Klava.Application.DTOs;
using Klava.Domain.Entities;
using Klava.Domain.Enums;
using Klava.Infrastructure.Data;
using Klava.Infrastructure.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace Klava.Tests.Services;

public class SubjectFileServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<IFileStorageService> _mockFileStorage;
    private readonly SubjectFileService _subjectFileService;

    public SubjectFileServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _mockFileStorage = new Mock<IFileStorageService>();
        _subjectFileService = new SubjectFileService(_context, _mockFileStorage.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task UploadFileAsync_WithValidRequest_ReturnsSubjectFile()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Math", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        _mockFileStorage.Setup(x => x.SaveFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync("stored-file-name.pdf");

        var stream = new MemoryStream();
        var request = new UploadFileRequest
        {
            SubjectId = subject.Id,
            FileName = "document.pdf",
            ContentType = "application/pdf",
            Size = 1024,
            FileStream = stream
        };

        // Act
        var result = await _subjectFileService.UploadFileAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subject.Id, result.SubjectId);
        Assert.Equal("document.pdf", result.DisplayName);
        Assert.Equal("stored-file-name.pdf", result.StorageName);
        Assert.Equal("application/pdf", result.ContentType);
        Assert.Equal(1024, result.Size);
        _mockFileStorage.Verify(x => x.SaveFileAsync(stream, "document.pdf", subject.Id), Times.Once);
    }

    [Fact]
    public async Task UploadFileAsync_WhenStorageFails_ReturnsNull()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Math", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        _mockFileStorage.Setup(x => x.SaveFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception("Storage error"));

        var stream = new MemoryStream();
        var request = new UploadFileRequest
        {
            SubjectId = subject.Id,
            FileName = "document.pdf",
            ContentType = "application/pdf",
            Size = 1024,
            FileStream = stream
        };

        // Act
        var result = await _subjectFileService.UploadFileAsync(request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetFileByIdAsync_WithExistingFile_ReturnsFile()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Math", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var file = new SubjectFile
        {
            SubjectId = subject.Id,
            DisplayName = "document.pdf",
            StorageName = "stored.pdf",
            ContentType = "application/pdf",
            Size = 1024,
            UploadedAt = DateTime.UtcNow
        };
        _context.SubjectFiles.Add(file);
        await _context.SaveChangesAsync();

        // Act
        var result = await _subjectFileService.GetFileByIdAsync(file.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("document.pdf", result.DisplayName);
        Assert.NotNull(result.Subject);
    }

    [Fact]
    public async Task GetFileByIdAsync_WithNonExistentFile_ReturnsNull()
    {
        // Act
        var result = await _subjectFileService.GetFileByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetFilesBySubjectAsync_ReturnsFilesOrderedByUploadDate()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Math", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var now = DateTime.UtcNow;
        _context.SubjectFiles.AddRange(
            new SubjectFile { SubjectId = subject.Id, DisplayName = "file1.pdf", StorageName = "stored1.pdf", UploadedAt = now.AddDays(-2) },
            new SubjectFile { SubjectId = subject.Id, DisplayName = "file2.pdf", StorageName = "stored2.pdf", UploadedAt = now.AddDays(-1) }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _subjectFileService.GetFilesBySubjectAsync(subject.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("file2.pdf", result[0].DisplayName); // Most recent first
        Assert.Equal("file1.pdf", result[1].DisplayName);
    }

    [Fact]
    public async Task GetFilesBySubjectAsync_WithNoFiles_ReturnsEmptyList()
    {
        // Act
        var result = await _subjectFileService.GetFilesBySubjectAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task DownloadFileAsync_WithExistingFile_ReturnsStream()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Math", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var file = new SubjectFile
        {
            SubjectId = subject.Id,
            DisplayName = "document.pdf",
            StorageName = "stored.pdf"
        };
        _context.SubjectFiles.Add(file);
        await _context.SaveChangesAsync();

        var expectedStream = new MemoryStream();
        _mockFileStorage.Setup(x => x.GetFileAsync("stored.pdf", subject.Id))
            .ReturnsAsync(expectedStream);

        // Act
        var result = await _subjectFileService.DownloadFileAsync(file.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedStream, result);
        _mockFileStorage.Verify(x => x.GetFileAsync("stored.pdf", subject.Id), Times.Once);
    }

    [Fact]
    public async Task DownloadFileAsync_WithNonExistentFile_ReturnsNull()
    {
        // Act
        var result = await _subjectFileService.DownloadFileAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteFileAsync_WithExistingFile_ReturnsTrue()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Math", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var file = new SubjectFile
        {
            SubjectId = subject.Id,
            DisplayName = "document.pdf",
            StorageName = "stored.pdf"
        };
        _context.SubjectFiles.Add(file);
        await _context.SaveChangesAsync();

        _mockFileStorage.Setup(x => x.DeleteFileAsync("stored.pdf", subject.Id)).ReturnsAsync(true);

        // Act
        var result = await _subjectFileService.DeleteFileAsync(file.Id);

        // Assert
        Assert.True(result);
        _mockFileStorage.Verify(x => x.DeleteFileAsync("stored.pdf", subject.Id), Times.Once);
        var deleted = await _context.SubjectFiles.FindAsync(file.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteFileAsync_WithNonExistentFile_ReturnsFalse()
    {
        // Act
        var result = await _subjectFileService.DeleteFileAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteFileAsync_WhenStorageDeleteFails_ReturnsFalse()
    {
        // Arrange
        var subject = new Subject { TeamId = 1, Title = "Math", Status = SubjectStatus.Exam };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var file = new SubjectFile
        {
            SubjectId = subject.Id,
            DisplayName = "document.pdf",
            StorageName = "stored.pdf"
        };
        _context.SubjectFiles.Add(file);
        await _context.SaveChangesAsync();

        _mockFileStorage.Setup(x => x.DeleteFileAsync("stored.pdf", subject.Id)).ReturnsAsync(false);

        // Act
        var result = await _subjectFileService.DeleteFileAsync(file.Id);

        // Assert
        Assert.False(result);
        _mockFileStorage.Verify(x => x.DeleteFileAsync("stored.pdf", subject.Id), Times.Once);
        var stillExists = await _context.SubjectFiles.FindAsync(file.Id);
        Assert.NotNull(stillExists); // File should still exist in DB since storage delete failed
    }
}
