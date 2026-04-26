using System;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class PluginDiagnosticsReporter : IDisposable
{
    private readonly IPluginRegistry _pluginRegistry;
    private readonly IDiagnosticsSink _diagnosticsSink;
    private bool _disposed;

    public PluginDiagnosticsReporter(
        IPluginRegistry pluginRegistry,
        IDiagnosticsSink diagnosticsSink)
    {
        _pluginRegistry = pluginRegistry;
        _diagnosticsSink = diagnosticsSink;
        _pluginRegistry.PluginsChanged += PluginRegistry_PluginsChanged;
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

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _pluginRegistry.PluginsChanged -= PluginRegistry_PluginsChanged;
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private void PluginRegistry_PluginsChanged(object? sender, EventArgs e)
    {
        PublishSnapshot();
    }
}
