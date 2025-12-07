using System.Windows;

namespace Klava.WPF.Services;

public class DialogService : IDialogService
{
    public bool? ShowDialog(string title, object viewModel)
    {
        // This will be implemented later when we create dialog windows
        throw new NotImplementedException("Dialog windows will be implemented in later phases");
    }

    public bool ShowConfirmation(string message, string title = "Confirm")
    {
        var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        return result == MessageBoxResult.Yes;
    }

    public void ShowMessage(string message, string title = "Information")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public void ShowError(string message, string title = "Error")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
