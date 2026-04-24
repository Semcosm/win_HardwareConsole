using System;
using System.Collections.Generic;

namespace Semcosm.HardwareConsole.App.Services;

public interface INavigationRouteRegistry
{
    event EventHandler? RoutesChanged;

    IReadOnlyList<NavigationRoute> GetRoutes();
    NavigationRoute? GetRoute(string tag);
}
