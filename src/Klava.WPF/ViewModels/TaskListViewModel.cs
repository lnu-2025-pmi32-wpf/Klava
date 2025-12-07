using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Klava.Application.Services.Interfaces;
using Klava.Domain.Entities;
using Klava.WPF.Services;
using System.Collections.ObjectModel;
using TaskAsync = System.Threading.Tasks.Task;
using TaskEntity = Klava.Domain.Entities.Task;

namespace Klava.WPF.ViewModels;

public partial class TaskListViewModel : ViewModelBase, INavigationAware
{
    private readonly ITaskService _taskService;
    private readonly ISubjectService _subjectService;
    private readonly ITeamService _teamService;
    private readonly IMemberService _memberService;
    private readonly SessionService _sessionService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private int _teamId;

    [ObservableProperty]
    private int? _subjectId;

    [ObservableProperty]
    private Team? _team;

    [ObservableProperty]
    private Subject? _subject;

    [ObservableProperty]
    private ObservableCollection<TaskEntity> _tasks = new();

    [ObservableProperty]
    private ObservableCollection<Subject> _availableSubjects = new();

    [ObservableProperty]
    private bool _isHeadman;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _showCreateDialog;

    [ObservableProperty]
    private string _newTaskName = string.Empty;

    [ObservableProperty]
    private string _newTaskDescription = string.Empty;

    [ObservableProperty]
    private DateTime? _newTaskDeadline;

    [ObservableProperty]
    private int _newTaskSubjectId;

    [ObservableProperty]
    private TaskEntity? _editingTask;

    public TaskListViewModel(
        ITaskService taskService,
        ISubjectService subjectService,
        ITeamService teamService,
        IMemberService memberService,
        SessionService sessionService,
        IDialogService dialogService,
        INavigationService navigationService)
    {
        _taskService = taskService;
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
            SubjectId = null;
        }
        else if (parameter is { } obj)
        {
            var props = obj.GetType().GetProperties();
            var teamIdProp = props.FirstOrDefault(p => p.Name == "TeamId");
            var subjectIdProp = props.FirstOrDefault(p => p.Name == "SubjectId");

            if (teamIdProp != null)
                TeamId = (int)teamIdProp.GetValue(obj)!;
            if (subjectIdProp != null)
                SubjectId = (int)subjectIdProp.GetValue(obj)!;
        }

        _ = LoadDataAsync();
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

            if (SubjectId.HasValue)
            {
                Subject = await _subjectService.GetSubjectByIdAsync(SubjectId.Value);
                var tasksList = await _taskService.GetTasksBySubjectAsync(SubjectId.Value);
                Tasks.Clear();
                foreach (var task in tasksList)
                {
                    Tasks.Add(task);
                }
            }
            else
            {
                var tasksList = await _taskService.GetTasksByTeamAsync(TeamId);
                Tasks.Clear();
                foreach (var task in tasksList)
                {
                    Tasks.Add(task);
                }
            }

            var subjects = await _subjectService.GetSubjectsByTeamAsync(TeamId);
            AvailableSubjects.Clear();
            foreach (var subj in subjects)
            {
                AvailableSubjects.Add(subj);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading tasks: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void ShowCreateTask()
    {
        EditingTask = null;
        NewTaskName = string.Empty;
        NewTaskDescription = string.Empty;
        NewTaskDeadline = null;
        NewTaskSubjectId = SubjectId ?? (AvailableSubjects.FirstOrDefault()?.Id ?? 0);
        ShowCreateDialog = true;
    }

    [RelayCommand]
    private void ShowEditTask(TaskEntity task)
    {
        EditingTask = task;
        NewTaskName = task.Name;
        NewTaskDescription = task.Description ?? string.Empty;
        NewTaskDeadline = task.Deadline;
        NewTaskSubjectId = task.SubjectId;
        ShowCreateDialog = true;
    }

    [RelayCommand]
    private void CancelTaskDialog()
    {
        ShowCreateDialog = false;
        EditingTask = null;
    }

    [RelayCommand]
    private async TaskAsync SaveTaskAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTaskName))
        {
            _dialogService.ShowMessage("Please enter a task name.");
            return;
        }

        if (NewTaskSubjectId == 0)
        {
            _dialogService.ShowMessage("Please select a subject.");
            return;
        }

        try
        {
            if (EditingTask == null)
            {
                await _taskService.CreateTaskAsync(NewTaskSubjectId, NewTaskName, NewTaskDescription, NewTaskDeadline);
                _dialogService.ShowMessage("Task created successfully!");
            }
            else
            {
                await _taskService.UpdateTaskAsync(EditingTask.Id, NewTaskName, NewTaskDescription, NewTaskDeadline);
                _dialogService.ShowMessage("Task updated successfully!");
            }

            ShowCreateDialog = false;
            EditingTask = null;
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessage($"Error saving task: {ex.Message}");
        }
    }

    [RelayCommand]
    private async TaskAsync DeleteTaskAsync(TaskEntity task)
    {
        var confirmed = _dialogService.ShowConfirmation(
            $"Delete task '{task.Name}'?",
            "Confirm Delete");

        if (!confirmed)
            return;

        try
        {
            var success = await _taskService.DeleteTaskAsync(task.Id);
            if (success)
            {
                await LoadDataAsync();
                _dialogService.ShowMessage("Task deleted successfully!");
            }
            else
            {
                _dialogService.ShowMessage("Failed to delete task.");
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
        if (SubjectId.HasValue)
        {
            _navigationService.NavigateTo<SubjectListViewModel>(TeamId);
        }
        else
        {
            _navigationService.NavigateTo<TeamDashboardViewModel>(TeamId);
        }
    }
}
