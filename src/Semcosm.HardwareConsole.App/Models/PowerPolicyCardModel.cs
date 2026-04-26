using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Models;

public sealed class PowerPolicyCardModel
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public HardwareRiskLevel RiskLevel { get; set; }
    public string ScopeText { get; set; } = string.Empty;
    public string PlanText { get; set; } = string.Empty;
    public string TriggerSummary { get; set; } = string.Empty;
    public string ActionCountText { get; set; } = string.Empty;
    public string AcBehaviorText { get; set; } = string.Empty;
    public string DcBehaviorText { get; set; } = string.Empty;
    public string ControlSummary { get; set; } = string.Empty;
}
