using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record ProfileApplyResult(
    bool Success,
    string ProfileId,
    ProfileApplyMode Mode,
    string Message,
    ProfileApplyFailureCode FailureCode,
    IReadOnlyList<ProfileControlActionDescriptor> WouldSetActions,
    IReadOnlyList<ProfileControlActionDescriptor> AppliedActions,
    IReadOnlyList<ProfileControlActionDescriptor> BlockedActions,
    IReadOnlyList<string> Diagnostics);
