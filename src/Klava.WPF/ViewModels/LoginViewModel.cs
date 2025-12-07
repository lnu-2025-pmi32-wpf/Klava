using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        IDialogService dialogService)
    {
        _authService = authService;
        _sessionService = sessionService;
        _navigationService = navigationService;
        _dialogService = dialogService;
    }

    [RelayCommand]
    private async TaskAsync LoginAsync()
    {
        ErrorMessage = null;
        
        // Basic validation
        if (string.IsNullOrWhiteSpace(Firstname) || string.IsNullOrWhiteSpace(Lastname) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please fill in all fields";
            return;
        }

        IsLoading = true;

        try
        {
            var user = await _authService.LoginAsync(Firstname, Lastname, Password);

            if (user == null)
            {
                ErrorMessage = "Invalid credentials. Please check your name and password.";
                return;
            }

            _sessionService.SetUser(user);
            _navigationService.NavigateTo<TeamListViewModel>();
        }
        catch (Exception ex)
        {
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
        _navigationService.NavigateTo<RegisterViewModel>();
    }
}
