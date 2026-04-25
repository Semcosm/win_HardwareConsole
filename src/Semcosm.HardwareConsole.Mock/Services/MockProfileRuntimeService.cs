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
                GetActiveProfile(),
                Array.Empty<ProfileControlActionDescriptor>(),
                mode,
                false,
                false,
                "Profile not found.");
        }

        if (mode == ProfileApplyMode.Activate)
        {
            _activeProfileId = profile.Id;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        return new ProfileApplyResult(
            true,
            mode == ProfileApplyMode.Activate ? profile : GetActiveProfile(),
            profile.Actions,
            mode,
            false,
            RequiresConfirmation(profile),
            mode == ProfileApplyMode.Activate
                ? "Mock runtime updated. Active profile changed without writing real hardware."
                : "Simulation only. Active profile was not changed.");
    }

    private static bool RequiresConfirmation(ProfileDescriptor profile)
    {
        foreach (var action in profile.Actions)
        {
            if (action.RequiresConfirmation)
            {
                return true;
            }
        }

        return false;
    }
}
