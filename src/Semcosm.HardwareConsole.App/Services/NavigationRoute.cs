using System;

namespace Semcosm.HardwareConsole.App.Services;

public sealed record NavigationRoute(
    string Tag,
    string Title,
    Type PageType,
    string Icon,
    bool IsFooter = false,
    bool IsPluginProvided = false);
