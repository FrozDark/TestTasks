using System.ComponentModel;
using System.Runtime.CompilerServices;
using TestViewModels.Dialogs;
using TestViewModels.Interfaces;

namespace TestViewModels;

public enum NavigatingType
{
    Root,
    Back,
    Forward,
    New
}

public interface INavigatable
{
    NavigationPageViewModelBase? Navigation { get; set; }

    bool OnNavigatingInternal(PageViewModelBase? fromPage, NavigatingType type);

    void OnNavigatedInternal(PageViewModelBase? fromPage, NavigatingType type);

    bool OnNavigatingFromInternal(NavigatingType type);

    void OnNavigatedFromInternal(PageViewModelBase? toPage, NavigatingType type);
}

/// <summary>
/// An abstract class for enabling page navigation.
/// </summary>
public abstract partial class PageViewModelBase : ViewModelBase, INavigatable, IDialogExceptionHandler, IExceptionHandler
{
    bool _isBusy;
    string? _title;
    bool _isHeaderVisible = true;
    bool _isBackButtonVisible = true;
    bool? _canNavigatePrevious;
    DialogsManager? _dialogs;

    internal PageViewModelBase()
    {
    }

    public virtual DialogsManager? Dialogs => _dialogs ??= new(this);

    public virtual bool IsBusy
    {
        get => _isBusy;
        protected set => SetProperty(ref _isBusy, value);
    }

    public virtual bool IsHeaderVisible
    {
        get => _isHeaderVisible;
        protected set => SetProperty(ref _isHeaderVisible, value);
    }

    public virtual string? Title
    {
        get => _title;
        protected set => SetProperty(ref _title, value);
    }

    public virtual bool IsBackButtonVisible
    {
        get => _isBackButtonVisible;
        protected set => SetProperty(ref _isBackButtonVisible, value);
    }

    public virtual MainViewModel? Main => Navigation?.Main;

    public virtual NavigationPageViewModelBase? Navigation { get; protected set; }

    private void Instance_LanguageChanged(object? sender, string lang)
    {
        OnPropertyChanged(nameof(Title));
    }

    /// <summary>
    /// Gets if the user can navigate to the previous page
    /// </summary>
    public virtual bool? CanNavigatePrevious
    {
        get => _canNavigatePrevious;
        protected set => SetProperty(ref _canNavigatePrevious, value);
    }

    /// <summary>
    /// Для мобилок. Физическая кнопка назад
    /// </summary>
    public virtual bool OnBackHardwareRequested()
    {
        return false;
    }

    // [CallerArgumentExpression(nameof(value))]
    protected void HandleException(Exception ex, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        HandleException(this, ex, methodName, filePath, lineNumber);
    }

    protected void HandleException(object sender, Exception ex, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        if (Main is null)
        {
            MainViewModel.GlobalExceptionHandler(sender, ex, methodName, filePath, lineNumber);
        }
        else
        {
            Main.OnExceptionThrown(sender, Dialogs!, ex, methodName, filePath, lineNumber);
        }
    }

    protected void HandleDialogException(IDialog sender, Exception ex, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        if (Main is null)
        {
            MainViewModel.GlobalExceptionHandler(sender, ex, methodName, filePath, lineNumber);
        }
        else
        {
            Main.OnExceptionThrown(sender, Dialogs!, ex, methodName, filePath, lineNumber);
        }
    }

    protected virtual bool OnNavigatingTo(PageViewModelBase? fromPage, NavigatingType type)
    {
        return true;
    }

    protected virtual void OnNavigatedTo(PageViewModelBase? fromPage, NavigatingType type)
    {

    }

    protected virtual bool OnNavigatingFrom(NavigatingType type)
    {
        return true;
    }

    protected virtual void OnNavigatedFrom(PageViewModelBase? toPage, NavigatingType type)
    {

    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    void IExceptionHandler.HandleException(object sender, Exception ex, string methodName, string filePath, int lineNumber)
        => HandleException(sender, ex, methodName, filePath, lineNumber);

    [EditorBrowsable(EditorBrowsableState.Never)]
    void IDialogExceptionHandler.HandleException(IDialog sender, Exception ex, string methodName, string filePath, int lineNumber)
        => HandleDialogException(sender, ex, methodName, filePath, lineNumber);

    [EditorBrowsable(EditorBrowsableState.Never)]
    bool INavigatable.OnNavigatingInternal(PageViewModelBase? fromPage, NavigatingType type)
        => OnNavigatingTo(fromPage, type);

    [EditorBrowsable(EditorBrowsableState.Never)]
    void INavigatable.OnNavigatedInternal(PageViewModelBase? fromPage, NavigatingType type)
        => OnNavigatedTo(fromPage, type);

    [EditorBrowsable(EditorBrowsableState.Never)]
    void INavigatable.OnNavigatedFromInternal(PageViewModelBase? toPage, NavigatingType type)
    {
        OnNavigatedFrom(toPage, type);
        if (_dialogs is not null &&
            (type == NavigatingType.Back || type == NavigatingType.Root))
        {
            _dialogs.Clear();
            _dialogs = null;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    bool INavigatable.OnNavigatingFromInternal(NavigatingType type)
        => OnNavigatingFrom(type);

    [EditorBrowsable(EditorBrowsableState.Never)]
    NavigationPageViewModelBase? INavigatable.Navigation { get => Navigation; set => Navigation = value; }
}
