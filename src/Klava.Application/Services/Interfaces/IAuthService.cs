namespace Klava.Application.Services.Interfaces;

using Klava.Domain.Entities;

public interface IAuthService
{
    Task<User?> RegisterAsync(string firstname, string lastname, string password);
    Task<User?> LoginAsync(string firstname, string lastname, string password);
    Task<bool> UserExistsAsync(string firstname, string lastname);
}
