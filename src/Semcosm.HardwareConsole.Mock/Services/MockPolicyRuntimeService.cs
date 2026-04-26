using System;
using System.Collections.Generic;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.Mock.Services;

public sealed class MockPolicyRuntimeService : IPolicyRuntimeService
{
    private readonly IPolicyValidator<ThermalPolicyDescriptor, ThermalPolicyValidationResult> _thermalPolicyValidator;

    public MockPolicyRuntimeService(
        IPolicyValidator<ThermalPolicyDescriptor, ThermalPolicyValidationResult> thermalPolicyValidator)
    {
        _thermalPolicyValidator = thermalPolicyValidator;
    }

    public IReadOnlyList<FanCurvePolicyDescriptor> GetAvailableFanPolicies() => MockHardwareData.FanCurvePolicies;

    public IReadOnlyList<PowerPolicyDescriptor> GetAvailablePowerPolicies() => MockHardwareData.PowerPolicies;

    public IReadOnlyList<SchedulerPolicyDescriptor> GetAvailableSchedulerPolicies() => MockHardwareData.SchedulerPolicies;

    public IReadOnlyList<ThermalPolicyDescriptor> GetAvailableThermalPolicies() => MockHardwareData.ThermalPolicies;

    public PolicyRuntimePreview PreviewFanPolicy(FanCurvePolicyDescriptor policy)
    {
        if (policy.Points.Count == 0)
        {
            return new PolicyRuntimePreview(
                false,
                policy.Id,
                policy,
                PolicyPreviewFailureCode.InvalidPolicy,
                Array.Empty<string>(),
                Array.Empty<string>(),
                new[]
                {
                    "Fan curve contains no points."
                },
                new[]
                {
                    "Mock runtime rejected the fan policy because it has no curve points."
                },
                "Preview failed because the fan curve is empty.");
        }

        if (!MockHardwareData.Sensors.Any(sensor => sensor.Id == policy.InputSensorId))
        {
            return new PolicyRuntimePreview(
                false,
                policy.Id,
                policy,
                PolicyPreviewFailureCode.MissingRequiredSensor,
                Array.Empty<string>(),
                new[] { policy.InputSensorId },
                new[]
                {
                    $"Required sensor '{policy.InputSensorId}' is not available in the current mock inventory."
                },
                new[]
                {
                    "Mock runtime could not resolve the requested input sensor."
                },
                "Preview failed because the input sensor is missing.");
        }

        if (!MockHardwareData.Controls.Any(control => control.Id == policy.OutputControlId))
        {
            return new PolicyRuntimePreview(
                false,
                policy.Id,
                policy,
                PolicyPreviewFailureCode.UnsupportedControl,
                new[] { policy.OutputControlId },
                new[] { policy.InputSensorId },
                new[]
                {
                    $"Output control '{policy.OutputControlId}' is not available in the current mock inventory."
                },
                new[]
                {
                    "Mock runtime could not resolve the requested output control."
                },
                "Preview failed because the output control is unsupported.");
        }

        return new PolicyRuntimePreview(
            true,
            policy.Id,
            policy,
            PolicyPreviewFailureCode.None,
            new[] { policy.OutputControlId },
            new[] { policy.InputSensorId },
            Array.Empty<string>(),
            new[]
            {
                $"Curve points: {policy.Points.Count}",
                $"Hysteresis {policy.HysteresisDegrees:0.#}°C",
                $"Ramp-up {policy.RampUpSeconds:0.#}s",
                $"Ramp-down {policy.RampDownSeconds:0.#}s",
                "Preview only. No real hardware controller write is performed."
            },
            "Preview only. No real hardware controller write is performed.");
    }

    public PowerPolicyPreview PreviewPowerPolicy(PowerPolicyDescriptor policy)
    {
        if (policy.Actions.Count == 0)
        {
            return new PowerPolicyPreview(
                false,
                policy.Id,
                policy,
                PolicyPreviewFailureCode.InvalidPolicy,
                Array.Empty<string>(),
                Array.Empty<string>(),
                new[]
                {
                    "Power policy contains no actions."
                },
                new[]
                {
                    "Mock runtime rejected the power policy because it has no AC/DC action set."
                },
                "Preview failed because the power policy has no actions.");
        }

        var missingSensors = policy.InputSensorIds
            .Where(sensorId => !MockHardwareData.Sensors.Any(sensor => sensor.Id == sensorId))
            .Distinct()
            .ToArray();

        if (missingSensors.Length > 0)
        {
            return new PowerPolicyPreview(
                false,
                policy.Id,
                policy,
                PolicyPreviewFailureCode.MissingRequiredSensor,
                missingSensors,
                Array.Empty<string>(),
                missingSensors.Select(sensorId =>
                    $"Required sensor '{sensorId}' is not available in the current mock inventory.")
                    .ToArray(),
                new[]
                {
                    "Mock runtime could not resolve one or more required power-policy sensors."
                },
                "Preview failed because one or more required sensors are missing.");
        }

        var unsupportedControls = policy.Actions
            .Select(action => action.ControlId)
            .Where(controlId => !MockHardwareData.Controls.Any(control => control.Id == controlId))
            .Distinct()
            .ToArray();

        if (unsupportedControls.Length > 0)
        {
            return new PowerPolicyPreview(
                false,
                policy.Id,
                policy,
                PolicyPreviewFailureCode.UnsupportedControl,
                policy.InputSensorIds,
                unsupportedControls,
                unsupportedControls.Select(controlId =>
                    $"Output control '{controlId}' is not available in the current mock inventory.")
                    .ToArray(),
                new[]
                {
                    "Mock runtime could not resolve one or more power-policy target controls."
                },
                "Preview failed because one or more target controls are unsupported.");
        }

        return new PowerPolicyPreview(
            true,
            policy.Id,
            policy,
            PolicyPreviewFailureCode.None,
            policy.InputSensorIds,
            policy.Actions.Select(action => action.ControlId).Distinct().ToArray(),
            Array.Empty<string>(),
            new[]
            {
                $"Power plan: {policy.PowerPlanName}",
                $"AC actions: {policy.Actions.Count(action => string.Equals(action.ConditionLabel, "AC", StringComparison.OrdinalIgnoreCase))}",
                $"DC actions: {policy.Actions.Count(action => string.Equals(action.ConditionLabel, "DC", StringComparison.OrdinalIgnoreCase))}",
                "Preview only. No real Windows power plan, PL1/PL2, or GPU limit write is performed."
            },
            "Preview only. No real Windows power plan, PL1/PL2, or GPU limit write is performed.");
    }

