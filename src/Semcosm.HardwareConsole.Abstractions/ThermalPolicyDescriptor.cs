using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record ThermalPolicyDescriptor(
    string Id,
    string PolicyId,
    string Scope,
    string DisplayName,
    string Description,
    IReadOnlyList<string> InputSensorIds,
    IReadOnlyList<ThermalThresholdActionDescriptor> Actions,
    double PollIntervalSeconds,
    double CooldownSeconds,
    HardwareRiskLevel RiskLevel);
