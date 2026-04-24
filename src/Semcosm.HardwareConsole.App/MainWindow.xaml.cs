using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.App.Services;

namespace Semcosm.HardwareConsole.App
{
    public sealed partial class MainWindow : Window
    {
        private readonly INavigationService _navigationService;

        public MainWindow(INavigationService navigationService)
        {
            _navigationService = navigationService;
            InitializeComponent();

            _navigationService.Initialize(ContentFrame);
            _navigationService.Navigate("dashboard");
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

            string? route = selectedItem.Tag?.ToString();
            if (!string.IsNullOrWhiteSpace(route))
            {
                _navigationService.Navigate(route);
            }
        }
    }
}
