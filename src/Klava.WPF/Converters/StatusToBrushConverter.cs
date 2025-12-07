using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Klava.Domain.Enums;

namespace Klava.WPF.Converters;

public class StatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SubjectStatus status)
        {
            return status == SubjectStatus.Exam 
                ? new SolidColorBrush(Color.FromRgb(33, 150, 243)) // Blue
                : new SolidColorBrush(Color.FromRgb(76, 175, 80));  // Green
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
