using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.App.ViewModels;

namespace Semcosm.HardwareConsole.App.Views;

public sealed partial class DevicesPage : Page
{
    public DevicesViewModel ViewModel { get; }

    public DevicesPage()
    {
        ViewModel = App.GetService<DevicesViewModel>();
        InitializeComponent();
    }
}
