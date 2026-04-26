using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record PluginSensorDeclaration(
    string Id,
    string DisplayName,
    string DeviceId,
    SensorKind Kind,
    string Unit,
    IReadOnlyList<string> CapabilityTags);
