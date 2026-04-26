using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Services;

public sealed record PluginManifestLoadResult(
    string ManifestPath,
    PluginManifestDescriptor? Manifest,
    string? LoadError);
