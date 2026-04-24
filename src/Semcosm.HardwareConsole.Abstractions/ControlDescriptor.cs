namespace Semcosm.HardwareConsole.Abstractions;

public sealed record ControlDescriptor(
    string Id,
    string DisplayName,
    string DeviceId,
    ControlKind Kind,
    ControlRiskLevel RiskLevel);
