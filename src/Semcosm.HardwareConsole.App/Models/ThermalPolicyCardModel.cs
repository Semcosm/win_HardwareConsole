using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Models;

public sealed class ThermalPolicyCardModel
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public HardwareRiskLevel RiskLevel { get; set; }
    public string ScopeText { get; set; } = string.Empty;
    public string SensorSummary { get; set; } = string.Empty;
    public string ActionCountText { get; set; } = string.Empty;
    public string TimingText { get; set; } = string.Empty;
    public string ControlSummary { get; set; } = string.Empty;
}
