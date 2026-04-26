using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record PowerPolicyValidationResult(
    bool IsValid,
    PolicyPreviewFailureCode FailureCode,
    string RelatedPolicyId,
    IReadOnlyList<string> RequiredSensorIds,
    IReadOnlyList<string> WouldSetControlIds,
    IReadOnlyList<PolicyValidationIssue> Issues,
    IReadOnlyList<string> BlockedReasons,
    IReadOnlyList<string> Diagnostics,
    string Message)
    : PolicyValidationResult(
        IsValid,
        RelatedPolicyId,
        RequiredSensorIds,
        WouldSetControlIds,
        Issues,
        BlockedReasons,
        Diagnostics,
        Message);
