# Architecture

## Current Layering

```text
Semcosm.HardwareConsole.App
  -> Semcosm.HardwareConsole.Mock
    -> Semcosm.HardwareConsole.Abstractions
```

Responsibilities:

- `App`: WinUI shell, pages, view models, navigation, page composition
- `Abstractions`: shared hardware contracts, descriptors, values, and provider interfaces
- `Mock`: development-time implementations of inventory, plugin registry, and snapshot providers

Current navigation shape:

```text
Navigation route providers
  -> composite route registry
  -> route content factories
    -> page factory
    -> navigation service
      -> MainWindow
```

This keeps page registration centralized and makes it possible to add plugin-provided routes later without putting more `switch` logic into the shell.

Current route types:

- `BuiltInNavigationRoute`: WinUI pages owned by the app shell
- future `PluginPage` routes: pages hosted from plugin-provided UI surfaces
- future `ExternalPanel` routes: embedded extension panels or out-of-process UI hosts

Current route-content boundary:

- `IRouteContentFactory` is the extension point that maps a `NavigationRoute` to shell content
- `BuiltInPageRouteContentFactory` currently handles `BuiltInNavigationRoute`
- plugin page hosting and external panel hosting are intentionally not implemented yet, but now have a clear seam instead of being hidden inside `PageFactory`

Current `NavigationService` scope:

- singleton, single-window shell service
- owns one active `Frame` through `Initialize(Frame frame)`
- must move to a per-window navigation context before multi-window shells, tray popups, or diagnostics windows are introduced

Current route registry behavior:

- built-in routes currently come from `BuiltInNavigationRouteProvider`
- `CompositeNavigationRouteRegistry` merges route providers into the shell-facing registry
- the registry now exposes `RoutesChanged`
- `MainWindow` rebuilds its menu when route metadata changes
- plugin route contribution is not implemented yet, but the registry no longer requires plugins to mutate the built-in route source directly

## Direction

The repository is being shaped so that UI code depends on abstractions and data providers rather than device-specific implementations.

Target flow:

```text
real hardware adapter or plugin host
  -> inventory / registry / snapshot provider
    -> view model
      -> WinUI page
```

This keeps the UI stable when mock data is replaced with real hardware integration.
