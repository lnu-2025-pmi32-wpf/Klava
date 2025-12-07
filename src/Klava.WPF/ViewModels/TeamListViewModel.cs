using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        IDialogService dialogService)
    {
        _teamService = teamService;
        _sessionService = sessionService;
        _navigationService = navigationService;
        _dialogService = dialogService;

        LoadTeamsCommand.Execute(null);
    }

    [RelayCommand]
    private async TaskAsync LoadTeamsAsync()
    {
        if (!_sessionService.IsAuthenticated || _sessionService.CurrentUser == null)
        {
            return;
        }

        IsLoading = true;

        try
        {
            var teams = await _teamService.GetUserTeamsAsync(_sessionService.CurrentUser.Id);
            Teams.Clear();
            foreach (var team in teams)
            {
                Teams.Add(team);
            }
        }
        catch (Exception ex)
        {
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
            _dialogService.ShowError("Please enter a valid 8-character code.", "Invalid Code");
            return;
        }

        if (_sessionService.CurrentUser == null)
        {
            return;
        }

        try
        {
            var success = await _teamService.JoinTeamAsync(_sessionService.CurrentUser.Id, JoinCode.ToUpper());

            if (!success)
            {
                _dialogService.ShowError("Invalid code or you are already a member.", "Join Failed");
                return;
            }

            _dialogService.ShowMessage("Successfully joined the team!", "Success");
            ShowJoinDialog = false;
            await LoadTeamsAsync();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Failed to join team: {ex.Message}", "Error");
        }
    }

    [RelayCommand]
    private void ViewTeam(Team team)
    {
        if (team != null)
        {
            _navigationService.NavigateTo<TeamDashboardViewModel>(team.Id);
        }
    }

    [RelayCommand]
    private void Logout()
    {
        _sessionService.Logout();
        _navigationService.NavigateTo<LoginViewModel>();
    }
}
