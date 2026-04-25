using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Models;

public sealed class ProfilePreviewModel
{
    public bool ShowEmptyState { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;
    public string SourceText { get; set; } = string.Empty;
    public HardwareRiskLevel RiskLevel { get; set; }
    public bool RequiresConfirmation { get; set; }
    public string ConfirmationText { get; set; } = string.Empty;
    public string EmptyTitle { get; set; } = "No Profile Preview";
    public string EmptyDescription { get; set; } = "Select Preview on any profile to inspect the control actions it would apply.";

    public static ProfilePreviewModel CreateEmpty()
    {
        return new ProfilePreviewModel
        {
            ShowEmptyState = true
        };
    }
}
