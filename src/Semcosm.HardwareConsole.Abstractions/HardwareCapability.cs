namespace Semcosm.HardwareConsole.Abstractions;

public sealed record HardwareCapability(
    string Id,
    string Category,
    string DisplayName,
    string Description);
