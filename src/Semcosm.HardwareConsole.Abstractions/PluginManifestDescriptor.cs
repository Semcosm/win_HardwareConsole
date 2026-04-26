using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record PluginManifestDescriptor(
    string SchemaVersion,
    string Id,
    string DisplayName,
    string Vendor,
    string Version,
    HardwareRiskLevel RiskLevel,
    bool RequiresConfirmation,
    IReadOnlyList<PluginCapabilityDeclaration> Capabilities,
    IReadOnlyList<PluginDeviceDeclaration> Devices,
    IReadOnlyList<PluginSensorDeclaration> Sensors,
    IReadOnlyList<PluginControlDeclaration> Controls,
    IReadOnlyList<PluginRouteDeclaration> Routes);
