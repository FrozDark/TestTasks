namespace TestViewModels.Interfaces;

internal interface IPageInstance<T> where T : PageViewModelBase, IPageInstance<T>
{
    internal static abstract T CreateInstance(NavigationPageViewModelBase navigation);
}
