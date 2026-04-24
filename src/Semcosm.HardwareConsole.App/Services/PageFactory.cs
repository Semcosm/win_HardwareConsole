using System;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class PageFactory : IPageFactory
{
    private readonly INavigationRouteRegistry _routeRegistry;

    public PageFactory(INavigationRouteRegistry routeRegistry)
    {
        _routeRegistry = routeRegistry;
    }

    public Type? ResolvePageType(string route)
    {
        return _routeRegistry.GetRoute(route) switch
        {
            BuiltInNavigationRoute builtInRoute => builtInRoute.PageType,
            _ => null
        };
    }
}
