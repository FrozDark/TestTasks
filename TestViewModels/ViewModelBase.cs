using CommunityToolkit.Mvvm.ComponentModel;

namespace TestViewModels;

public abstract partial class ViewModelBase : ObservableValidator // Быстрее и проще черз библиотку с SourceGenerator CommunityToolkit.Mvvm
{
    internal ViewModelBase()
    {

    }
}
