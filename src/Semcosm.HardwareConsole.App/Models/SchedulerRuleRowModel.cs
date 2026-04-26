using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Models;

public sealed class SchedulerRuleRowModel
{
    public string DisplayName { get; set; } = string.Empty;
    public string MatchText { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ActionSummary { get; set; } = string.Empty;
    public HardwareRiskLevel RiskLevel { get; set; }
}
