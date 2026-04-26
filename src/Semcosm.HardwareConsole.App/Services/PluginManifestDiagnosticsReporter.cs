using System;
using System.Collections.Generic;
using System.IO;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class PluginManifestDiagnosticsReporter
{
    private readonly PluginManifestCatalog _manifestCatalog;
    private readonly IDiagnosticsSink _diagnosticsSink;

    public PluginManifestDiagnosticsReporter(
        PluginManifestCatalog manifestCatalog,
        IDiagnosticsSink diagnosticsSink)
    {
        _manifestCatalog = manifestCatalog;
        _diagnosticsSink = diagnosticsSink;
    }

    public void PublishSnapshot()
    {
        PublishSnapshot(_manifestCatalog.GetValidationResults());
    }

    public void PublishSnapshot(IReadOnlyList<PluginManifestValidationResult> validationResults)
    {
        if (validationResults.Count == 0)
        {
            _diagnosticsSink.Report(new DiagnosticRecord(
                DiagnosticSeverity.Warning,
                DiagnosticSource.Plugins,
                "plugins.manifest.invalid",
                "No plugin manifests were found under the current plugins root.",
                "plugins",
                DateTimeOffset.UtcNow));
            return;
        }

        foreach (var result in validationResults)
        {
            if (result.IsValid && result.Manifest is not null)
            {
                _diagnosticsSink.Report(new DiagnosticRecord(
                    DiagnosticSeverity.Info,
                    DiagnosticSource.Plugins,
                    "plugins.manifest.loaded",
                    $"{result.Manifest.DisplayName} manifest loaded from {Path.GetFileName(result.ManifestPath)}.",
                    result.Manifest.Id,
                    DateTimeOffset.UtcNow));
                continue;
            }

            _diagnosticsSink.Report(new DiagnosticRecord(
                DiagnosticSeverity.Error,
                DiagnosticSource.Plugins,
                "plugins.manifest.invalid",
                result.Message,
                ResolveRelatedId(result),
                DateTimeOffset.UtcNow));

            foreach (var issue in result.Issues)
            {
                _diagnosticsSink.Report(new DiagnosticRecord(
                    issue.Severity,
                    DiagnosticSource.Plugins,
                    $"plugins.{issue.Code}",
                    issue.Message,
                    issue.RelatedControlId
                        ?? issue.RelatedRouteTag
                        ?? issue.RelatedPluginId
                        ?? issue.RelatedManifestPath
                        ?? result.ManifestPath,
                    DateTimeOffset.UtcNow));
            }
        }
    }

    private static string ResolveRelatedId(PluginManifestValidationResult result)
    {
        if (!string.IsNullOrWhiteSpace(result.PluginId))
        {
            return result.PluginId;
        }

        if (!string.IsNullOrWhiteSpace(result.Manifest?.Id))
        {
            return result.Manifest.Id;
        }

        return result.ManifestPath;
    }
}
