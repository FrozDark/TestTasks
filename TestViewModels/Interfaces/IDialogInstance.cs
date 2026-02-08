using TestViewModels.Dialogs;

namespace TestViewModels.Interfaces;

internal interface IDialogInstance<T> where T : DialogViewModelBase, IDialogInstance<T>
{
    internal static abstract T CreateInstance(DialogsManager dialogs);
}
