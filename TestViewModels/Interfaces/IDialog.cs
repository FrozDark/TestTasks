using TestViewModels.Dialogs;
using System.ComponentModel;

namespace TestViewModels.Interfaces;

public interface IDialog
{
    event EventHandler? DismissRequested;

    [EditorBrowsable(EditorBrowsableState.Never)]
    DialogsManager? DialogsManager { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    void OnDialogHidden();

    [EditorBrowsable(EditorBrowsableState.Never)]
    void OnDialogShown();

    [EditorBrowsable(EditorBrowsableState.Never)]
    void ForcedDismiss();

    [EditorBrowsable(EditorBrowsableState.Never)]
    void OnDialogAdded();

    [EditorBrowsable(EditorBrowsableState.Never)]
    void OnDialogRemoved();

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal bool OnBackHardwareRequested();
}
