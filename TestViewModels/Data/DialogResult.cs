namespace TestViewModels.Data;

internal readonly struct DialogResult<T>
{
    public static readonly DialogResult<T> Failed = new();

    public DialogResult()
    {
        Value = default;
    }

    public DialogResult(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    public bool IsSuccess { get; }
    public T? Value { get; }
}
