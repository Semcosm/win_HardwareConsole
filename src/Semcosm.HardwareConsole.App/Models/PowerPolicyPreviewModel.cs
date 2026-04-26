using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Models;

public sealed class PowerPolicyPreviewModel
{
    public string Title { get; set; } = "No Power Policy Preview";
    public string Description { get; set; } = "Select Preview on any mock power policy to inspect AC/DC behavior and would-set controls.";
    public string ScopeText { get; set; } = string.Empty;
    public string PlanText { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;
    public string FailureCodeText { get; set; } = string.Empty;
    public string RequiredSensorsSummary { get; set; } = string.Empty;
    public string WouldSetControlsSummary { get; set; } = string.Empty;
    public string AcBehaviorText { get; set; } = string.Empty;
    public string DcBehaviorText { get; set; } = string.Empty;
    public string BlockedReasonsSummary { get; set; } = string.Empty;
    public string DiagnosticsSummary { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public HardwareRiskLevel RiskLevel { get; set; } = HardwareRiskLevel.ReadOnly;

    public static PowerPolicyPreviewModel CreateEmpty()
    {
        return new PowerPolicyPreviewModel();
    }
}
