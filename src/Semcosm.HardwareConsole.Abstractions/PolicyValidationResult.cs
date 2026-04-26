using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public abstract record PolicyValidationResult(
    bool IsValid,
    string RelatedPolicyId,
    IReadOnlyList<string> RequiredSensorIds,
    IReadOnlyList<string> WouldSetControlIds,
    IReadOnlyList<PolicyValidationIssue> Issues,
    IReadOnlyList<string> BlockedReasons,
    IReadOnlyList<string> Diagnostics,
    string Message);
