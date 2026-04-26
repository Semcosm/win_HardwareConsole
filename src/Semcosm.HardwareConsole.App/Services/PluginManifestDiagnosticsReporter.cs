using System;
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
        var validationResults = _manifestCatalog.GetValidationResults();
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
                result.PluginId ?? result.ManifestPath,
                DateTimeOffset.UtcNow));

            foreach (var issue in result.Issues)
            {
                _diagnosticsSink.Report(new DiagnosticRecord(
                    issue.Severity,
                    DiagnosticSource.Plugins,
                    $"plugins.{issue.Code}",
                    issue.Message,
                    issue.RelatedControlId
                        ?? issue.RelatedPluginId
                        ?? issue.RelatedManifestPath
                        ?? result.ManifestPath,
                    DateTimeOffset.UtcNow));
            }
        }
    }
}
