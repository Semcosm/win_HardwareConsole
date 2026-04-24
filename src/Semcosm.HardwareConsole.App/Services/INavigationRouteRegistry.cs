using System.Collections.Generic;

namespace Semcosm.HardwareConsole.App.Services;

public interface INavigationRouteRegistry
{
    IReadOnlyList<NavigationRoute> GetRoutes();
    NavigationRoute? GetRoute(string tag);
}
