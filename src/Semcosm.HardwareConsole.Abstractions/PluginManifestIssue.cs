namespace Semcosm.HardwareConsole.Abstractions;

public sealed record PluginManifestIssue(
    string Code,
    DiagnosticSeverity Severity,
    string Message,
    string? RelatedManifestPath = null,
    string? RelatedPluginId = null,
    string? RelatedControlId = null,
    string? RelatedRouteTag = null);
