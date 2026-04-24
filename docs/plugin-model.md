# Plugin Model

## Current Position

The app is moving toward a capability-driven hardware console instead of a brand-specific tuning panel.

Today, `PluginManifestModel` is the UI-side shape used to describe:

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
