using System;
using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public interface IProfileRuntimeService
{
    event EventHandler? StateChanged;

    ProfileDescriptor? GetActiveProfile();
    IReadOnlyList<ProfileDescriptor> GetAvailableProfiles();
    ProfilePreview PreviewProfile(string profileId);
    ProfileApplyResult ApplyProfile(string profileId, ProfileApplyMode mode);
}
