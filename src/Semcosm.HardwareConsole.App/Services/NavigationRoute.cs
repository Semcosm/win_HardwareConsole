namespace Semcosm.HardwareConsole.App.Services;

public abstract record NavigationRoute(
    string Tag,
    string Title,
    NavigationRouteKind Kind,
    string ProviderId,
    bool IsFooter = false,
    bool IsPluginProvided = false);
