using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Models;

public sealed class ControlRowModel
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string CurrentValue { get; set; } = string.Empty;
    public string UnitText { get; set; } = string.Empty;
    public string SourceText { get; set; } = string.Empty;
    public HardwareRiskLevel RiskLevel { get; set; }
}
