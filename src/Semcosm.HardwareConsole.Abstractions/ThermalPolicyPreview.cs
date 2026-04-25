using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record ThermalPolicyPreview(
    bool Success,
    string PolicyId,
    ThermalPolicyDescriptor? Policy,
    ThermalPolicyFailureCode FailureCode,
    IReadOnlyList<string> RequiredSensorIds,
    IReadOnlyList<string> WouldSetControlIds,
    IReadOnlyList<string> BlockedReasons,
    IReadOnlyList<string> Diagnostics,
    string Message);
