using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

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
        _routeContentFactories = routeContentFactories
            .OrderByDescending(factory => factory.Priority)
            .ToList();
    }

    public Type? ResolvePageType(string route)
    {
        var navigationRoute = _routeRegistry.GetRoute(route);
        if (navigationRoute is null)
        {
            return null;
        }

        var matchingFactories = _routeContentFactories
            .Where(factory => factory.CanCreate(navigationRoute))
            .ToList();

        if (matchingFactories.Count == 0)
        {
            return null;
        }

        if (matchingFactories.Count > 1)
        {
            Debug.WriteLine(
                $"Multiple route content factories matched route '{navigationRoute.Tag}'. " +
                $"Using '{matchingFactories[0].GetType().Name}' with priority {matchingFactories[0].Priority}.");
        }

        return matchingFactories[0].ResolvePageType(navigationRoute);
    }
}
