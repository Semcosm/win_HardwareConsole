using System;
using System.Collections.Generic;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class ManifestBackedPluginRegistry : IPluginRegistry
{
    private readonly IReadOnlyList<PluginDescriptor> _installedPlugins;
    private readonly IReadOnlyDictionary<string, PluginState> _stateByPluginId;

    public event EventHandler? PluginsChanged;

    public ManifestBackedPluginRegistry(PluginManifestCatalog manifestCatalog)
    {
        _installedPlugins = manifestCatalog
            .GetValidManifests()
            .Select(MapDescriptor)
            .ToArray();

        _stateByPluginId = _installedPlugins.ToDictionary(
            plugin => plugin.Id,
            plugin => ResolvePluginState(plugin.Id),
            StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyList<PluginDescriptor> GetInstalledPlugins() => _installedPlugins;

    public PluginState GetPluginState(string pluginId)
    {
        return _stateByPluginId.TryGetValue(pluginId, out var state)
            ? state
            : PluginState.Unknown;
    }

    private static PluginDescriptor MapDescriptor(PluginManifestDescriptor manifest)
    {
        var capabilityIds = manifest.Capabilities
            .SelectMany(capability => capability.CapabilityTags.Count > 0
                ? capability.CapabilityTags
                : new[] { capability.Id })
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new PluginDescriptor(
            manifest.Id,
            manifest.DisplayName,
            manifest.Vendor,
            manifest.Version,
            manifest.RiskLevel,
            capabilityIds,
            manifest.Devices.Select(device => device.DisplayName).Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
            manifest.Devices.Select(device => device.Id).Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
            manifest.Sensors.Select(sensor => sensor.Id).Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
            manifest.Controls.Select(control => control.Id).Distinct(StringComparer.OrdinalIgnoreCase).ToArray());
    }

    private static PluginState ResolvePluginState(string pluginId)
    {
        return pluginId switch
        {
            "semcosm.windows.power" => PluginState.Enabled,
            _ => PluginState.Mocked
        };
    }
}
