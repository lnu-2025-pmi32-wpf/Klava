using System.Windows.Controls;
using System.Windows.Input;

namespace Klava.WPF.Views;

public partial class TaskListView : UserControl
{
    public TaskListView()
    {
        InitializeComponent();
    }

    private void Overlay_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source == sender && DataContext is ViewModels.TaskListViewModel vm)
        {
            vm.CancelTaskDialogCommand.Execute(null);
        }
    }
}
