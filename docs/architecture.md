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
