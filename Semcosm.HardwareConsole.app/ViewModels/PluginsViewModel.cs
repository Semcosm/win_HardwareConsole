using System.Collections.ObjectModel;
using Semcosm.HardwareConsole.app.Models;
using Semcosm.HardwareConsole.app.Services;

namespace Semcosm.HardwareConsole.app.ViewModels;

public sealed class PluginsViewModel
{
    public ObservableCollection<PluginManifestModel> InstalledPlugins { get; }

    public PluginsViewModel()
    {
        var mockHardwareService = new MockHardwareService();

        InstalledPlugins = new ObservableCollection<PluginManifestModel>(
            mockHardwareService.GetInstalledPlugins());
    }
}
