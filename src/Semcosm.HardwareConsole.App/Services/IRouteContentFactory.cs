using System;

namespace Semcosm.HardwareConsole.App.Services;

public interface IRouteContentFactory
{
    bool CanCreate(NavigationRoute route);
    Type? ResolvePageType(NavigationRoute route);
}
