using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record SchedulerRuleDescriptor(
    string Id,
    string DisplayName,
    string MatchText,
    string Description,
    IReadOnlyList<SchedulerPolicyActionDescriptor> Actions);
