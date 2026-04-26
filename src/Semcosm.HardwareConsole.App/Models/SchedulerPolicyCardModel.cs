using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Models;

public sealed class SchedulerPolicyCardModel
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public HardwareRiskLevel RiskLevel { get; set; }
    public string ScopeText { get; set; } = string.Empty;
    public string RuleCountText { get; set; } = string.Empty;
    public string TriggerSummary { get; set; } = string.Empty;
    public string ForegroundStrategyText { get; set; } = string.Empty;
    public string BackgroundStrategyText { get; set; } = string.Empty;
    public string ControlSummary { get; set; } = string.Empty;
}
