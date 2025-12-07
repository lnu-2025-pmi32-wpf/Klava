using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Klava.Application.Services.Interfaces;
using Klava.Domain.Entities;
using Klava.WPF.Services;
using TaskAsync = System.Threading.Tasks.Task;

namespace Klava.WPF.ViewModels;

public partial class TeamDashboardViewModel : ViewModelBase, INavigationAware
{
    private readonly ITeamService _teamService;
    private readonly IMemberService _memberService;
    private readonly SessionService _sessionService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    private int _teamId;

    [ObservableProperty]
    private Team? _team;

    [ObservableProperty]
    private bool _isHeadman;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private ObservableCollection<TeamMember> _members = new();

    public TeamDashboardViewModel(
        ITeamService teamService,
        IMemberService memberService,
        SessionService sessionService,
        INavigationService navigationService,
        IDialogService dialogService)
    {
        _teamService = teamService;
        _memberService = memberService;
        _sessionService = sessionService;
        _navigationService = navigationService;
        _dialogService = dialogService;
    }

    public void OnNavigatedTo(object parameter)
    {
        if (parameter is int teamId)
        {
            _teamId = teamId;
            LoadTeamCommand.Execute(null);
        }
    }

    [RelayCommand]
    private async TaskAsync LoadTeamAsync()
    {
        if (_sessionService.CurrentUser == null)
        {
            return;
        }

        IsLoading = true;

        try
        {
            Team = await _teamService.GetTeamByIdAsync(_teamId);
            
            if (Team != null)
            {
                var members = await _memberService.GetTeamMembersAsync(_teamId);
                Members.Clear();
                foreach (var member in members)
                {
                    Members.Add(member);
                }

                IsHeadman = await _memberService.IsHeadmanAsync(_sessionService.CurrentUser.Id, _teamId);
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Failed to load team: {ex.Message}", "Error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void ManageMembers()
    {
        if (!IsHeadman)
        {
            _dialogService.ShowMessage("Only headmen can manage members.", "Access Denied");
            return;
        }

        _navigationService.NavigateTo<ManageMembersViewModel>(_teamId);
    }

    [RelayCommand]
    private void ViewSubjects()
    {
        _navigationService.NavigateTo<SubjectListViewModel>(_teamId);
    }

    [RelayCommand]
    private void ViewTasks()
    {
        _navigationService.NavigateTo<TaskListViewModel>(_teamId);
    }

    [RelayCommand]
    private void BackToTeamList()
    {
        _navigationService.NavigateTo<TeamListViewModel>();
    }
}
