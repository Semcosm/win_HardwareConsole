using System;
using System.Collections.Generic;
using System.Linq;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class PageFactory : IPageFactory
{
    private readonly IReadOnlyList<IRouteContentFactory> _routeContentFactories;
    private readonly INavigationRouteRegistry _routeRegistry;

    public PageFactory(
        INavigationRouteRegistry routeRegistry,
        IEnumerable<IRouteContentFactory> routeContentFactories)
    {
        _routeRegistry = routeRegistry;
        _routeContentFactories = routeContentFactories.ToList();
    }

    public Type? ResolvePageType(string route)
    {
        var navigationRoute = _routeRegistry.GetRoute(route);
        if (navigationRoute is null)
        {
            return null;
        }

        return _routeContentFactories
            .FirstOrDefault(factory => factory.CanCreate(navigationRoute))
            ?.ResolvePageType(navigationRoute);
    }
}
