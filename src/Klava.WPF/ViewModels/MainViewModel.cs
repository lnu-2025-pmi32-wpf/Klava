using CommunityToolkit.Mvvm.ComponentModel;
using Klava.WPF.Services;

namespace Klava.WPF.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly SessionService _sessionService;

    [ObservableProperty]
    private ViewModelBase? _currentViewModel;

    [ObservableProperty]
    private string _currentUserName = "Guest";

    [ObservableProperty]
    private bool _isAuthenticated;

    public MainViewModel(INavigationService navigationService, SessionService sessionService)
    {
        _navigationService = navigationService;
        _sessionService = sessionService;

        // Subscribe to navigation changes
        _navigationService.OnNavigated += OnNavigated;
        
        // Subscribe to session changes
        _sessionService.OnUserChanged += OnSessionChanged;

        // Set initial view
        CurrentViewModel = _navigationService.CurrentViewModel;
        UpdateAuthenticationState();
    }

    private void OnNavigated()
    {
        CurrentViewModel = _navigationService.CurrentViewModel;
    }

    private void OnSessionChanged()
    {
        UpdateAuthenticationState();
    }

    private void UpdateAuthenticationState()
    {
        IsAuthenticated = _sessionService.IsAuthenticated;
        CurrentUserName = _sessionService.GetUserDisplayName();
    }
}
