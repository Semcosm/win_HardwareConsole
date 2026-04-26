using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record PluginControlDeclaration(
    string Id,
    string DisplayName,
    string DeviceId,
    ControlKind Kind,
    ControlRiskLevel RiskLevel,
    IReadOnlyList<string> CapabilityTags);
