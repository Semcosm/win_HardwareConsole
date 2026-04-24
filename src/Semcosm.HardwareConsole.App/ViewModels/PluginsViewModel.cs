using System.Collections.ObjectModel;
using System.Linq;
using Semcosm.HardwareConsole.App.Models;
using Semcosm.HardwareConsole.Mock.Services;

namespace Semcosm.HardwareConsole.App.ViewModels;

public sealed class PluginsViewModel
{
    public ObservableCollection<PluginManifestModel> InstalledPlugins { get; }

    public PluginsViewModel()
    {
        var mockHardwareService = new MockHardwareService();

        InstalledPlugins = new ObservableCollection<PluginManifestModel>(
            mockHardwareService
                .GetInstalledPluginDescriptors()
                .Select(descriptor => PluginManifestModel.FromDescriptor(
                    descriptor,
                    mockHardwareService.GetInstalledPluginState(descriptor.Id))));
    }
}
