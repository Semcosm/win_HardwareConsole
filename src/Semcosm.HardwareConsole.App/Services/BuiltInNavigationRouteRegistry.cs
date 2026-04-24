using System;
using System.Collections.Generic;
using System.Linq;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class BuiltInNavigationRouteRegistry : INavigationRouteRegistry
{
    private static readonly IReadOnlyList<NavigationRoute> Routes =
        new List<NavigationRoute>
        {
            new("dashboard", "Dashboard", typeof(Views.DashboardPage), "Home"),
            new("performance", "Performance", typeof(Views.PerformancePage), "Repair"),
            new("fans", "Fans", typeof(Views.FansPage), "Bullets"),
            new("power", "Power", typeof(Views.PowerPage), "Calculator"),
            new("thermal", "Thermal", typeof(Views.ThermalPage), "Like"),
            new("scheduler", "Scheduler", typeof(Views.SchedulerPage), "Clock"),
            new("devices", "Devices", typeof(Views.DevicesPage), "World"),
            new("plugins", "Plugins", typeof(Views.PluginsPage), "Puzzle"),
            new("profiles", "Profiles", typeof(Views.ProfilesPage), "Contact"),
            new("diagnostics", "Diagnostics", typeof(Views.DiagnosticsPage), "Help"),
            new("settings", "Settings", typeof(Views.SettingsPage), "Setting", IsFooter: true)
        };

    public IReadOnlyList<NavigationRoute> GetRoutes() => Routes;

    public NavigationRoute? GetRoute(string tag)
    {
        return Routes.FirstOrDefault(route =>
            string.Equals(route.Tag, tag, StringComparison.OrdinalIgnoreCase));
    }
}
