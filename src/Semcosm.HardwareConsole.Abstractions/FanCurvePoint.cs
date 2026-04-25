namespace Semcosm.HardwareConsole.Abstractions;

public sealed record FanCurvePoint(
    double InputValue,
    double OutputPercent);
