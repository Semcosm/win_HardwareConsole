using Microsoft.UI.Xaml.Controls;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class NavigationService : INavigationService
{
    private readonly IPageFactory _pageFactory;
    private Frame? _frame;

    public NavigationService(IPageFactory pageFactory)
    {
        _pageFactory = pageFactory;
    }

    public void Initialize(Frame frame)
    {
        _frame = frame;
    }

    public bool Navigate(string route)
    {
        if (_frame is null)
        {
            return false;
        }

        var pageType = _pageFactory.ResolvePageType(route);
        if (pageType is null)
        {
            return false;
        }

        if (_frame.CurrentSourcePageType == pageType)
        {
            return true;
        }

        return _frame.Navigate(pageType);
    }
}
