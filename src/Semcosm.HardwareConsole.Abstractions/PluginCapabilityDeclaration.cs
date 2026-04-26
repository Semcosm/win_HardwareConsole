using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record PluginCapabilityDeclaration(
    string Id,
    string DisplayName,
    string Description,
    IReadOnlyList<string> CapabilityTags);
