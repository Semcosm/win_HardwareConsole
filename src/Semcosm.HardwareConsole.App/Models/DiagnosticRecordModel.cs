using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Models;

public sealed class DiagnosticRecordModel
{
    public DiagnosticSeverity Severity { get; set; }
    public string SeverityText { get; set; } = string.Empty;
    public string SourceText { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string RelatedId { get; set; } = string.Empty;
    public string TimestampText { get; set; } = string.Empty;
}
