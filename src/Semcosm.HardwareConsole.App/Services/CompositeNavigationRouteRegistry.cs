using System;
using System.Collections.Generic;
using System.Linq;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class CompositeNavigationRouteRegistry : INavigationRouteRegistry
{
    private readonly IReadOnlyList<INavigationRouteProvider> _routeProviders;

    public event EventHandler? RoutesChanged;

    public CompositeNavigationRouteRegistry(IEnumerable<INavigationRouteProvider> routeProviders)
    {
        _routeProviders = routeProviders.ToList();

        foreach (var routeProvider in _routeProviders)
        {
            routeProvider.RoutesChanged += RouteProvider_RoutesChanged;
        }
    }

    public IReadOnlyList<NavigationRoute> GetRoutes()
    {
        return _routeProviders
            .SelectMany(provider => provider.GetRoutes())
            .ToList();
    }

    public NavigationRoute? GetRoute(string tag)
    {
        return GetRoutes().FirstOrDefault(route =>
            string.Equals(route.Tag, tag, StringComparison.OrdinalIgnoreCase));
    }

    private void RouteProvider_RoutesChanged(object? sender, EventArgs e)
    {
        RoutesChanged?.Invoke(this, EventArgs.Empty);
    }
}
