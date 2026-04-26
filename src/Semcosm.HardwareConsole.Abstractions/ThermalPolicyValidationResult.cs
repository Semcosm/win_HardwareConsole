using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record ThermalPolicyValidationResult(
    bool IsValid,
    ThermalPolicyFailureCode FailureCode,
    IReadOnlyList<string> RequiredSensorIds,
    IReadOnlyList<string> WouldSetControlIds,
    IReadOnlyList<PolicyValidationIssue> Issues,
    IReadOnlyList<string> BlockedReasons,
    IReadOnlyList<string> Diagnostics,
    string Message);
