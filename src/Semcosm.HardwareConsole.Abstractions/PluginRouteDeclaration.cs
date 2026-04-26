namespace Semcosm.HardwareConsole.Abstractions;

public sealed record PluginRouteDeclaration(
    string Tag,
    string Title,
    string Kind,
    string Icon,
    bool IsFooter);
