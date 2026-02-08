using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TestViewModels.Dialogs;

public sealed partial class ConfirmDialogViewModel : ResultDialogViewModelBase<bool?>
{
    [ObservableProperty]
    bool _hasCloseButton = true;

    [ObservableProperty]
    bool _hasCancelButton;

    [ObservableProperty]
    bool _hasConfirmButton;

    public ConfirmDialogViewModel(string? title, string message) : this(message)
    {
        Title = title;
    }

    public ConfirmDialogViewModel(string message)
    {
        Message = message;
    }

    public string? Title { get; }
    public string Message { get; }

    public bool CanClose()
    {
        return HasCloseButton;
    }

    public bool CanCancel()
    {
        return HasCancelButton;
    }

    public bool CanConfirm()
    {
        return HasConfirmButton;
    }

    partial void OnHasCancelButtonChanged(bool value)
    {
        CancelCommand.NotifyCanExecuteChanged();
    }

    partial void OnHasCloseButtonChanged(bool value)
    {
        CloseCommand.NotifyCanExecuteChanged();
    }

    partial void OnHasConfirmButtonChanged(bool value)
    {
        ConfirmCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanClose))]
    void Close()
    {
        Dismiss(null);
    }

    [RelayCommand(CanExecute = nameof(CanCancel))]
    void Cancel()
    {
        Dismiss(false);
    }

    [RelayCommand(CanExecute = nameof(CanConfirm))]
    void Confirm()
    {
        Dismiss(true);
    }
}
