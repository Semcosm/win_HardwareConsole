using System;
using System.Collections.Generic;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.Mock.Services;

public sealed class MockSchedulerPolicyValidator
    : IPolicyValidator<SchedulerPolicyDescriptor, SchedulerPolicyValidationResult>
{
    private readonly IReadOnlyDictionary<string, ControlDescriptor> _controls;
    private readonly IReadOnlyDictionary<string, SensorDescriptor> _sensors;

    public MockSchedulerPolicyValidator()
    {
        _sensors = MockHardwareData.Sensors.ToDictionary(sensor => sensor.Id);
        _controls = MockHardwareData.Controls.ToDictionary(control => control.Id);
    }

    public SchedulerPolicyValidationResult Validate(SchedulerPolicyDescriptor policy)
    {
        var requiredSensorIds = policy.InputSensorIds
            .Distinct()
            .ToArray();

        var wouldSetControlIds = policy.Rules
            .SelectMany(rule => rule.Actions)
            .Select(action => action.ControlId)
            .Distinct()
            .ToArray();

        var descriptorIssues = ValidateDescriptor(policy);
        if (descriptorIssues.Count > 0)
        {
            return new SchedulerPolicyValidationResult(
                false,
                PolicyPreviewFailureCode.InvalidPolicy,
                policy.Id,
                requiredSensorIds,
                wouldSetControlIds,
                descriptorIssues,
                descriptorIssues.Select(issue => issue.Message).ToArray(),
                new[]
                {
                    "Mock scheduler policy validator rejected descriptor-level constraints."
                },
                "Preview failed because the scheduler policy descriptor is invalid.");
        }

        var missingSensorIds = requiredSensorIds
            .Where(sensorId => !_sensors.ContainsKey(sensorId))
            .ToArray();

        if (missingSensorIds.Length > 0)
        {
            var issues = missingSensorIds
                .Select(sensorId => CreateIssue(
                    policy.Id,
                    "scheduler.policy.missing_sensor",
                    $"Required sensor '{sensorId}' is not available in the current mock inventory.",
                    relatedSensorId: sensorId))
                .ToArray();

            return new SchedulerPolicyValidationResult(
                false,
                PolicyPreviewFailureCode.MissingRequiredSensor,
                policy.Id,
                requiredSensorIds,
                wouldSetControlIds,
                issues,
                issues.Select(issue => issue.Message).ToArray(),
                new[]
                {
                    "Mock scheduler policy validator could not resolve all required sensors."
                },
                "Preview failed because one or more scheduler-policy sensors are missing.");
        }

        var missingControlIds = wouldSetControlIds
            .Where(controlId => !_controls.ContainsKey(controlId))
            .ToArray();

        if (missingControlIds.Length > 0)
        {
            var issues = missingControlIds
                .Select(controlId => CreateIssue(
                    policy.Id,
                    "scheduler.policy.unsupported_control",
                    $"Scheduler action control '{controlId}' is not available in the current mock inventory.",
                    relatedControlId: controlId))
                .ToArray();

            return new SchedulerPolicyValidationResult(
                false,
                PolicyPreviewFailureCode.UnsupportedControl,
                policy.Id,
                requiredSensorIds,
                wouldSetControlIds,
                issues,
                issues.Select(issue => issue.Message).ToArray(),
                new[]
                {
                    "Mock scheduler policy validator could not resolve all target controls."
                },
                "Preview failed because one or more scheduler controls are unsupported.");
        }

        return new SchedulerPolicyValidationResult(
            true,
            PolicyPreviewFailureCode.None,
            policy.Id,
            requiredSensorIds,
            wouldSetControlIds,
            [],
            [],
            new[]
            {
                "Mock scheduler policy validator accepted the descriptor."
            },
            "Scheduler policy descriptor validation passed.");
    }

    private List<PolicyValidationIssue> ValidateDescriptor(SchedulerPolicyDescriptor policy)
    {
        var issues = new List<PolicyValidationIssue>();

        if (policy.InputSensorIds.Count == 0)
        {
            issues.Add(CreateIssue(
                policy.Id,
                "scheduler.policy.input_sensors_required",
                "Scheduler policy must declare at least one input sensor."));
        }

        if (policy.Rules.Count == 0)
        {
            issues.Add(CreateIssue(
                policy.Id,
                "scheduler.policy.rules_required",
                "Scheduler policy must declare at least one process rule."));
        }

        foreach (var rule in policy.Rules)
        {
            if (string.IsNullOrWhiteSpace(rule.DisplayName))
            {
                issues.Add(CreateIssue(
                    policy.Id,
                    "scheduler.policy.rule_display_name_required",
                    $"Scheduler rule '{rule.Id}' must declare a display name."));
            }

            if (string.IsNullOrWhiteSpace(rule.MatchText))
            {
                issues.Add(CreateIssue(
                    policy.Id,
                    "scheduler.policy.rule_match_required",
                    $"Scheduler rule '{rule.Id}' must declare a process match expression."));
            }

            if (rule.Actions.Count == 0)
            {
                issues.Add(CreateIssue(
                    policy.Id,
                    "scheduler.policy.rule_actions_required",
                    $"Scheduler rule '{rule.Id}' must declare at least one action."));
            }

            foreach (var action in rule.Actions)
            {
                if (_controls.TryGetValue(action.ControlId, out var control)
                    && !IsTargetValueCompatible(control, action.TargetValue))
                {
                    issues.Add(CreateIssue(
                        policy.Id,
                        "scheduler.policy.target_incompatible",
                        $"Scheduler action target value is not compatible with control '{action.ControlId}'.",
                        relatedControlId: action.ControlId));
                }

                if (IsHighRisk(action.RiskLevel) && !action.RequiresConfirmation)
                {
                    issues.Add(CreateIssue(
                        policy.Id,
                        "scheduler.policy.confirmation_required",
                        $"Scheduler action for control '{action.ControlId}' must require confirmation because it targets a high-risk control.",
                        relatedControlId: action.ControlId));
                }
            }
        }

        return issues;
    }

    private static PolicyValidationIssue CreateIssue(
        string policyId,
        string code,
        string message,
        string? relatedControlId = null,
        string? relatedSensorId = null)
    {
        return new PolicyValidationIssue(
            code,
            PolicyValidationSeverity.Error,
            message,
            policyId,
            relatedControlId,
            relatedSensorId);
    }

    private static bool IsHighRisk(ControlRiskLevel riskLevel)
    {
        return riskLevel is ControlRiskLevel.HardwareWrite
            or ControlRiskLevel.KernelDriverRequired
            or ControlRiskLevel.Experimental;
    }

    private static bool IsTargetValueCompatible(ControlDescriptor control, ControlValue targetValue)
    {
        return control.Kind switch
        {
            ControlKind.Toggle => targetValue.NumericValue.HasValue || !string.IsNullOrWhiteSpace(targetValue.TextValue),
            ControlKind.Mode => !string.IsNullOrWhiteSpace(targetValue.TextValue),
            ControlKind.Range => targetValue.NumericValue.HasValue || !string.IsNullOrWhiteSpace(targetValue.TextValue),
            ControlKind.Curve => !string.IsNullOrWhiteSpace(targetValue.TextValue),
            ControlKind.Fan => targetValue.NumericValue.HasValue,
            ControlKind.FanCurve => !string.IsNullOrWhiteSpace(targetValue.TextValue),
            _ => !string.IsNullOrWhiteSpace(targetValue.FormattedValue)
        };
    }
}
