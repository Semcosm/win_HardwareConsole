namespace Semcosm.HardwareConsole.Abstractions;

public sealed record SensorDescriptor(
    string Id,
    string DisplayName,
    string DeviceId,
    SensorKind Kind,
    string Unit);
