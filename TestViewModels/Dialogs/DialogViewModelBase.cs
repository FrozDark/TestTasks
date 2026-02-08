using TestViewModels.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestViewModels.Dialogs;

public abstract partial class DialogViewModelBase : ViewModelBase, IDialog, IExceptionHandler
{
    DialogsManager? _dialogs;
    public DialogsManager? Dialogs
    {
        get => _dialogs;
        private set
        {
            if (_dialogs != value)
            {
                _dialogs = value;
            }
        }
    }

    public PageViewModelBase? Page => Dialogs?.Page;
    public NavigationPageViewModelBase? Navigation => Page?.Navigation;

    [EditorBrowsable(EditorBrowsableState.Never)]
    DialogsManager? IDialog.DialogsManager { get => Dialogs; set => Dialogs = value; }

    public event EventHandler? DismissRequested;

    public void RequestDismiss()
    {
        if (OnDismissRequested())
        {
            Dismiss();
        }
    }

    protected virtual bool OnDismissRequested()
    {
        return true;
    }

    protected virtual void Dismiss()
    {
        DismissRequested?.Invoke(this, EventArgs.Empty);
        OnDismissed();
    }

    protected virtual void OnDialogShown()
    {

    }

    protected virtual void OnDialogHidden()
    {

    }

    protected virtual void OnDialogAdded()
    {

    }

    protected virtual void OnDialogRemoved()
    {

    }

    protected virtual void OnDismissed()
    {

    }

    // [CallerArgumentExpression(nameof(value))]
    protected void HandleException(Exception ex, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        if (Dialogs is null)
        {
            MainViewModel.GlobalExceptionHandler(this, ex, methodName, filePath, lineNumber);
        }
        else
        {
            Unsafe.As<IDialogExceptionHandler>(Dialogs).HandleException(this, ex, methodName, filePath, lineNumber);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual bool OnBackHardwareRequested()
    {
        RequestDismiss();
        return true;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void ForcedDismiss()
    {

    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    void IDialog.ForcedDismiss()
    {
        ForcedDismiss();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    void IDialog.OnDialogHidden()
    {
        OnDialogHidden();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    void IDialog.OnDialogShown()
    {
        OnDialogShown();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    void IDialog.OnDialogAdded()
    {
        OnDialogAdded();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    void IDialog.OnDialogRemoved()
    {
        OnDialogRemoved();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    bool IDialog.OnBackHardwareRequested()
    {
        return OnBackHardwareRequested();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    void IExceptionHandler.HandleException(object sender, Exception ex, string methodName, string filePath, int lineNumber)
        =>Unsafe.As<IExceptionHandler>(Dialogs)?.HandleException(sender, ex, methodName, filePath, lineNumber);
}
