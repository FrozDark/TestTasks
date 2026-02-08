using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace TestTasks.Converters;

public sealed partial class IsNotNullZeroOrEmptyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var result = !GetResult(value);
        if (targetType == typeof(Visibility))
        {
            return result ? Visibility.Visible : Visibility.Collapsed;
        }
        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    static bool GetResult(object value)
    {
        if (value is null)
        {
            return true;
        }
        if (value.GetType().IsPrimitive && value is IConvertible convertible)
        {
            var num = convertible.ToInt32(CultureInfo.InvariantCulture);
            return num == 0;
        }
        if (value is string str)
        {
            return str.Length == 0;
        }
        if (value is Array array)
        {
            return array.Length == 0;
        }
        if (value is ICollection colection)
        {
            return colection.Count == 0;
        }
        if (value is IEnumerable enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            enumerator.Reset();
            return !enumerator.MoveNext();
        }
        return false;
    }
}
