using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.app.ViewModels;

namespace Semcosm.HardwareConsole.app.Views;

public sealed partial class DashboardPage : Page
{
    public DashboardViewModel ViewModel { get; } = new();

    public DashboardPage()
    {
        InitializeComponent();
    }
}
