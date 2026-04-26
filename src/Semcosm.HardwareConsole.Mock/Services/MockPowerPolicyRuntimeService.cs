using System;
using System.Collections.Generic;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.Mock.Services;

public sealed class MockPowerPolicyRuntimeService : IPowerPolicyRuntimeService
{
    private readonly IPolicyValidator<PowerPolicyDescriptor, PowerPolicyValidationResult> _powerPolicyValidator;

    public MockPowerPolicyRuntimeService(
        IPolicyValidator<PowerPolicyDescriptor, PowerPolicyValidationResult> powerPolicyValidator)
    {
        _powerPolicyValidator = powerPolicyValidator;
    }

    public IReadOnlyList<PowerPolicyDescriptor> GetAvailablePowerPolicies() => MockHardwareData.PowerPolicies;

    public PowerPolicyPreview PreviewPowerPolicy(PowerPolicyDescriptor policy)
    {
        var validationResult = _powerPolicyValidator.Validate(policy);
        if (!validationResult.IsValid)
        {
            return new PowerPolicyPreview(
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

        return new PowerPolicyPreview(
            true,
            policy.Id,
            policy,
            PolicyPreviewFailureCode.None,
            validationResult.RequiredSensorIds,
            validationResult.WouldSetControlIds,
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
}
