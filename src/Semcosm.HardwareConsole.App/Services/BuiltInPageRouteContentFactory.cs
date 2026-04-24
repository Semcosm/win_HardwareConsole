using System;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class BuiltInPageRouteContentFactory : IRouteContentFactory
{
    public int Priority => 0;

    public bool CanCreate(NavigationRoute route)
    {
        return route is BuiltInNavigationRoute;
    }

    public Type? ResolvePageType(NavigationRoute route)
    {
        return route is BuiltInNavigationRoute builtInRoute
            ? builtInRoute.PageType
            : null;
    }
}
