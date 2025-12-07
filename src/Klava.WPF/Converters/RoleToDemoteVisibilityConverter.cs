using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Klava.Domain.Enums;

namespace Klava.WPF.Converters;

public class RoleToDemoteVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TeamMemberRole role)
        {
            return role == TeamMemberRole.Headman ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
