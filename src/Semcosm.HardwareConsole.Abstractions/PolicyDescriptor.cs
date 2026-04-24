using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record PolicyDescriptor(
    string Id,
    string DisplayName,
    string Description,
    IReadOnlyList<string> TriggerIds,
    IReadOnlyList<string> CapabilityIds);
