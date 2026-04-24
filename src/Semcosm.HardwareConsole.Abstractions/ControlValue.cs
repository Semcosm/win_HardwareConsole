namespace Semcosm.HardwareConsole.Abstractions;

public sealed record ControlValue(
    string ControlId,
    double? NumericValue,
    string? TextValue,
    string Unit,
    string FormattedValue,
    DateTimeOffset Timestamp,
    ControlQuality Quality);
