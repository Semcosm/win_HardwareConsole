using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Models;

public sealed class ProfileCardModel
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public HardwareRiskLevel RiskLevel { get; set; }
    public string SourceText { get; set; } = string.Empty;
    public string CapabilityCountText { get; set; } = string.Empty;
    public string ActionCountText { get; set; } = string.Empty;
    public string PolicyCountText { get; set; } = string.Empty;
    public bool RequiresConfirmation { get; set; }
    public string ConfirmationText { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
