using System;
using System.Collections.Generic;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.Mock.Services;

public sealed class MockPolicyRuntimeService : IPolicyRuntimeService
{
    public IReadOnlyList<FanCurvePolicyDescriptor> GetAvailableFanPolicies() => MockHardwareData.FanCurvePolicies;

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

    public ThermalPolicyPreview PreviewThermalPolicy(ThermalPolicyDescriptor policy)
    {
        if (policy.Actions.Count == 0)
        {
            return new ThermalPolicyPreview(
                false,
                policy.Id,
                policy,
                ThermalPolicyFailureCode.InvalidPolicy,
                policy.InputSensorIds,
                Array.Empty<string>(),
                new[]
                {
                    "Thermal policy contains no threshold actions."
                },
                new[]
                {
                    "Mock runtime rejected the thermal policy because the action chain is empty."
                },
                "Preview failed because the thermal action chain is empty.");
        }

        var missingSensors = policy.InputSensorIds
            .Where(sensorId => !MockHardwareData.Sensors.Any(sensor => sensor.Id == sensorId))
            .Distinct()
            .ToArray();

        if (missingSensors.Length > 0)
        {
            return new ThermalPolicyPreview(
                false,
                policy.Id,
                policy,
                ThermalPolicyFailureCode.MissingRequiredSensor,
                missingSensors,
                Array.Empty<string>(),
                missingSensors.Select(sensorId => $"Required sensor '{sensorId}' is not available in the current mock inventory.").ToArray(),
                new[]
                {
                    "Mock runtime could not resolve all thermal trigger sensors."
                },
                "Preview failed because one or more thermal trigger sensors are missing.");
        }

        var missingControls = policy.Actions
            .Select(action => action.ControlId)
            .Where(controlId => !MockHardwareData.Controls.Any(control => control.Id == controlId))
            .Distinct()
            .ToArray();

        if (missingControls.Length > 0)
        {
            return new ThermalPolicyPreview(
                false,
                policy.Id,
                policy,
                ThermalPolicyFailureCode.UnsupportedControl,
                policy.InputSensorIds,
                missingControls,
                missingControls.Select(controlId => $"Output control '{controlId}' is not available in the current mock inventory.").ToArray(),
                new[]
                {
                    "Mock runtime could not resolve all thermal output controls."
                },
                "Preview failed because one or more thermal controls are unsupported.");
        }

        return new ThermalPolicyPreview(
            true,
            policy.Id,
            policy,
            ThermalPolicyFailureCode.None,
            policy.InputSensorIds,
            policy.Actions.Select(action => action.ControlId).Distinct().ToArray(),
            Array.Empty<string>(),
            new[]
            {
                $"Thermal stages: {policy.Actions.Count}",
                $"Polling every {policy.PollIntervalSeconds:0.#}s",
                $"Cooldown {policy.CooldownSeconds:0.#}s",
                "Preview only. No real thermal controller or hardware write is performed."
            },
            "Preview only. No real thermal controller or hardware write is performed.");
    }
}
