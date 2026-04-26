using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Models;

namespace Semcosm.HardwareConsole.App.ViewModels;

public sealed class DiagnosticsViewModel : INotifyPropertyChanged
{
    private readonly IDiagnosticsProvider _diagnosticsProvider;

    public event PropertyChangedEventHandler? PropertyChanged;

    public DiagnosticsViewModel(IDiagnosticsProvider diagnosticsProvider)
    {
        _diagnosticsProvider = diagnosticsProvider;
        _diagnosticsProvider.RecordsChanged += DiagnosticsProvider_RecordsChanged;

        SystemHealth = new ObservableCollection<DiagnosticCardModel>();
        DiagnosticLog = new ObservableCollection<DiagnosticRecordModel>();

        Refresh();
    }

    public ObservableCollection<DiagnosticCardModel> SystemHealth { get; }

    public ObservableCollection<DiagnosticRecordModel> DiagnosticLog { get; }

    private void DiagnosticsProvider_RecordsChanged(object? sender, EventArgs e)
    {
        Refresh();
    }

    private void Refresh()
    {
        var records = _diagnosticsProvider.GetRecords();

        RebuildSystemHealth(records);
        RebuildDiagnosticLog(records);
        OnPropertyChanged(nameof(SystemHealth));
        OnPropertyChanged(nameof(DiagnosticLog));
    }

    private void RebuildSystemHealth(IReadOnlyList<DiagnosticRecord> records)
    {
        SystemHealth.Clear();

        SystemHealth.Add(BuildSourceCard(
            "Routes",
            DiagnosticSource.Routes,
            _diagnosticsProvider.GetLatest(DiagnosticSource.Routes),
            "No route diagnostics yet."));

        SystemHealth.Add(BuildPluginCard(records));

        SystemHealth.Add(BuildSourceCard(
            "Profiles",
            DiagnosticSource.Profiles,
            _diagnosticsProvider.GetLatest(DiagnosticSource.Profiles),
            "No profile apply diagnostics yet."));

        SystemHealth.Add(BuildSourceCard(
            "Fans",
            DiagnosticSource.Fans,
            _diagnosticsProvider.GetLatest(DiagnosticSource.Fans),
            "No fan preview diagnostics yet."));

        SystemHealth.Add(BuildSourceCard(
            "Thermal",
            DiagnosticSource.Thermal,
            _diagnosticsProvider.GetLatest(DiagnosticSource.Thermal),
            "No thermal preview diagnostics yet."));
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

        var severity = latestPluginRecords.Any(record => record.Severity == DiagnosticSeverity.Error)
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
            DiagnosticSeverity.Error => "Error",
            DiagnosticSeverity.Warning => "Warning",
            _ => "Info"
        };
    }

    private static string GetStatusText(DiagnosticSeverity severity)
    {
        return severity switch
        {
            DiagnosticSeverity.Error => "Attention Required",
            DiagnosticSeverity.Warning => "Warning",
            _ => "OK"
        };
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
