using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Models;

public sealed class ThermalPolicyPreviewModel
{
    public string Title { get; set; } = "No Thermal Policy Preview";
    public string Description { get; set; } = "Select Preview on any mock thermal chain to inspect staged temperature responses.";
    public string ScopeText { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;
    public string FailureCodeText { get; set; } = string.Empty;
    public string InputSensorsSummary { get; set; } = string.Empty;
    public string WouldSetControlsSummary { get; set; } = string.Empty;
    public string TimingSummary { get; set; } = string.Empty;
    public string BlockedReasonsSummary { get; set; } = string.Empty;
    public string DiagnosticsSummary { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public HardwareRiskLevel RiskLevel { get; set; } = HardwareRiskLevel.ReadOnly;

    public static ThermalPolicyPreviewModel CreateEmpty()
    {
        return new ThermalPolicyPreviewModel();
    }
}
