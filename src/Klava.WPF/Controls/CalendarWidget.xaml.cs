using System.Windows;
using System.Windows.Controls;

namespace Klava.WPF.Views.Controls
{
    public partial class CalendarWidget : UserControl
    {
        public CalendarWidget()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DeadlinesProperty =
            DependencyProperty.Register(
                nameof(Deadlines),
                typeof(IEnumerable<DateTime>),
                typeof(CalendarWidget),
                new PropertyMetadata(null));

        public IEnumerable<DateTime> Deadlines
        {
            get => (IEnumerable<DateTime>)GetValue(DeadlinesProperty);
            set => SetValue(DeadlinesProperty, value);
        }
    }
}