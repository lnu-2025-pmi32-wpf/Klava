using System.Windows.Controls;
using System.Windows.Input;

namespace Klava.WPF.Views;

public partial class SubjectListView : UserControl
{
    public SubjectListView()
    {
        InitializeComponent();
    }

    private void Overlay_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source == sender && DataContext is ViewModels.SubjectListViewModel vm)
        {
            vm.CancelCreateCommand.Execute(null);
        }
    }
}
