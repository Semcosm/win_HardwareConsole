using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record PowerPolicyPreview(
    bool Success,
    string PolicyId,
    PowerPolicyDescriptor? Policy,
    PolicyPreviewFailureCode FailureCode,
    IReadOnlyList<string> RequiredSensorIds,
    IReadOnlyList<string> WouldSetControlIds,
    IReadOnlyList<string> BlockedReasons,
    IReadOnlyList<string> Diagnostics,
    string Message);
