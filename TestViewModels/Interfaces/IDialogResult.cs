using TestViewModels.Data;
using System.ComponentModel;

namespace TestViewModels.Interfaces;

internal interface IDialogResult<T> : IDialog
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<DialogResult<T>> WaitForResultAsync();
}
