namespace Semcosm.HardwareConsole.Abstractions;

public sealed record ProfileControlActionDescriptor(
    string ControlId,
    ControlValue TargetValue,
    ControlRiskLevel RiskLevel,
    bool RequiresConfirmation);
