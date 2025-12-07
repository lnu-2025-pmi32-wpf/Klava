using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Klava.Application.Services.Interfaces;
using Klava.Domain.Entities;
using Klava.Domain.Enums;
using Klava.WPF.Services;
using System.Collections.ObjectModel;
using TaskAsync = System.Threading.Tasks.Task;

namespace Klava.WPF.ViewModels;

public partial class ManageMembersViewModel : ViewModelBase, INavigationAware
{
    private readonly IMemberService _memberService;
    private readonly ITeamService _teamService;
    private readonly SessionService _sessionService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private int _teamId;

    [ObservableProperty]
    private Team? _team;

    [ObservableProperty]
    private ObservableCollection<TeamMember> _members = new();

    [ObservableProperty]
    private bool _isHeadman;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    public ManageMembersViewModel(
        IMemberService memberService,
        ITeamService teamService,
        SessionService sessionService,
        IDialogService dialogService,
        INavigationService navigationService)
    {
        _memberService = memberService;
        _teamService = teamService;
        _sessionService = sessionService;
        _dialogService = dialogService;
        _navigationService = navigationService;
    }

    public void OnNavigatedTo(object? parameter)
    {
        if (parameter is int teamId)
        {
            TeamId = teamId;
            _ = LoadDataAsync();
        }
    }

    [RelayCommand]
    private async TaskAsync LoadDataAsync()
    {
        if (_sessionService.CurrentUser == null)
            return;

        IsLoading = true;
        ErrorMessage = null;

        try
        {
            Team = await _teamService.GetTeamByIdAsync(TeamId);
            if (Team == null)
            {
                ErrorMessage = "Team not found.";
                return;
            }

            IsHeadman = await _memberService.IsHeadmanAsync(_sessionService.CurrentUser.Id, TeamId);

            if (!IsHeadman)
            {
                ErrorMessage = "You do not have permission to manage members.";
                return;
            }

            var membersList = await _memberService.GetTeamMembersAsync(TeamId);
            Members.Clear();
            foreach (var member in membersList)
            {
                Members.Add(member);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading members: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async TaskAsync PromoteToHeadmanAsync(TeamMember member)
    {
        if (member.UserId == _sessionService.CurrentUser?.Id)
        {
            _dialogService.ShowMessage("Cannot change your own role.");
            return;
        }

        var confirmed = _dialogService.ShowConfirmation(
            $"Promote {member.User.Firstname} {member.User.Lastname} to Headman?",
            "Confirm Promotion");

        if (!confirmed)
            return;

        try
        {
            var success = await _memberService.UpdateMemberRoleAsync(TeamId, member.UserId, TeamMemberRole.Headman);
            if (success)
            {
                await LoadDataAsync();
                _dialogService.ShowMessage("Member promoted successfully!");
            }
            else
            {
                _dialogService.ShowMessage("Failed to promote member.");
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage($"Error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async TaskAsync DemoteToStudentAsync(TeamMember member)
    {
        if (member.UserId == _sessionService.CurrentUser?.Id)
        {
            _dialogService.ShowMessage("Cannot change your own role.");
            return;
        }

        var confirmed = _dialogService.ShowConfirmation(
            $"Demote {member.User.Firstname} {member.User.Lastname} to Student?",
            "Confirm Demotion");

        if (!confirmed)
            return;

        try
        {
            var success = await _memberService.UpdateMemberRoleAsync(TeamId, member.UserId, TeamMemberRole.Student);
            if (success)
            {
                await LoadDataAsync();
                _dialogService.ShowMessage("Member demoted successfully!");
            }
            else
            {
                _dialogService.ShowMessage("Failed to demote member.");
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage($"Error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async TaskAsync RemoveMemberAsync(TeamMember member)
    {
        if (member.UserId == _sessionService.CurrentUser?.Id)
        {
            _dialogService.ShowMessage("Cannot remove yourself from the team.");
            return;
        }

        var confirmed = _dialogService.ShowConfirmation(
            $"Remove {member.User.Firstname} {member.User.Lastname} from the team?",
            "Confirm Removal");

        if (!confirmed)
            return;

        try
        {
            var success = await _memberService.RemoveMemberAsync(TeamId, member.UserId);
            if (success)
            {
                await LoadDataAsync();
                _dialogService.ShowMessage("Member removed successfully!");
            }
            else
            {
                _dialogService.ShowMessage("Failed to remove member.");
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage($"Error: {ex.Message}");
        }
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.NavigateTo<TeamDashboardViewModel>(TeamId);
    }
}
