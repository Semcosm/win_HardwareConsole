using System.Collections.Generic;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.Mock.Services;

internal sealed class MockThermalPolicyValidator
{
    private readonly IReadOnlyDictionary<string, ControlDescriptor> _controls;
    private readonly IReadOnlyDictionary<string, SensorDescriptor> _sensors;

    public MockThermalPolicyValidator()
    {
        _sensors = MockHardwareData.Sensors.ToDictionary(sensor => sensor.Id);
        _controls = MockHardwareData.Controls.ToDictionary(control => control.Id);
    }

    public ThermalPolicyValidationResult Validate(ThermalPolicyDescriptor policy)
    {
        var requiredSensorIds = policy.InputSensorIds
            .Concat(policy.Actions.Select(action => action.TriggerSensorId))
            .Distinct()
            .ToArray();

        var wouldSetControlIds = policy.Actions
            .Select(action => action.ControlId)
            .Distinct()
            .ToArray();

        var descriptorErrors = ValidateDescriptor(policy);
        if (descriptorErrors.Count > 0)
        {
            return new ThermalPolicyValidationResult(
                false,
                ThermalPolicyFailureCode.InvalidPolicy,
                requiredSensorIds,
                wouldSetControlIds,
                descriptorErrors,
                new[]
                {
                    "Mock thermal policy validator rejected descriptor-level constraints."
                },
                "Preview failed because the thermal policy descriptor is invalid.");
        }

        var missingSensorIds = requiredSensorIds
            .Where(sensorId => !_sensors.ContainsKey(sensorId))
            .Distinct()
            .ToArray();

        if (missingSensorIds.Length > 0)
        {
            return new ThermalPolicyValidationResult(
                false,
                ThermalPolicyFailureCode.MissingRequiredSensor,
                requiredSensorIds,
                wouldSetControlIds,
                missingSensorIds
                    .Select(sensorId => $"Required sensor '{sensorId}' is not available in the current mock inventory.")
                    .ToArray(),
                new[]
                {
                    "Mock thermal policy validator could not resolve all required sensors."
                },
                "Preview failed because one or more thermal trigger sensors are missing.");
        }

        var missingControlIds = wouldSetControlIds
            .Where(controlId => !_controls.ContainsKey(controlId))
            .Distinct()
            .ToArray();

        if (missingControlIds.Length > 0)
        {
            return new ThermalPolicyValidationResult(
                false,
                ThermalPolicyFailureCode.UnsupportedControl,
                requiredSensorIds,
                wouldSetControlIds,
                missingControlIds
                    .Select(controlId => $"Output control '{controlId}' is not available in the current mock inventory.")
                    .ToArray(),
                new[]
                {
                    "Mock thermal policy validator could not resolve all target controls."
                },
                "Preview failed because one or more thermal controls are unsupported.");
        }

        return new ThermalPolicyValidationResult(
            true,
            ThermalPolicyFailureCode.None,
            requiredSensorIds,
            wouldSetControlIds,
            [],
            new[]
            {
                "Mock thermal policy validator accepted the descriptor."
            },
            "Thermal policy descriptor validation passed.");
    }

    private List<string> ValidateDescriptor(ThermalPolicyDescriptor policy)
    {
        var errors = new List<string>();

        if (policy.InputSensorIds.Count == 0)
        {
            errors.Add("Thermal policy must declare at least one input sensor.");
        }

        if (policy.PollIntervalSeconds <= 0)
        {
            errors.Add("Poll interval must be greater than 0 seconds.");
        }

        if (policy.CooldownSeconds < 0)
        {
            errors.Add("Cooldown must be 0 seconds or greater.");
        }

        if (policy.Actions.Count == 0)
        {
            errors.Add("Thermal policy must declare at least one threshold action.");
        }

        foreach (var sensorId in policy.InputSensorIds)
        {
            if (_sensors.TryGetValue(sensorId, out var sensor) && sensor.Kind != SensorKind.Temperature)
            {
                errors.Add($"Input sensor '{sensorId}' must be a temperature sensor for a thermal policy chain.");
            }
        }

        foreach (var action in policy.Actions)
        {
            if (!policy.InputSensorIds.Contains(action.TriggerSensorId))
            {
                errors.Add($"Action '{action.StageLabel}' references trigger sensor '{action.TriggerSensorId}' which is not declared in InputSensorIds.");
            }

            if (_sensors.TryGetValue(action.TriggerSensorId, out var triggerSensor)
                && triggerSensor.Kind != SensorKind.Temperature)
            {
                errors.Add($"Action '{action.StageLabel}' must use a temperature sensor as its trigger source.");
            }

            if (action.TriggerThreshold < 0 || action.TriggerThreshold > 120)
            {
                errors.Add($"Action '{action.StageLabel}' has an out-of-range trigger threshold '{action.TriggerThreshold:0.#}°C'.");
            }

            if (IsHighRisk(action.RiskLevel) && !action.RequiresConfirmation)
            {
                errors.Add($"Action '{action.StageLabel}' must require confirmation because it targets a high-risk control.");
            }

            if (_controls.TryGetValue(action.ControlId, out var control)
                && !IsTargetValueCompatible(control, action.TargetValue))
            {
                errors.Add($"Action '{action.StageLabel}' target value is not compatible with control '{action.ControlId}'.");
            }
        }

        foreach (var grouping in policy.Actions.GroupBy(action => action.TriggerSensorId))
        {
            double? previousThreshold = null;

            foreach (var action in grouping)
            {
                if (previousThreshold.HasValue && action.TriggerThreshold < previousThreshold.Value)
                {
                    errors.Add($"Trigger thresholds for sensor '{grouping.Key}' must be non-decreasing across the thermal chain.");
                    break;
                }

                previousThreshold = action.TriggerThreshold;
            }
        }

        return errors;
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
