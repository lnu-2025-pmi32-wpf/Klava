using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Klava.Application.Services.Interfaces;
using Klava.WPF.Services;
using TaskAsync = System.Threading.Tasks.Task;

namespace Klava.WPF.ViewModels;

public partial class CreateTeamViewModel : ViewModelBase
{
    private readonly ITeamService _teamService;
    private readonly SessionService _sessionService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private string _teamName = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _isLoading;

    public CreateTeamViewModel(
        ITeamService teamService,
        SessionService sessionService,
        INavigationService navigationService,
        IDialogService dialogService)
    {
        _teamService = teamService;
        _sessionService = sessionService;
        _navigationService = navigationService;
        _dialogService = dialogService;
    }

    [RelayCommand]
    private async TaskAsync CreateTeamAsync()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(TeamName))
        {
            ErrorMessage = "Team name is required";
            return;
        }

        if (_sessionService.CurrentUser == null)
        {
            ErrorMessage = "You must be logged in to create a team";
            return;
        }

        IsLoading = true;

        try
        {
            var team = await _teamService.CreateTeamAsync(TeamName, _sessionService.CurrentUser.Id);
            
            _dialogService.ShowMessage($"Team created successfully! Team code: {team.Code}", "Success");
            _navigationService.NavigateTo<TeamDashboardViewModel>(team.Id);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to create team: {ex.Message}";
            _dialogService.ShowError(ex.Message, "Create Team Error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        _navigationService.NavigateTo<TeamListViewModel>();
    }
}
