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
NavigationRoute registry
  -> page factory
    -> navigation service
      -> MainWindow
```

This keeps page registration centralized and makes it possible to add plugin-provided routes later without putting more `switch` logic into the shell.

Current route types:

- `BuiltInNavigationRoute`: WinUI pages owned by the app shell
- future `PluginPage` routes: pages hosted from plugin-provided UI surfaces
- future `ExternalPanel` routes: embedded extension panels or out-of-process UI hosts

Current `NavigationService` scope:

- singleton, single-window shell service
- owns one active `Frame` through `Initialize(Frame frame)`
- must move to a per-window navigation context before multi-window shells, tray popups, or diagnostics windows are introduced

Current route registry behavior:

- the registry now exposes `RoutesChanged`
- `MainWindow` rebuilds its menu when route metadata changes
- built-in routes are still static today; dynamic plugin route contribution is the next layer, not already implemented

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
