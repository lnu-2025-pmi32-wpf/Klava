using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Klava.Application.Services.Implementations;
using Klava.Domain.Entities;
using Klava.Infrastructure.Data;
using Task = System.Threading.Tasks.Task;

namespace Klava.Tests.Services;

public class AuthServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        var mockLogger = new Mock<ILogger<AuthService>>();
        _authService = new AuthService(_context, mockLogger.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task RegisterAsync_WithNewUser_ReturnsUserWithHashedPassword()
    {
        // Act
        var result = await _authService.RegisterAsync("John", "Doe", "password123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.Firstname);
        Assert.Equal("Doe", result.Lastname);
        Assert.NotEqual("password123", result.Password);
        Assert.True(BCrypt.Net.BCrypt.Verify("password123", result.Password));
    }

    [Fact]
    public async Task RegisterAsync_WithExistingUser_ReturnsNull()
    {
        // Arrange
        _context.Users.Add(new User { Firstname = "John", Lastname = "Doe", Password = "hashed" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _authService.RegisterAsync("John", "Doe", "password123");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsUser()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
        _context.Users.Add(new User { Firstname = "John", Lastname = "Doe", Password = hashedPassword });
        await _context.SaveChangesAsync();

        // Act
        var result = await _authService.LoginAsync("John", "Doe", "password123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.Firstname);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnsNull()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
        _context.Users.Add(new User { Firstname = "John", Lastname = "Doe", Password = hashedPassword });
        await _context.SaveChangesAsync();

        // Act
        var result = await _authService.LoginAsync("John", "Doe", "wrongpassword");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentUser_ReturnsNull()
    {
        // Act
        var result = await _authService.LoginAsync("John", "Doe", "password123");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UserExistsAsync_WithExistingUser_ReturnsTrue()
    {
        // Arrange
        _context.Users.Add(new User { Firstname = "John", Lastname = "Doe", Password = "hashed" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _authService.UserExistsAsync("John", "Doe");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UserExistsAsync_WithNonExistentUser_ReturnsFalse()
    {
        // Act
        var result = await _authService.UserExistsAsync("John", "Doe");

        // Assert
        Assert.False(result);
    }
}
