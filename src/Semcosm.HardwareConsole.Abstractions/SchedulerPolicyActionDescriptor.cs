namespace Semcosm.HardwareConsole.Abstractions;

public sealed record SchedulerPolicyActionDescriptor(
    string ControlId,
    ControlValue TargetValue,
    ControlRiskLevel RiskLevel,
    bool RequiresConfirmation);
