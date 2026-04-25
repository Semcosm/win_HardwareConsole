using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record ProfilePreview(
    ProfileDescriptor? Profile,
    IReadOnlyList<ProfileControlActionDescriptor> Actions,
    bool RequiresConfirmation,
    string Message);
