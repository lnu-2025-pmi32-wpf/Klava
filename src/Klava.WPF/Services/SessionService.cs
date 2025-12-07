using Klava.Domain.Entities;

namespace Klava.WPF.Services;

public class SessionService
{
    public User? CurrentUser { get; private set; }
    
    public event Action? OnUserChanged;
    
    public void SetUser(User user)
    {
        CurrentUser = user;
        OnUserChanged?.Invoke();
    }
    
    public void Logout()
    {
        CurrentUser = null;
        OnUserChanged?.Invoke();
    }
    
    public bool IsAuthenticated => CurrentUser != null;
    
    public int? GetCurrentUserId() => CurrentUser?.Id;
    
    public string GetUserDisplayName() => 
        CurrentUser != null ? $"{CurrentUser.Firstname} {CurrentUser.Lastname}" : "Guest";
}
