using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record PolicyRuntimePreview(
    bool Success,
    string PolicyId,
    FanCurvePolicyDescriptor? Policy,
    PolicyPreviewFailureCode FailureCode,
    IReadOnlyList<string> WouldSetControlIds,
    IReadOnlyList<string> RequiredSensorIds,
    IReadOnlyList<string> BlockedReasons,
    IReadOnlyList<string> Diagnostics,
    string Message);
