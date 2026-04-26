namespace Semcosm.HardwareConsole.Abstractions;

public sealed record PolicyValidationIssue(
    string Code,
    PolicyValidationSeverity Severity,
    string Message,
    string RelatedPolicyId,
    string RelatedControlId,
    string RelatedSensorId);
