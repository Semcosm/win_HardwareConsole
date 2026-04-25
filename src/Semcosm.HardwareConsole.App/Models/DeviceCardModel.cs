using System.Collections.Generic;

namespace Semcosm.HardwareConsole.App.Models;

public sealed class DeviceCardModel
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Vendor { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string CapabilityCountText { get; set; } = string.Empty;
    public string SensorCountText { get; set; } = string.Empty;
    public string ControlCountText { get; set; } = string.Empty;
    public string PluginSourceSummary { get; set; } = string.Empty;
    public IReadOnlyList<string> Capabilities { get; set; } = [];
    public IReadOnlyList<string> PluginSources { get; set; } = [];
    public IReadOnlyList<SensorRowModel> Sensors { get; set; } = [];
    public IReadOnlyList<ControlRowModel> Controls { get; set; } = [];
}
