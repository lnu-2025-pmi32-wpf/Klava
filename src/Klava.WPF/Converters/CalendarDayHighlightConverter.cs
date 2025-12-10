using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Klava.WPF.Converters
{
    public class CalendarDayHighlightConverter : IMultiValueConverter
    {
        private readonly SolidColorBrush _deadlineBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5252")); // Яскравий червоний
        private readonly SolidColorBrush _todayBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3"));    // Яскравий синій
        private readonly SolidColorBrush _transparentBrush = Brushes.Transparent;
        
        private readonly SolidColorBrush _whiteBrush = Brushes.White;
        private readonly SolidColorBrush _blackBrush = Brushes.Black;
        private readonly SolidColorBrush _grayBrush = Brushes.Gray;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 1 || values[0] == null || !(values[0] is DateTime date))
                return parameter as string == "Foreground" ? _blackBrush : _transparentBrush;

            var deadlines = values.Length > 1 ? values[1] as IEnumerable<DateTime> : null;
            string mode = parameter as string;

            if (deadlines != null && deadlines.Any(d => d.Date == date.Date))
            {
                return mode == "Foreground" ? _whiteBrush : _deadlineBrush;
            }

            if (date.Date == DateTime.Today)
            {
                return mode == "Foreground" ? _whiteBrush : _todayBrush;
            }

            return mode == "Foreground" ? _blackBrush : _transparentBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}