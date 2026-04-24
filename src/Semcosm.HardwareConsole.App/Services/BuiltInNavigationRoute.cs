using System;
using Microsoft.UI.Xaml.Controls;

namespace Semcosm.HardwareConsole.App.Services;

public sealed record BuiltInNavigationRoute(
    string Tag,
    string Title,
    Type PageType,
    Symbol Icon,
    bool IsFooter = false)
    : NavigationRoute(
        Tag,
        Title,
        NavigationRouteKind.BuiltInPage,
        BuiltInProviderId,
        IsFooter,
        false)
{
    public const string BuiltInProviderId = "semcosm.app.builtin";
}
