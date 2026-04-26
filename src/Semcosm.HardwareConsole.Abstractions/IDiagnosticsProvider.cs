using System;
using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public interface IDiagnosticsProvider
{
    event EventHandler? RecordsChanged;

    IReadOnlyList<DiagnosticRecord> GetRecords();
    DiagnosticRecord? GetLatest(DiagnosticSource source);
}
