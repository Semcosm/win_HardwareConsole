namespace Semcosm.HardwareConsole.Abstractions;

public sealed record PolicyValidationIssue(
    string Code,
    PolicyValidationSeverity Severity,
    string Message,
    string? RelatedPolicyId = null,
    string? RelatedControlId = null,
    string? RelatedSensorId = null);
