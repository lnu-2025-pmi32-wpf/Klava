namespace Klava.Application.Services.Implementations;

using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Klava.Application.Services.Interfaces;
using Klava.Domain.Entities;
using Klava.Infrastructure.Data;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AuthService> _logger;
    
    public AuthService(AppDbContext context, ILogger<AuthService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<User?> RegisterAsync(string firstname, string lastname, string password)
    {
        _logger.LogInformation("Attempting to register user: {Firstname} {Lastname}", firstname, lastname);
        
        if (await UserExistsAsync(firstname, lastname))
        {
            _logger.LogWarning("Registration failed: User {Firstname} {Lastname} already exists", firstname, lastname);
            return null;
        }
        
        try
        {
            var hashedPassword = BCrypt.HashPassword(password);
            
            var user = new User
            {
                Firstname = firstname,
                Lastname = lastname,
                Password = hashedPassword
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("User registered successfully: {Firstname} {Lastname} (ID: {UserId})", firstname, lastname, user.Id);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register user: {Firstname} {Lastname}", firstname, lastname);
            throw;
        }
    }
    
    public async Task<User?> LoginAsync(string firstname, string lastname, string password)
    {
        _logger.LogInformation("Login attempt for user: {Firstname} {Lastname}", firstname, lastname);
        
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Firstname == firstname && u.Lastname == lastname);
            
            if (user == null)
            {
                _logger.LogWarning("Login failed: User {Firstname} {Lastname} not found", firstname, lastname);
                return null;
            }
            
            if (!BCrypt.Verify(password, user.Password))
            {
                _logger.LogWarning("Login failed: Invalid password for user {Firstname} {Lastname}", firstname, lastname);
                return null;
            }
            
            _logger.LogInformation("User logged in successfully: {Firstname} {Lastname} (ID: {UserId})", firstname, lastname, user.Id);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error for user: {Firstname} {Lastname}", firstname, lastname);
            throw;
        }
    }
    
    public async Task<bool> UserExistsAsync(string firstname, string lastname)
    {
        return await _context.Users
            .AnyAsync(u => u.Firstname == firstname && u.Lastname == lastname);
    }
}
