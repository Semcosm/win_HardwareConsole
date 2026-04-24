# Architecture

## Current Layering

```text
Semcosm.HardwareConsole.App
  -> Semcosm.HardwareConsole.Mock
    -> Semcosm.HardwareConsole.Abstractions
```

Responsibilities:

- `App`: WinUI shell, pages, view models, navigation, page composition
- `Abstractions`: shared UI-facing contracts and portable models
- `Mock`: development-time data providers and fake hardware feeds

## Direction

The repository is being shaped so that UI code depends on abstractions and data providers rather than device-specific implementations.

Target flow:

```text
real hardware adapter or plugin
  -> service implementation
    -> view model
      -> WinUI page
```

This keeps the UI stable when mock data is replaced with real hardware integration.
