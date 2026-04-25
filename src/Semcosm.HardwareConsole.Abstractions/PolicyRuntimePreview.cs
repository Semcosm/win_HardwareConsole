namespace Semcosm.HardwareConsole.Abstractions;

public sealed record PolicyRuntimePreview(
    FanCurvePolicyDescriptor? Policy,
    string WouldSetSummary,
    string TimingSummary,
    string Message);
