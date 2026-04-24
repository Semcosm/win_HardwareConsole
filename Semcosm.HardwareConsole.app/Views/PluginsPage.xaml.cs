using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.app.ViewModels;

namespace Semcosm.HardwareConsole.app.Views;

public sealed partial class PluginsPage : Page
{
    public PluginsViewModel ViewModel { get; } = new();

    public PluginsPage()
    {
        InitializeComponent();
    }
}
