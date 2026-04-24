using System.Collections.ObjectModel;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Models;

namespace Semcosm.HardwareConsole.App.ViewModels;

public sealed class PluginsViewModel
{
    public ObservableCollection<PluginManifestModel> InstalledPlugins { get; }

    public PluginsViewModel(IHardwareDataService hardwareDataService)
    {
        InstalledPlugins = new ObservableCollection<PluginManifestModel>(
            hardwareDataService
                .GetInstalledPluginDescriptors()
                .Select(descriptor => PluginManifestModel.FromDescriptor(
                    descriptor,
                    hardwareDataService.GetInstalledPluginState(descriptor.Id))));
    }
}
