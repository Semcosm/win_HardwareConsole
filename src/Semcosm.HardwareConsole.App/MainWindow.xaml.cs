using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using Semcosm.HardwareConsole.App.Services;

namespace Semcosm.HardwareConsole.App
{
    public sealed partial class MainWindow : Window
    {
        private readonly INavigationService _navigationService;
        private readonly INavigationRouteRegistry _routeRegistry;

        public MainWindow(
            INavigationService navigationService,
            INavigationRouteRegistry routeRegistry)
        {
            _navigationService = navigationService;
            _routeRegistry = routeRegistry;
            InitializeComponent();

            BuildNavigationMenu();
            _navigationService.Initialize(ContentFrame);

            var defaultRoute = _routeRegistry
                .GetRoutes()
                .FirstOrDefault(route => !route.IsFooter);

            if (defaultRoute is not null)
            {
                _navigationService.Navigate(defaultRoute.Tag);
                RootNavigationView.SelectedItem = FindNavigationItem(defaultRoute.Tag);
            }
        }

        private void RootNavigationView_SelectionChanged(
            NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is not NavigationViewItem selectedItem)
            {
                return;
            }

            string? route = selectedItem.Tag?.ToString();
            if (!string.IsNullOrWhiteSpace(route))
            {
                _navigationService.Navigate(route);
            }
        }

        private void BuildNavigationMenu()
        {
            RootNavigationView.MenuItems.Clear();
            RootNavigationView.FooterMenuItems.Clear();

            foreach (var route in _routeRegistry.GetRoutes())
            {
                var item = CreateNavigationItem(route);

                if (route.IsFooter)
                {
                    RootNavigationView.FooterMenuItems.Add(item);
                }
                else
                {
                    RootNavigationView.MenuItems.Add(item);
                }
            }
        }

        private NavigationViewItem CreateNavigationItem(NavigationRoute route)
        {
            return new NavigationViewItem
            {
                Content = route.Title,
                Tag = route.Tag,
                Icon = CreateIcon(route.Icon)
            };
        }

        private IconElement CreateIcon(string icon)
        {
            return Enum.TryParse<Symbol>(icon, ignoreCase: true, out var symbol)
                ? new SymbolIcon(symbol)
                : new SymbolIcon(Symbol.Document);
        }

        private NavigationViewItem? FindNavigationItem(string tag)
        {
            var item = RootNavigationView
                .MenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(candidate => string.Equals(candidate.Tag?.ToString(), tag));

            return item ?? RootNavigationView
                .FooterMenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(candidate => string.Equals(candidate.Tag?.ToString(), tag));
        }
    }
}
