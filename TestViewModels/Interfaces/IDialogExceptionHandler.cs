using System.ComponentModel;

namespace TestViewModels.Interfaces;

public interface IDialogExceptionHandler
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    void HandleException(IDialog sender, Exception ex, string methodName, string filePath, int lineNumber);
}