using System;

namespace Semcosm.HardwareConsole.App.Services;

public interface IRouteContentFactory
{
    int Priority { get; }
    bool CanCreate(NavigationRoute route);
    Type? ResolvePageType(NavigationRoute route);
}
