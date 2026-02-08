using System.Globalization;
using System.Windows.Data;
using TestModels;

namespace TestTasks.Converters;

public sealed partial class GroupNameConverter : IValueConverter
{
    // Хотел сделать через текст, но подумал что для групп лучше иметь свой шаблон с поддержкой иконок, к примеру
    // Поэтому конвертер этот не задействован
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Type type)
        {
            if (type == typeof(Keyboard))
            {
                return "Клавиатуры";
            }
            if (type == typeof(Mouse))
            {
                return "Мыши";
            }
            if (type == typeof(Printer))
            {
                return "Принтеры";
            }

            // Если типы выше не определены всеми модели, можем ориентироваться от базовых
            if (type.IsAssignableTo(typeof(InputDevice)))
            {
                return "Устройство ввода";
            }
            if (type.IsAssignableTo(typeof(OutputDevice)))
            {
                return "Устройство вывода";
            }
        }

        // Неизвестные устройства
        return "Неизвестно";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
