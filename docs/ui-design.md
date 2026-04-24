# UI Design

## Current State

The current UI is a WinUI 3 shell with a left navigation rail and placeholder pages for the main hardware-control areas.

Implemented binding-driven pages:

- `Dashboard`
- `Plugins`

Placeholder pages:

- `Performance`
- `Fans`
- `Power`
- `Thermal`
- `Scheduler`
- `Devices`
- `Profiles`
- `Diagnostics`
- `Settings`

## Design Rules

- Prefer data binding over hard-coded page values
- Keep page layout independent from vendor-specific hardware logic
- Surface plugin capabilities and profile state as first-class UI concepts
- Add reusable controls only after two or more pages need the same layout pattern
