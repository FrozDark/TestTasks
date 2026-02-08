using CommunityToolkit.Mvvm.Input;

namespace TestViewModels.Dialogs;

public sealed partial class ErrorDialogViewModel : ResultDialogViewModelBase<bool?>
{
    public ErrorDialogViewModel(string message)
    {
        Title = "Ошибка";
        Message = message;
    }

    public string? Title { get; }

    public string Message { get; }

    [RelayCommand]
    void Close()
    {
        Dismiss(null);
    }
}
