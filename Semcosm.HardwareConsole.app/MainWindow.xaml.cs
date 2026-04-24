using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Semcosm.HardwareConsole.app
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ContentFrame.Navigate(typeof(Views.DashboardPage));
            RootNavigationView.SelectedItem = RootNavigationView.MenuItems[0];
        }

        private void RootNavigationView_SelectionChanged(
            NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is not NavigationViewItem selectedItem)
            {
                return;
            }

            string? tag = selectedItem.Tag?.ToString();

            Type? pageType = tag switch
            {
                "dashboard" => typeof(Views.DashboardPage),
                "performance" => typeof(Views.PerformancePage),
                "fans" => typeof(Views.FansPage),
                "power" => typeof(Views.PowerPage),
                "thermal" => typeof(Views.ThermalPage),
                "scheduler" => typeof(Views.SchedulerPage),
                "devices" => typeof(Views.DevicesPage),
                "plugins" => typeof(Views.PluginsPage),
                "profiles" => typeof(Views.ProfilesPage),
                "diagnostics" => typeof(Views.DiagnosticsPage),
                "settings" => typeof(Views.SettingsPage),
                _ => null
            };

            if (pageType is not null && ContentFrame.CurrentSourcePageType != pageType)
            {
                ContentFrame.Navigate(pageType);
            }
        }
    }
}
