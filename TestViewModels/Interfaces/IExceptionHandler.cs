using System.ComponentModel;

namespace TestViewModels.Interfaces;

public interface IExceptionHandler
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    void HandleException(object sender, Exception ex, string methodName, string filePath, int lineNumber = 0);
}
