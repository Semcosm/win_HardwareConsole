using System;
using System.Collections.Generic;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.Mock.Services;

public sealed class MockPowerPolicyValidator
    : IPolicyValidator<PowerPolicyDescriptor, PowerPolicyValidationResult>
{
    private readonly IReadOnlyDictionary<string, ControlDescriptor> _controls;
    private readonly IReadOnlyDictionary<string, SensorDescriptor> _sensors;

    public MockPowerPolicyValidator()
    {
        _sensors = MockHardwareData.Sensors.ToDictionary(sensor => sensor.Id);
        _controls = MockHardwareData.Controls.ToDictionary(control => control.Id);
    }

    public PowerPolicyValidationResult Validate(PowerPolicyDescriptor policy)
    {
        var requiredSensorIds = policy.InputSensorIds
            .Distinct()
            .ToArray();

        var wouldSetControlIds = policy.Actions
            .Select(action => action.ControlId)
            .Distinct()
            .ToArray();

        var descriptorIssues = ValidateDescriptor(policy);
        if (descriptorIssues.Count > 0)
        {
            return BuildInvalidResult(
                policy,
                requiredSensorIds,
                wouldSetControlIds,
                descriptorIssues,
                PolicyPreviewFailureCode.InvalidPolicy,
                "Preview failed because the power policy descriptor is invalid.",
                "Mock power policy validator rejected descriptor-level constraints.");
        }

        var missingSensorIds = requiredSensorIds
            .Where(sensorId => !_sensors.ContainsKey(sensorId))
            .ToArray();

        if (missingSensorIds.Length > 0)
        {
            var issues = missingSensorIds
                .Select(sensorId => new PolicyValidationIssue(
                    "power.policy.missing_sensor",
                    PolicyValidationSeverity.Error,
                    $"Required sensor '{sensorId}' is not available in the current mock inventory.",
                    policy.Id,
                    null,
                    sensorId))
                .ToArray();

            return new PowerPolicyValidationResult(
                false,
                PolicyPreviewFailureCode.MissingRequiredSensor,
                policy.Id,
                requiredSensorIds,
                wouldSetControlIds,
                issues,
                issues.Select(issue => issue.Message).ToArray(),
                new[]
                {
                    "Mock power policy validator could not resolve all required sensors."
                },
                "Preview failed because one or more required power-policy sensors are missing.");
        }

        var missingControlIds = wouldSetControlIds
            .Where(controlId => !_controls.ContainsKey(controlId))
            .ToArray();

        if (missingControlIds.Length > 0)
        {
            var issues = missingControlIds
                .Select(controlId => new PolicyValidationIssue(
                    "power.policy.unsupported_control",
                    PolicyValidationSeverity.Error,
                    $"Output control '{controlId}' is not available in the current mock inventory.",
                    policy.Id,
                    controlId,
                    null))
                .ToArray();

            return new PowerPolicyValidationResult(
                false,
                PolicyPreviewFailureCode.UnsupportedControl,
                policy.Id,
                requiredSensorIds,
                wouldSetControlIds,
                issues,
                issues.Select(issue => issue.Message).ToArray(),
                new[]
                {
                    "Mock power policy validator could not resolve all target controls."
                },
                "Preview failed because one or more power-policy controls are unsupported.");
        }

        return new PowerPolicyValidationResult(
            true,
            PolicyPreviewFailureCode.None,
            policy.Id,
            requiredSensorIds,
            wouldSetControlIds,
            [],
            [],
            new[]
            {
                "Mock power policy validator accepted the descriptor."
            },
            "Power policy descriptor validation passed.");
    }

    private List<PolicyValidationIssue> ValidateDescriptor(PowerPolicyDescriptor policy)
    {
        var issues = new List<PolicyValidationIssue>();

        if (policy.InputSensorIds.Count == 0)
        {
            issues.Add(CreateIssue(
                policy.Id,
                "power.policy.input_sensors_required",
                "Power policy must declare at least one input sensor."));
        }

        if (policy.Actions.Count == 0)
        {
            issues.Add(CreateIssue(
                policy.Id,
                "power.policy.actions_required",
                "Power policy must declare at least one AC/DC action."));
        }

        var hasAcAction = false;
        var hasDcAction = false;

        foreach (var action in policy.Actions)
        {
            if (string.IsNullOrWhiteSpace(action.ConditionLabel))
            {
                issues.Add(CreateIssue(
                    policy.Id,
                    "power.policy.condition_required",
                    "Power policy action must declare an AC/DC condition label.",
                    relatedControlId: action.ControlId));
                continue;
            }

            if (string.Equals(action.ConditionLabel, "AC", StringComparison.OrdinalIgnoreCase))
            {
                hasAcAction = true;
            }
            else if (string.Equals(action.ConditionLabel, "DC", StringComparison.OrdinalIgnoreCase))
            {
                hasDcAction = true;
            }
            else
            {
                issues.Add(CreateIssue(
                    policy.Id,
                    "power.policy.condition_invalid",
                    $"Power policy action condition '{action.ConditionLabel}' must be AC or DC.",
                    relatedControlId: action.ControlId));
            }

            if (_controls.TryGetValue(action.ControlId, out var control)
                && !IsTargetValueCompatible(control, action.TargetValue))
            {
                issues.Add(CreateIssue(
                    policy.Id,
                    "power.policy.target_incompatible",
                    $"Power policy action target value is not compatible with control '{action.ControlId}'.",
                    relatedControlId: action.ControlId));
            }

            if (IsHighRisk(action.RiskLevel) && !action.RequiresConfirmation)
            {
                issues.Add(CreateIssue(
                    policy.Id,
                    "power.policy.confirmation_required",
                    $"Power policy action for control '{action.ControlId}' must require confirmation because it targets a high-risk control.",
                    relatedControlId: action.ControlId));
            }
        }

        if (policy.Actions.Count > 0 && !hasAcAction)
        {
            issues.Add(CreateIssue(
                policy.Id,
                "power.policy.ac_missing",
                "Power policy must declare at least one AC action."));
        }

        if (policy.Actions.Count > 0 && !hasDcAction)
        {
            issues.Add(CreateIssue(
                policy.Id,
                "power.policy.dc_missing",
                "Power policy must declare at least one DC action."));
        }

        return issues;
    }

    private static PowerPolicyValidationResult BuildInvalidResult(
        PowerPolicyDescriptor policy,
        IReadOnlyList<string> requiredSensorIds,
        IReadOnlyList<string> wouldSetControlIds,
        IReadOnlyList<PolicyValidationIssue> issues,
        PolicyPreviewFailureCode failureCode,
        string message,
        string diagnostic)
    {
        return new PowerPolicyValidationResult(
            false,
            failureCode,
            policy.Id,
            requiredSensorIds,
            wouldSetControlIds,
            issues,
            issues.Select(issue => issue.Message).ToArray(),
            new[] { diagnostic },
            message);
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
