namespace Semcosm.HardwareConsole.Abstractions;

public sealed record PowerPolicyActionDescriptor(
    string ConditionLabel,
    string ControlId,
    ControlValue TargetValue,
    ControlRiskLevel RiskLevel,
    bool RequiresConfirmation);
