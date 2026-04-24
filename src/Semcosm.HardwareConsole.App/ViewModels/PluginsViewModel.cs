using System.Collections.ObjectModel;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Models;

namespace Semcosm.HardwareConsole.App.ViewModels;

public sealed class PluginsViewModel
{
    public ObservableCollection<PluginManifestModel> InstalledPlugins { get; }

    public PluginsViewModel(IPluginRegistry pluginRegistry)
    {
        InstalledPlugins = new ObservableCollection<PluginManifestModel>(
            pluginRegistry
                .GetInstalledPlugins()
                .Select(descriptor => PluginManifestModel.FromDescriptor(
                    descriptor,
                    pluginRegistry.GetPluginState(descriptor.Id))));
    }
}
