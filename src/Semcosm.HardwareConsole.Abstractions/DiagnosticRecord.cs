using System;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record DiagnosticRecord(
    DiagnosticSeverity Severity,
    DiagnosticSource Source,
    string Code,
    string Message,
    string RelatedId,
    DateTimeOffset Timestamp);
