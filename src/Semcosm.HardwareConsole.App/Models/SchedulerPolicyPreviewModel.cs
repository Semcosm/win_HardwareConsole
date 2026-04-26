using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Models;

public sealed class SchedulerPolicyPreviewModel
{
    public string Title { get; set; } = "No Scheduler Policy Preview";
    public string Description { get; set; } = "Select Preview on any mock scheduler policy to inspect process rules and would-set controls.";
    public string ScopeText { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;
    public string FailureCodeText { get; set; } = string.Empty;
    public string RequiredSensorsSummary { get; set; } = string.Empty;
    public string WouldSetControlsSummary { get; set; } = string.Empty;
    public string ForegroundStrategyText { get; set; } = string.Empty;
    public string BackgroundStrategyText { get; set; } = string.Empty;
    public string BlockedReasonsSummary { get; set; } = string.Empty;
    public string DiagnosticsSummary { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public HardwareRiskLevel RiskLevel { get; set; } = HardwareRiskLevel.ReadOnly;

    public static SchedulerPolicyPreviewModel CreateEmpty()
    {
        return new SchedulerPolicyPreviewModel();
    }
}
