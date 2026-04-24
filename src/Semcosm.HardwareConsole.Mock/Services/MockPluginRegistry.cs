using System.Collections.Generic;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.Mock.Services;

public sealed class MockPluginRegistry : IPluginRegistry
{
    public IReadOnlyList<PluginDescriptor> GetInstalledPlugins() => MockHardwareData.InstalledPlugins;

    public string GetPluginState(string pluginId) => MockHardwareData.GetPluginState(pluginId);
}
