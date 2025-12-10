using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<CreateTeamViewModel> _logger;

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
        IDialogService dialogService,
        ILogger<CreateTeamViewModel> logger)
    {
        _teamService = teamService;
        _sessionService = sessionService;
        _navigationService = navigationService;
        _dialogService = dialogService;
        _logger = logger;
    }

    [RelayCommand]
    private async TaskAsync CreateTeamAsync()
    {
        _logger.LogInformation("CreateTeam command executed");
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(TeamName))
        {
            _logger.LogWarning("CreateTeam validation failed: empty team name");
            ErrorMessage = "Team name is required";
            return;
        }

        if (_sessionService.CurrentUser == null)
        {
            _logger.LogWarning("CreateTeam failed: user not authenticated");
            ErrorMessage = "You must be logged in to create a team";
            return;
        }

        IsLoading = true;

        try
        {
            _logger.LogInformation("Creating team {TeamName} for user {UserId}", TeamName, _sessionService.CurrentUser.Id);
            var team = await _teamService.CreateTeamAsync(TeamName, _sessionService.CurrentUser.Id);
            
            _logger.LogInformation("Team created: {TeamName} (ID: {TeamId}, Code: {TeamCode})", team.Name, team.Id, team.Code);
            _dialogService.ShowMessage($"Team created successfully! Team code: {team.Code}", "Success");
            _navigationService.NavigateTo<TeamDashboardViewModel>(team.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create team {TeamName}", TeamName);
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
        _logger.LogInformation("CreateTeam cancelled, navigating to TeamListView");
        _navigationService.NavigateTo<TeamListViewModel>();
    }
}
