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
    private const string AllFilterId = "all";
    private static readonly DiagnosticHealthSurface[] HealthSurfaces =
    [
        new("Routes", DiagnosticSource.Routes, "No route diagnostics yet."),
        new("Plugins", DiagnosticSource.Plugins, "No plugin diagnostics yet."),
        new("Profiles", DiagnosticSource.Profiles, "No profile apply diagnostics yet."),
        new("Fans", DiagnosticSource.Fans, "No fan preview diagnostics yet."),
        new("Power", DiagnosticSource.Power, "No power preview diagnostics yet."),
        new("Scheduler", DiagnosticSource.Scheduler, "No scheduler preview diagnostics yet."),
        new("Thermal", DiagnosticSource.Thermal, "No thermal preview diagnostics yet.")
    ];

    private readonly DispatcherQueue? _dispatcherQueue;
    private readonly IDiagnosticsSessionController _diagnosticsSessionController;
    private readonly IDiagnosticsProvider _diagnosticsProvider;
    private bool _disposed;
    private string _selectedSeverityFilterId = AllFilterId;
    private string _selectedSourceFilterId = AllFilterId;

    public event PropertyChangedEventHandler? PropertyChanged;

    public DiagnosticsViewModel(
        IDiagnosticsProvider diagnosticsProvider,
        IDiagnosticsSessionController diagnosticsSessionController)
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _diagnosticsProvider = diagnosticsProvider;
        _diagnosticsSessionController = diagnosticsSessionController;
        _diagnosticsProvider.RecordsChanged += DiagnosticsProvider_RecordsChanged;

        SystemHealth = new ObservableCollection<DiagnosticCardModel>();
        DiagnosticLog = new ObservableCollection<DiagnosticRecordModel>();
        SeverityFilters = new ObservableCollection<SelectionOptionModel>(BuildSeverityFilters());
        SourceFilters = new ObservableCollection<SelectionOptionModel>(BuildSourceFilters());

        Refresh();
    }

    public ObservableCollection<DiagnosticCardModel> SystemHealth { get; }

    public ObservableCollection<DiagnosticRecordModel> DiagnosticLog { get; }

    public ObservableCollection<SelectionOptionModel> SeverityFilters { get; }

    public ObservableCollection<SelectionOptionModel> SourceFilters { get; }

    public string SelectedSeverityFilterId
    {
        get => _selectedSeverityFilterId;
        set
        {
            if (SetProperty(ref _selectedSeverityFilterId, value))
            {
                Refresh();
            }
        }
    }

    public string SelectedSourceFilterId
    {
        get => _selectedSourceFilterId;
        set
        {
            if (SetProperty(ref _selectedSourceFilterId, value))
            {
                Refresh();
            }
        }
    }

    public bool CanClear => _diagnosticsProvider.GetRecords().Count > 0;

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
        OnPropertyChanged(nameof(CanClear));
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

        foreach (var record in records.Where(MatchesSelectedFilters))
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

    public void ClearDiagnostics()
    {
        _diagnosticsSessionController.Clear();
    }

    private bool MatchesSelectedFilters(DiagnosticRecord record)
    {
        return MatchesSeverityFilter(record)
            && MatchesSourceFilter(record);
    }

    private bool MatchesSeverityFilter(DiagnosticRecord record)
    {
        if (string.Equals(SelectedSeverityFilterId, AllFilterId, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return Enum.TryParse<DiagnosticSeverity>(SelectedSeverityFilterId, ignoreCase: true, out var severity)
            && record.Severity == severity;
    }

    private bool MatchesSourceFilter(DiagnosticRecord record)
    {
        if (string.Equals(SelectedSourceFilterId, AllFilterId, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return Enum.TryParse<DiagnosticSource>(SelectedSourceFilterId, ignoreCase: true, out var source)
            && record.Source == source;
    }

    private static IReadOnlyList<SelectionOptionModel> BuildSeverityFilters()
    {
        return new[]
        {
            new SelectionOptionModel { Id = AllFilterId, DisplayName = "All Severities" },
            new SelectionOptionModel { Id = DiagnosticSeverity.Critical.ToString(), DisplayName = "Critical" },
            new SelectionOptionModel { Id = DiagnosticSeverity.Error.ToString(), DisplayName = "Error" },
            new SelectionOptionModel { Id = DiagnosticSeverity.Warning.ToString(), DisplayName = "Warning" },
            new SelectionOptionModel { Id = DiagnosticSeverity.Info.ToString(), DisplayName = "Info" }
        };
    }

    private static IReadOnlyList<SelectionOptionModel> BuildSourceFilters()
    {
        var options = new List<SelectionOptionModel>
        {
            new() { Id = AllFilterId, DisplayName = "All Sources" }
        };

        foreach (var source in Enum.GetValues<DiagnosticSource>())
        {
            options.Add(new SelectionOptionModel
            {
                Id = source.ToString(),
                DisplayName = source.ToString()
            });
        }

        return options;
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
            .GroupBy(record => NormalizePluginDiagnosticCode(record.Code))
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

    private static string NormalizePluginDiagnosticCode(string code)
    {
        if (code.StartsWith("plugins.state.", StringComparison.OrdinalIgnoreCase))
        {
            return code.Replace("plugins.state.", string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        if (code.StartsWith("plugins.manifest.", StringComparison.OrdinalIgnoreCase))
        {
            return code.Replace("plugins.manifest.", string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        return code;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private sealed record DiagnosticHealthSurface(
        string Title,
        DiagnosticSource Source,
        string EmptyText);
}
