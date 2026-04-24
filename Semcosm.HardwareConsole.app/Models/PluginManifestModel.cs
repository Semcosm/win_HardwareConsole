using System.Collections.Generic;
using System.Linq;

namespace Semcosm.HardwareConsole.app.Models;

public enum PluginRiskLevel
{
    ReadOnly,
    SafeControl,
    HardwareWrite,
    KernelDriverRequired,
    Experimental
}

public sealed class PluginManifestModel
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Vendor { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public PluginRiskLevel RiskLevel { get; set; }

    public List<string> Capabilities { get; set; } = new();
    public List<string> MatchedDevices { get; set; } = new();

    public string RiskLevelText => RiskLevel switch
    {
        PluginRiskLevel.ReadOnly => "Read Only",
        PluginRiskLevel.SafeControl => "Safe Control",
        PluginRiskLevel.HardwareWrite => "Hardware Write",
        PluginRiskLevel.KernelDriverRequired => "Kernel Driver Required",
        PluginRiskLevel.Experimental => "Experimental",
        _ => "Unknown"
    };

    public string CapabilitySummary =>
        !Capabilities.Any() ? "No capabilities declared" : string.Join(" · ", Capabilities);

    public string MatchedDeviceSummary =>
        !MatchedDevices.Any() ? "No matched device" : string.Join(" · ", MatchedDevices);
}
