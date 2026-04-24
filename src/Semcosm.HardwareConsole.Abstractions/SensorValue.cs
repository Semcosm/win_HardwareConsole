namespace Semcosm.HardwareConsole.Abstractions;

public sealed record SensorValue(
    string SensorId,
    double? NumericValue,
    string? TextValue,
    string Unit,
    string FormattedValue,
    DateTimeOffset Timestamp,
    SensorQuality Quality);
