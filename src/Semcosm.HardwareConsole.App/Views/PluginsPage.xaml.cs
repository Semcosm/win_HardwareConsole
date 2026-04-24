using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.App.ViewModels;

namespace Semcosm.HardwareConsole.App.Views;

public sealed partial class PluginsPage : Page
{
    public PluginsViewModel ViewModel { get; } = new();

    public PluginsPage()
    {
        InitializeComponent();
    }
}
