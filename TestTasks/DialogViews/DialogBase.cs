using System.Windows;
using System.Windows.Controls;

namespace TestTasks.DialogViews;

public abstract partial class DialogBase : Border
{
    public DialogBase()
    {
        SetResourceReference(StyleProperty, typeof(DialogBase));

        Loaded += (s, e) =>
        {
            Focus();
        };
    }
}
