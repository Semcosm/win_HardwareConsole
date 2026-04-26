using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record PluginDeviceDeclaration(
    string Id,
    string DisplayName,
    string Vendor,
    string Model,
    IReadOnlyList<string> CapabilityIds);
