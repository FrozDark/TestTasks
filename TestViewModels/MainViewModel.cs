using System.ComponentModel;
using TestViewModels.Dialogs;

namespace TestViewModels;

public sealed partial class MainViewModel : NavigationPageViewModelBase
{
    public MainViewModel()
    {
        NavigateToInstance<DevicesViewModel>();
    }

    public override MainViewModel? Main => this;

    protected override void OnCurrentPagePropertyChanged(PageViewModelBase page, PropertyChangedEventArgs e)
    {
        if (e.PropertyName!.Equals(nameof(IsHeaderVisible)))
        {
            IsHeaderVisible = page.IsHeaderVisible;
        }
        else
        {
            base.OnCurrentPagePropertyChanged(page, e);
        }
    }

    // Для отображение ошибок
    internal void OnExceptionThrown(object sender, DialogsManager dialogs, Exception ex, string methodName, string filePath, int lineNumber)
    {
        GlobalExceptionHandler(sender, ex, methodName, filePath, lineNumber);

        // Здесь можно отлавливать разные исключения и отображать разные диалоги с соответствующей информацией.
        // Но пока что обобщённо показываем всё, что выдаст нам рантайм
        dialogs.PushDialog(new ErrorDialogViewModel(ex.ToString()));
    }

    public override bool OnBackHardwareRequested()
    {
        if (base.OnBackHardwareRequested())
        {
            return true;
        }

        return false;
    }

    // Для логов
    internal static void GlobalExceptionHandler(object sender, Exception ex, string methodName, string filePath, int lineNumber)
    {

    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
    }
}