using CommunityToolkit.Mvvm.ComponentModel;
using TestViewModels.Data;
using TestViewModels.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestViewModels.Dialogs;

public sealed partial class DialogsManager(PageViewModelBase page) : ObservableObject, IDialogExceptionHandler, IExceptionHandler
{
    int _count;
    bool _isReversed;
    IDialog? _currentDialog;

    List<IDialog>? _dialogs;

    public PageViewModelBase Page => page;
    public NavigationPageViewModelBase? Navigation => Page.Navigation;

    public int Count
    {
        get => _count;
        private set
        {
            if (_count != value)
            {
                OnPropertyChanging();
                _count = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsReversed
    {
        get => _isReversed;
        private set
        {
            if (_isReversed != value)
            {
                OnPropertyChanging();
                _isReversed = value;
                OnPropertyChanged();
            }
        }
    }

    public IDialog? CurrentDialog
    {
        get => _currentDialog;
        private set
        {
            if (_currentDialog != value)
            {
                _currentDialog?.OnDialogHidden();
                OnPropertyChanging();
                _currentDialog = value;
                OnPropertyChanged();
                _currentDialog?.OnDialogShown();
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal bool OnBackHardwareRequested()
    {
        return CurrentDialog?.OnBackHardwareRequested() ?? false;
    }

    internal void PushDialog<TDialog>(Action<TDialog>? onInstance = null) 
        where TDialog : DialogViewModelBase, IDialog, IDialogInstance<TDialog>
    {
        var dialog = TDialog.CreateInstance(this);
        onInstance?.Invoke(dialog);

        PushDialog(dialog);
    }

    internal void PushDialog<T>(T dialog) where T : IDialog
    {
        dialog.DialogsManager = this;
        if (_dialogs is null)
        {
            _dialogs = [dialog];
        }
        else
        {
            _dialogs.Add(dialog);
        }
        dialog.DismissRequested += Dialog_DismissRequested;
        IsReversed = false;
        CurrentDialog = dialog;

        Count = _dialogs.Count;

        dialog.OnDialogAdded();
    }

    internal Task<DialogResult<TResult>> ShowDialog<TResult, TDialog>(Action<TDialog>? onInstance = null)
        where TDialog : DialogViewModelBase, IDialogInstance<TDialog>, IDialogResult<TResult>
    {
        var dialog = TDialog.CreateInstance(this);
        onInstance?.Invoke(dialog);

        PushDialog(dialog);

        return dialog.WaitForResultAsync();
    }

    internal Task<DialogResult<TResult>> ShowDialog<TResult>(IDialogResult<TResult> dialog)
    {
        PushDialog(dialog);

        return dialog.WaitForResultAsync();
    }

    bool RemoveDialog<T>(T dialog) 
        where T : IDialog
    {
        if (_dialogs is null)
        {
            throw new InvalidOperationException("No dialogs attached");
        }

        if (_dialogs.Remove(dialog))
        {
            dialog.DialogsManager = null;
            dialog.DismissRequested -= Dialog_DismissRequested;
            if (CurrentDialog == Unsafe.As<IDialog>(dialog))
            {
                IsReversed = true;
                CurrentDialog = _dialogs.LastOrDefault();
            }
            Count = _dialogs.Count;

            dialog.OnDialogRemoved();

            return true;
        }
        return false;
    }

    internal void Clear()
    {
        if (_dialogs is not null)
        {
            foreach (var dialog in _dialogs)
            {
                dialog.ForcedDismiss();
                dialog.DialogsManager = null;
                dialog.DismissRequested -= Dialog_DismissRequested;
            }
            CurrentDialog = null;
            Count = 0;
            _dialogs.Clear();
            _dialogs = null;
        }
    }

    void Dialog_DismissRequested(object? sender, EventArgs e)
    {
        RemoveDialog(Unsafe.As<IDialog>(sender)!);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    void IExceptionHandler.HandleException(object sender, Exception ex, string methodName, string filePath, int lineNumber)
        => Unsafe.As<IExceptionHandler>(Page).HandleException(sender, ex, methodName, filePath, lineNumber);

    [EditorBrowsable(EditorBrowsableState.Never)]
    void IDialogExceptionHandler.HandleException(IDialog sender, Exception ex, string methodName, string filePath, int lineNumber)
        => Unsafe.As<IDialogExceptionHandler>(Page).HandleException(sender, ex, methodName, filePath, lineNumber);
}
