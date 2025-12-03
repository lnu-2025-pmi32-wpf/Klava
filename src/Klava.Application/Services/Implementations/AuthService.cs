namespace Klava.Application.Services.Implementations;

using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Klava.Application.Services.Interfaces;
using Klava.Domain.Entities;
using Klava.Infrastructure.Data;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    
    public AuthService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<User?> RegisterAsync(string firstname, string lastname, string password)
    {
        if (await UserExistsAsync(firstname, lastname))
            return null;
        
        var hashedPassword = BCrypt.HashPassword(password);
        
        var user = new User
        {
            Firstname = firstname,
            Lastname = lastname,
            Password = hashedPassword
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        return user;
    }
    
    public async Task<User?> LoginAsync(string firstname, string lastname, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Firstname == firstname && u.Lastname == lastname);
        
        if (user == null)
            return null;
        
        if (!BCrypt.Verify(password, user.Password))
            return null;
        
        return user;
    }
    
    public async Task<bool> UserExistsAsync(string firstname, string lastname)
    {
        return await _context.Users
            .AnyAsync(u => u.Firstname == firstname && u.Lastname == lastname);
    }
}
