using System;
using System.Collections.Generic;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class BuiltInNavigationRouteProvider : INavigationRouteProvider
{
    private static readonly IReadOnlyList<NavigationRoute> Routes =
        new List<NavigationRoute>
        {
            new BuiltInNavigationRoute("dashboard", "Dashboard", typeof(Views.DashboardPage), Microsoft.UI.Xaml.Controls.Symbol.Home),
            new BuiltInNavigationRoute("performance", "Performance", typeof(Views.PerformancePage), Microsoft.UI.Xaml.Controls.Symbol.Repair),
            new BuiltInNavigationRoute("fans", "Fans", typeof(Views.FansPage), Microsoft.UI.Xaml.Controls.Symbol.Bullets),
            new BuiltInNavigationRoute("power", "Power", typeof(Views.PowerPage), Microsoft.UI.Xaml.Controls.Symbol.Calculator),
            new BuiltInNavigationRoute("thermal", "Thermal", typeof(Views.ThermalPage), Microsoft.UI.Xaml.Controls.Symbol.Like),
            new BuiltInNavigationRoute("scheduler", "Scheduler", typeof(Views.SchedulerPage), Microsoft.UI.Xaml.Controls.Symbol.Clock),
            new BuiltInNavigationRoute("devices", "Devices", typeof(Views.DevicesPage), Microsoft.UI.Xaml.Controls.Symbol.World),
            new BuiltInNavigationRoute("plugins", "Plugins", typeof(Views.PluginsPage), Microsoft.UI.Xaml.Controls.Symbol.Library),
            new BuiltInNavigationRoute("profiles", "Profiles", typeof(Views.ProfilesPage), Microsoft.UI.Xaml.Controls.Symbol.Contact),
            new BuiltInNavigationRoute("diagnostics", "Diagnostics", typeof(Views.DiagnosticsPage), Microsoft.UI.Xaml.Controls.Symbol.Help),
            new BuiltInNavigationRoute("settings", "Settings", typeof(Views.SettingsPage), Microsoft.UI.Xaml.Controls.Symbol.Setting, IsFooter: true)
        };

    public event EventHandler? RoutesChanged;

    public IReadOnlyList<NavigationRoute> GetRoutes() => Routes;

    public void NotifyRoutesChanged()
    {
        RoutesChanged?.Invoke(this, EventArgs.Empty);
    }
}
