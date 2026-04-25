using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Models;

public sealed class PolicyRuntimePreviewModel
{
    public string Title { get; set; } = "No Fan Policy Preview";
    public string Description { get; set; } = "Select Preview on any mock fan policy to inspect the would-set behavior.";
    public string ScopeText { get; set; } = string.Empty;
    public string InputSensorText { get; set; } = string.Empty;
    public string OutputControlText { get; set; } = string.Empty;
    public string WouldSetSummary { get; set; } = string.Empty;
    public string TimingSummary { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public HardwareRiskLevel RiskLevel { get; set; } = HardwareRiskLevel.ReadOnly;

    public static PolicyRuntimePreviewModel CreateEmpty()
    {
        return new PolicyRuntimePreviewModel();
    }
}
