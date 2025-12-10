using System.Windows;
using System.Windows.Controls;

namespace Klava.WPF.Controls;

public partial class BindablePasswordBox : UserControl
{
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register("Label", typeof(string), typeof(BindablePasswordBox), new PropertyMetadata("Пароль"));

    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register("Password", typeof(string), typeof(BindablePasswordBox), 
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string Password
    {
        get => (string)GetValue(PasswordProperty);
        set => SetValue(PasswordProperty, value);
    }

    private bool _isUpdating;

    public BindablePasswordBox()
    {
        InitializeComponent();
    }

    private void PbBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (!_isUpdating)
        {
            _isUpdating = true;
            Password = PbBox.Password;
            _isUpdating = false;
        }
    }
}