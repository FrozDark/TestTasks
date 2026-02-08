using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TestModels;

namespace TestTasks.DataTemplates;

public sealed partial class DevicesHeaderTemplates : DataTemplateSelector
{
    public DataTemplate? PrinterTemplate { get; set; }
    public DataTemplate? MouseTemplate { get; set; }
    public DataTemplate? KeyboardTemplate { get; set; }
    public DataTemplate? InputDeviceTemplate { get; set; }
    public DataTemplate? OutputDeviceTemplate { get; set; }
    public DataTemplate? DefaultTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is Type type)
        {
            if (type == typeof(Printer))
            {
                return PrinterTemplate ?? base.SelectTemplate(item, container);
            }
            if (type == typeof(Keyboard))
            {
                return KeyboardTemplate ?? base.SelectTemplate(item, container);
            }
            if (type == typeof(Mouse))
            {
                return MouseTemplate ?? base.SelectTemplate(item, container);
            }
            if (type.IsAssignableTo(typeof(InputDevice)))
            {
                return InputDeviceTemplate ?? base.SelectTemplate(item, container);
            }
            if (type.IsAssignableTo(typeof(OutputDevice)))
            {
                return OutputDeviceTemplate ?? base.SelectTemplate(item, container);
            }
        }

        return DefaultTemplate ?? base.SelectTemplate(item, container);
    }
}
