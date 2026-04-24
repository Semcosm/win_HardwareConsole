using System;
using System.Collections.Generic;

namespace Semcosm.HardwareConsole.App.Services;

public interface INavigationRouteProvider
{
    event EventHandler? RoutesChanged;

    IReadOnlyList<NavigationRoute> GetRoutes();
}
