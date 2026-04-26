using System;
using System.Collections.Generic;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class DiagnosticsStore : IDiagnosticsSink, IDiagnosticsProvider
{
    private const int MaxRecords = 250;
    private readonly object _syncRoot = new();
    private readonly List<DiagnosticRecord> _records = [];

    public event EventHandler? RecordsChanged;

    public IReadOnlyList<DiagnosticRecord> GetRecords()
    {
        lock (_syncRoot)
        {
            return _records
                .OrderByDescending(record => record.Timestamp)
                .ToArray();
        }
    }

    public DiagnosticRecord? GetLatest(DiagnosticSource source)
    {
        lock (_syncRoot)
        {
            return _records
                .Where(record => record.Source == source)
                .OrderByDescending(record => record.Timestamp)
                .FirstOrDefault();
        }
    }

    public void Report(DiagnosticRecord record)
    {
        lock (_syncRoot)
        {
            _records.Add(record);

            if (_records.Count > MaxRecords)
            {
                _records.RemoveRange(0, _records.Count - MaxRecords);
            }
        }

        RecordsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Clear()
    {
        lock (_syncRoot)
        {
            _records.Clear();
        }

        RecordsChanged?.Invoke(this, EventArgs.Empty);
    }
}
