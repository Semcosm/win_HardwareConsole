namespace Semcosm.HardwareConsole.App.Models;

public sealed class FanPolicyDraftModel
{
    public string PolicyId { get; set; } = string.Empty;
    public string SelectedInputSensorId { get; set; } = string.Empty;
    public string SelectedOutputControlId { get; set; } = string.Empty;
    public bool IsDirty { get; set; }
}
