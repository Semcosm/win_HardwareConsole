namespace Semcosm.HardwareConsole.App.Models;

public sealed class SensorRowModel
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string CurrentValue { get; set; } = string.Empty;
    public string UnitText { get; set; } = string.Empty;
    public string QualityText { get; set; } = string.Empty;
}
