using System.Collections.Generic;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.Mock.Services;

public sealed class MockFanPolicyValidator
    : IPolicyValidator<FanCurvePolicyDescriptor, FanPolicyValidationResult>
{
    private const string FanPolicyOutputTag = "fan.policy.output";

    private readonly IReadOnlyDictionary<string, ControlDescriptor> _controls;
    private readonly IReadOnlyDictionary<string, SensorDescriptor> _sensors;

    public MockFanPolicyValidator()
    {
        _sensors = MockHardwareData.Sensors.ToDictionary(sensor => sensor.Id);
        _controls = MockHardwareData.Controls.ToDictionary(control => control.Id);
    }

    public FanPolicyValidationResult Validate(FanCurvePolicyDescriptor policy)
    {
        var requiredSensorIds = new[] { policy.InputSensorId };
        var wouldSetControlIds = new[] { policy.OutputControlId };

        var descriptorIssues = ValidateDescriptor(policy);
        if (descriptorIssues.Count > 0)
        {
            return new FanPolicyValidationResult(
                false,
                PolicyPreviewFailureCode.InvalidPolicy,
                policy.Id,
                requiredSensorIds,
                wouldSetControlIds,
                descriptorIssues,
                descriptorIssues.Select(issue => issue.Message).ToArray(),
                new[]
                {
                    "Mock fan policy validator rejected descriptor-level constraints."
                },
                "Preview failed because the fan policy descriptor is invalid.");
        }

        if (!_sensors.TryGetValue(policy.InputSensorId, out var sensor))
        {
            var issues = new[]
            {
                new PolicyValidationIssue(
                    "fan.policy.missing_sensor",
                    PolicyValidationSeverity.Error,
                    $"Required sensor '{policy.InputSensorId}' is not available in the current mock inventory.",
                    policy.Id,
                    null,
                    policy.InputSensorId)
            };

            return new FanPolicyValidationResult(
                false,
                PolicyPreviewFailureCode.MissingRequiredSensor,
                policy.Id,
                requiredSensorIds,
                wouldSetControlIds,
                issues,
                issues.Select(issue => issue.Message).ToArray(),
                new[]
                {
                    "Mock fan policy validator could not resolve the input sensor."
                },
                "Preview failed because the input sensor is missing.");
        }

        if (sensor.Kind != SensorKind.Temperature)
        {
            var issues = new[]
            {
                new PolicyValidationIssue(
                    "fan.policy.invalid_sensor_kind",
                    PolicyValidationSeverity.Error,
                    $"Fan policy input sensor '{policy.InputSensorId}' must be a temperature sensor.",
                    policy.Id,
                    null,
                    policy.InputSensorId)
            };

            return new FanPolicyValidationResult(
                false,
                PolicyPreviewFailureCode.InvalidPolicy,
                policy.Id,
                requiredSensorIds,
                wouldSetControlIds,
                issues,
                issues.Select(issue => issue.Message).ToArray(),
                new[]
                {
                    "Mock fan policy validator rejected the input sensor kind."
                },
                "Preview failed because the input sensor kind is invalid.");
        }

        if (!_controls.TryGetValue(policy.OutputControlId, out var control))
        {
            var issues = new[]
            {
                new PolicyValidationIssue(
                    "fan.policy.unsupported_control",
                    PolicyValidationSeverity.Error,
                    $"Output control '{policy.OutputControlId}' is not available in the current mock inventory.",
                    policy.Id,
                    policy.OutputControlId,
                    null)
            };

            return new FanPolicyValidationResult(
                false,
                PolicyPreviewFailureCode.UnsupportedControl,
                policy.Id,
                requiredSensorIds,
                wouldSetControlIds,
                issues,
                issues.Select(issue => issue.Message).ToArray(),
                new[]
                {
                    "Mock fan policy validator could not resolve the output control."
                },
                "Preview failed because the output control is unsupported.");
        }

        if ((control.Kind is not ControlKind.Fan and not ControlKind.FanCurve)
            || !control.CapabilityTags.Contains(FanPolicyOutputTag))
        {
            var issues = new[]
            {
                new PolicyValidationIssue(
                    "fan.policy.invalid_output",
                    PolicyValidationSeverity.Error,
                    $"Output control '{policy.OutputControlId}' does not support fan policy output.",
                    policy.Id,
                    policy.OutputControlId,
                    null)
            };

            return new FanPolicyValidationResult(
                false,
                PolicyPreviewFailureCode.UnsupportedControl,
                policy.Id,
                requiredSensorIds,
                wouldSetControlIds,
                issues,
                issues.Select(issue => issue.Message).ToArray(),
                new[]
                {
                    "Mock fan policy validator rejected the selected output control."
                },
                "Preview failed because the output control does not support fan policy output.");
        }

        return new FanPolicyValidationResult(
            true,
            PolicyPreviewFailureCode.None,
            policy.Id,
            requiredSensorIds,
            wouldSetControlIds,
            [],
            [],
            new[]
            {
                "Mock fan policy validator accepted the descriptor."
            },
            "Fan policy descriptor validation passed.");
    }

    private static List<PolicyValidationIssue> ValidateDescriptor(FanCurvePolicyDescriptor policy)
    {
        var issues = new List<PolicyValidationIssue>();

        if (string.IsNullOrWhiteSpace(policy.InputSensorId))
        {
            issues.Add(new PolicyValidationIssue(
                "fan.policy.input_required",
                PolicyValidationSeverity.Error,
                "Fan policy must declare an input sensor.",
                policy.Id,
                null,
                null));
        }

        if (string.IsNullOrWhiteSpace(policy.OutputControlId))
        {
            issues.Add(new PolicyValidationIssue(
                "fan.policy.output_required",
                PolicyValidationSeverity.Error,
                "Fan policy must declare an output control.",
                policy.Id));
        }

        if (policy.Points.Count == 0)
        {
            issues.Add(new PolicyValidationIssue(
                "fan.policy.points_required",
                PolicyValidationSeverity.Error,
                "Fan policy must declare at least one curve point.",
                policy.Id));
        }

        if (policy.HysteresisDegrees < 0)
        {
            issues.Add(new PolicyValidationIssue(
                "fan.policy.hysteresis_invalid",
                PolicyValidationSeverity.Error,
                "Fan policy hysteresis must be 0°C or greater.",
                policy.Id));
        }

        if (policy.RampUpSeconds < 0)
        {
            issues.Add(new PolicyValidationIssue(
                "fan.policy.ramp_up_invalid",
                PolicyValidationSeverity.Error,
                "Fan policy ramp-up time must be 0s or greater.",
                policy.Id));
        }

        if (policy.RampDownSeconds < 0)
        {
            issues.Add(new PolicyValidationIssue(
                "fan.policy.ramp_down_invalid",
                PolicyValidationSeverity.Error,
                "Fan policy ramp-down time must be 0s or greater.",
                policy.Id));
        }

        double? previousInput = null;
        foreach (var point in policy.Points)
        {
            if (point.InputValue < 0 || point.InputValue > 120)
            {
                issues.Add(new PolicyValidationIssue(
                    "fan.policy.point_input_out_of_range",
                    PolicyValidationSeverity.Error,
                    $"Fan curve input '{point.InputValue:0.#}°C' must be within 0..120°C.",
                    policy.Id));
            }

            if (point.OutputPercent < 0 || point.OutputPercent > 100)
            {
                issues.Add(new PolicyValidationIssue(
                    "fan.policy.point_output_out_of_range",
                    PolicyValidationSeverity.Error,
                    $"Fan curve output '{point.OutputPercent:0.#}%' must be within 0..100%.",
                    policy.Id));
            }

            if (previousInput.HasValue && point.InputValue < previousInput.Value)
            {
                issues.Add(new PolicyValidationIssue(
                    "fan.policy.points_not_sorted",
                    PolicyValidationSeverity.Error,
                    "Fan curve input values must be non-decreasing.",
                    policy.Id));
                break;
            }

            previousInput = point.InputValue;
        }

        return issues;
    }
}
