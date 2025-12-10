using System.Windows;
using System.Windows.Controls;

namespace Klava.WPF.Views;

/// <summary>
/// Interaction logic for TeamListView.xaml
/// </summary>
public partial class TeamListView : UserControl
{
    public TeamListView()
    {
        InitializeComponent();
    }

    private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        // Close dialog when clicking on background overlay
        if (sender is Border border && DataContext is ViewModels.TeamListViewModel vm)
        {
            vm.CloseJoinDialogCommand.Execute(null);
        }
    }
}
