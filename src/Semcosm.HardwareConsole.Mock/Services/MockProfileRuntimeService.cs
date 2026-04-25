using System;
using System.Collections.Generic;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.Mock.Services;

public sealed class MockProfileRuntimeService : IProfileRuntimeService
{
    private string _activeProfileId = "profile.performance";

    public event EventHandler? StateChanged;

    public ProfileDescriptor? GetActiveProfile()
    {
        return MockHardwareData.GetProfile(_activeProfileId);
    }

    public IReadOnlyList<ProfileDescriptor> GetAvailableProfiles()
    {
        return MockHardwareData.Profiles;
    }

    public ProfilePreview PreviewProfile(string profileId)
    {
        var profile = MockHardwareData.GetProfile(profileId);
        if (profile is null)
        {
            return new ProfilePreview(
                null,
                Array.Empty<ProfileControlActionDescriptor>(),
                false,
                "Profile not found.");
        }

        var requiresConfirmation = RequiresConfirmation(profile);

        return new ProfilePreview(
            profile,
            profile.Actions,
            requiresConfirmation,
            "Preview only. Applying this profile in mock mode will not write real hardware controls.");
    }

    public ProfileApplyResult ApplyProfile(string profileId, ProfileApplyMode mode)
    {
        var profile = MockHardwareData.GetProfile(profileId);
        if (profile is null)
        {
            return new ProfileApplyResult(
                false,
                profileId,
                mode,
                "Profile not found.",
                ProfileApplyFailureCode.UnknownProfile,
                Array.Empty<ProfileControlActionDescriptor>(),
                Array.Empty<ProfileControlActionDescriptor>(),
                Array.Empty<ProfileControlActionDescriptor>(),
                new[] { $"Mock runtime could not find profile '{profileId}'." });
        }

        if (mode == ProfileApplyMode.Simulate)
        {
            return new ProfileApplyResult(
                true,
                profile.Id,
                mode,
                "Simulation only. Active profile was not changed and no real hardware write was performed.",
                ProfileApplyFailureCode.None,
                profile.Actions,
                Array.Empty<ProfileControlActionDescriptor>(),
                Array.Empty<ProfileControlActionDescriptor>(),
                new[]
                {
                    "Mock runtime simulation only.",
                    "No real hardware write was performed."
                });
        }

        if (RequiresConfirmation(profile) && mode != ProfileApplyMode.ActivateConfirmed)
        {
            return new ProfileApplyResult(
                false,
                profile.Id,
                mode,
                "Preview and confirmation are required before applying this mock hardware-write profile.",
                ProfileApplyFailureCode.ConfirmationRequired,
                profile.Actions,
                Array.Empty<ProfileControlActionDescriptor>(),
                GetBlockedActions(profile),
                new[]
                {
                    "This profile is confirmation-gated in the mock runtime.",
                    "No real hardware write was performed."
                });
        }

        _activeProfileId = profile.Id;
        StateChanged?.Invoke(this, EventArgs.Empty);

        return new ProfileApplyResult(
            true,
            profile.Id,
            mode,
            "Mock runtime updated. Active profile changed without writing real hardware.",
            ProfileApplyFailureCode.None,
            profile.Actions,
            profile.Actions,
            Array.Empty<ProfileControlActionDescriptor>(),
            new[]
            {
                "Mock runtime active profile changed.",
                "No real hardware write was performed."
            });
    }

    private static bool RequiresConfirmation(ProfileDescriptor profile)
    {
        if (profile.RiskLevel is HardwareRiskLevel.HardwareWrite
            or HardwareRiskLevel.KernelDriverRequired
            or HardwareRiskLevel.Experimental)
        {
            return true;
        }

        foreach (var action in profile.Actions)
        {
            if (action.RequiresConfirmation)
            {
                return true;
            }
        }

        return false;
    }

    private static IReadOnlyList<ProfileControlActionDescriptor> GetBlockedActions(ProfileDescriptor profile)
    {
        var blockedActions = new List<ProfileControlActionDescriptor>();

        foreach (var action in profile.Actions)
        {
            if (action.RequiresConfirmation
                || action.RiskLevel is ControlRiskLevel.HardwareWrite
                    or ControlRiskLevel.KernelDriverRequired
                    or ControlRiskLevel.Experimental)
            {
                blockedActions.Add(action);
            }
        }

        return blockedActions.Count == 0 ? profile.Actions : blockedActions;
    }
}
