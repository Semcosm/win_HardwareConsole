using System.Collections.Generic;

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
}
