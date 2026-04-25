using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Models;

public sealed class ThermalActionRowModel
{
    public string StageLabel { get; set; } = string.Empty;
    public string TriggerText { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string TargetValue { get; set; } = string.Empty;
    public HardwareRiskLevel RiskLevel { get; set; }
    public bool RequiresConfirmation { get; set; }
    public string ConfirmationText { get; set; } = string.Empty;
}
