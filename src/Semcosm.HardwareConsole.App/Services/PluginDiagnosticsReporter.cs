using System;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class PluginDiagnosticsReporter
{
    private readonly IPluginRegistry _pluginRegistry;
    private readonly IDiagnosticsSink _diagnosticsSink;

    public PluginDiagnosticsReporter(
        IPluginRegistry pluginRegistry,
        IDiagnosticsSink diagnosticsSink)
    {
        _pluginRegistry = pluginRegistry;
        _diagnosticsSink = diagnosticsSink;
    }

    public void PublishSnapshot()
    {
        foreach (var plugin in _pluginRegistry.GetInstalledPlugins())
        {
            var state = _pluginRegistry.GetPluginState(plugin.Id);
            var severity = state switch
            {
                PluginState.Failed or PluginState.Blocked or PluginState.Unsupported => DiagnosticSeverity.Error,
                PluginState.Mocked or PluginState.Disabled or PluginState.Unknown => DiagnosticSeverity.Warning,
                _ => DiagnosticSeverity.Info
            };

            _diagnosticsSink.Report(new DiagnosticRecord(
                severity,
                DiagnosticSource.Plugins,
                $"plugins.state.{state.ToString().ToLowerInvariant()}",
                $"{plugin.DisplayName} is currently {state}.",
                plugin.Id,
                DateTimeOffset.UtcNow));
        }
    }
}
