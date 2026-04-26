using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class ManifestBackedPluginRegistry : IPluginRegistry
{
    private readonly PluginManifestCatalog _manifestCatalog;
    private IReadOnlyList<PluginDescriptor> _installedPlugins = [];
    private IReadOnlyDictionary<string, PluginState> _stateByPluginId = new Dictionary<string, PluginState>(StringComparer.OrdinalIgnoreCase);

    public event EventHandler? PluginsChanged;

    public ManifestBackedPluginRegistry(PluginManifestCatalog manifestCatalog)
    {
        _manifestCatalog = manifestCatalog;
        ReloadInternal(raiseEvent: false);
    }

    public IReadOnlyList<PluginDescriptor> GetInstalledPlugins() => _installedPlugins;

    public PluginState GetPluginState(string pluginId)
    {
        return _stateByPluginId.TryGetValue(pluginId, out var state)
            ? state
            : PluginState.Unknown;
    }

    public void Reload()
    {
        ReloadInternal(raiseEvent: true);
    }

    private static PluginDescriptor MapDescriptor(PluginManifestValidationResult result)
    {
        var manifest = result.Manifest;
        var pluginId = ResolvePluginId(result);
        var displayName = manifest?.DisplayName;
        if (string.IsNullOrWhiteSpace(displayName))
        {
            displayName = GetFallbackDisplayName(pluginId);
        }

        var capabilityIds = manifest is null
            ? []
            : manifest.Capabilities
                .SelectMany(capability => capability.CapabilityTags.Count > 0
                    ? capability.CapabilityTags
                    : new[] { capability.Id })
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

        return new PluginDescriptor(
            pluginId,
            displayName,
            manifest?.Vendor ?? "Unknown Vendor",
            manifest?.Version ?? "Unknown",
            manifest?.RiskLevel ?? HardwareRiskLevel.ReadOnly,
            capabilityIds,
            manifest?.Devices.Select(device => device.DisplayName).Distinct(StringComparer.OrdinalIgnoreCase).ToArray() ?? [],
            manifest?.Devices.Select(device => device.Id).Distinct(StringComparer.OrdinalIgnoreCase).ToArray() ?? [],
            manifest?.Sensors.Select(sensor => sensor.Id).Distinct(StringComparer.OrdinalIgnoreCase).ToArray() ?? [],
            manifest?.Controls.Select(control => control.Id).Distinct(StringComparer.OrdinalIgnoreCase).ToArray() ?? []);
    }

    private void ReloadInternal(bool raiseEvent)
    {
        var validationResults = _manifestCatalog.Reload();
        _installedPlugins = validationResults
            .Select(MapDescriptor)
            .ToArray();

        _stateByPluginId = validationResults
            .Select(result => new
            {
                PluginId = ResolvePluginId(result),
                State = ResolvePluginState(result)
            })
            .Where(entry => !string.IsNullOrWhiteSpace(entry.PluginId))
            .GroupBy(entry => entry.PluginId, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => ResolveAggregateState(group.Select(entry => entry.State)),
                StringComparer.OrdinalIgnoreCase);

        if (raiseEvent)
        {
            PluginsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private static PluginState ResolvePluginState(PluginManifestValidationResult result)
    {
        if (result.Manifest is null)
        {
            return PluginState.Failed;
        }

        if (result.IsValid)
        {
            return RequiresConfirmation(result.Manifest.RiskLevel) || result.Manifest.RequiresConfirmation
                ? PluginState.Mocked
                : PluginState.Enabled;
        }

        if (result.Issues.Any(issue => string.Equals(
                issue.Code,
                "manifest.unsupported_schema_version",
                StringComparison.OrdinalIgnoreCase)))
        {
            return PluginState.Unsupported;
        }

        if (result.Issues.Any(issue => string.Equals(
                issue.Code,
                "manifest.risk_requires_confirmation",
                StringComparison.OrdinalIgnoreCase)))
        {
            return PluginState.Blocked;
        }

        return PluginState.Failed;
    }

    private static bool RequiresConfirmation(HardwareRiskLevel riskLevel)
    {
        return riskLevel is HardwareRiskLevel.HardwareWrite
            or HardwareRiskLevel.KernelDriverRequired
            or HardwareRiskLevel.Experimental;
    }

    private static PluginState ResolveAggregateState(IEnumerable<PluginState> states)
    {
        if (states.Contains(PluginState.Failed))
        {
            return PluginState.Failed;
        }

        if (states.Contains(PluginState.Blocked))
        {
            return PluginState.Blocked;
        }

        if (states.Contains(PluginState.Unsupported))
        {
            return PluginState.Unsupported;
        }

        if (states.Contains(PluginState.Mocked))
        {
            return PluginState.Mocked;
        }

        if (states.Contains(PluginState.Enabled))
        {
            return PluginState.Enabled;
        }

        if (states.Contains(PluginState.Available))
        {
            return PluginState.Available;
        }

        if (states.Contains(PluginState.Disabled))
        {
            return PluginState.Disabled;
        }

        return PluginState.Unknown;
    }

    private static string GetFallbackPluginId(string manifestPath)
    {
        var directoryName = Path.GetFileName(Path.GetDirectoryName(manifestPath));
        return string.IsNullOrWhiteSpace(directoryName)
            ? Path.GetFileNameWithoutExtension(manifestPath)
            : directoryName;
    }

    private static string ResolvePluginId(PluginManifestValidationResult result)
    {
        if (!string.IsNullOrWhiteSpace(result.Manifest?.Id))
        {
            return result.Manifest.Id;
        }

        if (!string.IsNullOrWhiteSpace(result.PluginId))
        {
            return result.PluginId;
        }

        return GetFallbackPluginId(result.ManifestPath);
    }

    private static string GetFallbackDisplayName(string pluginId)
    {
        return $"{pluginId} (Manifest Error)";
    }
}
