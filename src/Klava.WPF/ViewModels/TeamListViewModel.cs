using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Klava.Application.Services.Interfaces;
using Klava.Domain.Entities;
using Klava.WPF.Services;
using TaskAsync = System.Threading.Tasks.Task;

namespace Klava.WPF.ViewModels;

public partial class TeamListViewModel : ViewModelBase
{
    private readonly ITeamService _teamService;
    private readonly SessionService _sessionService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;
    private readonly ILogger<TeamListViewModel> _logger;

    [ObservableProperty]
    private ObservableCollection<Team> _teams = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _joinCode = string.Empty;

    [ObservableProperty]
    private bool _showJoinDialog;

    public TeamListViewModel(
        ITeamService teamService,
        SessionService sessionService,
        INavigationService navigationService,
        IDialogService dialogService,
        ILogger<TeamListViewModel> logger)
    {
        _teamService = teamService;
        _sessionService = sessionService;
        _navigationService = navigationService;
        _dialogService = dialogService;
        _logger = logger;

        LoadTeamsCommand.Execute(null);
    }

    [RelayCommand]
    private async TaskAsync LoadTeamsAsync()
    {
        if (!_sessionService.IsAuthenticated || _sessionService.CurrentUser == null)
        {
            _logger.LogWarning("LoadTeams called without authenticated user");
            return;
        }

        _logger.LogInformation("Loading teams for user {UserId}", _sessionService.CurrentUser.Id);
        IsLoading = true;

        try
        {
            var teams = await _teamService.GetUserTeamsAsync(_sessionService.CurrentUser.Id);
            Teams.Clear();
            foreach (var team in teams)
            {
                Teams.Add(team);
            }
            _logger.LogInformation("Loaded {TeamCount} teams", teams.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load teams");
            _dialogService.ShowError($"Failed to load teams: {ex.Message}", "Error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void CreateTeam()
    {
        _logger.LogInformation("Navigating to CreateTeamView");
        _navigationService.NavigateTo<CreateTeamViewModel>();
    }

    [RelayCommand]
    private void OpenJoinDialog()
    {
        JoinCode = string.Empty;
        ShowJoinDialog = true;
    }

    [RelayCommand]
    private void CloseJoinDialog()
    {
        ShowJoinDialog = false;
    }

    [RelayCommand]
    private async TaskAsync JoinTeamAsync()
    {
        if (string.IsNullOrWhiteSpace(JoinCode) || JoinCode.Length != 8)
        {
            _logger.LogWarning("Join team validation failed: invalid code format");
            _dialogService.ShowError("Please enter a valid 8-character code.", "Invalid Code");
            return;
        }

        if (_sessionService.CurrentUser == null)
        {
            return;
        }

        _logger.LogInformation("User {UserId} attempting to join team with code {TeamCode}", _sessionService.CurrentUser.Id, JoinCode);

        try
        {
            var success = await _teamService.JoinTeamAsync(_sessionService.CurrentUser.Id, JoinCode.ToUpper());

            if (!success)
            {
                _logger.LogWarning("Join team failed for code {TeamCode}", JoinCode);
                _dialogService.ShowError("Invalid code or you are already a member.", "Join Failed");
                return;
            }

            _logger.LogInformation("Successfully joined team with code {TeamCode}", JoinCode);
            _dialogService.ShowMessage("Successfully joined the team!", "Success");
            ShowJoinDialog = false;
            await LoadTeamsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining team with code {TeamCode}", JoinCode);
            _dialogService.ShowError($"Failed to join team: {ex.Message}", "Error");
        }
    }

    [RelayCommand]
    private void ViewTeam(Team team)
    {
        if (team != null)
        {
            _logger.LogInformation("Navigating to TeamDashboard for team {TeamId}", team.Id);
            _navigationService.NavigateTo<TeamDashboardViewModel>(team.Id);
        }
    }

    [RelayCommand]
    private void Logout()
    {
        _logger.LogInformation("User logging out");
        _sessionService.Logout();
        _navigationService.NavigateTo<LoginViewModel>();
    }
}
