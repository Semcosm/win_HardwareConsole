using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record PluginDescriptor(
    string Id,
    string DisplayName,
    string Vendor,
    string Version,
    HardwareRiskLevel RiskLevel,
    IReadOnlyList<string> Capabilities,
    IReadOnlyList<string> MatchedDevices,
    IReadOnlyList<string> ProvidedDeviceIds,
    IReadOnlyList<string> ProvidedSensorIds,
    IReadOnlyList<string> ProvidedControlIds);
