using System;
using System.Collections.Generic;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class PageFactory : IPageFactory
{
    private static readonly IReadOnlyDictionary<string, Type> RouteTable =
        new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            ["dashboard"] = typeof(Views.DashboardPage),
            ["performance"] = typeof(Views.PerformancePage),
            ["fans"] = typeof(Views.FansPage),
            ["power"] = typeof(Views.PowerPage),
            ["thermal"] = typeof(Views.ThermalPage),
            ["scheduler"] = typeof(Views.SchedulerPage),
            ["devices"] = typeof(Views.DevicesPage),
            ["plugins"] = typeof(Views.PluginsPage),
            ["profiles"] = typeof(Views.ProfilesPage),
            ["diagnostics"] = typeof(Views.DiagnosticsPage),
            ["settings"] = typeof(Views.SettingsPage)
        };

    public Type? ResolvePageType(string route)
    {
        return RouteTable.TryGetValue(route, out var pageType)
            ? pageType
            : null;
    }
}
