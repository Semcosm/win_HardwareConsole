using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record SchedulerPolicyPreview(
    bool Success,
    string PolicyId,
    SchedulerPolicyDescriptor? Policy,
    PolicyPreviewFailureCode FailureCode,
    IReadOnlyList<string> RequiredSensorIds,
    IReadOnlyList<string> WouldSetControlIds,
    IReadOnlyList<string> BlockedReasons,
    IReadOnlyList<string> Diagnostics,
    string Message);
