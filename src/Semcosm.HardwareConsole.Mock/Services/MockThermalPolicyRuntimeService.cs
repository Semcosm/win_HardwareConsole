using System;
using System.Collections.Generic;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.Mock.Services;

public sealed class MockThermalPolicyRuntimeService : IThermalPolicyRuntimeService
{
    private readonly IPolicyValidator<ThermalPolicyDescriptor, ThermalPolicyValidationResult> _thermalPolicyValidator;

    public MockThermalPolicyRuntimeService(
        IPolicyValidator<ThermalPolicyDescriptor, ThermalPolicyValidationResult> thermalPolicyValidator)
    {
        _thermalPolicyValidator = thermalPolicyValidator;
    }

    public IReadOnlyList<ThermalPolicyDescriptor> GetAvailableThermalPolicies() => MockHardwareData.ThermalPolicies;

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
