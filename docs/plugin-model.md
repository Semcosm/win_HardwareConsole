# Plugin Model

## Current Position

The app is moving toward a capability-driven hardware console instead of a brand-specific tuning panel.

`Semcosm.HardwareConsole.Abstractions` now contains the first plugin-facing contract layer, including:

- `IHardwareInventoryService`
- `IPluginRegistry`
- `ISensorSnapshotProvider`
- `PluginDescriptor`
- `DeviceDescriptor`
- `SensorDescriptor`
- `ControlDescriptor`
- `SensorValue`
- `ControlValue`
- `HardwareCapability`
- `ProfileDescriptor`
- `PolicyDescriptor`
- `HardwareRiskLevel`
- `ControlRiskLevel`
- `SensorKind`
- `ControlKind`

`PluginManifestModel` remains in the App project as a UI-facing projection. It is mapped from `PluginDescriptor` and plugin registry state instead of acting as the plugin protocol itself.

The current UI model still describes:

- plugin identity
- vendor
- version
- state
- risk level
- capabilities
- matched devices

## Intended Evolution

Later, real plugins should provide:

- hardware sensors
- writable controls
- device matching metadata
- policy hooks
- optional UI extensions

The `Plugins` page is the first UI surface that reflects this direction.
