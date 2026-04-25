using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record FanCurvePolicyDescriptor(
    string Id,
    string PolicyId,
    string Scope,
    string DisplayName,
    string Description,
    string InputSensorId,
    string OutputControlId,
    IReadOnlyList<FanCurvePoint> Points,
    double HysteresisDegrees,
    double RampUpSeconds,
    double RampDownSeconds,
    HardwareRiskLevel RiskLevel);
