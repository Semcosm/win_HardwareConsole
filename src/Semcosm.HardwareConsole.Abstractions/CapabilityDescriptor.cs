namespace Semcosm.HardwareConsole.Abstractions;

public sealed record CapabilityDescriptor(
    string Id,
    string DisplayName,
    string Category,
    string Description);
