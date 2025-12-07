using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Klava.Application.Services.Interfaces;
using Klava.Domain.Entities;
using Klava.Domain.Enums;
using Klava.WPF.Services;
using System.Collections.ObjectModel;
using TaskAsync = System.Threading.Tasks.Task;

namespace Klava.WPF.ViewModels;

public partial class SubjectListViewModel : ViewModelBase, INavigationAware
{
    private readonly ISubjectService _subjectService;
    private readonly ITeamService _teamService;
    private readonly IMemberService _memberService;
    private readonly SessionService _sessionService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private int _teamId;

    [ObservableProperty]
    private Team? _team;

    [ObservableProperty]
    private ObservableCollection<Subject> _subjects = new();

    [ObservableProperty]
    private bool _isHeadman;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private string _newSubjectTitle = string.Empty;

    [ObservableProperty]
    private string _newSubjectInfo = string.Empty;

    [ObservableProperty]
    private SubjectStatus _newSubjectStatus = SubjectStatus.Exam;

    [ObservableProperty]
    private bool _showCreateDialog;

    public SubjectListViewModel(
        ISubjectService subjectService,
        ITeamService teamService,
        IMemberService memberService,
        SessionService sessionService,
        IDialogService dialogService,
        INavigationService navigationService)
    {
        _subjectService = subjectService;
        _teamService = teamService;
        _memberService = memberService;
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

            var subjectsList = await _subjectService.GetSubjectsByTeamAsync(TeamId);
            Subjects.Clear();
            foreach (var subject in subjectsList)
            {
                Subjects.Add(subject);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading subjects: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void ShowCreateSubject()
    {
        NewSubjectTitle = string.Empty;
        NewSubjectInfo = string.Empty;
        NewSubjectStatus = SubjectStatus.Exam;
        ShowCreateDialog = true;
    }

    [RelayCommand]
    private void CancelCreate()
    {
        ShowCreateDialog = false;
    }

    [RelayCommand]
    private async TaskAsync CreateSubjectAsync()
    {
        if (string.IsNullOrWhiteSpace(NewSubjectTitle))
        {
            _dialogService.ShowMessage("Please enter a subject title.");
            return;
        }

        try
        {
            await _subjectService.CreateSubjectAsync(TeamId, NewSubjectTitle, NewSubjectInfo, NewSubjectStatus);
            ShowCreateDialog = false;
            await LoadDataAsync();
            _dialogService.ShowMessage("Subject created successfully!");
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage($"Error creating subject: {ex.Message}");
        }
    }

    [RelayCommand]
    private async TaskAsync DeleteSubjectAsync(Subject subject)
    {
        var confirmed = _dialogService.ShowConfirmation(
            $"Delete subject '{subject.Title}'? This will also delete all tasks in this subject.",
            "Confirm Delete");

        if (!confirmed)
            return;

        try
        {
            var success = await _subjectService.DeleteSubjectAsync(subject.Id);
            if (success)
            {
                await LoadDataAsync();
                _dialogService.ShowMessage("Subject deleted successfully!");
            }
            else
            {
                _dialogService.ShowMessage("Failed to delete subject.");
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage($"Error: {ex.Message}");
        }
    }

    [RelayCommand]
    private void ViewTasks(Subject subject)
    {
        _navigationService.NavigateTo<TaskListViewModel>(new { TeamId, SubjectId = subject.Id });
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.NavigateTo<TeamDashboardViewModel>(TeamId);
    }
}
