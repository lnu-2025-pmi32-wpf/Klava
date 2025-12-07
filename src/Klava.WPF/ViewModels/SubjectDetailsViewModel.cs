using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Klava.Application.DTOs;
using Klava.Application.Services.Interfaces;
using Klava.Domain.Entities;
using Klava.WPF.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using TaskAsync = System.Threading.Tasks.Task;

namespace Klava.WPF.ViewModels;

public partial class SubjectDetailsViewModel : ViewModelBase, INavigationAware
{
    private readonly ISubjectService _subjectService;
    private readonly ISubjectFileService _fileService;
    private readonly IMemberService _memberService;
    private readonly SessionService _sessionService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private int _subjectId;

    [ObservableProperty]
    private Subject? _subject;

    [ObservableProperty]
    private ObservableCollection<SubjectFile> _files = new();

    [ObservableProperty]
    private bool _isHeadman;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _statusMessage;

    public SubjectDetailsViewModel(
        ISubjectService subjectService,
        ISubjectFileService fileService,
        IMemberService memberService,
        SessionService sessionService,
        IDialogService dialogService,
        INavigationService navigationService)
    {
        _subjectService = subjectService;
        _fileService = fileService;
        _memberService = memberService;
        _sessionService = sessionService;
        _dialogService = dialogService;
        _navigationService = navigationService;
    }

    public void OnNavigatedTo(object? parameter)
    {
        if (parameter is int subjectId)
        {
            SubjectId = subjectId;
            _ = LoadDataAsync();
        }
    }

    [RelayCommand]
    private async TaskAsync LoadDataAsync()
    {
        if (_sessionService.CurrentUser == null) return;

        IsLoading = true;
        StatusMessage = null;

        try
        {
            Subject = await _subjectService.GetSubjectByIdAsync(SubjectId);
            
            if (Subject != null)
            {
                var files = await _fileService.GetFilesBySubjectAsync(SubjectId);
                Files.Clear();
                foreach (var file in files)
                {
                    Files.Add(file);
                }

                IsHeadman = await _memberService.IsHeadmanAsync(_sessionService.CurrentUser.Id, Subject.TeamId);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async TaskAsync UploadFileAsync()
    {
        if (!IsHeadman)
        {
            _dialogService.ShowMessage("Only headmen can upload files.", "Access Denied");
            return;
        }

        var openFileDialog = new OpenFileDialog
        {
            Title = "Select files to upload",
            Multiselect = true,
            Filter = "All Files (*.*)|*.*"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            IsLoading = true;
            StatusMessage = "Uploading files...";

            try
            {
                int successCount = 0;
                
                foreach (var fileName in openFileDialog.FileNames)
                {
                    try
                    {
                        var fileInfo = new FileInfo(fileName);
                        
                        // Check file size (max 10MB)
                        if (fileInfo.Length > 10 * 1024 * 1024)
                        {
                            _dialogService.ShowError($"File {fileInfo.Name} is too large (max 10MB)", "Upload Error");
                            continue;
                        }

                        using var stream = File.OpenRead(fileName);
                        var request = new UploadFileRequest
                        {
                            SubjectId = SubjectId,
                            FileName = fileInfo.Name,
                            ContentType = GetContentType(fileInfo.Extension),
                            Size = fileInfo.Length,
                            FileStream = stream
                        };

                        var result = await _fileService.UploadFileAsync(request);
                        if (result != null)
                        {
                            successCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _dialogService.ShowError($"Error uploading {Path.GetFileName(fileName)}: {ex.Message}", "Upload Error");
                    }
                }

                if (successCount > 0)
                {
                    StatusMessage = $"Successfully uploaded {successCount} file(s)";
                    await LoadDataAsync();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Upload failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    [RelayCommand]
    private async TaskAsync DownloadFileAsync(SubjectFile file)
    {
        try
        {
            var saveFileDialog = new SaveFileDialog
            {
                FileName = file.DisplayName,
                Filter = "All Files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                IsLoading = true;
                StatusMessage = "Downloading file...";

                var stream = await _fileService.DownloadFileAsync(file.Id);
                if (stream != null)
                {
                    using var fileStream = File.Create(saveFileDialog.FileName);
                    await stream.CopyToAsync(fileStream);
                    
                    StatusMessage = "File downloaded successfully";
                    _dialogService.ShowMessage($"File saved to: {saveFileDialog.FileName}", "Download Complete");
                }
                else
                {
                    _dialogService.ShowError("File not found", "Download Error");
                }
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Download failed: {ex.Message}";
            _dialogService.ShowError(ex.Message, "Download Error");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async TaskAsync DeleteFileAsync(SubjectFile file)
    {
        if (!IsHeadman)
        {
            _dialogService.ShowMessage("Only headmen can delete files.", "Access Denied");
            return;
        }

        var confirmed = _dialogService.ShowConfirmation(
            $"Are you sure you want to delete '{file.DisplayName}'?",
            "Confirm Delete");

        if (confirmed)
        {
            IsLoading = true;
            StatusMessage = "Deleting file...";

            try
            {
                var success = await _fileService.DeleteFileAsync(file.Id);
                if (success)
                {
                    StatusMessage = "File deleted successfully";
                    await LoadDataAsync();
                }
                else
                {
                    _dialogService.ShowError("Failed to delete file", "Delete Error");
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Delete failed: {ex.Message}";
                _dialogService.ShowError(ex.Message, "Delete Error");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    [RelayCommand]
    private void GoBack()
    {
        if (Subject != null)
        {
            _navigationService.NavigateTo<SubjectListViewModel>(Subject.TeamId);
        }
    }

    private string GetContentType(string extension)
    {
        return extension.ToLower() switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".txt" => "text/plain",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".zip" => "application/zip",
            _ => "application/octet-stream"
        };
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
