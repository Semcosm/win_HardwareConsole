using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class CompositeNavigationRouteRegistry : INavigationRouteRegistry, IDisposable
{
    private readonly IReadOnlyList<INavigationRouteProvider> _routeProviders;
    private IReadOnlyList<NavigationRoute> _routes = Array.Empty<NavigationRoute>();
    private bool _disposed;

    public event EventHandler? RoutesChanged;

    public CompositeNavigationRouteRegistry(IEnumerable<INavigationRouteProvider> routeProviders)
    {
        _routeProviders = routeProviders.ToList();

        foreach (var routeProvider in _routeProviders)
        {
            routeProvider.RoutesChanged += RouteProvider_RoutesChanged;
        }

        RefreshRoutes();
    }

    public IReadOnlyList<NavigationRoute> GetRoutes()
    {
        return _routes;
    }

    public NavigationRoute? GetRoute(string tag)
    {
        return GetRoutes().FirstOrDefault(route =>
            string.Equals(route.Tag, tag, StringComparison.OrdinalIgnoreCase));
    }

    private void RouteProvider_RoutesChanged(object? sender, EventArgs e)
    {
        RefreshRoutes();
        RoutesChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        foreach (var routeProvider in _routeProviders)
        {
            routeProvider.RoutesChanged -= RouteProvider_RoutesChanged;
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private void RefreshRoutes()
    {
        var mergedRoutes = new List<NavigationRoute>();
        var registeredTags = new Dictionary<string, NavigationRoute>(StringComparer.OrdinalIgnoreCase);

        foreach (var routeProvider in _routeProviders)
        {
            foreach (var route in routeProvider.GetRoutes())
            {
                if (string.IsNullOrWhiteSpace(route.Tag))
                {
                    Debug.WriteLine(
                        $"Ignoring navigation route with empty tag from provider '{route.ProviderId}'.");
                    continue;
                }

                if (registeredTags.TryGetValue(route.Tag, out var existingRoute))
                {
                    Debug.WriteLine(
                        $"Duplicate navigation route tag '{route.Tag}' from provider '{route.ProviderId}'. " +
                        $"Keeping provider '{existingRoute.ProviderId}' and ignoring the duplicate.");
                    continue;
                }

                registeredTags.Add(route.Tag, route);
                mergedRoutes.Add(route);
            }
        }

        _routes = mergedRoutes;
    }
}
