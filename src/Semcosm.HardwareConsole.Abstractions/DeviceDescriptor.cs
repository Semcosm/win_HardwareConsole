using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record DeviceDescriptor(
    string Id,
    string DisplayName,
    string Vendor,
    string Model,
    IReadOnlyList<string> CapabilityIds);
