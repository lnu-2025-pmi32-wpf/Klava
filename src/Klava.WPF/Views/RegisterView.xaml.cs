using System.Windows;
using System.Windows.Controls;
using Klava.WPF.ViewModels;

namespace Klava.WPF.Views;

/// <summary>
/// Interaction logic for RegisterView.xaml
/// </summary>
public partial class RegisterView : UserControl
{
    public RegisterView()
    {
        InitializeComponent();
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is RegisterViewModel viewModel)
        {
            viewModel.Password = PasswordBox.Password;
        }
    }
}
