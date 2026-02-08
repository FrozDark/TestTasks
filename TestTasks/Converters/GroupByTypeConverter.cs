using System.Globalization;
using System.Windows.Data;

namespace TestTasks.Converters;


public sealed partial class GroupByTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.GetType();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
