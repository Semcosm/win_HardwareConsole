using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record ProfileDescriptor(
    string Id,
    string DisplayName,
    string Description,
    IReadOnlyList<string> CapabilityIds);
