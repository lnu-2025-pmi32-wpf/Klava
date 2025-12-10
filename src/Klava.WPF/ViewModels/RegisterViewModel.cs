using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Klava.Application.Services.Interfaces;
using Klava.WPF.Services;
using TaskAsync = System.Threading.Tasks.Task;

namespace Klava.WPF.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;
    private readonly ILogger<RegisterViewModel> _logger;

    [ObservableProperty]
    private string _firstname = string.Empty;

    [ObservableProperty]
    private string _lastname = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private string? _successMessage;

    [ObservableProperty]
    private bool _isLoading;

    public RegisterViewModel(
        IAuthService authService,
        INavigationService navigationService,
        IDialogService dialogService,
        ILogger<RegisterViewModel> logger)
    {
        _authService = authService;
        _navigationService = navigationService;
        _dialogService = dialogService;
        _logger = logger;
    }

    [RelayCommand]
    private async TaskAsync RegisterAsync()
    {
        _logger.LogInformation("Register command executed");
        ErrorMessage = null;
        SuccessMessage = null;

        if (string.IsNullOrWhiteSpace(Firstname) || string.IsNullOrWhiteSpace(Lastname))
        {
            _logger.LogWarning("Registration validation failed: empty name fields");
            ErrorMessage = "First name and last name are required";
            return;
        }

        if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
        {
            _logger.LogWarning("Registration validation failed: invalid password");
            ErrorMessage = "Password must be at least 6 characters";
            return;
        }

        IsLoading = true;

        try
        {
            var user = await _authService.RegisterAsync(Firstname, Lastname, Password);

            if (user == null)
            {
                _logger.LogWarning("Registration failed: user {Firstname} {Lastname} already exists", Firstname, Lastname);
                ErrorMessage = "User already exists with this name.";
                return;
            }

            _logger.LogInformation("Registration successful for {Firstname} {Lastname}, navigating to LoginView", Firstname, Lastname);
            _dialogService.ShowMessage("Registration successful! You can now log in.", "Success");
            _navigationService.NavigateTo<LoginViewModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration error for {Firstname} {Lastname}", Firstname, Lastname);
            ErrorMessage = $"An error occurred: {ex.Message}";
            _dialogService.ShowError(ex.Message, "Registration Error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void NavigateToLogin()
    {
        _logger.LogInformation("Navigating to LoginView");
        _navigationService.NavigateTo<LoginViewModel>();
    }
}
