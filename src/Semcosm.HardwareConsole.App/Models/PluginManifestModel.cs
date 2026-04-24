using System.Collections.Generic;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Models;

public sealed class PluginManifestModel
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Vendor { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public PluginState State { get; set; }
    public HardwareRiskLevel RiskLevel { get; set; }

    public List<string> Capabilities { get; set; } = new();
    public List<string> MatchedDevices { get; set; } = new();

    public string StateText => State switch
    {
        PluginState.Available => "Available",
        PluginState.Enabled => "Enabled",
        PluginState.Disabled => "Disabled",
        PluginState.Mocked => "Mocked",
        PluginState.Failed => "Failed",
        PluginState.Blocked => "Blocked",
        PluginState.Unsupported => "Unsupported",
        _ => "Unknown"
    };

    public string RiskLevelText => RiskLevel switch
    {
        HardwareRiskLevel.ReadOnly => "Read Only",
        HardwareRiskLevel.SafeControl => "Safe Control",
        HardwareRiskLevel.HardwareWrite => "Hardware Write",
        HardwareRiskLevel.KernelDriverRequired => "Kernel Driver Required",
        HardwareRiskLevel.Experimental => "Experimental",
        _ => "Unknown"
    };

    public string CapabilitySummary =>
        !Capabilities.Any() ? "No capabilities declared" : string.Join(" · ", Capabilities);

    public string MatchedDeviceSummary =>
        !MatchedDevices.Any() ? "No matched device" : string.Join(" · ", MatchedDevices);

    public static PluginManifestModel FromDescriptor(PluginDescriptor descriptor, PluginState state)
    {
        return new PluginManifestModel
        {
            Id = descriptor.Id,
            DisplayName = descriptor.DisplayName,
            Vendor = descriptor.Vendor,
            Version = descriptor.Version,
            State = state,
            RiskLevel = descriptor.RiskLevel,
            Capabilities = new List<string>(descriptor.Capabilities),
            MatchedDevices = new List<string>(descriptor.MatchedDevices)
        };
    }
}
