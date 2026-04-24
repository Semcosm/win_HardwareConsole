using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public interface IPluginRegistry
{
    IReadOnlyList<PluginDescriptor> GetInstalledPlugins();
    string GetPluginState(string pluginId);
}