    public SchedulerPolicyPreview PreviewSchedulerPolicy(SchedulerPolicyDescriptor policy)
    {
        if (policy.Rules.Count == 0)
        {
            return new SchedulerPolicyPreview(
                false,
                policy.Id,
                policy,
                PolicyPreviewFailureCode.InvalidPolicy,
                Array.Empty<string>(),
                Array.Empty<string>(),
                new[]
                {
                    "Scheduler policy contains no process rules."
                },
                new[]
                {
                    "Mock runtime rejected the scheduler policy because it has no rules."
                },
                "Preview failed because the scheduler policy has no rules.");
        }

        var missingSensors = policy.InputSensorIds
            .Where(sensorId => !MockHardwareData.Sensors.Any(sensor => sensor.Id == sensorId))
            .Distinct()
            .ToArray();

        if (missingSensors.Length > 0)
        {
            return new SchedulerPolicyPreview(
                false,
                policy.Id,
                policy,
                PolicyPreviewFailureCode.MissingRequiredSensor,
                missingSensors,
                Array.Empty<string>(),
                missingSensors.Select(sensorId =>
                    $"Required sensor '{sensorId}' is not available in the current mock inventory.")
                    .ToArray(),
                new[]
                {
                    "Mock runtime could not resolve one or more required scheduler-policy sensors."
                },
                "Preview failed because one or more required sensors are missing.");
        }

        var unsupportedControls = policy.Rules
            .SelectMany(rule => rule.Actions)
            .Select(action => action.ControlId)
            .Where(controlId => !MockHardwareData.Controls.Any(control => control.Id == controlId))
            .Distinct()
            .ToArray();

        if (unsupportedControls.Length > 0)
        {
            return new SchedulerPolicyPreview(
                false,
                policy.Id,
                policy,
                PolicyPreviewFailureCode.UnsupportedControl,
                policy.InputSensorIds,
                unsupportedControls,
                unsupportedControls.Select(controlId =>
                    $"Scheduler action control '{controlId}' is not available in the current mock inventory.")
                    .ToArray(),
                new[]
                {
                    "Mock runtime could not resolve one or more scheduler-policy target controls."
                },
                "Preview failed because one or more scheduler controls are unsupported.");
        }

        return new SchedulerPolicyPreview(
            true,
            policy.Id,
            policy,
            PolicyPreviewFailureCode.None,
            policy.InputSensorIds,
            policy.Rules.SelectMany(rule => rule.Actions).Select(action => action.ControlId).Distinct().ToArray(),
            Array.Empty<string>(),
            new[]
            {
                $"Rules: {policy.Rules.Count}",
                $"Foreground strategy: {policy.ForegroundStrategy}",
                $"Background strategy: {policy.BackgroundStrategy}",
                "Preview only. No real scheduler, EcoQoS, or efficiency-mode write is performed."
            },
            "Preview only. No real scheduler, EcoQoS, or efficiency-mode write is performed.");
    }

    public ThermalPolicyPreview PreviewThermalPolicy(ThermalPolicyDescriptor policy)
    {
        var validationResult = _thermalPolicyValidator.Validate(policy);
        if (!validationResult.IsValid)
        {
            return new ThermalPolicyPreview(
                false,
                policy.Id,
                policy,
                validationResult.FailureCode,
                validationResult.RequiredSensorIds,
                validationResult.WouldSetControlIds,
                validationResult.BlockedReasons,
                validationResult.Diagnostics,
                validationResult.Message);
        }

        return new ThermalPolicyPreview(
            true,
            policy.Id,
            policy,
            ThermalPolicyFailureCode.None,
            validationResult.RequiredSensorIds,
            validationResult.WouldSetControlIds,
            Array.Empty<string>(),
            new[]
            {
                $"Thermal stages: {policy.Actions.Count}",
                $"Polling every {policy.PollIntervalSeconds:0.#}s",
                $"Cooldown {policy.CooldownSeconds:0.#}s",
                "Descriptor validation passed.",
                "Preview only. No real thermal controller or hardware write is performed."
            },
            "Preview only. No real thermal controller or hardware write is performed.");
    }
}
