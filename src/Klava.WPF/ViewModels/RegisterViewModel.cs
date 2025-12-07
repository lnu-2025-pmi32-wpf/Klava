using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Klava.Application.Services.Interfaces;
using Klava.WPF.Services;
using TaskAsync = System.Threading.Tasks.Task;

namespace Klava.WPF.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
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
    private string? _successMessage;

    [ObservableProperty]
    private bool _isLoading;

    public RegisterViewModel(
        IAuthService authService,
        INavigationService navigationService,
        IDialogService dialogService)
    {
        _authService = authService;
        _navigationService = navigationService;
        _dialogService = dialogService;
    }

    [RelayCommand]
    private async TaskAsync RegisterAsync()
    {
        ErrorMessage = null;
        SuccessMessage = null;

        // Basic validation
        if (string.IsNullOrWhiteSpace(Firstname) || string.IsNullOrWhiteSpace(Lastname))
        {
            ErrorMessage = "First name and last name are required";
            return;
        }

        if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
        {
            ErrorMessage = "Password must be at least 6 characters";
            return;
        }

        IsLoading = true;

        try
        {
            var user = await _authService.RegisterAsync(Firstname, Lastname, Password);

            if (user == null)
            {
                ErrorMessage = "User already exists with this name.";
                return;
            }

            _dialogService.ShowMessage("Registration successful! You can now log in.", "Success");
            _navigationService.NavigateTo<LoginViewModel>();
        }
        catch (Exception ex)
        {
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
        _navigationService.NavigateTo<LoginViewModel>();
    }
}
