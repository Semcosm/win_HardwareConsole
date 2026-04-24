# Semcosm Hardware Console

WinUI 3 based Windows hardware control console prototype.

The current repository focuses on establishing the app shell and the UI architecture for a future plugin-driven hardware control platform.

This PR-level layout now follows PascalCase naming and a `src/`-based project structure:

```text
src/
  Semcosm.HardwareConsole.App/
  Semcosm.HardwareConsole.Abstractions/
  Semcosm.HardwareConsole.Mock/
```

Current app goals:

- `Dashboard` shows unified hardware state through bound view models instead of hard-coded UI.
- `Plugins` shows capability providers, risk level, matched devices, and extension metadata.
- Other pages already exist as placeholders for the next phase: `Performance`, `Fans`, `Power`, `Thermal`, `Scheduler`, `Devices`, `Profiles`, `Diagnostics`, `Settings`.

## Current Stack

- .NET 8
- WinUI 3
- Windows App SDK `1.8.260416003`
- Target framework: `net8.0-windows10.0.19041.0`

## Current Structure

```text
docs/
src/
  Semcosm.HardwareConsole.App/
    ViewModels/
    Views/
  Semcosm.HardwareConsole.Abstractions/
    Models/
  Semcosm.HardwareConsole.Mock/
    Services/
```

Current UI flow:

```text
Mock service
  -> ViewModel
    -> WinUI page
```

This is the intended direction for the real app as well:

```text
Hardware plugin / platform adapter
  -> abstraction-backed service layer
    -> view model
      -> WinUI page
```

## Implemented Pages

### Dashboard

Backed by:

- `src/Semcosm.HardwareConsole.Abstractions/Models/MetricCardModel.cs`
- `src/Semcosm.HardwareConsole.Mock/Services/MockHardwareService.cs`
- `src/Semcosm.HardwareConsole.App/ViewModels/DashboardViewModel.cs`
- `src/Semcosm.HardwareConsole.App/Views/DashboardPage.xaml`

Shows mock summary and control-state cards for:

- CPU
- GPU
- Active Profile
- Fans
- Power
- Thermal

### Plugins

Backed by:

- `src/Semcosm.HardwareConsole.Abstractions/Models/PluginManifestModel.cs`
- `src/Semcosm.HardwareConsole.Mock/Services/MockHardwareService.cs`
- `src/Semcosm.HardwareConsole.App/ViewModels/PluginsViewModel.cs`
- `src/Semcosm.HardwareConsole.App/Views/PluginsPage.xaml`

Shows mock installed plugins with:

- display name
- plugin id
- state
- risk level
- capabilities
- matched devices

Current mock plugins:

- `Windows Power Plugin`
- `NVIDIA NVAPI Plugin`
- `Mechrevo GM6PX0X Platform Plugin`

## Why This Shape

The goal is to avoid tying the UI directly to a specific laptop brand, EC interface, or GPU vendor. Instead, device and control capabilities should come from plugins or adapters, while pages render from view models and capability metadata.

That keeps the UI stable when the backend evolves from mock data to real hardware access.

## Run

Open `src/Semcosm.HardwareConsole.App/Semcosm.HardwareConsole.App.slnx` in Visual Studio and run the WinUI project.

If your environment has the .NET SDK installed, you can also use:

```bash
dotnet build src/Semcosm.HardwareConsole.App/Semcosm.HardwareConsole.App.csproj
```

## Next Suggested Steps

- Build the `Profiles` page with the same `Model + Service + ViewModel + Binding` pattern
- Introduce real plugin manifest loading
- Split mock services behind interfaces where the first real provider is ready
- Add reusable card controls and shared styles
