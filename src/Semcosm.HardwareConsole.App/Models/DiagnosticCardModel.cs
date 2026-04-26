using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Models;

public sealed class DiagnosticCardModel
{
    public string Title { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;
    public string DetailsText { get; set; } = string.Empty;
    public string SourceText { get; set; } = string.Empty;
    public DiagnosticSeverity Severity { get; set; }
    public string SeverityText { get; set; } = string.Empty;
}
