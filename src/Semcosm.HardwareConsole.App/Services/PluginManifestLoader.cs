using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class PluginManifestLoader
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IReadOnlyList<PluginManifestLoadResult> LoadManifests()
    {
        var pluginsRoot = ResolvePluginsRoot();
        if (pluginsRoot is null)
        {
            return [];
        }

        var manifestPaths = Directory.EnumerateDirectories(pluginsRoot)
            .Select(directory => Path.Combine(directory, "plugin.json"))
            .Where(File.Exists)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var results = new List<PluginManifestLoadResult>();

        foreach (var manifestPath in manifestPaths)
        {
            try
            {
                var json = File.ReadAllText(manifestPath);
                var document = JsonSerializer.Deserialize<PluginManifestJson>(json, _serializerOptions);

                if (document is null)
                {
                    results.Add(new PluginManifestLoadResult(
                        manifestPath,
                        null,
                        "Manifest file did not contain a valid JSON document."));
                    continue;
                }

                results.Add(new PluginManifestLoadResult(
                    manifestPath,
                    MapManifest(document),
                    null));
            }
            catch (Exception ex) when (ex is IOException or JsonException or FormatException)
            {
                results.Add(new PluginManifestLoadResult(
                    manifestPath,
                    null,
                    ex.Message));
            }
        }

        return results;
    }

    private static string? ResolvePluginsRoot()
    {
        foreach (var candidate in EnumerateCandidateRoots(AppContext.BaseDirectory)
                     .Concat(EnumerateCandidateRoots(Directory.GetCurrentDirectory())))
        {
            if (Directory.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    private static IEnumerable<string> EnumerateCandidateRoots(string startPath)
    {
        for (var directory = new DirectoryInfo(startPath); directory is not null; directory = directory.Parent)
        {
            yield return Path.Combine(directory.FullName, "plugins");
        }
    }

    private static PluginManifestDescriptor MapManifest(PluginManifestJson json)
    {
        return new PluginManifestDescriptor(
            json.SchemaVersion ?? string.Empty,
            json.Id ?? string.Empty,
            json.DisplayName ?? string.Empty,
            json.Vendor ?? string.Empty,
            json.Version ?? string.Empty,
            ParseEnum<HardwareRiskLevel>(json.RiskLevel, nameof(json.RiskLevel)),
            json.RequiresConfirmation,
            (json.Capabilities ?? []).Select(capability => new PluginCapabilityDeclaration(
                capability.Id ?? string.Empty,
                capability.DisplayName ?? string.Empty,
                capability.Description ?? string.Empty,
                capability.CapabilityTags ?? [])).ToArray(),
            (json.Devices ?? []).Select(device => new PluginDeviceDeclaration(
                device.Id ?? string.Empty,
                device.DisplayName ?? string.Empty,
                device.Vendor ?? string.Empty,
                device.Model ?? string.Empty,
                device.CapabilityIds ?? [])).ToArray(),
            (json.Sensors ?? []).Select(sensor => new PluginSensorDeclaration(
                sensor.Id ?? string.Empty,
                sensor.DisplayName ?? string.Empty,
                sensor.DeviceId ?? string.Empty,
                ParseEnum<SensorKind>(sensor.Kind, $"{nameof(json.Sensors)}.{nameof(sensor.Kind)}"),
                sensor.Unit ?? string.Empty,
                sensor.CapabilityTags ?? [])).ToArray(),
            (json.Controls ?? []).Select(control => new PluginControlDeclaration(
                control.Id ?? string.Empty,
                control.DisplayName ?? string.Empty,
                control.DeviceId ?? string.Empty,
                ParseEnum<ControlKind>(control.Kind, $"{nameof(json.Controls)}.{nameof(control.Kind)}"),
                ParseEnum<ControlRiskLevel>(control.RiskLevel, $"{nameof(json.Controls)}.{nameof(control.RiskLevel)}"),
                control.CapabilityTags ?? [])).ToArray(),
            (json.Routes ?? []).Select(route => new PluginRouteDeclaration(
                route.Tag ?? string.Empty,
                route.Title ?? string.Empty,
                route.Kind ?? string.Empty,
                route.Icon ?? string.Empty,
                route.IsFooter)).ToArray());
    }

    private static TEnum ParseEnum<TEnum>(string? value, string fieldName)
        where TEnum : struct, Enum
    {
        if (!string.IsNullOrWhiteSpace(value)
            && Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed))
        {
            return parsed;
        }

        throw new FormatException($"Manifest field '{fieldName}' contains unsupported enum value '{value}'.");
    }

    private sealed class PluginManifestJson
    {
        public string? SchemaVersion { get; set; }
        public string? Id { get; set; }
        public string? DisplayName { get; set; }
        public string? Vendor { get; set; }
        public string? Version { get; set; }
        public string? RiskLevel { get; set; }
        public bool RequiresConfirmation { get; set; }
        public List<PluginCapabilityJson>? Capabilities { get; set; }
        public List<PluginDeviceJson>? Devices { get; set; }
        public List<PluginSensorJson>? Sensors { get; set; }
        public List<PluginControlJson>? Controls { get; set; }
        public List<PluginRouteJson>? Routes { get; set; }
    }

    private sealed class PluginCapabilityJson
    {
        public string? Id { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public List<string>? CapabilityTags { get; set; }
    }

    private sealed class PluginDeviceJson
    {
        public string? Id { get; set; }
        public string? DisplayName { get; set; }
        public string? Vendor { get; set; }
        public string? Model { get; set; }
        public List<string>? CapabilityIds { get; set; }
    }

    private sealed class PluginSensorJson
    {
        public string? Id { get; set; }
        public string? DisplayName { get; set; }
        public string? DeviceId { get; set; }
        public string? Kind { get; set; }
        public string? Unit { get; set; }
        public List<string>? CapabilityTags { get; set; }
    }

    private sealed class PluginControlJson
    {
        public string? Id { get; set; }
        public string? DisplayName { get; set; }
        public string? DeviceId { get; set; }
        public string? Kind { get; set; }
        public string? RiskLevel { get; set; }
        public List<string>? CapabilityTags { get; set; }
    }

    private sealed class PluginRouteJson
    {
        public string? Tag { get; set; }
        public string? Title { get; set; }
        public string? Kind { get; set; }
        public string? Icon { get; set; }
        public bool IsFooter { get; set; }
    }
}
