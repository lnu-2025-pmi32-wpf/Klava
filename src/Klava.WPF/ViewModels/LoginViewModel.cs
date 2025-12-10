using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Klava.Application.Services.Interfaces;
using Klava.WPF.Services;
using TaskAsync = System.Threading.Tasks.Task;

namespace Klava.WPF.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly SessionService _sessionService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;
    private readonly ILogger<LoginViewModel> _logger;

    [ObservableProperty]
    private string _firstname = string.Empty;

    [ObservableProperty]
    private string _lastname = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _isLoading;

    public LoginViewModel(
        IAuthService authService,
        SessionService sessionService,
        INavigationService navigationService,
        IDialogService dialogService,
        ILogger<LoginViewModel> logger)
    {
        _authService = authService;
        _sessionService = sessionService;
        _navigationService = navigationService;
        _dialogService = dialogService;
        _logger = logger;
    }

    [RelayCommand]
    private async TaskAsync LoginAsync()
    {
        _logger.LogInformation("Login command executed");
        ErrorMessage = null;
        
        if (string.IsNullOrWhiteSpace(Firstname) || string.IsNullOrWhiteSpace(Lastname) || string.IsNullOrWhiteSpace(Password))
        {
            _logger.LogWarning("Login validation failed: empty fields");
            ErrorMessage = "Please fill in all fields";
            return;
        }

        IsLoading = true;

        try
        {
            var user = await _authService.LoginAsync(Firstname, Lastname, Password);

            if (user == null)
            {
                _logger.LogWarning("Login failed: invalid credentials for {Firstname} {Lastname}", Firstname, Lastname);
                ErrorMessage = "Invalid credentials. Please check your name and password.";
                return;
            }

            _sessionService.SetUser(user);
            _logger.LogInformation("Login successful, navigating to TeamListView");
            _navigationService.NavigateTo<TeamListViewModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error for {Firstname} {Lastname}", Firstname, Lastname);
            ErrorMessage = $"An error occurred: {ex.Message}";
            _dialogService.ShowError(ex.Message, "Login Error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void NavigateToRegister()
    {
        _logger.LogInformation("Navigating to RegisterView");
        _navigationService.NavigateTo<RegisterViewModel>();
    }
}
