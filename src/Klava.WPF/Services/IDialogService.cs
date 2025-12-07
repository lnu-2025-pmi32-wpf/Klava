namespace Klava.WPF.Services;

public interface IDialogService
{
    bool? ShowDialog(string title, object viewModel);
    bool ShowConfirmation(string message, string title = "Confirm");
    void ShowMessage(string message, string title = "Information");
    void ShowError(string message, string title = "Error");
}
