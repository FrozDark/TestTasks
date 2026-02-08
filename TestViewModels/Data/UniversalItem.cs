namespace TestViewModels.Data;

public sealed partial class UniversalItem(string name, string display)
{
    public string Name { get; } = name;
    public string Display { get; } = display;
}
