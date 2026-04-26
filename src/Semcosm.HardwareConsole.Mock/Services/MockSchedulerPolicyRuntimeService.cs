using System;
using System.Collections.Generic;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.Mock.Services;

public sealed class MockSchedulerPolicyRuntimeService : ISchedulerPolicyRuntimeService
{
    private readonly IPolicyValidator<SchedulerPolicyDescriptor, SchedulerPolicyValidationResult> _schedulerPolicyValidator;

    public MockSchedulerPolicyRuntimeService(
        IPolicyValidator<SchedulerPolicyDescriptor, SchedulerPolicyValidationResult> schedulerPolicyValidator)
    {
        _schedulerPolicyValidator = schedulerPolicyValidator;
    }

    public IReadOnlyList<SchedulerPolicyDescriptor> GetAvailableSchedulerPolicies() => MockHardwareData.SchedulerPolicies;

    public SchedulerPolicyPreview PreviewSchedulerPolicy(SchedulerPolicyDescriptor policy)
    {
        var validationResult = _schedulerPolicyValidator.Validate(policy);
        if (!validationResult.IsValid)
        {
            return new SchedulerPolicyPreview(
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

        return new SchedulerPolicyPreview(
            true,
            policy.Id,
            policy,
            PolicyPreviewFailureCode.None,
            validationResult.RequiredSensorIds,
            validationResult.WouldSetControlIds,
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
}
