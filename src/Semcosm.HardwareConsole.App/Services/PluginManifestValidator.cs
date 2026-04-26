using System;
using System.Collections.Generic;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class PluginManifestValidator
{
    private const string SupportedSchemaVersion = "0.1.0";

    public IReadOnlyList<PluginManifestValidationResult> Validate(
        IReadOnlyList<PluginManifestLoadResult> loadResults)
    {
        var issuesByPath = loadResults.ToDictionary(
            result => result.ManifestPath,
            _ => new List<PluginManifestIssue>(),
            StringComparer.OrdinalIgnoreCase);

        foreach (var loadResult in loadResults)
        {
            issuesByPath[loadResult.ManifestPath].AddRange(ValidateLoadResult(loadResult));
        }

        AddDuplicatePluginIdIssues(loadResults, issuesByPath);
        AddDuplicateControlIdIssues(loadResults, issuesByPath);

        return loadResults
            .Select(loadResult =>
            {
                var issues = issuesByPath[loadResult.ManifestPath].ToArray();
                var isValid = issues.Length == 0;

                return new PluginManifestValidationResult(
                    isValid,
                    loadResult.ManifestPath,
                    loadResult.Manifest?.Id,
                    isValid ? loadResult.Manifest : null,
                    issues,
                    isValid
                        ? "Plugin manifest validation passed."
                        : "Plugin manifest validation failed.");
            })
            .ToArray();
    }

    private static IReadOnlyList<PluginManifestIssue> ValidateLoadResult(PluginManifestLoadResult loadResult)
    {
        if (!string.IsNullOrWhiteSpace(loadResult.LoadError))
        {
            return
            [
                new PluginManifestIssue(
                    "manifest.invalid",
                    DiagnosticSeverity.Error,
                    $"Plugin manifest could not be loaded: {loadResult.LoadError}",
                    loadResult.ManifestPath)
            ];
        }

        if (loadResult.Manifest is null)
        {
            return
            [
                new PluginManifestIssue(
                    "manifest.invalid",
                    DiagnosticSeverity.Error,
                    "Plugin manifest did not produce a descriptor.",
                    loadResult.ManifestPath)
            ];
        }

        var manifest = loadResult.Manifest;
        var issues = new List<PluginManifestIssue>();

        if (string.IsNullOrWhiteSpace(manifest.SchemaVersion))
        {
            issues.Add(CreateIssue(
                "manifest.invalid",
                "Manifest schemaVersion is required.",
                loadResult.ManifestPath,
                manifest.Id));
        }
        else if (!string.Equals(manifest.SchemaVersion, SupportedSchemaVersion, StringComparison.OrdinalIgnoreCase))
        {
            issues.Add(CreateIssue(
                "manifest.unsupported_schema_version",
                $"Manifest schema version '{manifest.SchemaVersion}' is not supported. Expected '{SupportedSchemaVersion}'.",
                loadResult.ManifestPath,
                manifest.Id));
        }

        if (string.IsNullOrWhiteSpace(manifest.Id))
        {
            issues.Add(CreateIssue(
                "manifest.invalid",
                "Manifest id is required.",
                loadResult.ManifestPath));
        }

        if (string.IsNullOrWhiteSpace(manifest.DisplayName))
        {
            issues.Add(CreateIssue(
                "manifest.invalid",
                "Manifest displayName is required.",
                loadResult.ManifestPath,
                manifest.Id));
        }

        if (string.IsNullOrWhiteSpace(manifest.Vendor))
        {
            issues.Add(CreateIssue(
                "manifest.invalid",
                "Manifest vendor is required.",
                loadResult.ManifestPath,
                manifest.Id));
        }

        if (string.IsNullOrWhiteSpace(manifest.Version))
        {
            issues.Add(CreateIssue(
                "manifest.invalid",
                "Manifest version is required.",
                loadResult.ManifestPath,
                manifest.Id));
        }

        if (RequiresConfirmation(manifest.RiskLevel) && !manifest.RequiresConfirmation)
        {
            issues.Add(CreateIssue(
                "manifest.risk_requires_confirmation",
                "High-risk plugin manifests must declare requiresConfirmation=true.",
                loadResult.ManifestPath,
                manifest.Id));
        }

        var deviceIds = manifest.Devices
            .Select(device => device.Id)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var sensor in manifest.Sensors)
        {
            if (string.IsNullOrWhiteSpace(sensor.Id))
            {
                issues.Add(CreateIssue(
                    "manifest.invalid",
                    "Sensor declaration id is required.",
                    loadResult.ManifestPath,
                    manifest.Id));
            }

            if (!string.IsNullOrWhiteSpace(sensor.DeviceId) && !deviceIds.Contains(sensor.DeviceId))
            {
                issues.Add(CreateIssue(
                    "manifest.invalid",
                    $"Sensor '{sensor.Id}' references unknown device '{sensor.DeviceId}'.",
                    loadResult.ManifestPath,
                    manifest.Id));
            }
        }

        foreach (var control in manifest.Controls)
        {
            if (string.IsNullOrWhiteSpace(control.Id))
            {
                issues.Add(CreateIssue(
                    "manifest.invalid",
                    "Control declaration id is required.",
                    loadResult.ManifestPath,
                    manifest.Id));
            }

            if (!string.IsNullOrWhiteSpace(control.DeviceId) && !deviceIds.Contains(control.DeviceId))
            {
                issues.Add(CreateIssue(
                    "manifest.invalid",
                    $"Control '{control.Id}' references unknown device '{control.DeviceId}'.",
                    loadResult.ManifestPath,
                    manifest.Id,
                    control.Id));
            }
        }

        return issues;
    }

    private static void AddDuplicatePluginIdIssues(
        IReadOnlyList<PluginManifestLoadResult> loadResults,
        IReadOnlyDictionary<string, List<PluginManifestIssue>> issuesByPath)
    {
        var duplicateGroups = loadResults
            .Where(result => result.Manifest is not null && !string.IsNullOrWhiteSpace(result.Manifest.Id))
            .GroupBy(result => result.Manifest!.Id, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1);

        foreach (var group in duplicateGroups)
        {
            foreach (var result in group)
            {
                issuesByPath[result.ManifestPath].Add(CreateIssue(
                    "manifest.duplicate_plugin_id",
                    $"Duplicate plugin id '{group.Key}' was declared by more than one manifest.",
                    result.ManifestPath,
                    result.Manifest!.Id));
            }
        }
    }

    private static void AddDuplicateControlIdIssues(
        IReadOnlyList<PluginManifestLoadResult> loadResults,
        IReadOnlyDictionary<string, List<PluginManifestIssue>> issuesByPath)
    {
        var controlGroups = loadResults
            .Where(result => result.Manifest is not null)
            .SelectMany(result => result.Manifest!.Controls.Select(control => new
            {
                result.ManifestPath,
                result.Manifest.Id,
                ControlId = control.Id
            }))
            .Where(entry => !string.IsNullOrWhiteSpace(entry.ControlId))
            .GroupBy(entry => entry.ControlId, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1);

        foreach (var group in controlGroups)
        {
            foreach (var entry in group)
            {
                issuesByPath[entry.ManifestPath].Add(CreateIssue(
                    "manifest.duplicate_control_id",
                    $"Duplicate control id '{entry.ControlId}' was declared by more than one manifest.",
                    entry.ManifestPath,
                    entry.Id,
                    entry.ControlId));
            }
        }
    }

    private static bool RequiresConfirmation(HardwareRiskLevel riskLevel)
    {
        return riskLevel is HardwareRiskLevel.HardwareWrite
            or HardwareRiskLevel.KernelDriverRequired
            or HardwareRiskLevel.Experimental;
    }

    private static PluginManifestIssue CreateIssue(
        string code,
        string message,
        string manifestPath,
        string? pluginId = null,
        string? controlId = null,
        string? routeTag = null)
    {
        return new PluginManifestIssue(
            code,
            DiagnosticSeverity.Error,
            message,
            manifestPath,
            pluginId,
            controlId,
            routeTag);
    }
}
