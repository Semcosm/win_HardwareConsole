using System;
using System.Collections.Generic;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.Mock.Services;

public sealed class MockFanPolicyRuntimeService : IFanPolicyRuntimeService
{
    private readonly IPolicyValidator<FanCurvePolicyDescriptor, FanPolicyValidationResult> _fanPolicyValidator;

    public MockFanPolicyRuntimeService(
        IPolicyValidator<FanCurvePolicyDescriptor, FanPolicyValidationResult> fanPolicyValidator)
    {
        _fanPolicyValidator = fanPolicyValidator;
    }

    public IReadOnlyList<FanCurvePolicyDescriptor> GetAvailableFanPolicies() => MockHardwareData.FanCurvePolicies;

    public PolicyRuntimePreview PreviewFanPolicy(FanCurvePolicyDescriptor policy)
    {
        var validationResult = _fanPolicyValidator.Validate(policy);
        if (!validationResult.IsValid)
        {
            return new PolicyRuntimePreview(
                false,
                policy.Id,
                policy,
                validationResult.FailureCode,
                validationResult.WouldSetControlIds,
                validationResult.RequiredSensorIds,
                validationResult.BlockedReasons,
                validationResult.Diagnostics,
                validationResult.Message);
        }

        return new PolicyRuntimePreview(
            true,
            policy.Id,
            policy,
            PolicyPreviewFailureCode.None,
            validationResult.WouldSetControlIds,
            validationResult.RequiredSensorIds,
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
