using System;
using System.Collections.Generic;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class DiagnosticsStore : IDiagnosticsSink, IDiagnosticsProvider
{
    private const int MaxRecords = 250;
    private readonly List<DiagnosticRecord> _records = [];

    public event EventHandler? RecordsChanged;

    public IReadOnlyList<DiagnosticRecord> GetRecords()
    {
        return _records
            .OrderByDescending(record => record.Timestamp)
            .ToArray();
    }

    public DiagnosticRecord? GetLatest(DiagnosticSource source)
    {
        return _records
            .Where(record => record.Source == source)
            .OrderByDescending(record => record.Timestamp)
            .FirstOrDefault();
    }

    public void Report(DiagnosticRecord record)
    {
        _records.Add(record);

        if (_records.Count > MaxRecords)
        {
            _records.RemoveRange(0, _records.Count - MaxRecords);
        }

        RecordsChanged?.Invoke(this, EventArgs.Empty);
    }
}
