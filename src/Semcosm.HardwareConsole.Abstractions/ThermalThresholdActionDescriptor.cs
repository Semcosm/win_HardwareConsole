namespace Semcosm.HardwareConsole.Abstractions;

public sealed record ThermalThresholdActionDescriptor(
    string TriggerSensorId,
    double TriggerThreshold,
    string StageLabel,
    string ControlId,
    ControlValue TargetValue,
    ControlRiskLevel RiskLevel,
    bool RequiresConfirmation);
