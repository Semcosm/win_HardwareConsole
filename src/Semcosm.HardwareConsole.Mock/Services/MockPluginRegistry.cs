using System;
using System.Collections.Generic;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.Mock.Services;

public sealed class MockPluginRegistry : IPluginRegistry
{
    public event EventHandler? PluginsChanged;

    public IReadOnlyList<PluginDescriptor> GetInstalledPlugins() => MockHardwareData.InstalledPlugins;

    public PluginState GetPluginState(string pluginId) => MockHardwareData.GetPluginState(pluginId);

    public void NotifyPluginsChanged()
    {
        PluginsChanged?.Invoke(this, EventArgs.Empty);
    }
}
