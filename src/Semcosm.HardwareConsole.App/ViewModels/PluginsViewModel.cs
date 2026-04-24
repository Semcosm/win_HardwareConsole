using System.Collections.ObjectModel;
using Semcosm.HardwareConsole.Abstractions.Models;
using Semcosm.HardwareConsole.Mock.Services;

namespace Semcosm.HardwareConsole.App.ViewModels;

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
