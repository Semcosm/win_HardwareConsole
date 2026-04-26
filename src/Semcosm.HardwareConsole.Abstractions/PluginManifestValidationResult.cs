using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record PluginManifestValidationResult(
    bool IsValid,
    string ManifestPath,
    string? PluginId,
    PluginManifestDescriptor? Manifest,
    IReadOnlyList<PluginManifestIssue> Issues,
    string Message);
