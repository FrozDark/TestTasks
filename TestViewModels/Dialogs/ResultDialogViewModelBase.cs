using TestViewModels.Data;
using TestViewModels.Interfaces;
using System.ComponentModel;

namespace TestViewModels.Dialogs;

public abstract partial class ResultDialogViewModelBase<T> : DialogViewModelBase, IDialogResult<T>
{
    TaskCompletionSource<DialogResult<T>>? _tcs;
    bool _isBusy;

    public virtual bool IsBusy
    {
        get => _isBusy;
        protected set => SetProperty(ref _isBusy, value);
    }

    protected override bool OnDismissRequested()
    {
        return false;
    }

    protected void Dismiss(T result)
    {
        if (_tcs is { Task.IsCompleted: false })
        {
            _tcs.SetResult(new DialogResult<T>(result));
        }
        base.Dismiss();
    }

    protected sealed override void Dismiss()
    {
        if (_tcs is { Task.IsCompleted: false })
        {
            _tcs.SetResult(DialogResult<T>.Failed);
        }
        base.Dismiss();
    }

    protected override void OnDismissed()
    {
        _tcs = null;
    }

    protected sealed override void ForcedDismiss()
    {
        if (_tcs is { Task.IsCompleted: false })
        {
            _tcs.SetResult(DialogResult<T>.Failed);
        }
        _tcs = null;
    }

    protected sealed override void OnDialogRemoved()
    {
        if (_tcs is { Task.IsCompleted: false })
        {
            _tcs.SetResult(DialogResult<T>.Failed);
        }
        _tcs = null;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<DialogResult<T>> IDialogResult<T>.WaitForResultAsync()
    {
        _tcs ??= new();

        return _tcs.Task;
    }
}
