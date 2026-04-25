using System;
using System.Collections.Generic;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.Mock.Services;

public sealed class MockPolicyRuntimeService : IPolicyRuntimeService
{
    public IReadOnlyList<FanCurvePolicyDescriptor> GetAvailableFanPolicies() => MockHardwareData.FanCurvePolicies;

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
}
