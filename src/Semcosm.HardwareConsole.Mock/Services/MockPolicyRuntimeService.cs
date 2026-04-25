using System.Collections.Generic;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.Mock.Services;

public sealed class MockPolicyRuntimeService : IPolicyRuntimeService
{
    public IReadOnlyList<FanCurvePolicyDescriptor> GetAvailableFanPolicies() => MockHardwareData.FanCurvePolicies;

    public PolicyRuntimePreview PreviewFanPolicy(FanCurvePolicyDescriptor policy)
    {
        return new PolicyRuntimePreview(
            policy,
            $"{policy.OutputControlId} would follow {policy.InputSensorId} with {policy.Points.Count} curve points in mock mode.",
            $"Hysteresis {policy.HysteresisDegrees:0.#}°C · Ramp-up {policy.RampUpSeconds:0.#}s · Ramp-down {policy.RampDownSeconds:0.#}s",
            "Preview only. No real hardware controller write is performed.");
    }
}
