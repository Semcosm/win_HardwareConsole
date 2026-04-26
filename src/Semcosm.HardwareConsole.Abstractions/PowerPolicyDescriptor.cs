using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record PowerPolicyDescriptor(
    string Id,
    string PolicyId,
    string Scope,
    string DisplayName,
    string Description,
    string PowerPlanName,
    string AcBehaviorSummary,
    string DcBehaviorSummary,
    IReadOnlyList<string> InputSensorIds,
    IReadOnlyList<PowerPolicyActionDescriptor> Actions,
    HardwareRiskLevel RiskLevel);
