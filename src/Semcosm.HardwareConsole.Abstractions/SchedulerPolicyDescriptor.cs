using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record SchedulerPolicyDescriptor(
    string Id,
    string PolicyId,
    string Scope,
    string DisplayName,
    string Description,
    string ForegroundStrategy,
    string BackgroundStrategy,
    IReadOnlyList<string> InputSensorIds,
    IReadOnlyList<SchedulerRuleDescriptor> Rules,
    HardwareRiskLevel RiskLevel);
