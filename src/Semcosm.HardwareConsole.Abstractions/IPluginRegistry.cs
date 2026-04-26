using System;
using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public interface IPluginRegistry
{
    event EventHandler? PluginsChanged;

    IReadOnlyList<PluginDescriptor> GetInstalledPlugins();
    PluginState GetPluginState(string pluginId);
}
