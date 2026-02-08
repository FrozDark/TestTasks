using CommunityToolkit.Mvvm.Input;
using TestViewModels.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace TestViewModels;

/// <summary>
/// Многие свойства не используется, этот шаблон я скопировал из своего проекта по Avalonia
/// Такую архитектуру класса собрал сам, не скопировано и незагуглено
/// </summary>
public abstract partial class NavigationPageViewModelBase : PageViewModelBase
{
    bool? _isHeaderVisibleOverride;
    readonly ObservableCollection<PageViewModelBase> _pages = [];
    public IReadOnlyCollection<PageViewModelBase> Pages => _pages;

    PageViewModelBase? _currentPage;

    public event EventHandler? OnNavigated;

    RelayCommand? _goBackCommand;

    internal NavigationPageViewModelBase()
    {
    }

    public PageViewModelBase? CurrentPage
    {
        get => _currentPage;
        private set
        {
            if (_currentPage != value)
            {
                OnPropertyChanging();
                if (_currentPage is not null)
                {
                    _currentPage.PropertyChanged -= CurrentPage_PropertyChanged;
                }
                _currentPage = value;
                if (_currentPage is not null)
                {
                    _currentPage.PropertyChanged += CurrentPage_PropertyChanged;
                }
                OnPropertyChanged();

                OnNavigated?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public sealed override string? Title
    {
        get => CurrentPage?.Title ?? base.Title;
        protected set { }
    }

    public sealed override bool IsBusy
    {
        get => CurrentPage?.IsBusy ?? base.IsBusy;
        protected set { }
    }

    public sealed override bool IsBackButtonVisible
    {
        get => CurrentPage?.IsBackButtonVisible ?? base.IsBackButtonVisible;
        protected set { }
    }

    public override bool IsHeaderVisible 
    { 
        get => IsHeaderVisibleOverride ?? CurrentPage?.IsHeaderVisible ?? false;
        protected set { }
    }

    public bool? IsHeaderVisibleOverride
    {
        get => _isHeaderVisibleOverride;
        protected set
        {
            if (SetProperty(ref _isHeaderVisibleOverride, value))
            {
                OnPropertyChanged(nameof(IsHeaderVisible));
            }
        }
    }

    public override bool? CanNavigatePrevious 
    {
        get
        {
            if (CurrentPage?.CanNavigatePrevious.HasValue ?? false)
            {
                return CurrentPage.CanNavigatePrevious.Value;
            }

            return _pages.Count > 1;
        }
        protected set => base.CanNavigatePrevious = value; 
    }

    bool _isNavigatingBack;
    public bool IsNavigatingBack
    {
        get => _isNavigatingBack;
        private set => SetProperty(ref _isNavigatingBack, value);
    }

    public ICommand GoBackCommand => _goBackCommand ??= new RelayCommand(GoBack, CanGoBack);

    public bool CanGoBack()
    {
        return (CanNavigatePrevious ?? true) 
                && !IsBusy
                && _pages.Count > 1;
    }

    public void GoBack()
    {
        TryGoBack();
    }

    void AddPage(PageViewModelBase page)
    {
        Unsafe.As<INavigatable>(page).Navigation = this;
        _pages.Add(page);
    }

    static void FreePage(PageViewModelBase page)
    {
        Unsafe.As<INavigatable>(page).Navigation = null;
    }

    public void RemovePage(PageViewModelBase page)
    {
        if (page == CurrentPage)
        {
            throw new InvalidOperationException("Can not remove current page");
        }

        FreePage(page);
        _pages.Remove(page);

        OnPropertyChanged(nameof(CanNavigatePrevious));
    }

    void RemovePageAt(int index)
    {
        var page = _pages[index];
        FreePage(page);
        _pages.RemoveAt(index);
    }

    public void ClearPages()
    {
        for (int i = 0; i < _pages.Count; i++)
        {
            RemovePageAt(i--);
        }

        OnPropertyChanged(nameof(CanNavigatePrevious));
    }

    protected bool AllowNavigatingFromCurrentPage()
    {
        if (_currentPage is not null
            && !Unsafe.As<INavigatable>(_currentPage).OnNavigatingFromInternal(NavigatingType.Forward))
        {
            return false;
        }

        return true;
    }

    internal T? NavigateTo<T>(Action<T>? onInstance = null, bool skip_current = false) where T : PageViewModelBase, new()
    {
        if (!AllowNavigatingFromCurrentPage())
        {
            return null;
        }

        var page = new T();

        onInstance?.Invoke(page);

        if (!Unsafe.As<INavigatable>(page).OnNavigatingInternal(_currentPage, NavigatingType.New))
        {
            return null;
        }

        NavigatePrivate(NavigatingType.Forward, page, skip_current);

        return page;
    }

    internal T? NavigateToInstance<T>(Action<T>? onInstance = null, bool skip_current = false) where T : PageViewModelBase, IPageInstance<T>
    {
        if (!AllowNavigatingFromCurrentPage())
        {
            return null;
        }

        var page = T.CreateInstance(this);

        onInstance?.Invoke(page);

        NavigatePrivate(NavigatingType.Forward, page, skip_current);

        return page;
    }

    internal bool NavigateTo<T>(T page, bool skip_current = false) where T : PageViewModelBase
    {
        if (!AllowNavigatingFromCurrentPage()
            || !Unsafe.As<INavigatable>(page).OnNavigatingInternal(_currentPage, NavigatingType.New))
        {
            return false;
        }

        NavigatePrivate(NavigatingType.Forward, page, skip_current);

        return true;
    }

    internal T? RootNavigateTo<T>(Action<T>? onInstance = null) where T : PageViewModelBase, new()
    {
        if (!AllowNavigatingFromCurrentPage())
        {
            return null;
        }

        var page = new T();

        onInstance?.Invoke(page);

        NavigatePrivate(NavigatingType.Root, page, false);

        return page;
    }

    internal T? RootNavigateToInstance<T>(Action<T>? onInstance = null) where T : PageViewModelBase, IPageInstance<T>
    {
        if (!AllowNavigatingFromCurrentPage())
        {
            return null;
        }

        var page = T.CreateInstance(this);

        onInstance?.Invoke(page);

        NavigatePrivate(NavigatingType.Root, page, false);

        return page;
    }

    internal bool RootNavigateTo<T>(T page) where T : PageViewModelBase
    {
        if (!AllowNavigatingFromCurrentPage()
            || !Unsafe.As<INavigatable>(page).OnNavigatingInternal(_currentPage, NavigatingType.New))
        {
            return false;
        }

        NavigatePrivate(NavigatingType.Root, page, false);

        return true;
    }

    void NavigatePrivate(NavigatingType type, PageViewModelBase page, bool skip_current)
    {
        OnPageNavigating(type);

        var prevPage = _currentPage;

        IsNavigatingBack = false;

        if (type == NavigatingType.Root)
        {
            ClearPages();
        }

        AddPage(page);

        CurrentPage = page;

        if (prevPage is not null)
        {
            Unsafe.As<INavigatable>(prevPage).OnNavigatedFromInternal(page, type);

            if (skip_current)
            {
                RemovePage(prevPage);
            }
        }
        _goBackCommand?.NotifyCanExecuteChanged();

        Unsafe.As<INavigatable>(page).OnNavigatedInternal(prevPage, NavigatingType.New);

        OnPageNavigated(type);
    }

    public bool NavigateToRoot()
    {
        var page = _pages.FirstOrDefault();

        if (page is null || page == _currentPage)
        {
            return false;
        }

        if (!CanGoBack()
            || (_currentPage is not null
                && !Unsafe.As<INavigatable>(_currentPage).OnNavigatingFromInternal(NavigatingType.Back)))
        {
            return false;
        }

        if (!Unsafe.As<INavigatable>(page).OnNavigatingInternal(_currentPage, NavigatingType.Back))
        {
            return false;
        }

        OnPageNavigating(NavigatingType.Back);

        var prevPage = _currentPage;

        IsNavigatingBack = true;
        CurrentPage = page;
        RemovePageAt(_pages.Count - 1);
        _goBackCommand?.NotifyCanExecuteChanged();

        if (prevPage is not null)
        {
            Unsafe.As<INavigatable>(prevPage).OnNavigatedFromInternal(page, NavigatingType.Back);
        }
        Unsafe.As<INavigatable>(page).OnNavigatedInternal(prevPage, NavigatingType.Back);

        OnPageNavigated(NavigatingType.Back);

        return true;
    }

    public bool TryGoBack()
    {
        if (_pages.Count == 1)
        {
            return false;
        }

        if (!CanGoBack()
            || (_currentPage is not null
                && !Unsafe.As<INavigatable>(_currentPage).OnNavigatingFromInternal(NavigatingType.Back)))
        {
            return false;
        }

        var prevPage = _currentPage;
        PageViewModelBase? _pageToNavigate = null;

        for (int i = _pages.Count - 2; i >= 0; i--)
        {
            var page = _pages[i];

            if (!Unsafe.As<INavigatable>(page).OnNavigatingInternal(_currentPage, NavigatingType.Back))
            {
                RemovePageAt(i);
                continue;
            }

            _pageToNavigate = page;
            break;
        }

        if (_pageToNavigate is not null)
        {
            OnPageNavigating(NavigatingType.Back);

            IsNavigatingBack = true;
            CurrentPage = _pageToNavigate;
            if (prevPage is not null)
            {
                RemovePage(prevPage);
                Unsafe.As<INavigatable>(prevPage).OnNavigatedFromInternal(_pageToNavigate, NavigatingType.Back);
            }
            _goBackCommand?.NotifyCanExecuteChanged();

            Unsafe.As<INavigatable>(_pageToNavigate).OnNavigatedInternal(prevPage, NavigatingType.Back);

            OnPageNavigated(NavigatingType.Back);

            return true;
        }

        return false;
    }

    void CurrentPage_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e);
        OnCurrentPagePropertyChanged(Unsafe.As<PageViewModelBase>(sender)!, e);
    }

    protected virtual void OnPageNavigating(NavigatingType type)
    {

    }

    protected virtual void OnPageNavigated(NavigatingType type)
    {
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(IsBusy));
        OnPropertyChanged(nameof(IsBackButtonVisible));
        OnPropertyChanged(nameof(IsHeaderVisible));
        OnPropertyChanged(nameof(CanNavigatePrevious));
        _goBackCommand?.NotifyCanExecuteChanged();
    }

    public override bool OnBackHardwareRequested()
    {
        if (CurrentPage is not null)
        {
            if (CurrentPage.Dialogs!.OnBackHardwareRequested())
            {
                return true;
            }
            if (CurrentPage.OnBackHardwareRequested())
            {
                return true;
            }
        }

        return TryGoBack();
    }

    protected virtual void OnCurrentPagePropertyChanged(PageViewModelBase page, PropertyChangedEventArgs e)
    {
        if (e.PropertyName!.Equals(nameof(CanNavigatePrevious), StringComparison.Ordinal))
        {
            _goBackCommand?.NotifyCanExecuteChanged();
        }
        else if (e.PropertyName!.Equals(nameof(IsBusy), StringComparison.Ordinal))
        {
            _goBackCommand?.NotifyCanExecuteChanged();
        }
    }
}