using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.UI.Dispatching;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Models;

namespace Semcosm.HardwareConsole.App.ViewModels;

public sealed class DiagnosticsViewModel : INotifyPropertyChanged, IDisposable
{
    private static readonly DiagnosticHealthSurface[] HealthSurfaces =
    [
        new("Routes", DiagnosticSource.Routes, "No route diagnostics yet."),
        new("Plugins", DiagnosticSource.Plugins, "No plugin diagnostics yet."),
        new("Profiles", DiagnosticSource.Profiles, "No profile apply diagnostics yet."),
        new("Fans", DiagnosticSource.Fans, "No fan preview diagnostics yet."),
        new("Thermal", DiagnosticSource.Thermal, "No thermal preview diagnostics yet.")
    ];

    private readonly DispatcherQueue? _dispatcherQueue;
    private readonly IDiagnosticsProvider _diagnosticsProvider;
    private bool _disposed;

    public event PropertyChangedEventHandler? PropertyChanged;

    public DiagnosticsViewModel(IDiagnosticsProvider diagnosticsProvider)
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _diagnosticsProvider = diagnosticsProvider;
        _diagnosticsProvider.RecordsChanged += DiagnosticsProvider_RecordsChanged;

        SystemHealth = new ObservableCollection<DiagnosticCardModel>();
        DiagnosticLog = new ObservableCollection<DiagnosticRecordModel>();

        Refresh();
    }

    public ObservableCollection<DiagnosticCardModel> SystemHealth { get; }

    public ObservableCollection<DiagnosticRecordModel> DiagnosticLog { get; }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _diagnosticsProvider.RecordsChanged -= DiagnosticsProvider_RecordsChanged;
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private void DiagnosticsProvider_RecordsChanged(object? sender, EventArgs e)
    {
        if (_disposed)
        {
            return;
        }

        if (_dispatcherQueue is not null && !_dispatcherQueue.HasThreadAccess)
        {
            _dispatcherQueue.TryEnqueue(Refresh);
            return;
        }

        Refresh();
    }

    private void Refresh()
    {
        if (_disposed)
        {
            return;
        }

        var records = _diagnosticsProvider.GetRecords();
        var latestRecordsBySource = records
            .GroupBy(record => record.Source)
            .ToDictionary(
                group => group.Key,
                group => group.OrderByDescending(record => record.Timestamp).First());

        RebuildSystemHealth(records, latestRecordsBySource);
        RebuildDiagnosticLog(records);
        OnPropertyChanged(nameof(SystemHealth));
        OnPropertyChanged(nameof(DiagnosticLog));
    }

    private void RebuildSystemHealth(
        IReadOnlyList<DiagnosticRecord> records,
        IReadOnlyDictionary<DiagnosticSource, DiagnosticRecord> latestRecordsBySource)
    {
        SystemHealth.Clear();

        foreach (var surface in HealthSurfaces)
        {
            if (surface.Source == DiagnosticSource.Plugins)
            {
                SystemHealth.Add(BuildPluginCard(records));
                continue;
            }

            latestRecordsBySource.TryGetValue(surface.Source, out var record);
            SystemHealth.Add(BuildSourceCard(surface.Title, surface.Source, record, surface.EmptyText));
        }
    }

    private void RebuildDiagnosticLog(IReadOnlyList<DiagnosticRecord> records)
    {
        DiagnosticLog.Clear();

        foreach (var record in records)
        {
            DiagnosticLog.Add(new DiagnosticRecordModel
            {
                Severity = record.Severity,
                SeverityText = GetSeverityText(record.Severity),
                SourceText = record.Source.ToString(),
                Code = record.Code,
                Message = record.Message,
                RelatedId = string.IsNullOrWhiteSpace(record.RelatedId) ? "No related id" : record.RelatedId,
                TimestampText = record.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
    }

    private DiagnosticCardModel BuildSourceCard(
        string title,
        DiagnosticSource source,
        DiagnosticRecord? record,
        string emptyText)
    {
        if (record is null)
        {
            return new DiagnosticCardModel
            {
                Title = title,
                StatusText = "No Data",
                DetailsText = emptyText,
                SourceText = source.ToString(),
                Severity = DiagnosticSeverity.Info,
                SeverityText = GetSeverityText(DiagnosticSeverity.Info)
            };
        }

        return new DiagnosticCardModel
        {
            Title = title,
            StatusText = GetStatusText(record.Severity),
            DetailsText = record.Message,
            SourceText = source.ToString(),
            Severity = record.Severity,
            SeverityText = GetSeverityText(record.Severity)
        };
    }

    private DiagnosticCardModel BuildPluginCard(IReadOnlyList<DiagnosticRecord> records)
    {
        var latestPluginRecords = records
            .Where(record => record.Source == DiagnosticSource.Plugins)
            .GroupBy(record => string.IsNullOrWhiteSpace(record.RelatedId) ? record.Code : record.RelatedId)
            .Select(group => group.OrderByDescending(record => record.Timestamp).First())
            .ToArray();

        if (latestPluginRecords.Length == 0)
        {
            return new DiagnosticCardModel
            {
                Title = "Plugins",
                StatusText = "No Data",
                DetailsText = "No plugin diagnostics yet.",
                SourceText = DiagnosticSource.Plugins.ToString(),
                Severity = DiagnosticSeverity.Info,
                SeverityText = GetSeverityText(DiagnosticSeverity.Info)
            };
        }

        var severity = latestPluginRecords.Any(record => record.Severity == DiagnosticSeverity.Critical)
            ? DiagnosticSeverity.Critical
            : latestPluginRecords.Any(record => record.Severity == DiagnosticSeverity.Error)
                ? DiagnosticSeverity.Error
            : latestPluginRecords.Any(record => record.Severity == DiagnosticSeverity.Warning)
                ? DiagnosticSeverity.Warning
                : DiagnosticSeverity.Info;

        var stateSummary = latestPluginRecords
            .GroupBy(record => record.Code.Replace("plugins.state.", string.Empty, StringComparison.OrdinalIgnoreCase))
            .OrderBy(group => group.Key)
            .Select(group => $"{group.Key}: {group.Count()}")
            .ToArray();

        return new DiagnosticCardModel
        {
            Title = "Plugins",
            StatusText = GetStatusText(severity),
            DetailsText = string.Join(" · ", stateSummary),
            SourceText = DiagnosticSource.Plugins.ToString(),
            Severity = severity,
            SeverityText = GetSeverityText(severity)
        };
    }

    private static string GetSeverityText(DiagnosticSeverity severity)
    {
        return severity switch
        {
            DiagnosticSeverity.Critical => "Critical",
            DiagnosticSeverity.Error => "Error",
            DiagnosticSeverity.Warning => "Warning",
            _ => "Info"
        };
    }

    private static string GetStatusText(DiagnosticSeverity severity)
    {
        return severity switch
        {
            DiagnosticSeverity.Critical => "Critical",
            DiagnosticSeverity.Error => "Attention Required",
            DiagnosticSeverity.Warning => "Warning",
            _ => "OK"
        };
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private sealed record DiagnosticHealthSurface(
        string Title,
        DiagnosticSource Source,
        string EmptyText);
}
