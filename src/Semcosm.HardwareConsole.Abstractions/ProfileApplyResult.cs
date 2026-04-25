using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record ProfileApplyResult(
    bool Success,
    ProfileDescriptor? ActiveProfile,
    IReadOnlyList<ProfileControlActionDescriptor> WouldSetActions,
    ProfileApplyMode Mode,
    bool HardwareWritePerformed,
    bool RequiresConfirmation,
    string Message);
